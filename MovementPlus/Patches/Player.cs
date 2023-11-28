using HarmonyLib;
using MovementPlus.NewAbility;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class PlayerPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        public static ButtslapAbility buttslapAbility;

        private static bool tooFast;

        [HarmonyPatch(typeof(Player), nameof(Player.Init))]
        [HarmonyPostfix]
        private static void Player_Init_Postfix(Player __instance)
        {
            if (MovementPlusPlugin.player == null && !__instance.isAI)
            {
                MovementPlusPlugin.player = __instance;
                MovementPlusPlugin.defaultBoostSpeed = __instance.normalBoostSpeed;
                MovementPlusPlugin.defaultVertMaxSpeed = __instance.vertMaxSpeed;
                MovementPlusPlugin.defaultVertTopJumpSpeed = __instance.vertTopJumpSpeed;
                MovementPlusPlugin.defaultJumpSpeed = __instance.specialAirAbility.jumpSpeed;
                __instance.motor.maxFallSpeed = ConfigSettings.Misc.maxFallSpeed.Value;

                __instance.wallrunAbility.lastSpeed = MovementPlusPlugin.savedLastSpeed;
                __instance.vertBottomExitSpeedThreshold = 0f;

                buttslapAbility = new ButtslapAbility(__instance);

                if (ConfigSettings.Misc.collisionChangeEnabled.Value)
                {
                    var bodies = __instance.GetComponentsInChildren<Rigidbody>();
                    foreach (var body in bodies)
                    {
                        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    }
                }
            }
        }

        
        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdatePlayer))]
        [HarmonyPostfix]
        private static void Player_FixedUpdatePlayer_Postfix(Player __instance)
        {
            if (MovementPlusPlugin.timeInAir >= 1.5f)
            {
                tooFast = true;
            }
            else
            {
                tooFast = false;
            }

            buttslapAbility.Activation();
        }


        [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
        [HarmonyPrefix]
        private static bool Player_Jump_Prefix(Player __instance)
        {
            __instance.audioManager.PlayVoice(ref __instance.currentVoicePriority, __instance.character, AudioClipID.VoiceJump, __instance.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
            __instance.PlayAnim(__instance.jumpHash, false, false, -1f);
            float num = 1f;
            if (__instance.targetMovement == Player.MovementType.RUNNING && __instance.IsGrounded() && __instance.motor.groundCollider.gameObject.layer != 23 && Vector3.Dot(Vector3.ProjectOnPlane(__instance.motor.groundNormal, Vector3.up).normalized, __instance.dir) < -0.5f)
            {
                num = 1f + Mathf.Min(__instance.motor.groundAngle / 90f, 0.3f);
            }
            __instance.ForceUnground(true);
            float num2 = 0f;
            if (__instance.timeSinceGrinding <= __instance.JumpPostGroundingGraceTime)
            {
                num2 = __instance.bonusJumpSpeedGrind;
            }
            else if (__instance.timeSinceWallrunning <= __instance.JumpPostGroundingGraceTime)
            {
                num2 = __instance.bonusJumpSpeedWallrun;
            }
            float num3 = __instance.jumpSpeed * num + num2;
            if (num2 != 0f && __instance.slideButtonHeld)
            {
                num3 *= __instance.abilityShorthopFactor;
                __instance.maintainSpeedJump = true;
            }
            else
            {
                __instance.maintainSpeedJump = false;
            }
            if (__instance.onLauncher)
            {
                if (!__instance.onLauncher.parent.gameObject.name.Contains("Super"))
                {
                    __instance.motor.SetVelocityYOneTime(__instance.jumpSpeedLauncher);
                }
                else
                {
                    __instance.motor.SetVelocityYOneTime(__instance.jumpSpeedLauncher * 1.4f);
                }
                if (__instance.targetMovement == Player.MovementType.RUNNING && Vector3.Dot(__instance.dir, __instance.onLauncher.back()) > 0.7f && !__instance.onLauncher.parent.gameObject.name.Contains("flat"))
                {
                    __instance.SetForwardSpeed(__instance.GetForwardSpeed() + 5f);
                }
                __instance.audioManager.PlaySfxGameplay(SfxCollectionID.GenericMovementSfx, AudioClipID.launcher_woosh, __instance.playerOneShotAudioSource, 0f);
                __instance.DoHighJumpEffects(__instance.motor.groundNormalVisual * -1f);
            }
            else
            {
                __instance.DoJumpEffects(__instance.motor.groundNormalVisual * -1f);
                __instance.motor.SetVelocityYOneTime(num3);
                __instance.isJumping = true;
            }
            __instance.jumpRequested = false;
            __instance.jumpConsumed = true;
            __instance.jumpedThisFrame = true;
            __instance.timeSinceLastJump = 0f;
            if (__instance.ability != null)
            {
                __instance.ability.OnJump();
                if (__instance.ability != null && __instance.onLauncher && __instance.ability.autoAirTrickFromLauncher)
                {
                    __instance.ActivateAbility(__instance.airTrickAbility);
                    return false;
                }
            }
            else if (__instance.onLauncher)
            {
                __instance.ActivateAbility(__instance.airTrickAbility);
            }
            return false;
        }


        [HarmonyPatch(typeof(Player), nameof(Player.OnLanded))]
        [HarmonyPrefix]
        private static bool Player_OnLanded_Prefix(Player __instance)
        {
            __instance.OrientVisualInstant();
            if (__instance.motor.groundRigidbody != null)
            {
                Car component = __instance.motor.groundRigidbody.GetComponent<Car>();
                if (component != null)
                {
                    component.PlayerLandsOn();
                }
            }
            if (__instance.ability == null)
            {
                if (__instance.GetForwardSpeed() <= __instance.minMoveSpeed + 1f)
                {
                    __instance.PlayAnim(__instance.landHash, false, false, -1f);
                }
                else
                {
                    __instance.PlayAnim(__instance.landRunHash, false, false, -1f);
                }
                if (tooFast && __instance.moveStyle == MoveStyle.ON_FOOT)
                {
                    __instance.SetSpeedFlat(__instance.maxMoveSpeed);
                }
                if (__instance.slideButtonHeld && !__instance.slideAbility.locked)
                {
                    __instance.ActivateAbility(__instance.slideAbility);
                }
                else if (!ConfigSettings.ComboGeneral.NoAbilityEnabled.Value)
                {
                    __instance.LandCombo();
                }
            }
            else if (__instance.ability == __instance.boostAbility && !ConfigSettings.ComboGeneral.BoostEnabled.Value)
            {
                __instance.LandCombo();
            }
            __instance.audioManager.PlaySfxGameplay(__instance.moveStyle, AudioClipID.land, __instance.playerOneShotAudioSource, 0f);
            __instance.CreateCircleDustEffect(__instance.motor.groundNormalVisual * -1f);
            return false;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdateAbilities))]
        [HarmonyPrefix]
        private static bool Player_FixedUpdateAbilities_Prefix(Player __instance)
        {
            if (__instance.hitpause > 0f)
            {
                __instance.hitpause -= Core.dt;
                if (__instance.hitpause <= 0f)
                {
                    __instance.StopHitpause();
                    return false;
                }
            }
            else
            {
                bool flag = __instance.IsGrounded();
                __instance.abilityTimer += Core.dt;
                if (flag)
                {
                    __instance.RegainAirMobility();
                }
                __instance.grindAbility.PassiveUpdate();
                if (__instance.isAI)
                {
                    __instance.pseudoGraffitiAbility.PassiveUpdate();
                }
                __instance.wallrunAbility.PassiveUpdate();
                __instance.handplantAbility.PassiveUpdate();
                if (__instance.ability == null)
                {
                    if (!__instance.IsBusyWithSequence())
                    {
                        for (int i = 0; i < __instance.abilities.Count; i++)
                        {
                            if (__instance.abilities[i].CheckActivation())
                            {
                                return false;
                            }
                        }
                    }
                    if (flag && __instance.inWalkZone)
                    {
                        if (__instance.usingEquippedMovestyle)
                        {
                            __instance.ActivateAbility(__instance.switchMoveStyleAbility);
                        }
                    }
                    else if (__instance.switchStyleButtonNew && !__instance.switchToEquippedMovestyleLocked && !flag)
                    {
                        __instance.SwitchToEquippedMovestyle(!__instance.usingEquippedMovestyle, true, true, true);
                    }
                }
                else
                {
                    __instance.ability.FixedUpdateAbility();
                }
                if (__instance.ability == null && flag)
                {
                    if (!__instance.IsBusyWithSequence() && !__instance.motor.wasGrounded && __instance.slideButtonHeld && !__instance.slideAbility.locked)
                    {
                        __instance.ActivateAbility(__instance.slideAbility);
                        return false;
                    }
                    if (__instance.IsComboing() && ConfigSettings.ComboGeneral.NoAbilityEnabled.Value && !__instance.boosting)
                    {
                        float num = Mathf.Min(__instance.GetForwardSpeed() / __instance.boostSpeed, 0.95f);
                        __instance.DoComboTimeOut(Mathf.Max(Core.dt * (1f - num), Core.dt / 2f) * ConfigSettings.ComboGeneral.NoAbilityTimeout.Value);
                        //__instance.DoComboTimeOut(Core.dt * MovementPlusPlugin.noAbilityComboTimeout.Value);
                    }
                    else
                    {
                        __instance.LandCombo();
                    }
                }
            }
            return false;
        }
    }
}
