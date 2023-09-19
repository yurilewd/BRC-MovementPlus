﻿using HarmonyLib;
using Reptile;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class SlideAbilityPatch
    {
        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_FixedUpdateAbility_Postfix(SlideAbility __instance)
        {
            __instance.superSpeed = Mathf.Max(MovementPlusPlugin.superSlideSpeed.Value, __instance.p.GetForwardSpeed() + MovementPlusPlugin.superSlideIncrease.Value);
            __instance.slopeSlideSpeed = Mathf.Max(MovementPlusPlugin.superSlideSpeed.Value, __instance.p.GetForwardSpeed() + MovementPlusPlugin.superSlideIncrease.Value);
        }
    }
}
