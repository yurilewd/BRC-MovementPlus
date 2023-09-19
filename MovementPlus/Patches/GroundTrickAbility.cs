using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class GroundTrickAbilityPatch
    {




        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void GroundTrickAbility_OnStartAbility_Postfix(GroundTrickAbility __instance)
        {
            __instance.decc = 0f;
            
            if (__instance.p.moveStyle == MoveStyle.ON_FOOT)
            {
                __instance.allowNormalJump = false;
            }
            else
            {
                __instance.allowNormalJump = true;
            }
        }
    }
}
