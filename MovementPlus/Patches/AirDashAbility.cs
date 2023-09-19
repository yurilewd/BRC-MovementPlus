using HarmonyLib;
using Reptile;
using System.Runtime.Remoting.Messaging;
using Unity;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class AirDashAbilityPatch
    {

        [HarmonyPatch(typeof(AirDashAbility), nameof(AirDashAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static bool AirDashAbility_onStartAbility_Prefix(AirDashAbility __instance)
        {
            if (__instance.p.moveStyle == MoveStyle.SPECIAL_SKATEBOARD)
            {
                __instance.p.ActivateAbility(__instance.p.airTrickAbility);
                Vector3 velocity = __instance.p.GetVelocity();
                velocity.y = 5f;
                __instance.p.SetVelocity(velocity);
                return false;
            }
            __instance.p.ringParticles.Emit(1);
            __instance.airDashSpeed = __instance.airDashStartSpeed;
            Vector3 vector = __instance.p.moveInput;
            if (vector.sqrMagnitude == 0f)
            {
                vector = ((__instance.dirIfNoSteer != null) ? __instance.dirIfNoSteer.Value : __instance.p.dir);
            }
            vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;
            if (__instance.p.smoothRotation)
            {
                __instance.p.SetRotHard(vector);
            }
            else
            {
                __instance.p.SetRotation(vector);
            }
            __instance.p.OrientVisualInstantReset();
            if (__instance.p.GetFlatVelocity().magnitude > __instance.airDashStartSpeed)
            {
                float num = Vector3.Dot(vector, __instance.p.GetFlatVelocity().normalized);
                num = MovementPlusPlugin.remap(num, -1f, 1f, MovementPlusPlugin.airDashSpeed.Value, 1f);
                if (num < 0f)
                {
                    num = 0f;
                }
                __instance.airDashSpeed = __instance.airDashStartSpeed + (__instance.p.GetFlatVelocity().magnitude - __instance.airDashStartSpeed) * num;
            }
            __instance.p.SetVelocity(new Vector3(vector.x * __instance.airDashSpeed, Mathf.Max(__instance.airDashInitialUpSpeed, __instance.p.GetVelocity().y), vector.z * __instance.airDashSpeed));
            __instance.targetSpeed = __instance.airDashSpeed;
            __instance.haveAirDash = false;
            __instance.p.PlayAnim(__instance.airDashHash, true, false, -1f);
            Core.Instance.AudioManager.PlaySfxGameplay(SfxCollectionID.GenericMovementSfx, AudioClipID.airdash, __instance.p.playerOneShotAudioSource, 0f);
            __instance.p.wallrunAbility.cooldownTimer = 0f;
            __instance.p.wallrunAbility.wallrunLine = null;
            return false;
        }

    }
}
