using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ProtoDerp
{

    class XboxInput : Input
    {
        public GamePadState gamePadState;
        public GamePadState previousGamePadState;

        public PlayerIndex playerIndex = 0;

        public TimeSpan frozenElapsed;

        public XboxInput(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        public override void freezeJoystick()
        {
            this.joystickFrozen = true;
            this.frozenElapsed = TimeSpan.FromMilliseconds(0);
        }

        public override void Update(GameTime gameTime)
        {
            previousGamePadState = gamePadState;

            gamePadState = GamePad.GetState(playerIndex);

            this.frozenElapsed += gameTime.ElapsedGameTime;

            if (this.joystickFrozen && this.frozenElapsed > Constants.JOYSTICK_DELAY && this.gamePadState.ThumbSticks.Right.X == 0 && this.gamePadState.ThumbSticks.Right.Y == 0)
            {
                this.joystickFrozen = false;
            }
        }
        public float isLeftTriggerPressed()
        {
            return gamePadState.Triggers.Left;
        }
        public float isRightTriggerPressed()
        {
            return gamePadState.Triggers.Right;
        }
        public override bool IsPrecisionPressed()
        {
            return gamePadState.IsButtonDown(Constants.PRECISION_BUTTON);
        }

        public override bool IsPlayAgainPressed()
        {
            return this.IsNewButtonPressed(Buttons.Y);
        }

        public override float VerticalMovement()
        {
            return gamePadState.ThumbSticks.Left.Y;
        }

        public override float HorizontalMovement()
        {
            return gamePadState.ThumbSticks.Left.X;
        }

        public float VerticalMovementRight()
        {
            return gamePadState.ThumbSticks.Right.Y;
        }

        public float HorizontalMovementRight()
        {
            return gamePadState.ThumbSticks.Right.X;
        }

        public override bool IsNewButtonPressed(Buttons b)
        {
            return gamePadState.IsButtonDown(b) && previousGamePadState.IsButtonUp(b);
        }
        public bool isTriggerPressed()
        {
            if(gamePadState.IsButtonDown(Buttons.DPadDown))
                return true;
            return false;
        }

        public override bool IsButtonPressed(Buttons b)
        {
            return gamePadState.IsButtonDown(b);
        }

        public override bool IsNewButtonReleased(Buttons b)
        {
            return gamePadState.IsButtonUp(b) && previousGamePadState.IsButtonDown(b);
        }

        public override bool JustExpanded()
        {
            foreach (var button in Constants.EXPAND_BUTTONS)
                if (this.gamePadState.IsButtonDown(button))
                    return true;
            return false;
        }

        public override bool JustReleasedExpand()
        {
            foreach (var button in Constants.EXPAND_BUTTONS)
                if (this.IsNewButtonReleased(button))
                    return true;
            return false;
        }

        public override bool JustRecalled()
        {
            if (this.joystickFrozen)
                return false;
            foreach (var button in Constants.RELEASE_BUTTONS)
                if (this.IsNewButtonPressed(button))
                    return true;
            return false;
        }

        public override bool JustReleasedRecall()
        {
            if (this.joystickFrozen)
                return false;
            foreach (var button in Constants.RELEASE_BUTTONS)
                if (this.IsNewButtonReleased(button))
                    return true;
            return false;
        }

        public override bool JustCancelledExpand()
        {
            foreach (var expand in Constants.EXPAND_BUTTONS)
            {
                foreach (var release in Constants.RELEASE_BUTTONS)
                {
                    if (gamePadState.IsButtonDown(expand)
                        && previousGamePadState.IsButtonDown(expand)
                        && this.IsNewButtonPressed(release))
                        return true;
                }
            }
            return false;
        }

        public override bool JustCancelledRecall()
        {
            foreach (var expand in Constants.EXPAND_BUTTONS)
                foreach (var release in Constants.RELEASE_BUTTONS)

                    if (gamePadState.IsButtonDown(release)
                        && previousGamePadState.IsButtonDown(release)
                        && this.IsNewButtonPressed(expand))
                        return true;
            return false;
        }

        public override float GetJoyDirection()
        {
            return (float)Math.Atan2(-gamePadState.ThumbSticks.Right.Y, gamePadState.ThumbSticks.Right.X);
        }

        public override float getXDirection()
        {
            return gamePadState.ThumbSticks.Left.X;
        }
        public override float getYDirection()
        {
            return -gamePadState.ThumbSticks.Left.Y;
        }
        public override bool IsUpPressed()
        {
            return (this.gamePadState.ThumbSticks.Left.Y > 0 && this.previousGamePadState.ThumbSticks.Left.Y <= 0) || this.IsNewButtonPressed(Buttons.DPadUp);
        }

        public override bool IsDownPressed()
        {
            return (this.gamePadState.ThumbSticks.Left.Y < 0 && this.previousGamePadState.ThumbSticks.Left.Y >= 0) || this.IsNewButtonPressed(Buttons.DPadDown);
        }

        public override bool IsLeftPressed()
        {
            return (this.gamePadState.ThumbSticks.Left.X < 0 && this.previousGamePadState.ThumbSticks.Left.X >= 0) || this.IsNewButtonPressed(Buttons.DPadLeft);
        }

        public override bool IsRightPressed()
        {
            return (this.gamePadState.ThumbSticks.Left.X <= 0 && this.previousGamePadState.ThumbSticks.Left.X > 0) || this.IsNewButtonPressed(Buttons.DPadRight);
        }

        public override bool IsPausePressed()
        {
            return (this.IsNewButtonPressed(Buttons.Start));
        }

        public override bool IsSelectPressed()
        {
            return this.IsNewButtonPressed(Buttons.A);
        }

        public override bool IsBackPressed()
        {
            return this.IsNewButtonPressed(Buttons.Back);
        }

        public override bool isAPressed()
        {
            return this.IsNewButtonPressed(Buttons.A);
        }
        public override bool isXPressed()
        {
            return this.IsNewButtonPressed(Buttons.X);
        }

        public override bool IsJoyShooting()
        {
            if (this.joystickFrozen)
                return false;
            return (this.gamePadState.ThumbSticks.Right.Y * this.gamePadState.ThumbSticks.Right.Y + this.gamePadState.ThumbSticks.Right.X * this.gamePadState.ThumbSticks.Right.X) > 0.8;
        }
        public override bool IsRecallPressed()
        {
            foreach (Buttons b in Constants.RELEASE_BUTTONS)
            {
                if (this.IsNewButtonPressed(b))
                    return true;
            }
            return false;
        }
    }
}
