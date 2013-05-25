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


/**
 * 
 * Main class that playable objects will inherit. Shares all common data and operations
 * that player "ships" can perform.
 * 
 */
namespace ProtoDerp
{
    public class Button : Entity
    {
        String spriteName;
        Rectangle buttonBox;
        MouseState oldMouse;
        int spritePos;
        Sprite blockSprite;
        Vector2 origin;
        float heightDiff=30, widthDiff=30;
        bool isInTitle = false;
        int titleValue=0;
        
        public Button(Game g, Vector2 pos, int spritePos, String spriteName)
            : base(g)
        {
            this.game = g;
            this.pos = pos;
            this.spritePos = spritePos;
            this.IsVisible = false;
            this.oldMouse = Mouse.GetState();
            this.blockSprite = game.getSprite(spriteName);
            if(!isInTitle)
                this.buttonBox = new Rectangle((int)pos.X, (int)pos.Y, 20, 20);
            else
                this.buttonBox = new Rectangle((int)pos.X - blockSprite.index.Width / 2, (int)pos.Y - blockSprite.index.Height/2,
                    (int)(blockSprite.index.Width), (int)(blockSprite.index.Height));
            this.origin = new Vector2(1, 1);
        }

        public void setTitleValuee(int titleValue)
        {
            this.titleValue = titleValue;
            this.isInTitle = true;
            this.buttonBox=new Rectangle((int)(pos.X),
                 (int)(pos.Y),
                 (int)(blockSprite.index.Width*0.45f), (int)(blockSprite.index.Height*0.45f));
        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {
             if(!isInTitle)
                this.buttonBox = new Rectangle((int)pos.X, (int)pos.Y, 20, 20);
            else
                 this.buttonBox = new Rectangle((int)pos.X - (int)(blockSprite.index.Width / 4), (int)pos.Y - (int)(blockSprite.index.Height / 4f),
                (int)(blockSprite.index.Width/2), (int)(blockSprite.index.Height/2f));


            if (game.gMode == 2 && game.activateButtons&& game.backGroundNum==0)
            {
                isColliding();
                if (!isInTitle)
                {
                    int scrollWheel = (oldMouse.ScrollWheelValue - Mouse.GetState().ScrollWheelValue)*2;
                    pos = new Vector2(pos.X, pos.Y - (float)scrollWheel * 0.19999999f);
                    int xPos = (int)pos.X;
                    int yPos = 50*(int)Math.Round(pos.Y/50);
                    pos = new Vector2(xPos, yPos);
                    this.buttonBox = new Rectangle(xPos, yPos, (int)widthDiff + 10, (int)heightDiff + 10);
                    oldMouse = Mouse.GetState();
                }
            }
            else if (game.gMode==1|| game.gMode==6)
            {
                isColliding();
            }

        }

        public void moveButton(Vector2 moveDir)
        {
            this.pos += moveDir;
        }

        public bool isColliding()
        {
            
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Rectangle mouseRect = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 1, 1);
            if (mouseRect.Intersects(buttonBox))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!isInTitle)
                    {
                        game.isSelectingBlock = true;
                        game.isButtonSelect = true;
                        game.spriteBlockCounter = spritePos;
                    }
                    else
                    {
                        game.isCollidingWithButton = true;
                        game.gameTitleValue = titleValue;
                    }
                }
                
                return true;
            }
            return false;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (game.backGroundNum != 0)
                return;

            
            if ((game.gMode == 2 && game.activateButtons && game.cameraWindowValue==0))
            {
                //game.drawingTool.drawBorderImageFromPos(pos.X, pos.Y, 30, 30,spriteBatch);


                if (game.spriteBlockCounter == spritePos)
                {
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X-2.5),
                 (int)(pos.Y-2.5),
                 (int)widthDiff + 5, (int)heightDiff + 5), null, Color.Black, 0, origin, SpriteEffects.FlipHorizontally, 0f);
                    

                }
                spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                 (int)(pos.Y),
                 (int)widthDiff, (int)heightDiff), null, Color.White, 0, origin, SpriteEffects.FlipHorizontally, 0f);

                game.xValues[game.xycounter] = (int)pos.X;
                game.yValues[game.xycounter] = (int)pos.Y;
                game.xycounter = game.xycounter + 1;
            
            }
            if (game.gMode == 1 || game.gMode == 6)
            {
                origin = new Vector2(blockSprite.index.Width / 2f, blockSprite.index.Height/ 2f);
                float alpha = 1f;

                if (game.gameTitleValue == titleValue)
                    alpha = 0.5f;
                if (Constants.FULLSCREEN)
                {
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X+5),
                     (int)(pos.Y+5),
                     (int)(blockSprite.index.Width * 0.65f * game.drawingTool.zoomRatio), (int)(blockSprite.index.Height * 0.65f * game.drawingTool.zoomRatio)), null, Color.Black*game.fadeAlpha, 0, origin, SpriteEffects.None, 0f);
                
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                     (int)(pos.Y),
                     (int)(blockSprite.index.Width * 0.65f * game.drawingTool.zoomRatio), (int)(blockSprite.index.Height * 0.65f * game.drawingTool.zoomRatio)), null, Color.White * alpha * game.fadeAlpha, 0, origin, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X+5),
                     (int)(pos.Y),
                     (int)(blockSprite.index.Width * 0.45f * game.drawingTool.zoomRatio), (int)(blockSprite.index.Height * 0.45f * game.drawingTool.zoomRatio)), null, Color.Black*game.fadeAlpha, 0, origin, SpriteEffects.None, 0f);
                
                
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                     (int)(pos.Y),
                     (int)(blockSprite.index.Width * 0.45f * game.drawingTool.zoomRatio), (int)(blockSprite.index.Height * 0.45f * game.drawingTool.zoomRatio)), null, Color.White * alpha * game.fadeAlpha, 0, origin, SpriteEffects.None, 0f);
                
                }
                //spriteBatch.Draw(blockSprite.index, new Vector2((int)pos.X, (int)pos.Y), new Rectangle((int)(pos.X),
                // (int)(pos.Y),
               //  (int)widthDiff, (int)heightDiff), Color.White, 0, origin, 5, SpriteEffects.FlipHorizontally, 0f);


            }
            /*
            if (isInTitle)
            {
                spriteBatch.Draw(game.getSprite("Error").index, buttonBox, null, Color.Green, 0, new Vector2(0,0), SpriteEffects.None, 0f);


            }*/

        }


    }
}
