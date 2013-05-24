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
    public class TempBlock : EntityBlock
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
        SpriteStripAnimationHandler ani;
        public float width, height;
        public float rotationAngle = 0;
        int lifeTime = 100;
        int life = 0;
        public TempBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            LoadContent();
            this.width = ani.widthOf();
            this.height = ani.heightOf();
            
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
            //fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            //fixture.OnSeparation += new OnSeparationEventHandler(OnSeparation);


        }

        void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {

            LinkedList<PlayableCharacter> players = game.getEntitiesOfType<PlayableCharacter>();
            PlayableCharacter player = players.First();
            if (fixtureB == player.fixture)
            {
                game.world.Gravity = new Vector2(0, 5.0f);
            }
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
                    game.world.Gravity = new Vector2(0, 2.5f);
                    fixtureB.CollisionFilter.IgnoreCollisionWith(fixtureA);
                    fixtureA.CollisionFilter.IgnoreCollisionWith(fixtureB);
                    game.inWater = true;
                    game.waterCollisionTime = 15;
                    return true;

                }
            }
            return false;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world3;
            float mass = 1;
            fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);
            body = fixture.Body;
            fixture.Body.BodyType = BodyType.Dynamic;
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
            ani = new SpriteStripAnimationHandler(new Sprite(game.Content, "bubbleSpriteSheet"), 4, 240);
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
            if (ani.getCycles() < 3)
            {

                ani.Update();
                body.Position = new Vector2(body.Position.X, body.Position.Y - 0.05f);

            }
            else
            {

                this.dispose = true;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (ani.getCycles() >= 3)
            {
                return;

            }

            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.DarkBlue;
            if (isSelected)
                drawColor = Color.Green;
            //spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, drawColor, body.Rotation, origin, SpriteEffects.None, 0f);
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y),
                    (int)width, (int)height), null, drawColor * 0.25f, body.Rotation, origin, SpriteEffects.None, 0f);
            }
            else
            {
                ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                       origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                           (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), true, new Vector2(0, 0));
            }
        }

        public void DrawShadow(GameTime gameTime, SpriteBatch spriteBatch)
        {
           
        }

        private void drawHealthBar()
        {

        }




    }
}
