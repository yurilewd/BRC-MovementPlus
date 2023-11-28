using HarmonyLib;
using Reptile;
using System;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class HandplantAbilityPatch
    {
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        [HarmonyPatch(typeof(HandplantAbility), nameof(HandplantAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static bool HandplantAbility_FixedUpdateAbility_Prefix(HandplantAbility __instance)
        {
            if (__instance.p.abilityTimer < 0.35f)
            {
                __instance.p.hitbox.SetActive(true);
            }
            else
            {
                __instance.p.hitbox.SetActive(false);
            }
            if (__instance.screwPoleMode)
            {
                float num = 0f;
                if (__instance.p.transform.position.y + Mathf.Epsilon < __instance.screwPole.maxPoint.position.y)
                {
                    int num2 = 6;
                    float num3 = 1f;
                    if (__instance.p.AnyTrickInput())
                    {
                        if ((double)__instance.screwSpinTimer < 0.25)
                        {
                            __instance.screwSpinTimer = 0.5f;
                        }
                        if (__instance.screwFastSpinTimer > 0f)
                        {
                            __instance.screwFastPressCount++;
                            num = ((__instance.screwFastPressCount > num2) ? num3 : 0f);
                        }
                        else
                        {
                            __instance.screwFastPressCount = 0;
                        }
                        __instance.screwFastSpinTimer = 0.25f;
                    }
                    else if (__instance.screwFastSpinTimer > 0f)
                    {
                        __instance.screwFastSpinTimer -= Core.dt;
                        num = Mathf.Lerp(0f, num3, (float)Convert.ToInt32(__instance.screwFastPressCount > num2) * __instance.screwFastSpinTimer * 4f);
                    }
                }
                float num4;
                if (__instance.screwSpinTimer > 0f)
                {
                    __instance.screwSpinTimer -= Core.dt;
                    num4 = Mathf.Min(__instance.customVelocity.y + __instance.screwAcc * Core.dt, __instance.maxScrewSpeed + num);
                }
                else
                {
                    num4 = Mathf.Max(__instance.customVelocity.y - __instance.screwDecc * Core.dt, 0f);
                }
                if (__instance.p.transform.position.y + Mathf.Epsilon < __instance.screwPole.maxPoint.position.y)
                {
                    __instance.customVelocity.y = num4;
                    __instance.screwPole.Move(num4);
                    __instance.p.rotLean = Mathf.Min(num4, __instance.screwSpinTimer * 2f);
                }
                else
                {
                    if (__instance.customVelocity.y != 0f)
                    {
                        __instance.screwPole.StopPlayingSound();
                        __instance.p.AudioManager.PlaySfxGameplay(__instance.p.moveStyle, AudioClipID.ScrewPolePlant, __instance.p.playerOneShotAudioSource, 0f);
                    }
                    __instance.customVelocity.y = 0f;
                    __instance.p.rotLean = __instance.screwSpinTimer * 2f;
                }
            }
            if (/*__instance.p.abilityTimer > 0.12f && */(__instance.p.jumpButtonNew || __instance.p.isAI))
            {
                float num5 = __instance.p.maxMoveSpeed;
                if (__instance.screwPoleMode)
                {
                    num5 = __instance.p.maxMoveSpeed / 2f;
                }
                Vector3 vector;
                if (__instance.p.isAI)
                {
                    __instance.p.AI.HandplantJump();
                    if (__instance.p.AI.nextWaypoint != null)
                    {
                        vector = __instance.p.AI.dirToWaypoint;
                        vector.y = 0f;
                        vector = vector.normalized;
                        __instance.p.SetRotHard(Vector3.ProjectOnPlane(vector, Vector3.up));
                        vector *= num5;
                    }
                    else
                    {
                        vector = __instance.p.dir * num5;
                    }
                }
                else
                {
                    vector = __instance.p.cam.realTf.forward;
                    vector.y = 0f;
                    vector = vector.normalized;
                    __instance.p.SetRotHard(Vector3.ProjectOnPlane(vector, Vector3.up));
                    vector *= num5;
                }
                __instance.p.AudioManager.PlayVoice(ref __instance.p.currentVoicePriority, __instance.p.character, AudioClipID.VoiceJump, __instance.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
                vector.y = __instance.p.jumpSpeed + __instance.p.bonusJumpSpeedHandplantJump;
                __instance.p.SetVelocity(vector);
                __instance.p.CreateHighJumpDustEffect(Vector3.up);
                __instance.p.PlayAnim(Animator.StringToHash("jump"), false, false, -1f);
                if (__instance.p.abilityTimer <= 0.12f + ConfigSettings.RailFrameboost.Grace.Value)
                {
                    __instance.p.SetForwardSpeed(Mathf.Max(MovementPlusPlugin.noAbilitySpeed, __instance.p.GetForwardSpeed()));
                }
                __instance.p.StopCurrentAbility();
            }
            return false;
        }
    }
}