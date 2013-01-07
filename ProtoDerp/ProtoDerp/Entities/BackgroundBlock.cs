using System;
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


namespace ProtoDerp
{
    class BackgroundBlock :Block
    {
        public String spriteName;
        Sprite blockSprite;
        public Vector2 startPos;
        public BackgroundBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber, float height, float width)
            : base(g,a,pos,playerNum,spriteNumber,height,width,1,0)
        {
            this.spriteName = spriteNumber;
            this.height = height;
            this.width = width;
            this.startPos = pos;
            this.pos = pos+Constants.player1SpawnLocation;
            this.blockSprite = game.getSprite(spriteNumber);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(blockSprite.index.Width / 2, blockSprite.index.Height / 2);
            
            spriteBatch.Draw(blockSprite.index, new Rectangle((int)(pos.X),
                    (int)(pos.Y),
                    (int)(blockSprite.index.Width), (int)(blockSprite.index.Height)), null, Color.White, 0, origin, SpriteEffects.None, 0f);
           
        }


    }
}
