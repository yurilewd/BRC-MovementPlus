using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MovementPlus.Patches;
using Reptile;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace MovementPlus
{

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class MovementPlusPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.yuril.MovementPlus";
        private const string PluginName = "MovementPlus";
        private const string VersionString = "1.0.0";

        private Harmony harmony;
        public static Player player;

        public static ConfigEntry<float> railGoonStrength;
        public static ConfigEntry<float> railFrameBoostGrace;
        public static ConfigEntry<float> railFrameBoostAmount;

        public static ConfigEntry<float> wallFrameBoostGrace;
        public static ConfigEntry<float> wallFrameBoostAmount;

        public static ConfigEntry<float> superJumpStrength;

        public static ConfigEntry<float> pSlideGrace;
        public static ConfigEntry<float> pSlideAmount;
        public static ConfigEntry<float> pSlideCap;

        public static ConfigEntry<float> superSlideCap;
        public static ConfigEntry<float> superSlideIncreaseOverTime;

        public static ConfigEntry<float> fastFallAmount;

        public static ConfigEntry<float> maxFallSpeed;

        public static ConfigEntry<float> airDashRetainedSpeed;

        private bool slideBoost = false;
        private float slideTimer = 0f;
        private bool slideTimerStarted = false;
        


        private void Awake()
        {
            railGoonStrength = Config.Bind("Rail", "Rail Goon Strength", 0.9f);

            railFrameBoostGrace = Config.Bind("Rail", "Rail Frameboost Grace Period", 0.1f);
            railFrameBoostAmount = Config.Bind("Rail", "Rail Frameboost Amount", 5f);

            wallFrameBoostGrace = Config.Bind("Wall", "Wallride Frameboost Grace Period", 0.1f);
            wallFrameBoostAmount = Config.Bind("Wall", "Wallride Frameboost Amount", 11f);

            superJumpStrength = Config.Bind("General", "Super Trick Jump Strength", 0.9f);

            pSlideGrace = Config.Bind("General", "Perfect Slide Grace Period", 0.1f);
            pSlideAmount = Config.Bind("General", "Perfect Slide Amount", 7f);
            pSlideCap = Config.Bind("General", "Perfect Slide Cap", 23f);

            superSlideCap = Config.Bind("General", "Super Slide Cap", 20f);
            superSlideIncreaseOverTime = Config.Bind("General", "Super Slide Increase Over Time", 0.1f);

            fastFallAmount = Config.Bind("FastFall", "Fast Fall Amount", -13f);

            maxFallSpeed = Config.Bind("General", "Max Fall Speed", 40f);

            airDashRetainedSpeed = Config.Bind("General", "Air Dash Retained Speed", 0.5f);


            harmony = new Harmony(MyGUID);
            harmony.PatchAll(typeof(WallrunLineAbilityPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(BoostAbilityPatch));
            harmony.PatchAll(typeof(GrindAbilityPatchNEW));
            harmony.PatchAll(typeof(SpecialAirAbilityPatch));
            harmony.PatchAll(typeof(GroundTrickAbilityPatch));
            harmony.PatchAll(typeof(SlideAbilityPatch));
            harmony.PatchAll(typeof(AirDashAbilityPatch));

            Logger.LogInfo($"MovementPlus has been loaded!");



        }

        private void Update()
        {
            if (player != null)
            {
                SlideBoost();
            }
        }

        private void SlideBoost()
        {

            if (player.IsGrounded() == false)
            {
                slideBoost = false;
            }

            if (player.slideButtonNew)
            {
                slideTimer = 0f;
                slideTimerStarted = true;
            }

            if (slideTimerStarted)
            {
                slideTimer += Time.deltaTime;

                if (player.IsGrounded() && player.timeGrounded <= pSlideGrace.Value && slideBoost == false)
                {
                    if (player.GetForwardSpeed() + pSlideAmount.Value >= pSlideCap.Value)
                    {
                        if (player.GetForwardSpeed() <= pSlideCap.Value)
                        {
                            player.SetForwardSpeed(pSlideCap.Value);
                        }
                    }
                    else
                    {
                        player.SetForwardSpeed(player.GetForwardSpeed() + pSlideAmount.Value);
                    }
                    slideBoost = true;
                    player.CreateHighJumpDustEffect(new Vector3(0f, 0f, 0f));
                    player.DoTrick(Player.TrickType.GROUND, "Slide Boost", 1);
                }

                if (slideTimer >= pSlideGrace.Value)
                {
                    slideTimerStarted = false;
                }
            }
        }

        public static float remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }

    }
}