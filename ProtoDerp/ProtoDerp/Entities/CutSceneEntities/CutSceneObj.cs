using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoDerp
{
    class CutSceneObj
    {
        public int frameNum,endFrame;
        public Sprite itemSprite;
        protected SpriteStripAnimationHandler ani;
        public Vector2 pos;
        protected Game game;
        String spriteName;
        String fileName;
        bool isAReady = false;
        float scaleW = 1,scaleH;
        public CutSceneObj()
        {
        }
        public CutSceneObj(int frameNum,int endFrame, String spriteName, Vector2 pos, Game game)
        {
            this.frameNum = frameNum;
            this.endFrame = frameNum;
            this.spriteName = spriteName;
            this.pos = pos;
            this.game = game;
            LoadContent();
            isAReady = true;
            scaleW = game.drawingTool.worldDemension.X / Constants.GAME_WORLD_WIDTH;
            scaleH = game.drawingTool.worldDemension.Y / Constants.GAME_WORLD_HEIGHT;
            
        }

        public void LoadContent()
        {
            itemSprite = game.getSprite(spriteName);
            ani = game.getSpriteAnimation(spriteName);

        }

        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(itemSprite.index, new Rectangle((int)(pos.X),
                       (int)(pos.Y),
                       (int)(itemSprite.index.Width / 2*scaleW), (int)(itemSprite.index.Height / 2*scaleH)), null, Color.White, 0,
                       new Vector2(itemSprite.index.Width / 2, itemSprite.index.Height / 2), SpriteEffects.None, 0f);
            }
            else
            {
                ani.drawCurrentState(spriteBatch, new Vector2(pos.X, pos.Y), new Vector2(0, 0), true);
            }
        }


    }
}
