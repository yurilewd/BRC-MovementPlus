using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class BoostAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        private static float defaultBoostSpeed;


        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.Init))]
        [HarmonyPostfix]
        private static void BoostAbility_Init_Postfix(BoostAbility __instance)
        {
            if (!__instance.p.isAI)
            {
                __instance.decc = 0f;
                defaultBoostSpeed = __instance.p.normalBoostSpeed;
            }
        }


        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_FixedUpdateAbility_Postfix(BoostAbility __instance)
        {
            if (__instance.p.IsGrounded() && __instance.p.IsComboing() && ConfigSettings.ComboGeneral.BoostEnabled.Value)
            {
                __instance.p.DoComboTimeOut((Core.dt / 2f) * ConfigSettings.ComboGeneral.BoostTimeout.Value);
            }

            if (ConfigSettings.BoostGeneral.StartEnabled.Value)
            {
                if (__instance.state == BoostAbility.State.START_BOOST)
                {
                    float speed = (__instance.p.ability == __instance.p.grindAbility) ? MovementPlusPlugin.AverageForwardSpeed() : MovementPlusPlugin.AverageTotalSpeed();
                    float highestSpeed = Mathf.Max(defaultBoostSpeed, speed);
                    float newSpeed = MovementPlusPlugin.LosslessClamp(highestSpeed, ConfigSettings.BoostGeneral.StartAmount.Value, ConfigSettings.BoostGeneral.StartCap.Value);
                    __instance.p.normalBoostSpeed = newSpeed;
                    return;
                }
            }
            if (ConfigSettings.BoostGeneral.TotalSpeedEnabled.Value)
            {
                float newSpeed = MovementPlusPlugin.LosslessClamp(MovementPlusPlugin.AverageForwardSpeed(), MovementPlusPlugin.AverageTotalSpeed() - MovementPlusPlugin.AverageForwardSpeed(), ConfigSettings.BoostGeneral.TotalSpeedCap.Value);
                __instance.p.normalBoostSpeed = newSpeed;
            }
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnJump))]
        [HarmonyPrefix]
        private static void BoostAbility_OnJump_PreFix(BoostAbility __instance)
        {
            if (__instance.p.IsComboing())
            {
                __instance.p.DoComboTimeOut(ConfigSettings.ComboGeneral.BoostJumpAmount.Value);
            }
        }
    }
}
