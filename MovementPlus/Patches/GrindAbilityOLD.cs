using HarmonyLib;
using Reptile;
using System.Runtime.CompilerServices;
using System;
using Unity;
using UnityEngine;
using System.Reflection;
using BepInEx;

namespace MovementPlus.Patches
{
    public class GrindAbilityPatch
    {


        public static float defaultCornerBoost;
        public static float minCornerBoost = 3f;
        public static float maxCornerSpeed = 11f;




        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.Init))]
        [HarmonyPostfix]
        private static void GrindAbility_Init_Postfix(GrindAbility __instance)
        {
            __instance.grindDeccAboveNormal = 0f;
            __instance.grindDeccBelowNormal = 0f;
            defaultCornerBoost = __instance.cornerBoost;
            
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void GrindAbility_OnStartAbility_Postfix(GrindAbility __instance)
        {
            __instance.reTrickFail = false;
            __instance.trickTimer = 0f;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void GrindAbility_FixedUpdateAbility_Postfix(GrindAbility __instance)
        {
            if (__instance.p.jumpButtonNew)
            {
                if (__instance.p.abilityTimer <= MovementPlusPlugin.railFrameBoostGrace.Value)
                {
                    if (__instance.grindLine.isPole)
                    {
                        __instance.JumpOut(true);
                        __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed() + MovementPlusPlugin.railFrameBoostAmount.Value);
                        __instance.p.DoTrick(Player.TrickType.AIR, "Frameboost", 0);
                        //return;
                    }
                    else
                    {
                        if (Mathf.Abs(Vector3.Dot(__instance.p.tf.up, Vector3.up)) < 0.35f || __instance.grindLine.alwaysFlipBack)
                        {
                            __instance.JumpOut(true);
                            __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed() + MovementPlusPlugin.railFrameBoostAmount.Value);
                            __instance.p.DoTrick(Player.TrickType.AIR, "Frameboost", 0);
                            //return;
                        }
                        __instance.JumpOut(false);
                        __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed() + MovementPlusPlugin.railFrameBoostAmount.Value);
                        __instance.p.DoTrick(Player.TrickType.AIR, "Frameboost", 0);
                        //return;
                    }
                }    
                else
                {
                    if (__instance.grindLine.isPole)
                    {
                        __instance.JumpOut(true);
                        //return;
                    }
                    else
                    {
                        if (Mathf.Abs(Vector3.Dot(__instance.p.tf.up, Vector3.up)) < 0.35f || __instance.grindLine.alwaysFlipBack)
                        {
                            __instance.JumpOut(true);
                            //return;
                        }
                        __instance.JumpOut(false);
                        //return;
                    }
                }
            }
            //return;
        }


        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.RewardTilting))]
        [HarmonyPrefix]
        public static bool RewardTilting_Prefix(Vector3 rightDir, Vector3 nextLineDir, GrindAbility __instance)
        {
            if (!__instance.grindLine.cornerBoost)
            {
                return false;
            }
            float num = Vector3.Dot(rightDir, Vector3.ProjectOnPlane(nextLineDir, Vector3.up).normalized);
            Side side = Side.NONE;
            if (__instance.grindTiltBuffer.x < -0.25f)
            {
                side = Side.LEFT;
            }
            else if (__instance.grindTiltBuffer.x > 0.25f)
            {
                side = Side.RIGHT;
            }
            Side side2 = Side.NONE;
            if (num < -0.02f)
            {
                side2 = Side.LEFT;
            }
            else if (num > 0.02f)
            {
                side2 = Side.RIGHT;
            }
            if (side2 != Side.NONE)
            {
                __instance.softCornerBoost = false;
            }
            bool flag = Mathf.Abs(num) > 0.3f;
            if (side != Side.NONE && side == side2)
            {
                if (flag && __instance.lastPath.hardCornerBoostsAllowed)
                {


                    __instance.p.StartScreenShake(ScreenShakeType.LIGHT, 0.2f, false);
                    __instance.p.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, __instance.p.playerOneShotAudioSource, 0f);
                    __instance.p.ringParticles.Emit(1);


                    __instance.speed = Mathf.Max(__instance.p.stats.grindSpeed + __instance.cornerBoost, __instance.p.GetForwardSpeed());
                    __instance.p.HardCornerGrindLine(__instance.nextNode);
                    return false;
                }
                if (__instance.lastPath.softCornerBoostsAllowed)
                {
                    __instance.softCornerBoost = false;
                    __instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed());
                    __instance.p.DoTrick(Player.TrickType.SOFT_CORNER, "Corner", 0);
                }
            }
            return false;
        }
    }
}
