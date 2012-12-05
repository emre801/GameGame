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
    public class MagnetBlock : Entity
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
        bool isMagnet = true;
        public Vector2 magnetPulse;
        public float height, width;
        bool isInMagnet = false;
        public MagnetBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber,Vector2 magnetPulse, float height, float width)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            this.width = width;
            this.height = height;
            LoadContent();
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            fixture.OnSeparation += new OnSeparationEventHandler(OnSeparation);

            this.magnetPulse = magnetPulse;
            
            //game.Arena.player1.fixture.CollisionFilter.IgnoreCollisionWith(this.fixture);
        }
        void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
            PlayableCharacter player = players.First();
            isInMagnet = false;
            player.body.LinearVelocity = new Vector2(0, 0);
            //player.body.IgnoreGravity = false;

        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (contact.IsTouching())
            {
                LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
                PlayableCharacter player = players.First();
                if (fixtureB == player.fixture)
                {
                    player.body.ApplyLinearImpulse(magnetPulse);
                    if (!isInMagnet)
                    {
                        if(magnetPulse.X!=0)
                            player.body.LinearVelocity = new Vector2(0, body.LinearVelocity.Y);
                        else
                            player.body.LinearVelocity = new Vector2(body.LinearVelocity.X, 0);
                        isInMagnet = true;
                    }
                    //player.body.IgnoreGravity = true;
                    return !isMagnet;
                }
            }
            return true;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            float mass = 1;
            float width = this.width;//playerSprite.index.Width;
            float height = this.height;// playerSprite.index.Height;
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
        }

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

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.White;
            if (game.isInCreatorMode)
            {
                if(isSelected)
                    drawColor = Color.Green;
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)this.width, (int)this.height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            }
        }

        private void drawHealthBar()
        {

        }




    }
}

