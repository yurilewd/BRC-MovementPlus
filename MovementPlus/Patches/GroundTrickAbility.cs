using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class GroundTrickAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void GroundTrickAbility_OnStartAbility_Postfix(GroundTrickAbility __instance)
        {
            __instance.decc = ConfigSettings.Misc.groundTrickDecc.Value;

            if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
            {
                __instance.allowNormalJump = false;
            }
            else
            {
                __instance.allowNormalJump = true;
            }
        }

        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static void GroundTrickAbility_FixedUpdateAbility_Prefix(GroundTrickAbility __instance)
        {
            if (!__instance.p.TreatPlayerAsSortaGrounded() && __instance.p.jumpButtonNew && ConfigSettings.Buttslap.Enabled.Value)
            {
                //
            }
        }
    }
}
