using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using System.Diagnostics;

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

        private int frameRate;
        private int origFrameRate;
        public Stopwatch stopWatch = new Stopwatch();
        public int numberOfTotalCycles = 0;

        public SpriteStripAnimationHandler(Sprite strip, int stateCount,int frameRate)
        {
            this.spriteStrip = strip;
            if (strip.index.Width % stateCount != 0)
            {
                throw new Exception("SpriteStrip from " + strip.fileName + " was not divisible by the requested number of states: " + stateCount);
            }
            this.stateCount = stateCount;
            this.widthOfSingleState = strip.index.Width / stateCount;
            texBounds = new Rectangle(0, 0, widthOfSingleState, strip.index.Height);
            stopWatch.Start();
            this.frameRate = frameRate;
            this.origFrameRate = frameRate;
        }

        public void nextState()
        {
            setState((CurrentState + 1) % stateCount);
        }
        public int widthOf()
        {

            return this.widthOfSingleState;
        }
        public int heightOf()
        {
            return spriteStrip.index.Height;
        }

        public void previousState()
        {
            setState((CurrentState - 1) % stateCount);
        }
        public int getCycles()
        {
            return _currentState;
        }

        public void setStatePub(int state)
        {
            setState(state);
        }
        private void setState(int state)
        {
            texBounds.X = state * widthOfSingleState;
            _currentState = state;
        }
        public void changeFrameRate(int frameRate)
        {
            this.frameRate = frameRate;
        }
        public int getFrameRate()
        {
            return frameRate;
        }
        public void resetFrameRate()
        {
            this.frameRate = this.origFrameRate;
        }
        public void Update()
        {
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            if (ts.Milliseconds > frameRate)
            {
                stopWatch.Reset();
                nextState();
            }
            stopWatch.Start();

        }
        public void drawCurrentState(SpriteBatch spriteBatch, Entity owner, Vector2 drawPos,Vector2 origin, Body body, Rectangle rect,Boolean direction,Vector2 shiftPosition)
        {
            if(direction)
                spriteBatch.Draw(spriteStrip.index, drawPos, texBounds, owner.blend * owner.alpha, MathHelper.ToRadians(owner.angle), origin+shiftPosition, 1, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(spriteStrip.index, drawPos, texBounds, owner.blend * owner.alpha, MathHelper.ToRadians(owner.angle), origin-shiftPosition, 1, SpriteEffects.FlipHorizontally, 0);
        }
    }
}
