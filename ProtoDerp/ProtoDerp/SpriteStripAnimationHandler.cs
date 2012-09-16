using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProtoDerp
{
    public class SpriteStripAnimationHandler
    {
        public readonly Sprite spriteStrip;
        private readonly Texture2D index;
        private readonly int stateCount, widthOfSingleState;
        private int _currentState;
        private Rectangle texBounds;
        public int CurrentState
        {
            get { return _currentState; }
            set
            {
                if (value < 0 || value >= stateCount) throw new IndexOutOfRangeException();
                setState(value);
            }
        }

        /// <summary>
        /// Create a new sprite strip animation handler from the given spritestrip and a number of states.
        /// </summary>
        /// <param name="strip">The spritestrip.  The strip must be formatted as a series of equal size sprites concatenated horizontally in a single image.</param>
        /// <param name="stateCount">The number of states in the animation. This is how the animation handler knows where to split the spritestrip</param>
        public SpriteStripAnimationHandler(Sprite strip, int stateCount)
        {
            this.spriteStrip = strip;
            if (strip.index.Width % stateCount != 0)
            {
                throw new Exception("SpriteStrip from " + strip.fileName + " was not divisible by the requested number of states: " + stateCount);
            }
            this.stateCount = stateCount;
            this.widthOfSingleState = strip.index.Width / stateCount;
            texBounds = new Rectangle(0, 0, widthOfSingleState, strip.index.Height);
        }

        public void nextState()
        {
            setState((CurrentState + 1) % stateCount);
        }

        public void previousState()
        {
            setState((CurrentState - 1) % stateCount);
        }

        private void setState(int state)
        {
            texBounds.X = state * widthOfSingleState;
            _currentState = state;
        }

        public void Update(GameTime gameTime, float worldSpeed)
        {

        }

        public void drawCurrentState(SpriteBatch spriteBatch, Entity owner)
        {
            Vector2 drawPos = owner.game.drawingTool.getDrawingCoords(new Vector2(owner.pos.X - (widthOfSingleState / 2), owner.pos.Y - (spriteStrip.index.Height / 2)));
            spriteBatch.Draw(spriteStrip.index, drawPos, texBounds, owner.blend * owner.alpha, MathHelper.ToRadians(owner.angle), Vector2.Zero, owner.game.drawingTool.gameToScreen(owner.scale), SpriteEffects.None, 0);
        }
    }
}
