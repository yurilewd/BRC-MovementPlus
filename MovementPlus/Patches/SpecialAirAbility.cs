using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class SpecialAirAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;



        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void SpecialAirAbility_OnStartAbility_Prefix(SpecialAirAbility __instance)
        {
            if (ConfigSettings.SuperTrickJump.Enabled.Value)
            {
                var num = Mathf.Max(__instance.p.GetForwardSpeed() - ConfigSettings.SuperTrickJump.Threshold.Value, 0f);

                __instance.jumpSpeed = MovementPlusPlugin.LosslessClamp(__instance.jumpSpeed, num * ConfigSettings.SuperTrickJump.Amount.Value, ConfigSettings.SuperTrickJump.Cap.Value);
                __instance.duration = 0.3f;

            }
        }

        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void SpecialAirAbility_OnStopAbility_Prefix(SpecialAirAbility __instance)
        {
            __instance.jumpSpeed = MovementPlusPlugin.defaultJumpSpeed;
        }
    }
}
