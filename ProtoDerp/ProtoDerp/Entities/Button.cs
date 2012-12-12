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
            //if (blockSprite.index.Width < widthDiff)
            //    this.widthDiff = blockSprite.index.Width;
            //if (blockSprite.index.Height < heightDiff)
            //    this.heightDiff = blockSprite.index.Height;
            this.origin = new Vector2(1, 1);
        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {
            if (game.gMode==2)
            {
                if (isColliding())
                {
                    int k = 0;
                }
                int scrollWheel = oldMouse.ScrollWheelValue - Mouse.GetState().ScrollWheelValue;
                pos = new Vector2(pos.X, pos.Y-scrollWheel*0.2f);
                this.buttonBox = new Rectangle((int)pos.X, (int)pos.Y, (int)widthDiff+10, (int)heightDiff+10);
                oldMouse = Mouse.GetState();
            }

        }

        public bool isColliding()
        {
            
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Rectangle mouseRect = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 1, 1);
            if (mouseRect.Intersects(buttonBox))
            {
                game.isSelectingBlock = true;
                game.isButtonSelect = true;
                
                    game.spriteBlockCounter = spritePos;
                    
                
                return true;
            }
            return false;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (game.gMode==2)
            {
                spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                 (int)(pos.Y),
                 (int)widthDiff, (int)heightDiff), null, Color.White, 0, origin, SpriteEffects.FlipHorizontally, 0f);
            }

        }


    }
}
