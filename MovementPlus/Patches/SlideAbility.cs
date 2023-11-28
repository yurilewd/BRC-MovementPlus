using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class SlideAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_FixedUpdateAbility_Postfix(SlideAbility __instance)
        {
            float speed = Mathf.Max(ConfigSettings.SuperSlide.Speed.Value, __instance.p.GetForwardSpeed());
            if (ConfigSettings.SuperSlide.Enabled.Value)
            {
                speed = MovementPlusPlugin.LosslessClamp(speed, ConfigSettings.SuperSlide.Amount.Value, ConfigSettings.SuperSlide.Cap.Value);
            }
            __instance.superSpeed = speed;
            __instance.slopeSlideSpeed = speed;
        }
    }
}
