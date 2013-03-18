using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ProtoDerp
{
    class KeyboardInput : Input
    {
        public KeyboardState keyboardState;
        public KeyboardState previousKeyboardState;


        public KeyboardInput()
            : base()
        {
            previousKeyboardState = keyboardState;

            keyboardState = Keyboard.GetState();
        }

        public bool IsNewKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key));
        }

        public bool IsNewKeyReleased(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key));
        }

        public bool IsKeyPressed(Keys key)
        {
            if (keyboardState.IsKeyDown(key))
                return true;
            return false;
        }

        public override bool IsPrecisionPressed()
        {
            return this.IsKeyPressed(Keys.LeftShift);
        }

        public override bool IsPlayAgainPressed()
        {
            return this.IsNewKeyPressed(Keys.Enter);
        }

        public override float VerticalMovement()
        {
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                return 1;
            }
            else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                return -1;
            }
            return 0;
        }

        public override float HorizontalMovement()
        {
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                return -1;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                return 1;
            }
            return 0;
        }

        public override bool JustExpanded()
        {
            return this.IsKeyPressed(Keys.Z);
        }

        public override bool JustReleasedExpand()
        {
            return this.IsNewKeyReleased(Keys.Z);
        }

        public override bool JustRecalled()
        {
            return this.IsNewKeyPressed(Keys.X);
        }

        public override bool JustReleasedRecall()
        {
            return this.IsNewKeyReleased(Keys.X);
        }
        public override void Update(GameTime gameTime)
        {
            this.previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        public override bool JustCancelledExpand()
        {
            return keyboardState.IsKeyDown(Keys.Z)
                && previousKeyboardState.IsKeyDown(Keys.Z)
                && this.IsNewKeyPressed(Keys.X);
        }

        public override bool JustCancelledRecall()
        {
            return keyboardState.IsKeyDown(Keys.X)
                && previousKeyboardState.IsKeyDown(Keys.X)
                && this.IsNewKeyPressed(Keys.Z);
        }

        public override bool IsUpPressed()
        {
            return this.IsNewKeyPressed(Keys.Up) || this.IsNewKeyPressed(Keys.W);
        }


        public override bool IsDownPressed()
        {
            return this.IsNewKeyPressed(Keys.Down) || this.IsNewKeyPressed(Keys.S);
        }

        public override bool IsLeftPressed()
        {
            return this.IsNewKeyPressed(Keys.Left) || this.IsNewKeyPressed(Keys.A);
        }


        public override bool IsRightPressed()
        {
            return this.IsNewKeyPressed(Keys.Right) || this.IsNewKeyPressed(Keys.D);
        }

        public override bool IsSelectPressed()
        {
            return this.IsNewKeyPressed(Keys.Enter);
        }

        public override bool IsPausePressed()
        {
            return (this.IsNewKeyPressed(Keys.P));
        }

        public override bool IsBackPressed()
        {
            return this.IsNewKeyPressed(Keys.Back) || this.IsNewKeyPressed(Keys.X);
        }

        public override bool IsRecallPressed()
        {
            return this.IsNewKeyPressed(Keys.X);
        }
    }
}
