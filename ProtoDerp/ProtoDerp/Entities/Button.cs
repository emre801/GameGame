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
            this.buttonBox = new Rectangle((int)pos.X, (int)pos.Y, 20, 20);
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
            if (game.gMode == 2 && game.activateButtons)
            {
                isColliding();
                if (!isInTitle)
                {
                    int scrollWheel = oldMouse.ScrollWheelValue - Mouse.GetState().ScrollWheelValue;
                    pos = new Vector2(pos.X, pos.Y - scrollWheel * 0.2f);
                    this.buttonBox = new Rectangle((int)pos.X, (int)pos.Y, (int)widthDiff + 10, (int)heightDiff + 10);
                    oldMouse = Mouse.GetState();
                }
            }
            else if (game.gMode==1|| game.gMode==6)
            {
                isColliding();
            }

        }

        public bool isColliding()
        {
            
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Rectangle mouseRect = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 1, 1);
            if (mouseRect.Intersects(buttonBox))
            {
                if (oldMouse.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
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
            if ((game.gMode == 2 && game.activateButtons && game.cameraWindowValue==0))
            {
                if (game.spriteBlockCounter == spritePos)
                {
                    spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X-2.5),
                 (int)(pos.Y-2.5),
                 (int)widthDiff + 5, (int)heightDiff + 5), null, Color.Black, 0, origin, SpriteEffects.FlipHorizontally, 0f);
                }
                spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                 (int)(pos.Y),
                 (int)widthDiff, (int)heightDiff), null, Color.White, 0, origin, SpriteEffects.FlipHorizontally, 0f);
                
            
            }
            if (game.gMode == 1 || game.gMode == 6)
            {
                origin = new Vector2(blockSprite.index.Width / 2f, blockSprite.index.Height/ 2f);
                float alpha = 1f;

                if (game.gameTitleValue == titleValue)
                    alpha = 0.5f;

                spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                 (int)(pos.Y),
                 (int)(blockSprite.index.Width * 0.45f * game.drawingTool.zoomRatio), (int)(blockSprite.index.Height * 0.45f*game.drawingTool.zoomRatio)), null, Color.White * alpha * game.fadeAlpha, 0, origin, SpriteEffects.None, 0f);
                


            }

        }


    }
}
