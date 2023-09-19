using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class SpecialAirAbilityPatch
    {

        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void SpecialAirAbility_OnStartAbility_Prefix(SpecialAirAbility __instance)
        {
            float a = 9f;
            float b = 32f;
            float c = -1.4f;
            float x = __instance.p.GetForwardSpeed();

            var speedmath = (((a + b) * x) / (b + x) + c) * MovementPlusPlugin.superJumpStrength.Value;

            __instance.jumpSpeed = (Mathf.Max(11.5f, speedmath));

            __instance.duration = 0.3f;
        }
    }
}
