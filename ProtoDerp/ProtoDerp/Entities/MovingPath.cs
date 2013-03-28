﻿using System;
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
    public class MovingPath : EntityBlock
    {
        public Sprite playerSprite;

        
        public Body body;
        public Fixture fixture;
        public float centerOffset;
        public Vector2 origin;
        public String spriteNumber;
        public Vector2 origPos;
        bool isDeath = false;
        public float velObj;
        public Fixture origFix;
        SpriteStripAnimationHandler ani;
        float width, height;
        public Vector2 point1, point2;
        Vector2 path;
        bool moveDirection = true;
        Vector2 validPoint;
        Vector2 goalPoint;
        Sprite blackSprite,blackCircleSprite,redCircleSprite,redSprite;
        public MovingPath(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber, float velObj,
            Vector2 point1,Vector2 point2, bool isDeath)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            LoadContent();
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            this.velObj = velObj;
            //if (game.gMode == 0)
                //body.LinearVelocity = new Vector2(shootAngle.X * velObj, shootAngle.Y * velObj);
            this.isDeath = isDeath;
            this.point1 = point1;
            this.point2 = point2;
            double angel = Math.Atan2((double)(point2.Y - point1.Y), (double)(point2.X - point1.X)) * 180/Math.PI;
            float x = (float)Math.Cos(angel*Math.PI/180f);
            //if (x < 0.000000000001f)
                //x = 0;
            float y = (float)Math.Sin(angel*Math.PI/180f);
            //if (y < 0.000000000001f)
                //y = 0;
            ///this.path = point1 - point2;// new Vector2(x * 2, -y * 2);
            this.path = new Vector2(x, y);
            this.validPoint = ConvertUnits.ToSimUnits(point1);
            //body.ApplyLinearImpulse(path);
            goalPoint = ConvertUnits.ToSimUnits(point2);
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (contact.IsTouching())
            {
                LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
                if (players.Count == 0)
                    return false;

                PlayableCharacter player = players.First();
                if (fixtureB == player.fixture)
                {
                    if (game.gMode != 2)
                    {
                        game.sounds["Rage//Wave//explosion"].Play();
                        game.PlayerDies();
                    }
                }
            }
            return true;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            if (game.gMode == 2)
                world = game.world2;
            float mass = 1;
            this.width = ani.widthOf();
            this.height = ani.heightOf();
            if (this.spriteNumber.Equals("deathBall")
                || spriteNumber.Contains("circle")
                || spriteNumber.Equals("SawAni"))
            {
                fixture = FixtureFactory.CreateCircle(world, (float)ConvertUnits.ToSimUnits(width)/2,  mass);

            }

            else
            {
                fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);


            }
            this.origFix = fixture;
            body = fixture.Body;
            fixture.Body.BodyType = BodyType.Dynamic;
            fixture.Restitution = 0.3f;
            fixture.Friction = 0.1f;
            body.Position = ConvertUnits.ToSimUnits(position);
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y); //remember the offset from the center for drawing
            body.IgnoreGravity = true;
            body.FixedRotation = true;
            body.LinearDamping = 0.0f;
            body.AngularDamping = 0f;
            //body.LinearVelocity = new Vector2(shootAngle.X*velObj, shootAngle.Y*velObj);
        }

        //private Vector3[] baseHB = new Vector3[Constants.HEALTH_BAR_SEGMENT_COUNT];
        private void initializePrimitives()
        {

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
            blackSprite = game.getSprite("black");
            blackCircleSprite = game.getSprite("blackCircle");
            redCircleSprite = game.getSprite("redCircle");
            redSprite = game.getSprite("red");
        }

        public bool isExpanding()
        {
            return false;
        }

        public bool isPreviewing()
        {
            return true;
        }


        public void tutorialReset()
        {

        }

        public void damage(float health, int invinTime)
        {

        }

        public void updateHealthBar()
        {

        }

        public bool isInvincible()
        {
            return false;
        }

        public void expandingSquare(GameTime gameTime)
        {

        }



        public override void Update(GameTime gameTime, float worldSpeed)
        {
            ani.Update();
            if (game.gMode != 2)
            {
                //setMoveDirection();
                moveObject(gameTime);
                setMoveDirection();
            }
            
            
        }
        public void moveObject(GameTime gameTime)
        {
            Vector2 velo;
            Vector2 currentPosition = body.Position;
            
            double angel = Math.Atan2((double)(goalPoint.Y - currentPosition.Y), (double)(goalPoint.X - currentPosition.X)) * 180 / Math.PI;
            float x = (float)Math.Cos(angel * Math.PI / 180f);
            float y = (float)Math.Sin(angel * Math.PI / 180f);

            Vector2 dir = new Vector2(x, y);
            dir = goalPoint - currentPosition;
            dir.Normalize();
            body.LinearVelocity=dir*velObj;
        }

        public void setMoveDirection()
        {
            double dist = distance();
            if (distance() < 1)
            {
                if(moveDirection)
                    goalPoint = ConvertUnits.ToSimUnits(point2);
                else
                    goalPoint = ConvertUnits.ToSimUnits(point1);
                moveDirection = !moveDirection;
            }
        }

        public double distance()
        {
            return Math.Sqrt((goalPoint.X - body.Position.X) * (goalPoint.X - body.Position.X)
                + (goalPoint.Y - body.Position.Y) * (goalPoint.Y - body.Position.Y));

        }
        public bool isBetween()
        {
            
            Vector2 a = ConvertUnits.ToSimUnits(point1);
            Vector2 b = ConvertUnits.ToSimUnits(point2);
            Vector2 c= body.Position;
            float crossProduct = (c.Y - a.Y) * (b.X - a.X) - (c.X - a.X) * (b.Y - a.Y);
            if (Math.Abs(crossProduct) == 0)
                return false;
            float dotProduct = (c.X - a.X) * (b.X - a.X) + (c.Y - a.Y) * (b.Y - a.Y);
            if (dotProduct < 0)
                return false;

            float squaredLengthAB = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
            if (dotProduct > squaredLengthAB)
                return false;

            return true;
        }
        public void drawPath(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2)
        {
            int pixHeight = 10;
            int pixWeight = (int)Math.Sqrt(((p1.X - p2.X) * (p1.X - p2.X) +
                (p1.Y - p2.Y) * (p1.Y - p2.Y)));

            float Angle = (float)Math.Atan2(p1.Y - p2.Y, p1.X - p2.X) - (float)Math.PI;
            Vector2 midp = (p1 + p2) / 2f;
            int ballSize = 25;

            float xDiff = (float)(Math.Cos(Angle) * (pixWeight / 2)) + (float)(Math.Sin(Angle) * (pixHeight / 2));
            float yDiff = (float)(Math.Sin(Angle) * (pixHeight / 2)) + (float)(Math.Cos(Angle) * (pixWeight / 2)); ;

            //spriteBatch.Draw(blackSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(p1.X), (int)ConvertUnits.ToDisplayUnits(p1.Y), (int)pixWeight, (int)pixHeight), null, Color.White, Angle, midp, SpriteEffects.None, 0f);
            spriteBatch.Draw(blackSprite.index, new Rectangle((int)p1.X, (int)p1.Y, (int)pixWeight, (int)pixHeight), null, Color.White, Angle, new Vector2(0, 0), SpriteEffects.None, 0f);

            spriteBatch.Draw(blackCircleSprite.index, new Rectangle((int)p1.X - ballSize / 2, (int)p1.Y - ballSize / 2, (int)ballSize, (int)ballSize), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(blackCircleSprite.index, new Rectangle((int)p2.X - ballSize / 2, (int)p2.Y - ballSize / 2, (int)ballSize, (int)ballSize), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);

            spriteBatch.Draw(redCircleSprite.index, new Rectangle((int)p1.X - ballSize / 4, (int)p1.Y - ballSize / 4, (int)ballSize / 2, (int)ballSize / 2), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(redCircleSprite.index, new Rectangle((int)p2.X - ballSize / 4, (int)p2.Y - ballSize / 4, (int)ballSize / 2, (int)ballSize / 2), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(redSprite.index, new Rectangle((int)p1.X, (int)p1.Y, (int)pixWeight, (int)pixHeight / 4), null, Color.White, Angle, new Vector2(0, 0), SpriteEffects.None, 0f);
            
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            drawPath(spriteBatch,point1,point2);


            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            //int i = playerSprite.index.Width;
            Point bottomRight = new Point((int)width, (int)height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            if (isSelected)
                drawColor = Color.Green;
            //spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0, 0));
            }
            //if(game.gMode==2)
                //game.drawingTool.DrawLine(spriteBatch, 2, Color.Yellow, point1, point2);


        }

        private void drawHealthBar()
        {

        }




    }
}
