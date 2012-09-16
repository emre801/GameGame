/** Refactored all our input code.
 * By default Input does not respond to anything.
 * Use one of its subclasses XboxInput and KeyboardInput.
 */

using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace ProtoDerp
{
    public class Input
    {

        public bool joystickFrozen = false;
        public Input() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void freezeJoystick() { }

        public virtual bool IsPrecisionPressed()
        {
            return false;
        }

        public virtual bool IsPlayAgainPressed()
        {
            return false;
        }

        public virtual float VerticalMovement()
        {
            return 0;
        }

        public virtual float HorizontalMovement()
        {
            return 0;
        }

        public bool isMoving()
        {
            return this.HorizontalMovement() != 0 || this.VerticalMovement() != 0;
        }

        public virtual bool JustExpanded()
        {
            return false;
        }

        public virtual bool JustReleasedExpand()
        {
            return false;
        }

        public virtual bool JustRecalled()
        {
            return false;
        }

        public virtual bool JustReleasedRecall()
        {
            return false;
        }

        public virtual bool JustCancelledExpand()
        {
            return false;
        }

        public virtual bool JustCancelledRecall()
        {
            return false;
        }

        public virtual float GetJoyDirection()
        {
            return 0.0f;
        }

        public virtual bool IsJoyShooting()
        {
            return false;
        }

        public virtual bool IsUpPressed()
        {
            return false;
        }

        public virtual bool IsDownPressed()
        {
            return false;
        }

        public virtual bool IsLeftPressed()
        {
            return false;
        }

        public virtual bool IsRightPressed()
        {
            return false;
        }

        public virtual bool IsSelectPressed()
        {
            return false;
        }

        public virtual bool IsPausePressed()
        {
            return false;
        }

        public virtual bool IsBackPressed()
        {
            return false;
        }

        public virtual bool IsRecallPressed()
        {
            return false;
        }

        public virtual bool isAPressed()
        {
            return false;
        }
        public virtual bool isXPressed()
        {
            return false;
        }

        public virtual float getXDirection()
        {
            return 0;
        }
        public virtual float getYDirection()
        {
            return 0;
        }
        public virtual bool isLeft()
        {
            return false;
        }
        public virtual bool IsNewButtonPressed(Buttons b)
        {
            return false;
        }

        public virtual bool IsButtonPressed(Buttons b)
        {
            return false;
        }
        public virtual bool IsNewButtonReleased(Buttons b)
        {
            return false;
        }
    }
}