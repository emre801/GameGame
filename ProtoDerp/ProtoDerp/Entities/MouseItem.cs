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
using FarseerPhysics.Dynamics.Contacts;


namespace ProtoDerp
{
    class MouseItem : Entity
    {
        Vector2 mousePosition;
        public MouseItem(Game g, Arena a, Vector2 pos, int playerNum)
            : base(g)
        {
            MouseState mouseState = Mouse.GetState();
            mousePosition = new Vector2(mouseState.X, mouseState.Y);
        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
