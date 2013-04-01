using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;


namespace ProtoDerp
{
    class CutSceneItem : Entity
    {
        public int drawLevel;
        public int blockNumber;
        public float width, height;
        String spriteNumber;
        Vector2 origPos,origin;
        SpriteStripAnimationHandler ani;
        Sprite itemSprite;
        float startTime, endTime;


        public CutSceneItem(Game g, Arena a,float xPos,float yPos, int playerNum, String spriteNumber,float startTime,float endTime)
            : base(g)
        {
            this.pos = new Vector2(game.drawingTool.ActualScreenPixelWidth*xPos,game.drawingTool.ActualScreenPixelHeight*yPos);
            this.origPos = new Vector2(game.drawingTool.ActualScreenPixelWidth * xPos, game.drawingTool.ActualScreenPixelHeight * yPos);
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            //this.height = height;
            //this.width = width;
            this.blockNumber = game.blockNumber;
            this.startTime = startTime;
            this.endTime = endTime;
            this.IsVisible = false;
            game.blockNumber++;
            LoadContent();
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
        }
        public void LoadContent()
        {
            itemSprite = game.getSprite(spriteNumber);
            ani = game.getSpriteAnimation(spriteNumber);
        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            game.cutSceneStartTime.Stop();
            double itemTime =game.cutSceneStartTime.Elapsed.TotalMilliseconds;// game.cutSceneStartTime.Stop()
            ani.Update();
            game.cutSceneStartTime.Start();
            if (itemTime >= startTime)
            {
                this.IsVisible = true;

            }
            if (itemTime >= endTime)
            {
                this.IsVisible = false;
                this.dispose = true;
            }

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                if (ani.getStateCount() == 1)
                {
                    spriteBatch.Draw(itemSprite.index, new Rectangle((int)(pos.X),
                           (int)(pos.Y),
                           (int)(itemSprite.index.Width / 2), (int)(itemSprite.index.Height / 2)), null, Color.White, 0,
                           new Vector2(itemSprite.index.Width, itemSprite.index.Height), SpriteEffects.None, 0f);
                }
                else
                {
                    ani.drawCurrentState(spriteBatch,this,new Vector2(pos.X,pos.Y),new Vector2(0,0),true);
                }

            }


        }
    }
}
