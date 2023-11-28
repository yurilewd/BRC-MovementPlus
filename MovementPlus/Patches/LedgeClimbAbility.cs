using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class LedgeClimbAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        private static float savedSpeed;
        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_OnStartAbility_Prefix()
        {
            savedSpeed = MovementPlusPlugin.AverageTotalSpeed();
        }

        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_FixedUpdateAbility_Prefix(LedgeClimbAbility __instance)
        {
            if (__instance.p.jumpButtonNew && ConfigSettings.LedgeClimbGeneral.Enabled.Value)
            {
                float newSpeed = MovementPlusPlugin.LosslessClamp(savedSpeed, ConfigSettings.LedgeClimbGeneral.Amount.Value, ConfigSettings.LedgeClimbGeneral.Cap.Value);
                __instance.p.SetVelocity(__instance.p.dir * (newSpeed));
                __instance.p.ringParticles.Emit(1);
                __instance.p.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, __instance.p.playerOneShotAudioSource, 0f);
                __instance.p.Jump();
                __instance.p.StopCurrentAbility();
            }
        }
    }
}
