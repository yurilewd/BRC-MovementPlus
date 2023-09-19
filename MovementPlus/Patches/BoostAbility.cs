using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class BoostAbilityPatch
    {

        private static float defaultBoostSpeed;


        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.Init))]
        [HarmonyPostfix]
        private static void BoostAbility_Init_Postfix(BoostAbility __instance)
        {
            __instance.decc = 0f;
            defaultBoostSpeed = __instance.p.normalBoostSpeed;
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_FixedUpdateAbility_Postfix(BoostAbility __instance)
        {

            if (__instance.state == BoostAbility.State.START_BOOST)
            {
                if (__instance.p.ability == __instance.p.grindAbility)
                {
                    __instance.p.normalBoostSpeed = Mathf.Max(defaultBoostSpeed + 5f, __instance.p.GetForwardSpeed() + 5f);
                }    
                else
                {
                    __instance.p.normalBoostSpeed = Mathf.Max(defaultBoostSpeed + 5f, __instance.p.GetTotalSpeed() + 5f);
                }
                
            }
            else
            {
                __instance.p.normalBoostSpeed = __instance.p.GetTotalSpeed() + 0.2f;
            }
        }


    }
}
