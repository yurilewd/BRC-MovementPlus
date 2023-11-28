using BepInEx;
using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace MovementPlus
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class MovementPlusPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.yuril.MovementPlus";
        private const string PluginName = "MovementPlus";
        private const string VersionString = "2.0.0";

        private       Harmony  harmony;
        public static Player   player;
        public static MyConfig ConfigSettings;

        public static bool           canFastFall;                   //make sure we don't fast fall twice
        public static float          defaultBoostSpeed;             //vanilla boost speed
        public static float          defaultVertMaxSpeed;           //vanilla vert speed
        public static float          defaultVertTopJumpSpeed;       //vanilla vert jump height
        public static float          defaultJumpSpeed;              //vanilla jump height
        public static float          savedLastSpeed;                //wallrun last speed
        public static float          noAbilitySpeed;                //speed used for vertical rails
        public static float          timeInAir = 0f;                //used for on foot hard landing
        public static Queue<float>   forwardSpeeds;                 //container for average forward speed
        public static Queue<float>   totalSpeeds;                   //container for average total speed
        public static Queue<Vector3> forwardDirs;                   //container for player forward dir
        public static float          averageSpeedTimer = 0f;        //timer for both average speeds
        public static float          averageForwardTimer = 0f;      //timer for average player forward dir
        public static bool           hasGooned = false;             //make sure we only rail goon once
        public static bool           railGoonAppllied = false;      //make sure the goon boost is only applied once
        public static bool           jumpedFromRail = false;        //cursed rail check
        public static float          jumpedFromRailTimer = 0f;      //cursed rail timer
        private       bool           slideBoost = false;            //perfect manual
        private       float          slideTimer = 0f;               //perfect manual timer
        private       bool           slideTimerStarted = false;     //timer started


        private void Awake()
        {
            harmony = new Harmony(MyGUID);
            PatchAllInNamespace(harmony, "MovementPlus.Patches");
            ConfigSettings    = new MyConfig(Config);

            forwardSpeeds     = new Queue<float>();
            totalSpeeds       = new Queue<float>();
            forwardDirs       = new Queue<Vector3>();
            averageSpeedTimer = 0f;

            Logger.LogInfo($"MovementPlus has been loaded!");
        }

       

        private void FixedUpdate()
        {
            if (player != null)
            {
                FastFall();
                VertChanges();
                BoostChanges();
                SaveSpeed();
                TimeInAir();
                LogSpeed(ConfigSettings.Misc.averageSpeedTimer.Value);
                LogForward(0.032f);
                JustJumpedFromRail();
                SpeedLimit();

                if (ConfigSettings.PerfectManual.Enabled.Value)
                {
                    PerfectManual();
                }
            }
        }
        public static void PatchAllInNamespace(Harmony harmony, string namespaceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.Namespace == namespaceName);
            foreach (var type in types)
            {
                harmony.PatchAll(type);
            }
        }

        private void PerfectManual()
        {
            if (!player.IsGrounded())
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

                if (player.IsGrounded() && player.timeGrounded <= ConfigSettings.PerfectManual.Grace.Value && !slideBoost)
                {
                    DoPerfectManual();
                }

                if (slideTimer >= ConfigSettings.PerfectManual.Grace.Value)
                {
                    slideTimerStarted = false;
                }
            }
        }

        private void DoPerfectManual()
        {
            player.SetForwardSpeed(LosslessClamp(AverageForwardSpeed(), ConfigSettings.PerfectManual.Amount.Value, ConfigSettings.PerfectManual.Cap.Value));
            slideBoost = true;
            player.CreateHighJumpDustEffect(player.tf.up);
            player.DoTrick(Player.TrickType.GROUND, "Perfect Manual", 1);
        }

        private void FastFall()
        {
            if (player.slideButtonNew && !player.TreatPlayerAsSortaGrounded() && player.motor.velocity.y <= 0f && MovementPlusPlugin.canFastFall && ConfigSettings.FastFall.Enabled.Value)
            {
                player.motor.SetVelocityYOneTime(Mathf.Min(player.motor.velocity.y + ConfigSettings.FastFall.Amount.Value, ConfigSettings.FastFall.Amount.Value));
                player.ringParticles.Emit(1);
                player.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, player.playerOneShotAudioSource, 0f);
                MovementPlusPlugin.canFastFall = false;
            }
            if (player.TreatPlayerAsSortaGrounded())
            {
                MovementPlusPlugin.canFastFall = true;
            }
        }

        private void BoostChanges()
        {
            if (hasGooned)
            {
                ApplyRailGoon();
                return;
            }

            if (player.ability == player.wallrunAbility)
            {
                return;
            }

            if (player.ability == player.grindAbility)
            {
                //HandleGrindAbility();
                return;
            }

            if (player.ability != player.boostAbility)
            {
                player.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, AverageForwardSpeed());
            }
        }

        private void ApplyRailGoon()
        {
            if (!railGoonAppllied && ConfigSettings.RailGoon.Enabled.Value)
            {
                player.grindAbility.speed = LosslessClamp(Mathf.Max(AverageForwardSpeed(), defaultBoostSpeed), ConfigSettings.RailGoon.Amount.Value, ConfigSettings.RailGoon.Cap.Value);
                railGoonAppllied = true;
            }
        }

        private void HandleGrindAbility()
        {
            player.normalBoostSpeed = LosslessClamp(player.normalBoostSpeed, ConfigSettings.BoostGeneral.RailAmount.Value * Core.dt, ConfigSettings.BoostGeneral.RailCap.Value);
        }

        private void VertChanges()
        {
            if (player.ability != player.vertAbility)
            {
                HandleVertAbility();
            }
        }

        private void HandleVertAbility()
        {
            if (ConfigSettings.VertGeneral.Enabled.Value)
            {
                UpdateVertBottomExitSpeed();
            }

            if (ConfigSettings.VertGeneral.JumpEnabled.Value)
            {
                UpdateVertTopJumpSpeed();
            }
        }

        private void UpdateVertBottomExitSpeed()
        {
            float playerTotal = Mathf.Max(player.GetTotalSpeed(), AverageTotalSpeed());
            var x = playerTotal + ConfigSettings.VertGeneral.ExitSpeed.Value;
            x = Mathf.Min(x, ConfigSettings.VertGeneral.ExitSpeedCap.Value);
            player.vertMaxSpeed = Mathf.Max(defaultVertMaxSpeed, player.GetTotalSpeed());
            player.vertBottomExitSpeed = x;
        }

        private void UpdateVertTopJumpSpeed()
        {
            float playerTotal = Mathf.Max(player.GetTotalSpeed(), AverageTotalSpeed());
            var x = Mathf.Max(defaultVertTopJumpSpeed, playerTotal * ConfigSettings.VertGeneral.JumpStrength.Value);
            x = Mathf.Min(ConfigSettings.VertGeneral.JumpCap.Value, x);
            player.vertTopJumpSpeed = x;
        }

        private void JustJumpedFromRail()
        {
            if (jumpedFromRail)
            {
                player.SetForwardSpeed(Mathf.Max(AverageForwardSpeed(), player.GetForwardSpeed()));
                jumpedFromRailTimer -= Core.dt;
            }
            if (jumpedFromRailTimer <= 0f)
            {
                jumpedFromRail = false;
            }
        }

        private void SpeedLimit()
        {
            if (player.GetForwardSpeed() >= ConfigSettings.Misc.speedLimit.Value && player.ability != player.grindAbility && player.ability != player.wallrunAbility && ConfigSettings.Misc.speedLimit.Value > 0f)
            {
                float newSpeed = player.GetForwardSpeed() - ConfigSettings.Misc.speedLimitAmount.Value * Core.dt;
                player.SetForwardSpeed(newSpeed);
            }
        }

        private void LogSpeed(float time)
        {
            forwardSpeeds.Enqueue(player.GetForwardSpeed());
            totalSpeeds.Enqueue(player.GetTotalSpeed());
            if (averageSpeedTimer >= time)
            {
                forwardSpeeds.Dequeue();
                totalSpeeds.Dequeue();
            }
            else
            {
                averageSpeedTimer += Core.dt;
            }
        }

        public static float AverageForwardSpeed()
        {
            float sum = 0f;
            foreach (float forwardSpeed in forwardSpeeds)
            {
                sum += forwardSpeed;
            }
            return sum / forwardSpeeds.Count;
        }

        public static float AverageTotalSpeed()
        {
            float sum = 0f;
            foreach (float totalSpeed in totalSpeeds)
            {
                sum += totalSpeed;
            }
            return sum / totalSpeeds.Count;
        }

        private void LogForward(float time)
        {
            forwardDirs.Enqueue(player.tf.forward);
            if (averageForwardTimer >= time)
            {
                forwardDirs.Dequeue();
            }
            else
            {
                averageForwardTimer += Core.dt;
            }
        }

        public static Vector3 AverageForwardDir()
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 forwardDir in forwardDirs)
            {
                sum += forwardDir;
            }
            return sum / forwardDirs.Count;
        }

        private void SaveSpeed()
        {
            noAbilitySpeed = (player.ability == player.grindAbility || player.ability == player.handplantAbility)
            ? Mathf.Max(noAbilitySpeed, player.grindAbility.speed)
            : player.GetForwardSpeed();
        }

        private void TimeInAir()
        {
            timeInAir = player.TreatPlayerAsSortaGrounded() ? 0f : timeInAir + Core.dt;
        }

        public static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }

        public static float TableCurve(float a, float b, float c, float x)
        {
            return (((a + b) * x) / (b + x) + c);
        }

        public static float LosslessClamp(float input, float toAdd, float cap)
        {
            float output = input;
            float potentialOutput = output + toAdd;
            if (cap < 0)
            {
                return potentialOutput;
            }

            output = Mathf.Min(potentialOutput, cap);
            output = Mathf.Max(output, input);

            return output;
        }
    }
}