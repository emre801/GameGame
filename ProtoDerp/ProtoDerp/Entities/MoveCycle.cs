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

/**
 * 
 * Main class that playable objects will inherit. Shares all common data and operations
 * that player "ships" can perform.
 * 
 */
namespace ProtoDerp
{
    public class MovingCycle : EntityBlock
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
        Vector2 path;
        bool moveDirection = true;
        Vector2 goalPoint;
        List<Vector2> pathPoints;
        int pathCounter = 0;
        Sprite blackSprite, blackCircleSprite, redCircleSprite, redSprite;
        public MovingCycle(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber, float velObj,
            List<Vector2> pathPoints, bool isDeath)
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
            this.isDeath = isDeath;
            this.pathPoints = pathPoints;
            goalPoint = ConvertUnits.ToSimUnits((Vector2)pathPoints[pathCounter]);
            body.Awake = true;
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (contact.IsTouching())
            {
                LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
                if (players.Count==0)
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

        public int getSizeOfPaths()
        {
            return pathPoints.Count;
        }

        public String getVectorsAsString()
        {
            String value = "";
            foreach (Vector2 v in pathPoints)
                value += " " + (int)v.X + " " + (int)v.Y;
            return value;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            if (game.gMode == 2)
                world = game.world2;
            float mass = 10000;
            this.width = ani.widthOf();
            this.height = ani.heightOf();
            if (this.spriteNumber.Equals("deathBall")
                || spriteNumber.Contains("circle")
                || spriteNumber.Contains("ahLogo"))
            {
                fixture = FixtureFactory.CreateCircle(world, (float)ConvertUnits.ToSimUnits(width) / 2, mass);

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
            if (!game.isInCreatorMode)
            {
                ani.Update();
                moveObject(gameTime);
                setMoveDirection();
            }
        }
        public void moveObject(GameTime gameTime)
        {
            
            Vector2 currentPosition = body.Position;
            Vector2 dir;
            dir = goalPoint - currentPosition;
            dir.Normalize();
            body.LinearVelocity = dir * velObj;
        }

        public void setMoveDirection()
        {
            double dist = distance();
            if (distance() < 1)
            {
                goalPoint = ConvertUnits.ToSimUnits(
                    (Vector2)pathPoints[pathCounter]);
                moveDirection = !moveDirection;
                pathCounter++;
                if (pathCounter == pathPoints.Count)
                    pathCounter = 0;
            }
        }

        public double distance()
        {
            return Math.Sqrt((goalPoint.X - body.Position.X) * (goalPoint.X - body.Position.X)
                + (goalPoint.Y - body.Position.Y) * (goalPoint.Y - body.Position.Y));

        }

        public void drawPath(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2)
        {
            int pixHeight = 10;
            int pixWeight = (int)Math.Sqrt(((p1.X - p2.X) * (p1.X - p2.X) +
                (p1.Y - p2.Y) * (p1.Y - p2.Y)));

            float Angle = (float)Math.Atan2(p1.Y - p2.Y, p1.X - p2.X) - (float)Math.PI;
            Vector2 midp = (p1 + p2) / 2f;
            int ballSize = 25;
            //spriteBatch.Draw(blackSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(p1.X), (int)ConvertUnits.ToDisplayUnits(p1.Y), (int)pixWeight, (int)pixHeight), null, Color.White, Angle, midp, SpriteEffects.None, 0f);
            spriteBatch.Draw(blackSprite.index, new Rectangle((int)p1.X, (int)p1.Y, (int)pixWeight, (int)pixHeight), null, Color.White, Angle, new Vector2(0, 0), SpriteEffects.None, 0f);

            spriteBatch.Draw(blackCircleSprite.index, new Rectangle((int)p1.X - ballSize / 2, (int)p1.Y - ballSize / 2, (int)ballSize, (int)ballSize), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(blackCircleSprite.index, new Rectangle((int)p2.X - ballSize / 2, (int)p2.Y - ballSize / 2, (int)ballSize, (int)ballSize), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);

            spriteBatch.Draw(redCircleSprite.index, new Rectangle((int)p1.X - ballSize / 4, (int)p1.Y - ballSize / 4, (int)ballSize / 2, (int)ballSize / 2), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(redCircleSprite.index, new Rectangle((int)p2.X - ballSize / 4, (int)p2.Y - ballSize / 4, (int)ballSize / 2, (int)ballSize / 2), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(redSprite.index, new Rectangle((int)p1.X, (int)p1.Y, (int)pixWeight, (int)pixHeight / 4), null, Color.White, Angle, new Vector2(0, 0), SpriteEffects.None, 0f);
            
        }

        public void drawAllPaths(SpriteBatch spriteBatch)
        {
            Vector2 prevPoint= new Vector2(0,0);
            Vector2 firstPoint = new Vector2(0, 0);
            bool firstTime = true;
            foreach (Vector2 point in pathPoints)
            {
                if (firstTime)
                {
                    firstTime = false;
                    firstPoint = point;
                    
                }
                else
                {
                    drawPath(spriteBatch, prevPoint, point);

                }
                prevPoint = point;
            }
            drawPath(spriteBatch, firstPoint, prevPoint);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            drawAllPaths(spriteBatch);
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            Point bottomRight = new Point((int)width, (int)height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            if (isSelected)
                drawColor = Color.Green;
            Vector2 v1 = goalPoint;
            Vector2 v2 = body.Position;

            //v2.Normalize();
            //v1.Normalize();

            float Angle = (float)Math.Atan2(v1.Y - v2.Y, v1.X - v2.X)-(float)Math.PI;
            float angleDegree = Angle / (float)Math.PI * 180f;

            SpriteEffects sEff = SpriteEffects.None;
            if (Math.Abs(angleDegree) > 90 && Math.Abs(angleDegree)<180)
            {
                sEff = SpriteEffects.FlipVertically;
            }

            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, drawColor, Angle, origin, sEff, 0f);
                
            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0, 0),Angle);
            }
        }

        private void drawHealthBar()
        {

        }




    }
}
