using System;
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
    public class Block : EntityBlock
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
        public float grassPosition = 0;
        Texture2D dynamicPattern;
        float heightDiff = 0,widthDiff=0;
        Random r = new Random(801);
        public int blockNumber;
        public World world;
        public int associatedLevel;
        public Color FUCKYOUSHIT=Color.White;
        public Block(Game g, Arena a, Vector2 pos, int playerNum,String spriteNumber,float height, float width,int drawLevel, float rotation)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.spriteNumber = spriteNumber;
            this.height = height;
            this.width = width;
            this.blockNumber = game.blockNumber;
            game.blockNumber++;
            this.drawLevel = drawLevel;
            this.rotationAngle = rotation;
            this.associatedLevel = game.currentLevel;
            SetUpPhysics(Constants.player1SpawnLocation + pos);
            LoadContent();
            origin = new Vector2(ani.widthOf() / 2, ani.heightOf() / 2);
            fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
            if (!game.backToTitleScreen)
            {

                if (spriteNumber.Equals("bigBlock"))
                {
                    /*
                    float widthOfObject = game.getSprite("grassTemplate").index.Width;
                    float heighOfObject = game.getSprite("grassTemplate").index.Width;
                    float XX = XX = pos.X - width / 2;
                    for (XX = pos.X - width / 2; XX < pos.X + width / 2; XX += widthOfObject)
                    {
                        if (XX + widthOfObject < pos.X + width/2)
                        {
                            LayerBlock lb = new LayerBlock(game, a, new Vector2(XX + widthOfObject / 2, pos.Y - height / 2), 1, "grassTemplate", game.getSprite("grassTemplate").index.Height, game.getSprite("grassTemplate").index.Width, 0, blockNumber);
                            game.addEntity(lb);
                        }
                    }
                    
                    if (XX + widthOfObject > pos.X + width / 2)
                    {
                        float endLength = (XX + widthOfObject) - (pos.X + width / 2f);
                        //LayerBlock lb = new LayerBlock(game, a, new Vector2(XX - endLength/2, pos.Y - height / 2), 1, "grassTemplate", game.getSprite("grassTemplate").index.Height, endLength, 0, blockNumber);
                        //game.addEntity(lb);

                    }
                     * */

                }
            }
            
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
             world = game.world;
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
            //game.stopWatchLagTimer.Start();
            //XNA Framework HiDef profile supports a maximum Texture2D size of 4096
            //if ((spriteNumber.Equals("bigBlock") || spriteNumber.Equals("groundWall"))&& width < 4096 && height < 4096)
            //{
            if ((spriteNumber.Equals("bigBlock"))&& width < 4096 && height < 4096 && Constants.BLOCK_EFFECT)
            {
                Color[] cData = new Color[(int)((int)width * (int)height)];
                game.stopWatchLagTimer.Start();
                dynamicPattern = game.getCachedDirt(new Rectangle((int)body.Position.X, (int)body.Position.Y, (int)width, (int)height));
                game.stopWatchLagTimer.Stop();
                if (dynamicPattern == null)
                {


                    Color[] template;

                    template = new Color[255 * 255];
                    //int demi = 95;
                    //game.getSprite("dirtyBlock2").index.GetData<Color>(template);

                    int demi = 255;
                    game.getSprite("pixGroundTemp").index.GetData<Color>(template);
                    
                    //template = new Color[125 * 125];
                    //int demi = 125;
                    //game.getSprite("groundBad").index.GetData<Color>(template);
                    
                    if (game.currentWorld == 2)
                    {
                        template = new Color[95 * 95];
                        game.getSprite("seablock").index.GetData<Color>(template);
                        demi = 95;

                    }

                    int counter = 0;
                    int maxCounter = template.Length;
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            int xMod = (int)origPos.X-(int)width/2+ x;// % 95;
                            int yMod = (int)origPos.Y - (int)height / 2 + y; //y % 95;
                            //xMod = xMod / (int)width;
                            //yMod = yMod / (int)height;
                            xMod = xMod % demi;
                            yMod = yMod % demi;
                            
                            if (xMod < 0)
                            {
                                xMod = xMod+demi;
                            }
                            if (yMod < 0)
                            {
                                yMod =yMod+demi;
                            }
                            Color tempColor=template[xMod + yMod * demi];
                            if ((int)(x + y * (int)width) < cData.Length)
                            {
                                cData[(int)(x + y * (int)width)] = tempColor;
                                
                            }
                            counter++;
                            if (counter == maxCounter)
                                counter = 0;//new Color(121, 98, 45);
                        }
                    }
                     /*

                    Color dirtColor = new Color(147, 121, 47);
                    Color dirtColor2 = new Color(105, 87, 46);
                    Color dirtColor3 = new Color(150, 123, 48);
                    
                    for (int x = (int)width-1; x >= 0; x-=2)
                    {
                        for (int y = (int)height-1; y >= 0; y-=2)
                        {
                            float br = r.Next(255/2)+(255*0.9f);
                            
                                
                                    //Play with so that it looks like dirt :)
                            if (grassPosition == 0)
                            {
                                if (width - 30 < x)
                                {

                                    //cData[(int)(x + y * width)] = new Color(39, 152, 65); ;
                                    //continue;
                                }
                            }


                            if (r.Next(100) < 6)
                            {
                                Color tempColor = dirtColor;
                                if (r.Next(3) % 2 == 0)
                                {
                                    if (r.Next(5) < 3)
                                        tempColor = dirtColor2;
                                    else
                                        tempColor = dirtColor3;
                                }
                                
                                cData = drawPixel(2, x, y, cData, tempColor);
                                int number = r.Next(5, 12);
                                int downNum = (int)(number * 0.5f);
                                //for (int i = 0; i < number; i++)
                               // {
                                    cData = drawPixel(8, x , y, cData, tempColor);
                                    //if(i>=downNum)
                                       // cData = drawPixelDown(2, x + i, y, cData, tempColor);
                                //}
                            }

                                
                            
                        }
                    }*/
                     
                    this.dynamicPattern = new Texture2D(game.drawingTool.getGraphicsDevice(), (int)width, (int)height);
                    dynamicPattern.SetData<Color>(cData);
                    game.addCachedDirt(new Rectangle((int)body.Position.X, (int)body.Position.Y, (int)width, (int)height), dynamicPattern);
                }
                heightDiff = playerSprite.index.Height - height;
                widthDiff = playerSprite.index.Width - width;
                //origin = new Vector2(width / 2, height / 2);
            }

            /////////////////////////////
            /*
            if ((spriteNumber.Equals("groundWall")) && width < 4096 && height < 4096)
            {
                Color[] cData = new Color[(int)((int)width * (int)height)];
                dynamicPattern = game.getCachedDirt(new Rectangle(0, 0, (int)width, (int)height));

                if (dynamicPattern == null)
                {
                    Color[] groundTemp = new Color[74 * 200];
                    game.getSprite("groundWall").index.GetData<Color>(groundTemp);
                    Color blankColor = new Color(0, 0, 0, 0);
                    Color[] template = new Color[95 * 95];

                    game.getSprite("dirtyBlock2").index.GetData<Color>(template);
                    int counter = 0;
                    int maxCounter = template.Length;
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            int xMod = (int)origPos.X - (int)width / 2 + x;// % 95;
                            int yMod = (int)origPos.Y - (int)height / 2 + y; //y % 95;
                            //xMod = xMod / (int)width;
                            //yMod = yMod / (int)height;
                            xMod = xMod % 95;
                            yMod = yMod % 95;

                            if (xMod < 0)
                            {
                                xMod = xMod + 95;
                            }
                            if (yMod < 0)
                            {
                                yMod = yMod + 95;
                            }
                            Color tempColor = template[xMod + yMod * 95];
                            if ((int)(x + y * (int)width) < cData.Length)
                            {
                                if (!template[(int)(x + y * (int)width)].Equals(blankColor))
                                    cData[(int)(x + y * (int)width)] = tempColor;

                            }
                            counter++;
                            if (counter == maxCounter)
                                counter = 0;//new Color(121, 98, 45);
                        }
                    }

                    this.dynamicPattern = new Texture2D(game.drawingTool.getGraphicsDevice(), (int)width, (int)height);
                    dynamicPattern.SetData<Color>(cData);
                    game.addCachedDirt(new Rectangle(0, 0, (int)width, (int)height), dynamicPattern);
                }
                heightDiff = playerSprite.index.Height - height;
                widthDiff = playerSprite.index.Width - width;
                
                    //heightDiff = playerSprite.index.Width - width;
                    //widthDiff = playerSprite.index.Height - height;
                
                //origin = new Vector2(width / 2, height / 2);
            }*/
            
        }

        public void updateMagnetBlock()
        {
            if (game.magValue < 0)
                playerSprite = game.getSprite("magNeg");
            else
                playerSprite = game.getSprite("magPluse");
        }

        public Color[] drawPixel(int point, int x, int y, Color[] cData, Color color)
        {
            for (int x1 = x-point; x1 < point+x; x1++)
            {
                for (int y1 = y-point; y1 < y+point; y1++)
                {
                    if (x1 > 0 && x1 < width - 1 && y1 > 0 && y1 < height - 1)
                    {
                        cData[(int)(x1 + y * width)] = color;
                    }
                }

            }

            return cData;
        }
        public Color[] drawPixelDown(int point, int x, int y, Color[] cData, Color color)
        {
            //for (int x1 = x - point; x1 < point + x; x1++)
            //{
                for (int y1 = y
                    ; y1 < point + y; y1++)
                {
                    if ( y1 > 0 && y1 < height - 1)
                    {
                        cData[(int)(x + y1 * width)] = color;
                    }
                }

            //}

            return cData;
        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            //Texture2D test=sprite.index;
            //test.SetData<Color>(Color[])
            if(spriteNumber.Equals("magnet1"))
            {
                updateMagnetBlock();
            }


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
            if (!FUCKYOUSHIT.Equals(Color.White))
            {

                drawColor = FUCKYOUSHIT;
            }
            if (ani.getStateCount() == 1)
            {

                if ((spriteNumber.Equals("bigBlock")) && width < 4096 && height < 4096 && Constants.BLOCK_EFFECT)
                {
                    if (game.currentWorld == 2)
                        displayAlpha = 1f;
                    spriteBatch.Draw(dynamicPattern, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X + ConvertUnits.ToSimUnits(widthDiff / 2f)),
                        (int)ConvertUnits.ToDisplayUnits(body.Position.Y + ConvertUnits.ToSimUnits(heightDiff / 2f)), (int)dynamicPattern.Width, (int)dynamicPattern.Height), null, drawColor * displayAlpha, body.Rotation, origin, SpriteEffects.None, 0f);

                }
                /*else if ((spriteNumber.Equals("groundWall")) && width < 4096 && height < 4096)
                {
                    int widthdiffy = (int)(widthDiff / 2f);
                    int heightdiffy = (int)(heightDiff / 2f);
                    spriteBatch.Draw(dynamicPattern, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X + ConvertUnits.ToSimUnits(widthdiffy)),
                        (int)ConvertUnits.ToDisplayUnits(body.Position.Y + ConvertUnits.ToSimUnits(heightdiffy)), (int)dynamicPattern.Width, (int)dynamicPattern.Height), null, drawColor * displayAlpha, body.Rotation, origin, SpriteEffects.None, 0f);

                


                }*/
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
            if (!IsVisible)
                return;
            
            Vector2 ringDrawPoint = game.drawingTool.getDrawingCoords(body.Position);
            DrawingTool test = game.drawingTool;
            int i = playerSprite.index.Width;
            Point bottomRight = new Point(playerSprite.index.Width, playerSprite.index.Height);
            Rectangle targetRect = new Rectangle((int)ringDrawPoint.X, (int)ringDrawPoint.Y, bottomRight.X, bottomRight.Y);
            Color drawColor = Color.Black;
            if (blockNumber == 34)
            {
                int FUCK = 0;
                drawColor = Color.Red;
            }
            if (ani.getStateCount() == 1)
            {
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X) + 7, (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 5, (int)width, (int)height), null, drawColor * Constants.SHADOW_VALUE, body.Rotation, origin, SpriteEffects.None, 0f);
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
