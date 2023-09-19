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


        public static ConfigEntry<bool> railGoonEnabled;
        public static ConfigEntry<float> railGoonStrength;

        public static ConfigEntry<bool> railFrameboostEnabled;
        public static ConfigEntry<float> railFrameboostAmount;
        public static ConfigEntry<float> railFrameboostGrace;

        public static ConfigEntry<bool> wallFrameboostEnabled;
        public static ConfigEntry<float> wallFrameboostAmount;
        public static ConfigEntry<float> wallFrameboostGrace;

        public static ConfigEntry<bool> superTrickEnabled;
        public static ConfigEntry<float> superTrickStrength;

        public static ConfigEntry<bool> perfectManualEnabled;
        public static ConfigEntry<float> perfectManualAmount;
        public static ConfigEntry<float> perfectManualCap;
        public static ConfigEntry<float> perfectManualGrace;

        public static ConfigEntry<float> superSlideSpeed;
        public static ConfigEntry<float> superSlideIncrease;

        public static ConfigEntry<bool> fastFallEnabled;
        public static ConfigEntry<float> fastFallAmount;

        public static ConfigEntry<bool> vertEnabled;
        public static ConfigEntry<bool> vertJumpEnabled;
        public static ConfigEntry<float> vertJumpStrength;

        public static ConfigEntry<float> maxFallSpeed;
        public static ConfigEntry<float> airDashSpeed;
        public static ConfigEntry<bool> boostChangeEnabled;
        public static ConfigEntry<float> groundTrickDecc;
        public static ConfigEntry<float> railHardAmount;
        public static ConfigEntry<float> railHardCap;
        public static ConfigEntry<float> railDecc;


        private void Awake()
        {
            railGoonEnabled = Config.Bind("1:RailGoon", "Rail Goon Enabled", true, "Timing a boost at the start of rails results in a large speed boost.");
            railGoonStrength = Config.Bind("1:RailGoon", "Rail Goon Strength", 0.9f, "Rail goon multiplier.");

            railFrameboostEnabled = Config.Bind("2:RailFrameboost", "Rail Frameboost Enabled", true, "Timing a jump as you land on a rail generates a speed boost.");
            railFrameboostAmount = Config.Bind("2:RailFrameboost", "Rail Frameboost Amount", 5f, "Amount of speed to add when rail frameboosting.");
            railFrameboostGrace = Config.Bind("2:RailFrameboost", "Rail Frameboost Grace Period", 0.1f, "Amount of time to hit a rail frameboost.");

            wallFrameboostEnabled = Config.Bind("3:WallFrameboost", "Wall Frameboost Enabled", true, "Timing a jump as you land on a wallride generates a speed boost.");
            wallFrameboostAmount = Config.Bind("3:WallFrameboost", "Wall Frameboost Amount", 11f, "Amount of speed to add when wallride frameboosting.");
            wallFrameboostGrace = Config.Bind("3:WallFrameboost", "Wall Frameboost Grace Period", 0.1f, "Amount of time to hit a wallride frameboost.");

            superTrickEnabled = Config.Bind("4:SuperTrickJump", "Super Trick Jump Enabled", true, "On foot grounded trick jump scales with speed.");
            superTrickStrength = Config.Bind("4:SuperTrickJump", "Super Trick Jump Strength", 0.9f, "Super trick jump multiplier.");

            perfectManualEnabled = Config.Bind("5:PerfectManual", "Perfect Manual Enabled", true, "Timing a manual press just before landing results in a speed boost.");
            perfectManualAmount = Config.Bind("5:PerfectManual", "Perfect Manual Amount", 7f, "Amount of speed to add when landing a perfect manual.");
            perfectManualCap = Config.Bind("5:PerfectManual", "Perfect Manual Speed Cap", 23f, "Speed cap of the perfect manual.");
            perfectManualGrace = Config.Bind("5:PerfectManual", "Perfect Manual Grace Period", 0.1f, "The amount of time to land a perfect manual.");

            superSlideSpeed = Config.Bind("6:SuperSlide", "Super Slide Speed", 20f, "Base speed for slide track sliding.");
            superSlideIncrease = Config.Bind("6:SuperSlide", "Super Slide Speed Increase Over Time", 0.1f, "Speed to gain while sliding on slide tracks.");

            fastFallEnabled = Config.Bind("7:FastFall", "Fast Fall Enabled", true, "Pressing the manual button while moving down will launch you downwards.");
            fastFallAmount = Config.Bind("7:FastFall", "Fast Fall Amount", -13f, "Fast fall speed.");

            vertEnabled = Config.Bind("8:Vert", "Vert Ramp Speed Change Enabled", true, "Vert ramps scale with your speed instead of just being a flat speed.");
            vertJumpEnabled = Config.Bind("8:Vert", "Vert Ramp Jump Change Enabled", true, "Vert ramps jump height scales with your speed.");
            vertJumpStrength = Config.Bind("8:Vert", "Vert Ramp Jump Height Strength", 0.6f, "Vert jump heght multiplier.");

            maxFallSpeed = Config.Bind("9:General", "Max Fall Speed", 40f, "Maximum speed you can fall.");
            airDashSpeed = Config.Bind("9:General", "Air Dash Retain Speed", 0.5f, "Percent of speed to maintain when changing direction with an air dash.");
            boostChangeEnabled = Config.Bind("9:General", "Boost Change Enabled", true, "Boost scales with total speed.");
            groundTrickDecc = Config.Bind("9:General", "Ground Trick Deceleration", 1f, "Amount of deceleration ground tricks have.");
            railHardAmount = Config.Bind("9:General", "Rail Hard Corner Amount", 1f, "Amount of speed to add per hard corner.");
            railHardCap = Config.Bind("9:General", "Rail Hard Corner Speed Cap", 40f, "Speed cap of Hard Corners.");
            railDecc = Config.Bind("9:General", "Rail Deceleration", 0f, "Amount of deceleration rails have.");


            harmony = new Harmony(MyGUID);
            harmony.PatchAll(typeof(WallrunLineAbilityPatch));
            harmony.PatchAll(typeof(PlayerPatch));
            harmony.PatchAll(typeof(BoostAbilityPatch));
            harmony.PatchAll(typeof(GrindAbilityPatch));
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
                if (MovementPlusPlugin.perfectManualEnabled.Value)
                {
                    SlideBoost();
                }    
            }
        }


        private bool slideBoost = false;
        private float slideTimer = 0f;
        private bool slideTimerStarted = false;
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

                if (player.IsGrounded() && player.timeGrounded <= perfectManualGrace.Value && slideBoost == false)
                {
                    if (player.GetForwardSpeed() + perfectManualAmount.Value >= perfectManualCap.Value)
                    {
                        if (player.GetForwardSpeed() <= perfectManualCap.Value)
                        {
                            player.SetForwardSpeed(perfectManualCap.Value);
                        }
                    }
                    else
                    {
                        player.SetForwardSpeed(player.GetForwardSpeed() + perfectManualAmount.Value);
                    }
                    slideBoost = true;
                    player.CreateHighJumpDustEffect(new Vector3(0f, 0f, 0f));
                    player.DoTrick(Player.TrickType.GROUND, "Slide Boost", 1);
                }

                if (slideTimer >= perfectManualGrace.Value)
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