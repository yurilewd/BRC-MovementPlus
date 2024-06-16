using Reptile;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECM.Common;

namespace MovementPlus.NewAbility
{
    public class ButtslapAbility : Ability
    {
        public ButtslapAbility(Player player) : base(player) { }
        private static readonly MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        private float buttslapTimer;
		private int buttslapAmount;


        public override void Init()
        {
            allowNormalJump = false;
            normalRotation = true;
            buttslapTimer = 0f;
        }

        public void Activation()
        {
            if (!this.p.motor.isGrounded && this.p.jumpButtonNew && ConfigSettings.Buttslap.Enabled.Value && this.p.ability == this.p.groundTrickAbility && !this.p.isJumping)
            {
                this.p.ActivateAbility(this);
            }
        }

        public override void OnStartAbility()
        {
            if (!ConfigSettings.Buttslap.MultiEnabled.Value)
            {
                this.p.StopCurrentAbility();
            }
            else
            {
                this.buttslapTimer = ConfigSettings.Buttslap.Timer.Value;
				this.buttslapAmount = 0;
            }
            this.PerformButtslap();
            
        }

        public override void FixedUpdateAbility()
        {
            if (buttslapTimer <= 0f)
            {
                this.p.StopCurrentAbility();
                return;
            }

            if (this.p.AnyTrickInput() && this.p.abilityTimer >= 0.1f)
            {
                this.p.ActivateAbility(this.p.airTrickAbility);
            }

            this.buttslapTimer -= Core.dt;

            this.p.SetVisualRotLocal0();

            if (this.p.jumpButtonNew && !this.p.TreatPlayerAsSortaGrounded())
            {
                this.PerformButtslap();
            }
            else if (this.p.TreatPlayerAsSortaGrounded())
            {
                buttslapTimer = 0f;
            }
        }

        private void PerformButtslap()
        {
            // Play voice and animation
            this.p.audioManager.PlayVoice(ref this.p.currentVoicePriority, this.p.character, AudioClipID.VoiceJump, this.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
            this.p.PlayAnim(this.p.jumpHash, true, true, -1f);

            // Calculate jump amount
            float jumpAmount = ConfigSettings.Buttslap.JumpAmount.Value + Mathf.Max(this.p.GetVelocity().y, 0f);

            // Perform jump effects and set velocity
            this.p.DoJumpEffects(this.p.motor.groundNormalVisual * -1f);
            this.p.motor.SetVelocityYOneTime(jumpAmount);

            // Set jumping flags
            this.p.isJumping = true;
            this.p.jumpRequested = false;
            this.p.jumpConsumed = true;
            this.p.jumpedThisFrame = true;
            this.p.timeSinceLastJump = 0f;

            // Set forward speed and perform combo timeout
            this.p.SetForwardSpeed(MovementPlusPlugin.LosslessClamp(this.p.GetForwardSpeed(), ConfigSettings.Buttslap.Amount.Value, ConfigSettings.Buttslap.Cap.Value));
            this.p.DoComboTimeOut(ConfigSettings.Buttslap.ComboAmount.Value);

            // Perform trick
			this.buttslapAmount++;
            this.p.DoTrick(0, $"Buttslap x{this.buttslapAmount}");
        }
    }
}
