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
        protected SpriteStripAnimationHandler ani;
        Sprite itemSprite;
        float startTime, endTime;
        protected float aValue = -1;
        bool loadFromCT=false;
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
        public CutSceneItem(Game g, Arena a, float xPos, float yPos, int playerNum, String spriteNumber,float aValue, bool loadFromCT)
            : base(g)
        {
            this.pos = new Vector2(game.drawingTool.ActualScreenPixelWidth * xPos, game.drawingTool.ActualScreenPixelHeight * yPos);
            this.origPos = new Vector2(game.drawingTool.ActualScreenPixelWidth * xPos, game.drawingTool.ActualScreenPixelHeight * yPos);
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            //this.height = height;
            //this.width = width;
            this.blockNumber = game.blockNumber;
            this.aValue = aValue;
            this.IsVisible = false;
            game.blockNumber++;

            this.loadFromCT = loadFromCT;
            LoadContent();
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
        }
        public void LoadContent()
        {
            if (loadFromCT)
            {
                itemSprite = new Sprite(game.Content, "CutScene" + game.cutScene + "\\" + spriteNumber);
                ani = game.getSpriteAnimation("Error");
            }
            else
            {
                itemSprite = game.getSprite(spriteNumber);
                ani = game.getSpriteAnimation(spriteNumber);
            }
        }
        public void itemTimeUpdate(double itemTime)
        {
            if (itemTime >= startTime)
            {
                this.IsVisible = true;
                game.ignoreAInputs = true;

            }
            if (itemTime >= endTime)
            {
                this.IsVisible = false;
                this.dispose = true;
                game.ignoreAInputs = false;
                game.aButtonValue++;
            }
        }
        public void updateButtonValue()
        {

            if (aValue == game.aButtonValue)
            {
                this.IsVisible = true;
                
            }
            if (aValue < game.aButtonValue)
            {
                this.IsVisible = false;
                this.dispose = true;
                this.removeItself();
               
            }

        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            game.cutSceneStartTime.Stop();
            double itemTime =game.cutSceneStartTime.Elapsed.TotalMilliseconds;// game.cutSceneStartTime.Stop()
            ani.Update();
            game.cutSceneStartTime.Start();
            if (aValue < 0)
            {
                itemTimeUpdate(itemTime);
            }
            else
            {
                updateButtonValue();
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
                           new Vector2(itemSprite.index.Width/2, itemSprite.index.Height/2), SpriteEffects.None, 0f);
                }
                else
                {
                    ani.drawCurrentState(spriteBatch,this,new Vector2(pos.X,pos.Y),new Vector2(0,0),true);
                }

            }


        }
    }
}
