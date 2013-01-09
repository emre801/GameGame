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
using FarseerPhysics.Dynamics.Joints;

/**
 * 
 * Main class that playable objects will inherit. Shares all common data and operations
 * that player "ships" can perform.
 * 
 */
namespace ProtoDerp
{
    public class PlayableCharacter : Entity
    {
        // public PlayableCharacter(Game g, Arena a, Vector2 pos, int playerNum)

        public Sprite playerSprite;

        public enum Modes { GROUND, RUNNING1, RUNNING2, AIRUP, AIRDOWN, WALL, WAITING }

        public Modes modes;

        public Boolean onGround = false;

        public Input inputState = null;
        float playerAngle = 0;
        public Body body;
        public Fixture fixture;
        public float centerOffset;
        public Vector2 origin;
        public float maxSpeedOnGround = 10;
        public Vector2 maxSpeedInAir;
        bool faceRight = true;
        int running = 0;
        float yOld = 0f, yNew = 0f;
        int step = 0;
        float distance = 0;
        bool run = false;
        float max = 0;

        public Fixture wheel;
        public FixedAngleJoint fixedAngleJoint;
        public RevoluteJoint motor;
        SpriteStripAnimationHandler ani;
        KeyboardInput keyInput;
        Vector2 shiftPosition;

        bool isOnWall = false;
        int wallJumpCount = 0;
        bool jumpDirection = false;
        bool isFirstAni = false;

        bool mikeStandingStill = false;

        public int frameRate = 5;
        public int framCounter = 0;
        bool doAnimation = false;
        bool isWallOnRight = false;
        bool isOnJumpAnimation = false;
        public PlayableCharacter(Game g, Arena a, Vector2 pos, int playerNum)
            : base(g)
        {
            this.pos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            LoadContent();
            SetUpPhysics(pos);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            fixture.OnSeparation += new OnSeparationEventHandler(OnSeparation);

            //this.origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
            this.origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
            modes = Modes.AIRDOWN;
            shiftPosition = new Vector2(0, 0);
            yOld = body.Position.X;
            run = false;
            if (g.isInCreatorMode)
                body.IgnoreGravity = true;
            keyInput = new KeyboardInput();


        }
        void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (modes == Modes.WALL)
            {
                modes = Modes.AIRDOWN;
            }
            //modes = Modes.AIRUP;
            //body.IgnoreGravity = false;
            //onGround = false;
        }
        bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Vector2 movementBefore = body.LinearVelocity;
            body.LinearVelocity = new Vector2(body.LinearVelocity.X, 0);

            if (contact.IsTouching())
            {
                if (isMagnet(fixtureB))
                    return true;
                Manifold colis = new Manifold();
                contact.GetManifold(out colis);
                Vector2 pColis = colis.LocalNormal;
                if (pColis.X == 0 && pColis.Y == 1)
                {
                    onGround = true;
                    modes = Modes.GROUND;
                    return true;

                }
                if (pColis.X != 0 && pColis.Y == 0)
                {
                    if (onGround)
                    {
                        modes = Modes.WALL;
                        return false;
                    }

                    float direction = inputState.GetJoyDirection();
                    float x = (float)Math.Sin(direction);
                    if (pColis.X > 0)
                        isWallOnRight = true;
                    else
                        isWallOnRight = false;

                    float xMomentum = 0;
                    if (movementBefore.Y > 0)
                    {
                        xMomentum = Math.Abs(body.LinearVelocity.X * 0.3f);
                    }
                    else if (movementBefore.Y < 0)
                    {
                        xMomentum = -Math.Abs(body.LinearVelocity.X * 0.8f);
                    }
                    XboxInput xbi = (XboxInput)inputState;
                    if (xbi.getYDirection() < 0 || Keyboard.GetState().IsKeyDown(Keys.Up))
                        xMomentum += xbi.getYDirection() * 4f;
                    else
                        xMomentum = 0;
                    body.LinearVelocity = new Vector2(0, body.LinearVelocity.Y + xMomentum);
                    //body.IgnoreGravity = true;
                    onGround = true;
                    onGround = true;
                    modes = Modes.WALL;
                    return false;
                }

            }
            return true;
        }

        public bool isMagnet(Fixture fixture)
        {
            LinkedList<MagnetBlock> magnets = game.getEntitiesOfType<MagnetBlock>();

            foreach (MagnetBlock i in magnets)
            {
                if (i.fixture.Equals(fixture))
                    return true;
            }
            return false;
        }

        protected virtual void SetUpPhysics(Vector2 position)
        {
            World world = game.world;
            float mass = 0.8f;
            float width = game.getSpriteAnimation("player_strip12").widthOf();
            float height = game.getSpriteAnimation("player_strip12").heightOf();

            fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(height), mass);
            body = fixture.Body;
            fixture.Body.BodyType = BodyType.Dynamic;
            fixture.Restitution = 0.3f;
            fixture.Friction = 1f;
            body.Position = ConvertUnits.ToSimUnits(position);
            centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y); //remember the offset from the center for drawing
            body.IgnoreGravity = false;
            body.FixedRotation = true;
            body.AngularDamping = 1f;
        }
        public void LoadContent()
        {
            playerSprite = game.getSprite("MikeRun1");
            ani = game.getSpriteAnimation("player_strip12");
        }

        public bool isExpanding()
        {
            return false;
        }

        public bool isPreviewing()
        {
            return true;
        }

        public bool isInvincible()
        {
            return false;
        }



        public Vector2 Position
        {
            get
            {
                return (ConvertUnits.ToDisplayUnits(body.Position) + Vector2.UnitY * centerOffset);
            }
        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            keyInput.Update(gameTime);
            if (game.deathAnimation)
            {
                playerSprite = game.getSprite("fire1");
                return;

            }
            if (keyInput.IsNewKeyPressed(Keys.Enter))
            {
                game.isPausePressed = true;
            }
            if (keyInput.IsKeyPressed(Keys.Up))
            {
                game.isUpMenuSelect = 1;
            }
            if (keyInput.IsKeyPressed(Keys.Down))
            {
                game.isUpMenuSelect = 2;
            }
            ani.Update();
            doAnimation = false;
            shiftPosition = new Vector2(0, 0);

            float direction = inputState.GetJoyDirection();
            float x = (float)Math.Sin(direction);
            float y = (float)Math.Cos(direction) - 1;
            //if (body.LinearVelocity.X < 3 && body.LinearVelocity.X > -3)
            //body.ApplyLinearImpulse(new Vector2(inputState.getXDirection() * 2f, 0));// inputState.getYDirection() * 300f));

            float xDirection = inputState.getXDirection();
            xDirection += keyInput.HorizontalMovement();
            float yDirection = inputState.getYDirection();
            yDirection -= keyInput.VerticalMovement();
            float runningValue = 1.5f;
            if (isOnJumpAnimation)
                xDirection = 0;
            distance = (float)(distance + gameTime.ElapsedGameTime.TotalMilliseconds * 0.002f * Math.Abs(body.LinearVelocity.X));
            if (distance > 0.9f)
            {
                distance = 0;
                run = !run;
            }
            //Ignores input if it's in creator mode
            if (game.isInCreatorMode && !game.testLevel)
            {
                body.IgnoreGravity = true;
                body.LinearVelocity = new Vector2(0, 0);
                return;
            }

            if (inputState.IsButtonPressed(Buttons.RightShoulder) || keyInput.IsKeyPressed(Keys.Z))
                runningValue = 2.5f;


            if ((xDirection > 0 && body.LinearVelocity.X < 0) || (xDirection < 0 && body.LinearVelocity.X > 0))
            {
                body.LinearVelocity = new Vector2(body.LinearVelocity.X * 0.88f, body.LinearVelocity.Y);

            }

            if (onGround)
            {
                if ((Math.Abs(body.LinearVelocity.X) < 10 * runningValue))
                {
                    body.ApplyLinearImpulse(new Vector2(xDirection * 0.45f * runningValue, 0));// inputState.getYDirection() * 300f));
                }
                if (xDirection == 0)
                    fixture.Friction = 80;
                else
                    fixture.Friction = 0;
            }
            else
            {

                body.ApplyLinearImpulse(new Vector2(xDirection * 0.95f * runningValue, 0));// inputState.getYDirection() * 300f));
                //motor.MotorSpeed = inputState.getXDirection()*10;
                if (inputState.getXDirection() == 0)
                    fixture.Friction = 10;
                else
                    fixture.Friction = 0;

                if (yDirection > 0)
                    body.ApplyLinearImpulse(new Vector2(0, yDirection * 1.05f));
                else
                    body.ApplyLinearImpulse(new Vector2(0, yDirection * 0.12f));
            }

            if ((inputState.isAPressed() || keyInput.IsNewKeyPressed(Keys.Space)) && onGround)// && body.LinearVelocity.Y > -50)
            {
                body.IgnoreGravity = true;
                float oldYvalue = body.LinearVelocity.Y;
                if (oldYvalue > 0)
                    oldYvalue = 0;
                body.LinearVelocity = new Vector2(body.LinearVelocity.X, 0);
                modes = Modes.AIRUP;
                body.ApplyLinearImpulse(new Vector2(0, -30f+oldYvalue));
                if (isOnWall)
                {
                    if (jumpDirection)
                    {
                        body.ApplyLinearImpulse(new Vector2(-7.75f * runningValue, 0f));
                    }
                    else
                    {
                        body.ApplyLinearImpulse(new Vector2(7.75f * runningValue, 0f));
                    }

                }
                body.IgnoreGravity = false;
                onGround = false;
                maxSpeedInAir = body.LinearVelocity;
                game.sounds["Rage//Wave//jump"].Play();
                //modes = Modes.AIRUP;
            }

            if ((inputState.IsNewButtonReleased(Buttons.A) || keyInput.IsNewKeyReleased(Keys.Space)) && body.LinearVelocity.Y < -1)
            {
                body.LinearVelocity = new Vector2(body.LinearVelocity.X, body.LinearVelocity.Y / 2f);
                

            }
            if (body.LinearVelocity.X > 0)
            {
                faceRight = true;
            }
            if (body.LinearVelocity.X < 0)
            {
                faceRight = false;
            }

            if (body.LinearVelocity.Y < -1)
            {
                //playerSprite = game.getSprite("SpaceAir");
            }
            else
            {
                //playerSprite = game.getSprite("Space");
            }

            mikeStandingStill = false;

            if (onGround)//modes == Modes.GROUND)
            {
                //TODO: add running

                XboxInput xbi = (XboxInput)inputState;
                if (Math.Abs(body.LinearVelocity.X) > 0.00005 || Math.Abs(xbi.getXDirection()) > 0)
                {
                    if (run)
                        playerSprite = game.getSprite("MikeRun1");
                    else
                        playerSprite = game.getSprite("MikeRun2");
                    if (Math.Abs(body.LinearVelocity.X) < 5)
                    {
                        ani = game.getSpriteAnimation("sprite15_strip4");
                        ani.addSound(game.sounds["Rage//Wave//running"]);
                        ani.changeSoundRate(8);
                        int frameRate = 120 * (int)(5f / Math.Abs(body.LinearVelocity.X));
                        ani.changeFrameRate(frameRate);
                    }
                    else
                    {
                        ani = game.getSpriteAnimation("sprite16_strip6");
                        ani.addSound(game.sounds["Rage//Wave//running"]);
                        ani.changeSoundRate(6);
                        int frameRate = 30 * (int)(15f / Math.Abs(body.LinearVelocity.X));
                        ani.changeFrameRate(frameRate);
                    }
                    doAnimation = true;
                    isOnWall = false;
                    isOnJumpAnimation = false;
                }
                if (body.LinearVelocity.X == 0)
                {
                    playerSprite = game.getSprite("MikeStand");
                    mikeStandingStill = true;
                    ani = game.getSpriteAnimation("player_strip12");
                    doAnimation = true;
                    isOnWall = false;
                    isOnJumpAnimation = false;
                }
                if (xbi.IsButtonPressed(Buttons.LeftShoulder))
                {

                    body.LinearVelocity = new Vector2(body.LinearVelocity.X * 0.95f, body.LinearVelocity.Y);
                }

                // modes = Modes.WAITING;
            }
            if (Math.Abs(body.LinearVelocity.Y) >= 0.0001f)
            {
                if (!isOnWall)
                {
                    if (body.LinearVelocity.Y > 0.05f)
                    {
                        ani = game.getSpriteAnimation("sprite17");
                    }
                    if (body.LinearVelocity.Y < -0.05f)
                    {
                        ani = game.getSpriteAnimation("sprite17-2");
                    }
                    wallJumpCount = 0;
                }
                else
                {
                    ani = game.getSpriteAnimation("sprite18_strip4");
                    if (isFirstAni)
                    {
                        ani.setStatePub(0);
                        isFirstAni = false;
                    }

                    wallJumpCount++;
                    faceRight = jumpDirection;
                    if (ani.getCycles() >= 3)
                    {
                        isOnWall = false;
                        isOnJumpAnimation = false;
                        faceRight = !jumpDirection;
                    }


                }
            }
            if (modes == Modes.WALL)
            {
                isOnJumpAnimation = true;
                playerSprite = game.getSprite("MikeWall");
                fixture.Friction = 80;
                ani = game.getSpriteAnimation("sprite14_strip9");
                doAnimation = true;
                shiftPosition = new Vector2(8, 0);
                isOnWall = true;
                jumpDirection = isWallOnRight;// faceRight;
                faceRight = jumpDirection;
                isFirstAni = true;
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (game.deathAnimation || game.winningAnimation)
                return;

            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            float width = playerSprite.index.Width;
            float height = playerSprite.index.Height;
            float upperBodyHeight = height - (width / 2);
            //int i = playerSprite.index.Width;

            //if (mikeStandingStill||doAnimation)
            //{
            //framCounter++;
            //if(framCounter%frameRate==0)
            //ani.nextState();
            this.blend = Color.Black;
            this.alpha = 0.25f * game.pauseAlpha;
            ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                   origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                       (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), !faceRight, shiftPosition);
            this.blend = Color.White;
            this.alpha = 1f * game.pauseAlpha;
            ani.drawCurrentState(spriteBatch, this, new Vector2((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y)),
                origin, body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X),
                    (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)playerSprite.index.Width, (int)playerSprite.index.Height), !faceRight, shiftPosition + new Vector2(5, 2));
            return;
            //}
            /*
            if (!faceRight)
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y) , (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, Color.White, body.Rotation, origin, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y) , (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, Color.White, body.Rotation, origin, SpriteEffects.FlipHorizontally, 0f);
            */
            //spriteBatch.Draw(game.getSprite("DeathTime").index, new Rectangle((int)ConvertUnits.ToDisplayUnits(wheel.Body.Position.X),
            //(int)ConvertUnits.ToDisplayUnits(wheel.Body.Position.Y),
            //(int)playerSprite.index.Width, playerSprite.index.Width), null, Color.White, wheel.Body.Rotation, new Vector2(playerSprite.index.Width / 2, playerSprite.index.Width / 2), SpriteEffects.FlipHorizontally, 0f);

        }

        private void drawHealthBar()
        {

        }




    }
}