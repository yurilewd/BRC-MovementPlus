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
            if (MovementPlusPlugin.superTrickEnabled.Value)
            {
                var speedmath = MovementPlusPlugin.TableCurve(9f, 32f, -1.4f, __instance.p.GetForwardSpeed()) * MovementPlusPlugin.superTrickStrength.Value;

                __instance.jumpSpeed = (Mathf.Max(11.5f, speedmath));

                __instance.duration = 0.3f;
            }    
        }
    }
}
