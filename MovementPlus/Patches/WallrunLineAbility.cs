using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class WallrunLineAbilityPatch
    {


        private static float defaultMoveSpeed;

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Init))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Init_Postfix(WallrunLineAbility __instance)
        {
            __instance.minDurationBeforeJump = 0f;
            __instance.wallrunDecc = 0f;
            defaultMoveSpeed = __instance.wallRunMoveSpeed;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_FixedUpdateAbility_Postfix(WallrunLineAbility __instance)
        {
            //
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Jump))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Jump_Postfix(WallrunLineAbility __instance)
        {
            if (__instance.p.abilityTimer <= MovementPlusPlugin.wallFrameboostGrace.Value && MovementPlusPlugin.wallFrameboostEnabled.Value)
            {
                __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed() + MovementPlusPlugin.wallFrameboostAmount.Value);
                __instance.p.DoTrick(Player.TrickType.WALLRUN, "Frameboost", 0);
            }
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static bool WallrunLineAbility_OnStartAbility_Prefix(WallrunLineAbility __instance)
        {
            __instance.wallRunMoveSpeed = Mathf.Max(__instance.p.GetForwardSpeed(), defaultMoveSpeed);
            return true;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_OnStopAbility_Postfix(WallrunLineAbility __instance)
        {
            __instance.cooldownTimer = 0.2f;
        }
    }
}