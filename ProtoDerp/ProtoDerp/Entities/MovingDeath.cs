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
    public class MovingDeath : Entity
    {
        public Sprite playerSprite;

        float playerAngle = 0;
        public Body body;
        public Fixture fixture;
        public float centerOffset;
        public Vector2 origin;
        public String spriteNumber;
        public Vector2 origPos;
        bool isDead = false;
        public Vector2 shootAngle;
        public float velObj;
        public Fixture origFix;
        public bool telePort = false;
        public MovingDeath(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber,Vector2 shootAngle,float velObj)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            LoadContent();
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            this.shootAngle = shootAngle;
            this.velObj = velObj;
            body.LinearVelocity = new Vector2(shootAngle.X * velObj, shootAngle.Y * velObj);
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (contact.IsTouching())
            {
                LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
                PlayableCharacter player = players.First();
                if (fixtureB == player.fixture)
                {
                    game.PlayerDies();
                }
                else
                {
                    //Destroy block
                    //SetUpPhysics(Constants.player1SpawnLocation+origPos);
                    //body.SetTransform(ConvertUnits.ToSimUnits(pos), 0);
                    //Vector2 possi=ConvertUnits.ToSimUnits(pos);
                    //body.SetTransformIgnoreContacts(ref possi, 0);
                    //body.Awake = true;
                    //body.ApplyForce(new Vector2(1, 0));
                    //body.LinearVelocity=new Vector2(1, 0);
                    telePort = true;
                    
                }
            }
            return true;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            float mass = 1;
            float width = playerSprite.index.Width;
            float height = playerSprite.index.Height;
            fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);
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
            //body.ApplyForce(new Vector2(shootAngle.X*velObj, shootAngle.Y*velObj));
            if (telePort)
            {
                telePort = false;
                body.SetTransform(ConvertUnits.ToSimUnits(pos), 0);
                body.LinearVelocity = new Vector2(shootAngle.X * velObj, shootAngle.Y * velObj);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, Color.White, body.Rotation, origin, SpriteEffects.None, 0f);

        }

        private void drawHealthBar()
        {

        }




    }
}
