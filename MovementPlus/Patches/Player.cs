using HarmonyLib;
using Reptile;
using System.Runtime.CompilerServices;
using Unity;
using UnityEngine;
using MovementPlus;

namespace MovementPlus.Patches
{
    internal static class PlayerPatch
    {

        private static float defaultBoostSpeed;
        private static float defaultVertMaxSpeed;
        private static float defaultVertTopJumpSpeed;

        private static bool canFastFall;



        [HarmonyPatch(typeof(Player), nameof(Player.Init))]
        [HarmonyPostfix]
        private static void Player_Init_Postfix(Player __instance)
        {
            defaultBoostSpeed = __instance.normalBoostSpeed;
            defaultVertMaxSpeed = __instance.vertMaxSpeed;
            defaultVertTopJumpSpeed = __instance.vertTopJumpSpeed;
            __instance.motor.maxFallSpeed = MovementPlusPlugin.maxFallSpeed.Value;
            MovementPlusPlugin.player = __instance;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdatePlayer))]
        [HarmonyPostfix]
        private static void Player_FixedUpdatePlayer_Postfix(Player __instance)
        {

            if (__instance.slideButtonNew && !__instance.TreatPlayerAsSortaGrounded() && __instance.motor.velocity.y <= 0f && canFastFall && MovementPlusPlugin.fastFallEnabled.Value)
            {
                __instance.motor.SetVelocityYOneTime(Mathf.Min(__instance.motor.velocity.y + MovementPlusPlugin.fastFallAmount.Value, MovementPlusPlugin.fastFallAmount.Value));
                __instance.ringParticles.Emit(1);
                __instance.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, __instance.playerOneShotAudioSource, 0f);
                canFastFall = false;
            }

            if (__instance.TreatPlayerAsSortaGrounded())
            {
                canFastFall = true;
            }    

            if (__instance.ability == __instance.grindAbility)
            {
                if (__instance.grindAbility.posOnLine <= 0.1 && __instance.abilityTimer <= 0.1f && __instance.boostButtonHeld)
                {
                    if (!MovementPlusPlugin.railGoonEnabled.Value)
                    {
                        __instance.normalBoostSpeed = defaultBoostSpeed;
                    }    
                    else
                    {
                        __instance.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, __instance.GetForwardSpeed() * MovementPlusPlugin.railGoonStrength.Value);
                    }
                }    
                else
                {
                    __instance.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, __instance.GetForwardSpeed());
                }    
            }

            else if (__instance.ability == __instance.wallrunAbility)
            {
                return;
            }

            else if (__instance.ability != __instance.boostAbility)
            {
                __instance.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, __instance.GetForwardSpeed() + 0.2f);
            }
            
            float x = __instance.GetTotalSpeed();

            if (MovementPlusPlugin.vertEnabled.Value)
            {
                __instance.vertMaxSpeed = Mathf.Max(defaultVertMaxSpeed, x);
            }    
            if (MovementPlusPlugin.vertJumpEnabled.Value)
            {
                __instance.vertTopJumpSpeed = Mathf.Max(defaultVertTopJumpSpeed, x * MovementPlusPlugin.vertJumpStrength.Value);
            }    
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
    }    
}
