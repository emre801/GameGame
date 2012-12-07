﻿using System;
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
    public class Block : Entity
    {
        public Sprite playerSprite;

        float playerAngle = 0;
        public Body body;
        public Fixture fixture;
        public float centerOffset;
        public Vector2 origin;
        public String spriteNumber;
        public Vector2 origPos;
        public float height, width;
        public int drawLevel = 0;
        public int disappearTimer = -1,maxDisplay=-1;
        public bool isDisappearing = false;
        public Stopwatch stopWatch = new Stopwatch();
        public float displayAlpha = 1;
        float fadeOutRatio=0;
        SpriteStripAnimationHandler ani;
        public float rotationAngle = 0;
        Texture2D dynamicPattern;
        float heightDiff = 0,widthDiff=0;
        Random r = new Random(801);
        public Block(Game g, Arena a, Vector2 pos, int playerNum,String spriteNumber,float height, float width,int drawLevel, float rotation)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            this.height = height;
            this.width = width;
            LoadContent();
            this.drawLevel = drawLevel;
            this.rotationAngle = rotation;
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
        }

        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {            
            if (contact.IsTouching())
            {
                if (disappearTimer < 0)
                    return true;
                if (!isDisappearing)
                {
                    stopWatch.Start();
                    isDisappearing = true;
                }
            }
            return true;
        }
        public void setdisAppearTimer(int disappearTimer)
        {
            this.disappearTimer = disappearTimer;
            this.maxDisplay = disappearTimer;
            this.fadeOutRatio = 1f / (float)disappearTimer;

        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            if (drawLevel == 1)
                world = game.world2;
            if (drawLevel == 2)
                world = game.world3;
            float mass = 1000;
            float width = this.width;
            float height = this.height;            
            fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);
            body = fixture.Body;
            fixture.Body.BodyType = BodyType.Kinematic;
            fixture.Restitution = 0.3f;
            fixture.Friction = 0.1f;
            body.Position = ConvertUnits.ToSimUnits(position);
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y); //remember the offset from the center for drawing
            body.IgnoreGravity = true;
            body.FixedRotation = true;
            body.LinearDamping = 0.5f;
            body.AngularDamping = 1f;
            body.Rotation = rotationAngle*(float)Math.PI/180f; 
            
        }
        
        public Vector2 Position
        {
            get
            {
                return (ConvertUnits.ToDisplayUnits(body.Position) + Vector2.UnitY * centerOffset);
            }
        }
        public void addEnemyPlayer(PlayableCharacter enemy)
        {

        }

        public void LoadContent()
        {
            playerSprite = game.getSprite(spriteNumber);
            ani = game.getSpriteAnimation(spriteNumber);
            Color[] cData = new Color[(int)(width * height)];
            if (spriteNumber.Equals("bigBlock")&& width<4000 && height<4000)
            {
                dynamicPattern = game.getCachedDirt(new Rectangle(1,1,(int)width, (int)height));
                
                if (dynamicPattern == null)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            cData[(int)(x + y * width)] = new Color(89, 61, 41);
                        }
                    }


                    for (int x = 0; x < width; x=x+2)
                    {
                        for (int y = 0; y < height; y=y+2)
                        {
                            float br = r.Next(255/2)+(255*0.9f);
                            
                                
                                    //Play with so that it looks like dirt :)
                            if (r.Next(100)<75 )
                            {
                                if(x!=0)
                                cData[(int)(x + y * width)] = cData[(int)(x-1 + y * width)]; 
                            }
                            else
                            {
                                cData = drawPixel(2, x, y, cData, new Color(89, 61, 41) * (br / 252f));
                            }
                                
                            
                        }
                    }
                    this.dynamicPattern = new Texture2D(game.drawingTool.getGraphicsDevice(), (int)width, (int)height);
                    dynamicPattern.SetData<Color>(cData);
                    game.addCachedDirt(new Rectangle(1, 1, (int)width, (int)height), dynamicPattern);
                }
                heightDiff = playerSprite.index.Height - height;
                widthDiff = playerSprite.index.Width - width;
                //origin = new Vector2(width / 2, height / 2);
            }
        }

        public Color[] drawPixel(int point, int x, int y, Color[] cData, Color color)
        {
            for (int x1 = x-point; x1 < point+x; x1++)
            {
                for (int y1 = y-point; y1 < point+y; y1++)
                {
                    if (x1 > 0 && x1 < width - 1 && y1 > 0 && y1 < height - 1)
                    {
                        cData[(int)(x1 + y1 * width)] = color;
                    }
                }

            }

            return cData;


        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            //Texture2D test=sprite.index;
            //test.SetData<Color>(Color[])

            ani.Update();
            if (disappearTimer == 0)
            {
                isDisappearing = false;
                this.fixture.CollisionFilter.IgnoreCollisionWith(game.Arena.player1.fixture);
                game.Arena.player1.fixture.CollisionFilter.IgnoreCollisionWith(this.fixture);
            }
            

            if (isDisappearing)
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;                
                stopWatch.Start();                
                if(ts.CompareTo(new TimeSpan(0,0,1))>0)
                {
                    stopWatch.Reset();
                    disappearTimer--;
                    displayAlpha -= fadeOutRatio;
                }
            }

            if (drawLevel == 1)
            {
                if(game.moveBackGround.X!=0)
                    body.Position += new Vector2(game.moveBackGround.X / 100000f, game.moveBackGround.Y / 300000f);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            if (isSelected)
                drawColor = Color.Green;

            if (ani.getStateCount() == 1)
            {

                if(spriteNumber.Equals("bigBlock")&& width<4000 && height<4000)
                {
                    spriteBatch.Draw(dynamicPattern, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X+ConvertUnits.ToSimUnits(widthDiff/2f)),
                        (int)ConvertUnits.ToDisplayUnits(body.Position.Y+ConvertUnits.ToSimUnits(heightDiff/2f)), (int)dynamicPattern.Width, (int)dynamicPattern.Height), null, drawColor * displayAlpha, body.Rotation, origin, SpriteEffects.None, 0f);
                
                }
                else
                {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, drawColor * displayAlpha, body.Rotation, origin, SpriteEffects.None, 0f);
                }
            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0,0));
            }

            //ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
            //       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
            //           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0,0));
            //return;    
            //spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, drawColor * displayAlpha, body.Rotation, origin, SpriteEffects.None, 0f);
            //ani.drawCurrentState(spriteBatch,this, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),(int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height),body,origin);
        }
        public void DrawShadow(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.Black;
            
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X) + 5, (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 5, (int)width, (int)height), null, drawColor * 0.25f, body.Rotation, origin, SpriteEffects.None, 0f);
            }
            else
            {
                this.blend=Color.Black;
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(5, -5));
                this.blend= Color.White;
            }
        }
    }
}
