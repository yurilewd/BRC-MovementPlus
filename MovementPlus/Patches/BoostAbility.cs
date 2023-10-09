using HarmonyLib;
using Reptile;
using System;
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
            if (__instance.p.IsGrounded() && __instance.p.IsComboing())
            {
                __instance.p.DoComboTimeOut(Core.dt * MovementPlusPlugin.boostComboTimeout.Value);
            }

            if (MovementPlusPlugin.boostChangeEnabled.Value)
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

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnJump))]
        [HarmonyPrefix]
        private static void BoostAbility_OnJump_PreFix(BoostAbility __instance)
        {
            if (__instance.p.IsComboing())
            {
                __instance.p.DoComboTimeOut(MovementPlusPlugin.boostComboJumpAmount.Value);
            }
        }
    }
}
