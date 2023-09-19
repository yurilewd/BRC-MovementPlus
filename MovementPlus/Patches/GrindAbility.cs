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
    public class GrindAbilityPatchNEW
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
        [HarmonyPrefix]
        public static bool GrindAbility_FixedUpdateAbility_Prefix(GrindAbility __instance)
        {
            if (__instance.inRetour)
            {
                if (__instance.p.GetAnimTime() < 1f)
                {
                    return false;
                }
                __instance.EndRetour();
            }
            int curAnim = __instance.p.curAnim;
            bool flag = false;
            int num = 3;
            for (int i = 0; i < num; i++)
            {
                flag |= (curAnim == __instance.grindTrickHoldHashes[i] || curAnim == __instance.grindBoostTrickHoldHashes[i]);
            }
            __instance.timeSinceLastNode += Core.dt;
            __instance.p.SetDustEmission(0);
            __instance.UpdateSpeed();
            __instance.UpdateTricks();
            Vector3 vector = __instance.p.tf.position;
            Vector3 vector2 = __instance.p.tf.forward;
            Vector3 right = __instance.p.tf.right;
            if (!__instance.justFollowNodes)
            {
                __instance.nextNode = __instance.grindLine.GetNextNode(vector2);
            }
            vector = __instance.grindLine.SnapPosToLine(vector);
            vector += (__instance.nextNode - vector).normalized * __instance.speed * Core.dt;
            __instance.posOnLine = (__instance.justFollowNodes ? __instance.grindLine.GetRelativePosOnLine(vector, __instance.nextNode) : __instance.grindLine.GetRelativePosOnLine(vector, vector2));
            float num2 = __instance.justFollowNodes ? __instance.grindLine.GetAbsoluteLinePos(vector, __instance.nextNode) : __instance.grindLine.GetAbsoluteLinePos(vector, vector2);
            if (__instance.posOnLine >= 1f)
            {
                __instance.timeSinceLastNode = 0f;
                float d = __instance.posOnLine * __instance.grindLine.Length() - __instance.grindLine.Length();
                if (__instance.nextNode.IsEndpoint)
                {
                    if (__instance.nextNode.retour)
                    {
                        __instance.StartRetour();
                        return false;
                    }
                    if (__instance.grindLine.isPole)
                    {
                        if (Mathf.Abs(Vector3.Dot(vector2, Vector3.up)) > 0f)
                        {
                            __instance.p.handplantAbility.SetToPole(__instance.nextNode.position);
                            return false;
                        }
                        __instance.JumpOut(true);
                        return false;
                    }
                    else
                    {
                        if (Mathf.Abs(Vector3.Dot(__instance.p.tf.up, Vector3.up)) < 0.35f || __instance.grindLine.alwaysFlipBack)
                        {
                            __instance.JumpOut(true);
                            return false;
                        }
                        __instance.p.motor.SetPositionTeleport(vector + Vector3.up * __instance.p.motor.GetCapsule().height * Mathf.Clamp(__instance.p.tf.up.y, -1f, 0f));
                        __instance.normal = Vector3.up;
                        Vector3 a = __instance.p.FlattenRotationHard();
                        __instance.p.SetVelocity(a * __instance.speed);
                        if (__instance.p.boosting)
                        {
                            __instance.p.ActivateAbility(__instance.p.boostAbility);
                            __instance.p.boostAbility.StartFromRunOffGrindOrWallrun();
                            return false;
                        }
                        __instance.p.StopCurrentAbility();
                        __instance.p.FlattenRotationHard();
                        return false;
                    }
                }
                else
                {
                    GrindLine grindLine = __instance.justFollowNodes ? __instance.grindLine.GetNextLine(vector2, __instance.nextNode) : __instance.grindLine.GetNextLine(vector2, null);
                    Vector3 normalized = (grindLine.GetOtherNode(__instance.nextNode) - __instance.nextNode).normalized;
                    __instance.RewardTilting(right, normalized);
                    vector2 = normalized;
                    vector = __instance.nextNode.position + d * normalized;
                    if (__instance.grindLine.sfxCollection != grindLine.sfxCollection && grindLine.sfxCollection != SfxCollectionID.NONE)
                    {
                        __instance.p.AudioManager.PlaySfxGameplayLooping(grindLine.sfxCollection, AudioClipID.grindLoop, __instance.p.playerGrindLoopAudioSource, 0f, 0f);
                    }
                    __instance.grindLine = grindLine;
                    __instance.nextNode = (__instance.justFollowNodes ? __instance.grindLine.GetOtherNode(__instance.nextNode) : __instance.grindLine.GetNextNode(normalized));
                    __instance.posOnLine = (__instance.justFollowNodes ? __instance.grindLine.GetRelativePosOnLine(vector, __instance.nextNode) : __instance.grindLine.GetRelativePosOnLine(vector, normalized));
                    num2 = (__instance.justFollowNodes ? __instance.grindLine.GetAbsoluteLinePos(vector, __instance.nextNode) : __instance.grindLine.GetAbsoluteLinePos(vector, vector2));
                    __instance.normal = __instance.grindLine.GetNormalAtPos(vector);
                    __instance.p.SetRotation(normalized, __instance.normal);
                }
            }
            __instance.UpdateBoostpack();
            __instance.UpdateTilting();
            if (!__instance.grindLine.isPole)
            {
                __instance.normal = __instance.grindLine.GetNormalAtPos(vector);
            }
            float num3 = __instance.grindLine.Length();
            float num4 = Mathf.Min(num3 * 0.5f, 1.5f);
            Vector3 forward = Vector3.forward;
            if (!__instance.nextNode.IsEndpoint && num2 >= num3 - num4)
            {
                Vector3 normalized2 = ((__instance.justFollowNodes ? __instance.grindLine.GetNextLine(vector2, __instance.nextNode) : __instance.grindLine.GetNextLine(vector2, null)).GetOtherNode(__instance.nextNode) - __instance.nextNode).normalized;
                Vector3 normalized3 = (__instance.nextNode - vector).normalized;
                Vector3 vector3 = Vector3.Slerp(normalized3, normalized2, 0.5f);
                if (num2 >= num3)
                {
                    forward = vector3;
                }
                else
                {
                    forward = Vector3.Slerp(normalized3, vector3, 1f - (num3 - num2) / num4);
                }
                __instance.preGrindDir = normalized3;
                if (!__instance.p.smoothRotation)
                {
                    __instance.p.SetRotation(forward.normalized, __instance.normal);
                }
            }
            else if (__instance.preGrindDir != Vector3.zero && num2 <= num4)
            {
                Vector3 normalized4 = (__instance.nextNode - vector).normalized;
                Vector3 vector4 = Vector3.Slerp(normalized4, __instance.preGrindDir, 0.5f);
                if (num2 <= 0f)
                {
                    forward = vector4;
                }
                else
                {
                    forward = Vector3.Slerp(vector4, normalized4, num2 / num4);
                }
                if (!__instance.p.smoothRotation)
                {
                    __instance.p.SetRotation(forward.normalized, __instance.normal);
                }
            }
            else if (__instance.p.smoothRotation)
            {
                forward = __instance.nextNode - vector;
                __instance.preGrindDir = forward;
            }
            else
            {
                __instance.p.SetRotHard(Quaternion.LookRotation((__instance.nextNode - vector).normalized, __instance.normal));
            }
            if (__instance.p.smoothRotation)
            {
                Quaternion rotation = Quaternion.LookRotation(forward, __instance.normal);
                __instance.p.SetRotation(rotation);
            }
            __instance.customVelocity = (vector - __instance.p.tf.position) * 60f;
            __instance.p.lastElevationForSlideBoost = vector.y;
            if (__instance.p.jumpButtonNew /*&& (__instance.p.isAI || __instance.trickTimer < __instance.reTrickMargin)*/)
            {
                
                float num5 = Vector3.Dot(__instance.p.tf.up, Vector3.up);
                __instance.JumpOut(__instance.grindLine.isPole || (num5 > -0.25f && num5 < 0.35f));
                return false;
            }
            __instance.p.SetVisualRotLocalZ(__instance.grindTilt.x * -30f);
            return false;
        }


        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.JumpOut))]
        [HarmonyPrefix]
        public static bool GrindAbility_JumpOut_Prefix(bool flipOut, GrindAbility __instance)
        {
            __instance.p.AudioManager.PlayVoice(ref __instance.p.currentVoicePriority, __instance.p.character, AudioClipID.VoiceJump, __instance.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
            __instance.p.timeSinceLastJump = 0f;
            __instance.p.isJumping = true;
            __instance.p.jumpConsumed = true;
            __instance.p.jumpRequested = false;
            __instance.p.jumpedThisFrame = true;
            __instance.p.maintainSpeedJump = false;
            __instance.p.lastElevationForSlideBoost = float.PositiveInfinity;
            Vector3 up = __instance.p.tf.up;
            __instance.p.DoJumpEffects(up);
            __instance.p.tf.position += Vector3.up * __instance.p.motor.GetCapsule().height * Mathf.Clamp(__instance.p.tf.up.y, -1f, 0f);
            if (flipOut)
            {
                Vector3 normalized = Vector3.ProjectOnPlane(up, Vector3.up).normalized;
                __instance.p.SetRotHard(Quaternion.LookRotation(normalized));
                float num = __instance.p.jumpSpeed * 0.35f;
                float d = Mathf.Max(__instance.p.maxMoveSpeed * 0.7f, __instance.p.GetTotalSpeed() * 0.7f);
                __instance.p.SetVelocity(num * Vector3.up + normalized * d);
                __instance.p.ActivateAbility(__instance.p.flipOutJumpAbility);
            }
            else
            {
                __instance.p.PlayAnim(__instance.jumpHash, false, false, -1f);
                Vector3 a = __instance.p.FlattenRotationHard();
                float num2 = 1f + Mathf.Clamp01(__instance.p.dir.y) * 0.5f;
                if (!__instance.lastPath.upwardsGrindJumpAllowed || !__instance.grindLine.upwardsGrindJump)
                {
                    num2 = 1f;
                }
                float num3 = __instance.p.jumpSpeed + __instance.p.bonusJumpSpeedGrind;
                float num = (Vector3.Dot(up, Vector3.up) > -0.1f) ? (num3 * num2) : (-__instance.p.jumpSpeed * 0.5f);
                float d = Mathf.Min(__instance.speed, __instance.p.boostSpeed);
                if (__instance.p.abilityTimer <= MovementPlusPlugin.railFrameBoostGrace.Value)
                {
                    d = Mathf.Max(__instance.speed, __instance.p.GetForwardSpeed() + MovementPlusPlugin.railFrameBoostAmount.Value);
                    __instance.p.DoTrick(Player.TrickType.AIR, "Frameboost", 0);
                }    
                if (__instance.p.boosting)
                {
                    __instance.p.ActivateAbility(__instance.p.boostAbility);
                    __instance.p.boostAbility.StartFromJumpGrindOrWallrun();
                }
                else if (__instance.p.slideButtonHeld)
                {
                    num *= __instance.p.abilityShorthopFactor;
                    __instance.p.StopCurrentAbility();
                    __instance.p.maintainSpeedJump = true;
                }
                else
                {
                    __instance.p.StopCurrentAbility();
                }
                __instance.p.SetVelocity(num * Vector3.up + a * d);
            }
            __instance.p.ForceUnground(true);
            return false;
        }


        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.RewardTilting))]
        [HarmonyPrefix]
        public static bool GrindAbility_RewardTilting_Prefix(Vector3 rightDir, Vector3 nextLineDir, GrindAbility __instance)
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
                    __instance.speed = Mathf.Max(__instance.p.stats.grindSpeed + __instance.cornerBoost, __instance.p.GetForwardSpeed() + 2f);
                    __instance.p.HardCornerGrindLine(__instance.nextNode);
                    return false;
                }
                if (__instance.lastPath.softCornerBoostsAllowed)
                {
                    __instance.softCornerBoost = false;
                    //__instance.p.SetForwardSpeed(__instance.p.GetForwardSpeed());
                    //__instance.p.DoTrick(Player.TrickType.SOFT_CORNER, "Corner", 0);
                }
            }
            return false;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.UpdateSpeed))]
        [HarmonyPrefix]
        public static bool GrindAbility_UpdateSpeed_Prefix(GrindAbility __instance)
        {
            if (__instance.speed < __instance.speedTarget)
            {
                __instance.speed = Mathf.Min(__instance.speedTarget, __instance.speed + __instance.grindAcc * Core.dt);
            }
            else if (__instance.speed > __instance.speedTarget)
            {
                __instance.speed = Mathf.Max(__instance.speedTarget, __instance.speed - ((__instance.speed >= __instance.p.stats.grindSpeed) ? __instance.grindDeccAboveNormal : __instance.grindDeccBelowNormal) * Core.dt);
            }
            if (__instance.p.boosting)
            {
                if (__instance.speed <= __instance.p.boostSpeed)
                {
                    __instance.speed = (__instance.speedTarget = __instance.p.boostSpeed);
                }
                //__instance.speed = (__instance.speedTarget = Mathf.Max(__instance.p.boostSpeed, __instance.p.GetForwardSpeed()));
                return false;
            }
            if (__instance.softCornerBoost)
            {
                //__instance.speedTarget = Mathf.Max(__instance.p.boostSpeed, __instance.p.GetTotalSpeed());
                if (__instance.timeSinceLastNode > 1f)
                {
                    __instance.softCornerBoost = false;
                    return false;
                }
            }
            else
            {
                if (__instance.p.AirBraking() && !__instance.p.isAI)
                {
                    __instance.braking = true;
                    __instance.speedTarget = __instance.p.stats.grindSpeed * 0.35f;
                    return false;
                }
                __instance.braking = false;
                __instance.speedTarget = Mathf.Max(__instance.p.stats.grindSpeed, __instance.speed);
            }
            return false;
        }


    }
}
