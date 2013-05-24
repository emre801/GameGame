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
    class LayerBlock : Block
    {
        public String spriteName;
        Sprite blockSprite;
        public Vector2 startPos;
        float rotation;
        int owner;
        SpriteStripAnimationHandler ani;
        bool checkForIntersects = true;
        World world;
        public LayerBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber, float height, float width,float rotation,int ownerBlock)
            : base(g, a, pos, playerNum, spriteNumber, height, width, 1, 0)
        {
            this.pos = Constants.player1SpawnLocation;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            this.rotationAngle = rotation;
            this.width = width;
            this.height = height;
            LoadContent();
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(width / 2, height / 2);
            this.owner = ownerBlock;
            this.drawLevel = 100;
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            
        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            if (checkForIntersects)
            {
                interSectsOtherBlock();
            }
            
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (contact.IsTouching())
            {
                
            }
            return true;
        }

        public void LoadContent()
        {
            playerSprite = game.getSprite(spriteNumber);
            ani = game.getSpriteAnimation(spriteNumber);
        }
        protected override void SetUpPhysics(Vector2 position)
        {
            world = game.world3;
            float mass = 1;
            fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);
            body = fixture.Body;
            fixture.Body.BodyType = BodyType.Static;
            fixture.Restitution = 0.3f;
            fixture.Friction = 0.1f;
            body.Position = ConvertUnits.ToSimUnits(position);
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y); //remember the offset from the center for drawing
            body.IgnoreGravity = true;
            body.FixedRotation = true;
            body.LinearDamping = 0.5f;
            body.AngularDamping = 1f;
            body.Rotation = rotationAngle * (float)Math.PI / 180f;

        }

        public void interSectsOtherBlock()
        {
            List<Entity> entities = game.entities;
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    
                        Block b = (Block)e;
                        if (e is LayerBlock)
                        {
                            continue;
                        }
                            if (b.blockNumber != owner && b.spriteNumber.Equals("bigBlock")&& b.drawLevel==0)
                            {
                                Rectangle bBlock = new Rectangle((int)b.Position.X, (int)b.Position.Y, (int)b.width, (int)b.height);
                                Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, (int)width, (int)height);
                                if (bBlock.Contains(rect))
                                {
                                    IsVisible = false;
                                }
                                if (bBlock.Intersects(rect))
                                {
                                    IsVisible = false;
                                }
                            }
                        
                    
            
                }
            }
            checkForIntersects = false;

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;
            
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            //spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y),
                    (int)width, (int)height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0, 0));
            }

        }


    }
}
