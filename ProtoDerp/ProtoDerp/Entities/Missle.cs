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
    public class Missle : EntityBlock
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
        Vector2 target;
        Vector2 tarDir;
        int pathCounter = 0;
        bool hasFired = false;
        float fireAngle;
        int fireTime = 0;
        int fireDistance = 20;
        public Missle(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber, float velObj,
            bool isDeath)
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
            body.Awake = true;
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (IsVisible)
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
                    else
                    {
                        explosion();

                    }
                }
            }
            return true;
        }

        public override void removeItself()
        {
            body.Dispose();
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
            playerSprite = game.getSprite("nonFireMissle");
            ani = new SpriteStripAnimationHandler(new Sprite(game.Content, "missile_strip_strip4"),4,45);
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

        public void explosion()
        {
            body.Dispose();
            IsVisible = false;
            this.dispose = true;
            game.addEntity(new Missle(game, game.Arena, origPos, 1, spriteNumber, velObj, true));
                    

        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {
            if (IsVisible)
            {
                if (!game.isInCreatorMode)
                {
                    if(hasFired)
                        ani.Update();
                    moveObject(gameTime);

                }
            }
        }
        public void moveObject(GameTime gameTime)
        {
            if (!hasFired)
            {
                if (distance() < fireDistance)
                {
                    if (tarDir.X != 0 && tarDir.Y != 0)
                    {
                        hasFired = true;
                        /*Vector2 currentPosition = body.Position;
                        Vector2 dir;
                        dir = target - currentPosition;
                        dir.Normalize();*/
                        body.LinearVelocity = tarDir * velObj;
                    }
                }
                    target = game.Arena.player1.body.Position;
                    tarDir = target - body.Position;
                    tarDir.Normalize();
                    Vector2 v1 = target;
                    Vector2 v2 = body.Position;
                    fireAngle = (float)Math.Atan2(v1.Y - v2.Y, v1.X - v2.X) - (float)Math.PI;

                
            }
            else
            {
                fireTime++;
                //explodes if it's been flying for a certain time
                if (fireTime > 100)
                {
                    explosion();
                }



            }
        }

        public double distance()
        {
            Vector2 goalPoint=game.Arena.player1.body.Position;
            return Math.Sqrt((goalPoint.X - body.Position.X) * (goalPoint.X - body.Position.X)
                + (goalPoint.Y - body.Position.Y) * (goalPoint.Y - body.Position.Y));

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            Point bottomRight = new Point((int)width, (int)height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            if (isSelected)
                drawColor = Color.Green;
            Vector2 v1 = target;
            Vector2 v2 = body.Position;

            //v2.Normalize();
            //v1.Normalize();

            float Angle = fireAngle;
            float angleDegree = Angle / (float)Math.PI * 180f;

            SpriteEffects sEff = SpriteEffects.None;
            

            if (ani.getStateCount() == 1 || !hasFired)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, drawColor, Angle, origin, sEff, 0f);

            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0, 0), Angle);
            }
        }

        private void drawHealthBar()
        {

        }




    }
}
