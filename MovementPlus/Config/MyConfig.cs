using BepInEx.Configuration;

namespace MovementPlus;
public class MyConfig(ConfigFile config)
{
    public ConfigRailGoon          RailGoon                       = new(config, "Rail Goon");
    public ConfigRailFrameboost    RailFrameboost                 = new(config, "Rail Frameboost");
    public ConfigWallFrameboost    WallFrameboost                 = new(config, "Wall Frameboost");
    public ConfigWallGeneral       WallGeneral                    = new(config, "Wall General");
    public ConfigSuperTrickJump    SuperTrickJump                 = new(config, "Super Trick Jump");
    public ConfigPerfectManual     PerfectManual                  = new(config, "Perfect Manual");
    public ConfigSuperSlide        SuperSlide                     = new(config, "Super Slide");
    public ConfigFastFall          FastFall                       = new(config, "Fast Fall");
    public ConfigVertGeneral       VertGeneral                    = new(config, "Vert General");
    public ConfigBoostGeneral      BoostGeneral                   = new(config, "Boost General");
    public ConfigRailGeneral       RailGeneral                    = new(config, "Rail General");
    public ConfigRailSlope         RailSlope                      = new(config, "Rail Slope");
    public ConfigComboGeneral      ComboGeneral                   = new(config, "Combo General");
    public ConfigLedgeClimbGeneral LedgeClimbGeneral              = new(config, "Ledge Climb General");
    public ConfigButtslap          Buttslap                       = new(config, "Buttslap");
    public ConfigMisc              Misc                           = new(config, "Misc");


    public class ConfigRailGoon(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Rail Goon Enabled",
            true,
            "Trigered by landing on a rail corner."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Rail Goon Amount",
           15f,
           "Amount of speed added when triggering a rail goon."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Rail Goon Cap",
           -1f,
           "Maximum amount of speed that can be added when triggering a rail goon."
       );
    }

    public class ConfigRailFrameboost(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Rail Frameboost Enabled",
           true,
           "Trigered by jumping shortly after landing on a rail."
       );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Rail Frameboost Amount",
          5f,
          "Amount of speed added when triggering a rail frameboost."
      );

        public ConfigEntry<float> Grace = config.Bind(
          category,
          "Rail Frameboost Grace",
          0.1f,
          "Amount of time for a rail frameboost."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Rail Frameboost Cap",
          -1f,
          "Maximum speed that can be obtained from rail frameboosts."
      );
    }

    public class ConfigWallFrameboost(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Wall Frameboost Enabled",
           true,
           "Trigered by jumping after landing on a wallride."
       );
        public ConfigEntry<bool> RunoffEnabled = config.Bind(
           category,
           "Wall Frameboost Runoff Enabled",
           false,
           "Trigered by running off a wallride"
       );
        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Wall Frameboost Amount",
           5.0f,
           "Amount of speed added when triggering a wallride frameboost."
       );
        public ConfigEntry<float> Grace = config.Bind(
           category,
           "Wall Frameboost Grace",
           0.1f,
           "Amount of time for a wallride frameboost."
       );
        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Wall Frameboost Cap",
          -1f,
          "Maximum speed that can be obtained from wallride frameboosts."
       );
    }

    public class ConfigWallGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<float> wallTotalSpeedCap = config.Bind(
          category,
          "Wall Total Speed Cap",
          -1f,
          "Maximum amount of speed that can be added when landing on a wallride."
       );
    }

    public class ConfigSuperTrickJump(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
           category,
           "Super Trick Jump Enabled",
           true,
           "On foot trick jump height increases with forward speed."
       );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Super Trick Jump Amount",
           0.2f,
           "Amount of height gained with forward speed."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Super Trick Jump Cap",
           30f,
           "Maximum height gained from a super trick jump."
       );

        public ConfigEntry<float> Threshold = config.Bind(
           category,
           "Super Trick Jump Threshold",
           21f,
           "Minimum Speed required to add any jump height."
       );
    }

    public class ConfigPerfectManual(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Perfect Manual Enabled",
          true,
          "Pressing manual just before landing."
      );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Perfect Manual Amount",
          5f,
          "Amount of speed added when performing a perfect manual."
      );

        public ConfigEntry<float> Grace = config.Bind(
          category,
          "Perfect Manual Grace",
          0.1f,
          "Amount of time for a perfect manual."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Perfect Manual Cap",
          45f,
          "Maximum speed that can be obtained from perfect manuals."
      );
    }

    public class ConfigSuperSlide(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
         category,
         "Super Slide Enabled",
         true,
         "Carpet sliding changes."
     );

        public ConfigEntry<float> Speed = config.Bind(
          category,
          "Super Slide Base Speed",
          35f,
          "Base speed of the super slide, this speed is reached very quickly."
      );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Super Slide Amount",
          0.1f,
          "Amount of speed added while super sliding."
      );

        public ConfigEntry<float> Cap = config.Bind(
          category,
          "Super Slide Cap",
          55f,
          "Maximum speed that can be obtained from super sliding."
      );
    }

    public class ConfigFastFall(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
        category,
        "Fast Fall Enabled",
        true,
        "Pressing manual while falling."
    );

        public ConfigEntry<float> Amount = config.Bind(
          category,
          "Fast Fall Amount",
          -13f,
          "Amount of speed added when triggering a fast fall."
      );
    }

    public class ConfigVertGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
          category,
          "Vert Change Enabled",
          true,
          "Vert ramp speed is adjusted to your total speed."
      );

        public ConfigEntry<bool> JumpEnabled = config.Bind(
          category,
          "Vert Jump Change Enabled",
          true,
          "Jump height from vert ramps scales with your speed."
      );

        public ConfigEntry<float> JumpStrength = config.Bind(
          category,
          "Vert Jump Strength",
          0.4f,
          "Multiplier applied to your total speed to calculate vert jump height."
      );

        public ConfigEntry<float> JumpCap = config.Bind(
          category,
          "Vert Jump Cap",
          75f,
          "Maximum jump height from vert ramps."
      );

        public ConfigEntry<float> ExitSpeed = config.Bind(
          category,
          "Vert Bonus Bottom Exit Speed",
          7f,
          "Bonus speed added when exiting a vert ramp."
      );

        public ConfigEntry<float> ExitSpeedCap = config.Bind(
          category,
          "Vert Bonus Bottom Exit Speed Cap",
          75f,
          "Maximum speed that can be obtained from exiting a vert ramp."
      );
    }

    public class ConfigBoostGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> StartEnabled = config.Bind(
         category,
         "Boost Start Change Enabled",
         true,
         "Bonus speed when using a boost outside of a grind or wallride."
     );

        public ConfigEntry<float> StartAmount = config.Bind(
         category,
         "Boost Start Amount",
         2f,
         "Amount of speed added when starting a boost."
     );

        public ConfigEntry<float> StartCap = config.Bind(
         category,
         "Boost Start Cap",
         60f,
         "Maximum speed that can be obtained from starting a boost."
     );

        public ConfigEntry<float> RailAmount = config.Bind(
         category,
         "Boost On Rail Speed Over Time",
         4f,
         "Amount of speed added over time while boosting on a rail."
     );

        public ConfigEntry<float> WallAmount = config.Bind(
         category,
         "Boost On Wallride Speed Over Time",
         15f,
         "Amount of speed added over time while boosting on a wallride."
     );

        public ConfigEntry<float> RailCap = config.Bind(
         category,
         "Boost Rail Cap",
         55f,
         "Maximum amount of speed that can be obtained from boosting while on a rail."
     );

        public ConfigEntry<float> WallCap = config.Bind(
         category,
         "Boost Wallride Cap",
         70f,
         "Maximum amount of speed that can be obtained from boosting while on a wallride."
     );

        public ConfigEntry<bool> TotalSpeedEnabled = config.Bind(
         category,
         "Total Speed Change Enabled",
         true,
         "Boost speed scales with total speed."
     );

        public ConfigEntry<float> TotalSpeedCap = config.Bind(
         category,
         "Total Speed Change Cap",
         -1f,
         "Maximum amount of speed obtainable from the total speed boost change."
     );

    }

    public class ConfigRailGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<float> HardAmount = config.Bind(
         category,
         "Rail Hard Corner Amount",
         3f,
         "Amount of speed per hard corner."
     );

        public ConfigEntry<float> HardCap = config.Bind(
         category,
         "Rail Hard Corner Cap",
         45f,
         "Maximum amount of speed that can be obtained from rail hard corners."
     );

        public ConfigEntry<float> Decc = config.Bind(
         category,
         "Rail Deceleration",
         1f,
         "Deceleration while grinding."
     );

    }

    public class ConfigRailSlope(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
         category,
         "Rail Slope Change Enabled",
         true,
         "Jumping while on an upwards sloped rail gives a larger jump and a downwards slope gives bonus speed."
     );

        public ConfigEntry<float> SlopeJumpAmount = config.Bind(
         category,
         "Rail Slope Jump Height Amount",
         4f,
         "Bonus height amount for sloped rail jump. This is affected by your speed and the slope of the rail."
     );

        public ConfigEntry<float> SlopeSpeedAmount = config.Bind(
         category,
         "Rail Slope Jump Speed Amount",
         6f,
         "Bonus speed amount for sloped rail jump. This is affected by your speed and the slope of the rail."
     );

        public ConfigEntry<float> SlopeJumpMax = config.Bind(
         category,
         "Rail Slope Jump Height Max",
         20f,
         "Maximum jump height you can gain from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedCap = config.Bind(
         category,
         "Rail Slope Jump Speed Cap",
         -1f,
         "Maximum Speed you can gain from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeJumpMin = config.Bind(
         category,
         "Rail Slope Jump Height Minimum",
         -7f,
         "Minimum jump height from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedMin = config.Bind(
         category,
         "Rail Slope Jump Speed Minimum",
         -10f,
         "Minimum speed from a sloped rail jump."
     );

        public ConfigEntry<float> SlopeSpeedMax = config.Bind(
         category,
         "Rail Slope Jump Speed Max",
         7f,
         "Maximum speed gained from a single sloped rail jump."
     );

    }

    public class ConfigComboGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> BoostEnabled = config.Bind(
         category,
         "Combo During Boost Enabled",
         true,
         "Allows you to maintain combo while boosting."
     );

        public ConfigEntry<bool> NoAbilityEnabled = config.Bind(
         category,
         "Combo No Ability Enabled",
         true,
         "Forces combo meter when touching the ground even without a manual."
     );

        public ConfigEntry<float> BoostTimeout = config.Bind(
         category,
         "Combo Boost Timer",
         0.5f,
         "How fast the combo timer ticks while boosting."
     );

        public ConfigEntry<float> BoostJumpAmount = config.Bind(
         category,
         "Combo Boost Jump",
         0.1f,
         "Amount of the combo timer is removed when jumping while boosting."
     );

        public ConfigEntry<float> NoAbilityTimeout = config.Bind(
         category,
         "Combo No Ability Timer",
         5f,
         "How fast the combo timer ticks down while not manualing."
     );
    }

    public class ConfigLedgeClimbGeneral(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Ledge Climb Cancel Enabled",
            true,
            "Allows you to cancel the ledge climb animation with a jump."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Ledge Climb Cancel Amount",
           15f,
           "Amount of speed added when you cancel a ledge climb."
       );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Ledge Climb Cancel Cap",
           -1f,
           "Maximum amount of speed that can be added when canceling a ledge climb."
       );
    }

    public class ConfigButtslap(ConfigFile config, string category)
    {
        public ConfigEntry<bool> Enabled = config.Bind(
            category,
            "Buttslap Enabled",
            true,
            "Allows a jump during a ground trick animation while in the air."
        );

        public ConfigEntry<bool> MultiEnabled = config.Bind(
            category,
            "Buttslap Multi Enabled",
            true,
            "Allows multiple jumps during a buttslap."
        );

        public ConfigEntry<float> Amount = config.Bind(
           category,
           "Buttslap Amount",
           2f,
           "Forward speed added when performing a buttslap."
       );

        public ConfigEntry<float> ComboAmount = config.Bind(
          category,
          "Buttslap Combo Amount",
          -0.1f,
          "Amount of combo meter added when performing a buttslap."
      );

        public ConfigEntry<float> Cap = config.Bind(
           category,
           "Buttslap Cap",
           -1f,
           "Maximum amount of speed that can be added when performing a buttslap."
       );

        public ConfigEntry<float> JumpAmount = config.Bind(
          category,
          "Buttslap Jump Amount",
          5f,
          "Jump height per buttslap."
      );

        public ConfigEntry<float> Timer = config.Bind(
          category,
          "Buttslap Multi Time",
          0.3f,
          "Amount of time after the initial buttslap to perform multiple."
      );

    }

    public class ConfigMisc(ConfigFile config, string category)
    {
        public ConfigEntry<float> groundTrickDecc = config.Bind(
            category,
            "Ground Trick Deceleration",
            1f,
            "Deceleration while performing a ground trick."
        );

        public ConfigEntry<bool> collisionChangeEnabled = config.Bind(
            category,
            "Collision Change Enabled",
            true,
            "Changes the collision fixing almost all instances of clipping through objects."
        );

        public ConfigEntry<float> speedLimit = config.Bind(
            category,
            "Speed Limit",
            -1f,
            "Soft speed limit, if moving faster than this speed your speed will be reduced over time."
        );

        public ConfigEntry<float> speedLimitAmount = config.Bind(
            category,
            "Speed Limit Penalty Amount",
            5f,
            "Amount of speed to remove over time while above the speed limit."
        );

        public ConfigEntry<float> maxFallSpeed = config.Bind(
            category,
            "Max Fall Speed",
            40f,
            "Maximum speed you're allowed to fall."
        );

        public ConfigEntry<float> airDashStrength = config.Bind(
            category,
            "Air Dash Strength",
            0.3f,
            "How much speed you lose when changing direction with the air dash."
        );

        public ConfigEntry<float> averageSpeedTimer = config.Bind(
            category,
            "Average Speed Timer",
            0.4f,
            "Many mechanics use your average speed over a period of time this is that period of time, a lower time will be more responsive but less forgiving and might feel less smooth."
        );
    }

}