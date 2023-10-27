using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class WallrunLineAbilityPatch
    {


        private static float defaultMoveSpeed;
        private static float savedSpeed;

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Init))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Init_Postfix(WallrunLineAbility __instance)
        {
            __instance.minDurationBeforeJump = 0f;
            __instance.wallrunDecc = 0f;
            defaultMoveSpeed = __instance.wallRunMoveSpeed;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static bool WallrunLineAbility_OnStartAbility_Prefix(WallrunLineAbility __instance)
        {
            __instance.wallRunMoveSpeed = Mathf.Max(__instance.p.GetForwardSpeed(), 13f);
            __instance.p.motor.HaveCollision(false);
            //__instance.p.SetVelocity(Vector3.zero);
            __instance.scoreTimer = 0f;
            __instance.wallRunUpDownSpeed = 0f;
            if (Vector3.Dot(__instance.p.tf.right, __instance.wallrunFaceNormal) >= 0f)
            {
                __instance.animSide = Side.LEFT;
            }
            else
            {
                __instance.animSide = Side.RIGHT;
            }
            if (__instance.animSide == Side.RIGHT)
            {
                __instance.p.PlayAnim(__instance.wallRunRightHash, false, false, -1f);
            }
            else
            {
                __instance.p.PlayAnim(__instance.wallRunLeftHash, false, false, -1f);
            }
            if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
            {
                __instance.trickName = "Wallrun";
            }
            else
            {
                __instance.trickName = "Wallride";
            }
            bool flag = __instance.p.UseColliderInCombo(__instance.wallrunLine.GetComponent<Collider>());
            __instance.p.DoTrick(Player.TrickType.WALLRUN, __instance.trickName + (flag ? " (New!)" : ""), 0);
            __instance.betweenLines = false;
            __instance.SetNewNodes(__instance.wallrunLine.node0, __instance.wallrunLine.node1, __instance.wallrunFaceNormal, false);
            __instance.SetHeightLimits(__instance.wallrunLine);
            __instance.speed = Mathf.Max(__instance.p.GetForwardSpeed(), __instance.p.boosting ? __instance.p.boostSpeed : __instance.wallRunMoveSpeed);
            if (__instance.p.boosting)
            {
                if (!__instance.p.isAI)
                {
                    Core.Instance.GameInput.SetVibrationOnCurrentController(0.19f, 0.19f, 0.1f, 0);
                }
                __instance.p.AudioManager.PlaySfxGameplayLooping(SfxCollectionID.GenericMovementSfx, AudioClipID.ultraBoostLoop, __instance.p.playerUltraBoostLoopAudioSource, 0f, 0f);
            }
            __instance.p.AudioManager.PlaySfxGameplay(SfxCollectionID.GenericMovementSfx, AudioClipID.wallrunStart, __instance.p.playerOneShotAudioSource, 0f);
            __instance.p.AudioManager.PlaySfxGameplayLooping(__instance.p.moveStyle, AudioClipID.wallrunLoop, __instance.p.playerMovementLoopAudioSource, 0f, 0f);
            if (__instance.journey > 1f)
            {
                __instance.AtEndOfWallrunLine();
            }
            return false;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.RunOff))]
        [HarmonyPrefix]
        private static bool WallrunLineAbility_RunOff_Prefix(WallrunLineAbility __instance, Vector3 direction)
        {
            Vector3 vector;
            if (__instance.p.abilityTimer <= MovementPlusPlugin.railFrameboostGrace.Value)
            {
                if (MovementPlusPlugin.railFrameboostEnabled.Value && MovementPlusPlugin.wallFrameboostRunoffEnabled.Value)
                {
                    vector = direction * (Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude) + MovementPlusPlugin.wallFrameboostAmount.Value) + __instance.wallrunFaceNormal * 1f;
                    __instance.p.DoTrick(Player.TrickType.WALLRUN, "Frameboost", 0);
                }
                else
                {
                    vector = direction * (Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude)) + __instance.wallrunFaceNormal * 1f;
                }
                __instance.lastSpeed = Mathf.Max(savedSpeed, __instance.lastSpeed);
                savedSpeed = __instance.lastSpeed;
            }  
            else
            {
                vector = direction * (Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude, 13f)) + __instance.wallrunFaceNormal * 1f;
                savedSpeed = __instance.lastSpeed;
            }
            __instance.p.SetVelocity(vector);
            __instance.p.SetRotHard(Quaternion.LookRotation(vector.normalized));
            __instance.p.FlattenRotationHard();
            if (__instance.p.boosting)
            {
                __instance.p.ActivateAbility(__instance.p.boostAbility);
                __instance.p.boostAbility.StartFromRunOffGrindOrWallrun();
                return false;
            }
            __instance.p.StopCurrentAbility();
            __instance.p.PlayAnim(__instance.fallHash, false, false, -1f);
            return false;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_FixedUpdateAbility_Postfix(WallrunLineAbility __instance)
        {
            MovementPlusPlugin.savedLastSpeed = __instance.lastSpeed;
            if (__instance.p.abilityTimer > MovementPlusPlugin.wallFrameboostGrace.Value && MovementPlusPlugin.wallFrameboostEnabled.Value)
            {
                savedSpeed = __instance.lastSpeed;
            }    
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Jump))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Jump_Postfix(WallrunLineAbility __instance)
        {
            if (__instance.p.abilityTimer <= MovementPlusPlugin.wallFrameboostGrace.Value && MovementPlusPlugin.wallFrameboostEnabled.Value)
            {
                __instance.lastSpeed = __instance.lastSpeed + MovementPlusPlugin.wallFrameboostAmount.Value;
                __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed() + MovementPlusPlugin.wallFrameboostAmount.Value);
                __instance.p.DoTrick(Player.TrickType.WALLRUN, "Frameboost", 0);
                __instance.lastSpeed = Mathf.Max(savedSpeed, __instance.lastSpeed);
                savedSpeed = __instance.lastSpeed;
            }
        }


        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_OnStopAbility_Postfix(WallrunLineAbility __instance)
        {
            __instance.cooldownTimer = 0.2f;
        }
    }
}