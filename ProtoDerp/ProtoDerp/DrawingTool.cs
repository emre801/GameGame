using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoDerp
{
    public class DrawingTool
    {
        readonly GraphicsDeviceManager gdm;
        //BasicEffect effect;
        public SpriteBatch spriteBatch;
        readonly Game game;
        public Camera2d cam;

        public static DrawingTool instance { get; private set; }
        public int ActualScreenPixelWidth { get; private set; }
        public int ActualScreenPixelHeight { get; private set; }

        public int RenderingAreaWidth { get; private set; }
        public int RenderingAreaHeight { get; private set; }

        public float GameToScreen { get; private set; }
        public float ScreenToGame { get; private set; }

        public bool isLetterBoxing { get; private set; }
        public float ScreenAspectRatio { get; private set; } // This is the aspect ratio of the screen, not the aspect ratio of the game world. For that, use Constants.desired_aspect_ratio
        public int TopPadding { get; private set; }
        public int BottomPadding { get; private set; }
        public int LeftPadding { get; private set; }
        public int RightPadding { get; private set; }

        public int TopRenderingEdge { get; private set; }
        public int BottomRenderingEdge { get; private set; }
        public int LeftRenderingEdge { get; private set; }
        public int RightRenderingEdge { get; private set; }

        private PrimitiveDrawingElement letterBox;

        public float zoomRatio = 1.75f;
        public Texture2D rectangle;

        Vector2 startingCamPosition;

        public float zoomRatioValue = 0.75f;

        public Vector2 worldDemension;

        public DrawingTool(Game game)
        {
            this.game = game;
            instance = this;
            gdm = new GraphicsDeviceManager(game);
            gdm.PreferMultiSampling = true;
            gdm.ApplyChanges();

            if (Constants.FULLSCREEN && !Constants.OVERRIDE_FULLSCREEN_RES)
            {
                gdm.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                gdm.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                gdm.PreferredBackBufferWidth = Constants.DESIRED_GAME_RESOLUTION_WIDTH;
                gdm.PreferredBackBufferHeight = Constants.DESIRED_GAME_RESOLUTION_HEIGHT;
            }

            gdm.IsFullScreen = Constants.FULLSCREEN;
        }

        public GraphicsDevice getGraphicsDevice()
        {
            return gdm.GraphicsDevice;
        }

        public void initialize()
        {
            //effect = new BasicEffect(gdm.GraphicsDevice);
            //effect.Projection = Matrix.CreateOrthographicOffCenter(0, gdm.GraphicsDevice.Viewport.Width, gdm.GraphicsDevice.Viewport.Height, 0, -1, 1);
            //effect.VertexColorEnabled = true;

            spriteBatch = new SpriteBatch(gdm.GraphicsDevice);

            ActualScreenPixelWidth = gdm.GraphicsDevice.Viewport.Width;
            ActualScreenPixelHeight = gdm.GraphicsDevice.Viewport.Height;
            ScreenAspectRatio = (float)ActualScreenPixelWidth / (float)ActualScreenPixelHeight;

            if (ActualScreenPixelWidth != Constants.DESIRED_GAME_RESOLUTION_WIDTH || ActualScreenPixelHeight != Constants.DESIRED_GAME_RESOLUTION_HEIGHT)
            {
                Console.WriteLine("Could not get desired resolution of " + Constants.DESIRED_GAME_RESOLUTION_WIDTH + "x"
                    + Constants.DESIRED_GAME_RESOLUTION_HEIGHT + ". Using " + ActualScreenPixelWidth + "x" + ActualScreenPixelHeight + ".");
            }
            calculateRenderingArea();

            GameToScreen = RenderingAreaWidth / Constants.GAME_WORLD_WIDTH;
            ScreenToGame = 1 / GameToScreen;
            float h = 1;
            float w = 1;
            if (!Constants.FULLSCREEN)
            {
                cam = new Camera2d(Constants.GAME_WORLD_WIDTH, Constants.GAME_WORLD_HEIGHT);
                cam.Pos = new Vector2(Constants.GAME_WORLD_WIDTH * 0.5f, Constants.GAME_WORLD_HEIGHT * 0.6f);
                h = Constants.GAME_WORLD_HEIGHT;
                w = Constants.GAME_WORLD_WIDTH;
                zoomRatio = 1;
            }
            else
            {
                cam = new Camera2d(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                // cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.6f);
                cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 1, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 1f);

                h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + 3;
                w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                
                zoomRatio = 1.12505f;
            }
            startingCamPosition = cam.Pos;

            cam.Zoom = 0.75f;// *zoomRatio;
            cam.Zoom = 0.55f * zoomRatio;
            if (game.camZoomValue != -1)
            {
                cam.Zoom = game.camZoomValue;
                cam.Pos = game.camPosSet;
            }

            rectangle = new Texture2D(gdm.GraphicsDevice, 1, 1);
            rectangle.SetData(new[] { Color.White });
            worldDemension = new Vector2(w, h);

        }
        public void resetCamera()
        {
            
            
            if (!Constants.FULLSCREEN)
            {
                cam = new Camera2d(Constants.GAME_WORLD_WIDTH, Constants.GAME_WORLD_HEIGHT);
                cam.Pos = new Vector2(Constants.GAME_WORLD_WIDTH * 0.5f, Constants.GAME_WORLD_HEIGHT * 0.6f);

            }
            else
            {
                cam = new Camera2d(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                //if (game.gMode != 6)
                    cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.6f);
               // else
                   // cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 1, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.2f);

            }
            //cam.Pos = startingCamPosition;
            
            cam.Zoom = 0.55f * zoomRatio;

            if (game.camZoomValue != -1)
            {
                cam.Zoom = game.camZoomValue;
                cam.Pos = game.camPosSet;
            }
        }
        private void initializeLetterBox()
        {
            if (!isLetterBoxing) return;

            VertexPositionColor[] letterboxVertices = new VertexPositionColor[4];
            letterboxVertices[0] = new VertexPositionColor(Vector3.Zero, Constants.LETTERBOX_COLOR);
            letterboxVertices[1] = new VertexPositionColor(Vector3.UnitX, Constants.LETTERBOX_COLOR);
            letterboxVertices[2] = new VertexPositionColor(Vector3.UnitY, Constants.LETTERBOX_COLOR);
            letterboxVertices[3] = new VertexPositionColor(new Vector3(1, 1, 0), Constants.LETTERBOX_COLOR);

            letterBox = new PrimitiveDrawingElement(PrimitiveType.TriangleStrip, letterboxVertices, 0, letterboxVertices.Length - 2, Matrix.Identity);
        }

        public void drawLetterBox()
        {
            if (!isLetterBoxing) return;

            if (LeftPadding != 0)
            {
                letterBox.transform = Matrix.CreateScale(LeftPadding, ActualScreenPixelHeight, 0) * Matrix.CreateTranslation(Vector3.Zero);
                drawPrimitiveElement(letterBox);
            }

            if (RightPadding != 0)
            {
                letterBox.transform = Matrix.CreateScale(RightPadding, ActualScreenPixelHeight, 0) * Matrix.CreateTranslation(ActualScreenPixelWidth - RightPadding, 0, 0);
                drawPrimitiveElement(letterBox);
            }

            if (TopPadding != 0)
            {
                letterBox.transform = Matrix.CreateScale(ActualScreenPixelWidth, TopPadding, 0) * Matrix.CreateTranslation(Vector3.Zero);
                drawPrimitiveElement(letterBox);
            }

            if (BottomPadding != 0)
            {
                letterBox.transform = Matrix.CreateScale(ActualScreenPixelWidth, BottomPadding, 0) * Matrix.CreateTranslation(0, ActualScreenPixelHeight - BottomPadding, 0);
                drawPrimitiveElement(letterBox);
            }
        }

        private void calculateRenderingArea()
        {
            if (ScreenAspectRatio == Constants.GAMEWORLD_ASPECT_RATIO)
            {
                isLetterBoxing = false;
                BottomPadding = LeftPadding = RightPadding = TopPadding = 0;
            }
            else if (ScreenAspectRatio > Constants.GAMEWORLD_ASPECT_RATIO)
            {
                // Screen is wider/shorter than the game.  Need to add blank space on the sides.
                RenderingAreaHeight = ActualScreenPixelHeight;
                RenderingAreaWidth = (int)Math.Round(ActualScreenPixelHeight * Constants.GAMEWORLD_ASPECT_RATIO, 0);
                TopPadding = BottomPadding = 0;

                // Make sure we dont lose any pixels to truncation
                int horizontalPadding = ActualScreenPixelWidth - RenderingAreaWidth;
                LeftPadding = horizontalPadding / 2;
                RightPadding = horizontalPadding - LeftPadding;

                isLetterBoxing = true;
            }
            else
            {
                // Screen is taller/thinner than the game.  Need to add blank space on the sides.
                RenderingAreaWidth = ActualScreenPixelWidth;
                RenderingAreaHeight = (int)Math.Round(ActualScreenPixelWidth / Constants.GAMEWORLD_ASPECT_RATIO, 0);
                LeftPadding = RightPadding = 0;

                // Make sure we dont lose any pixels to truncation
                int verticalPadding = ActualScreenPixelHeight - RenderingAreaHeight;
                TopPadding = verticalPadding / 2;
                BottomPadding = verticalPadding - TopPadding;

                isLetterBoxing = true;
            }

            TopRenderingEdge = TopPadding;
            BottomRenderingEdge = ActualScreenPixelHeight - BottomPadding;
            LeftRenderingEdge = LeftPadding;
            RightRenderingEdge = ActualScreenPixelWidth - RightPadding;

            initializeLetterBox();
        }

        #region coordinate conversions

        // Returns a Point (x and y integers corresponding to a pixel) based on a Vector in game space
        public Point gameCoordsToScreenCoords(Vector2 loc)
        {
            return gameCoordsToScreenCoords(loc.X, loc.Y);
        }

        public int gameXCoordToScreenCoordX(float gc)
        {
            return LeftRenderingEdge + (int)gameToScreen(gc);
        }

        public int gameYCoordToScreenCoordY(float gc)
        {
            return TopRenderingEdge + (int)gameToScreen(gc);
        }

        // Returns a Point (x and y integers corresponding to a pixel) based on x and y game space coordinates
        public Point gameCoordsToScreenCoords(float x, float y)
        {
            return new Point(LeftRenderingEdge + (int)gameToScreen(x), TopRenderingEdge + (int)gameToScreen(y));
        }

        // Returns a Vector representing a location in game space given a pixel in screen space
        public Vector2 screenCoordsToGameCoords(Point pixel)
        {
            return screenCoordsToGameCoords(pixel.X, pixel.Y);
        }

        public Vector2 screenCoordsToGameCoords(int x, int y)
        {
            return new Vector2(screenToGame(x - LeftRenderingEdge), screenToGame(y - TopRenderingEdge));
        }

        public float gameToScreen(float c)
        {
            //return (int)Math.Round(c * Constants.GAME_TO_SCREEN, 0);
            return c * GameToScreen;
        }

        public Vector2 getDrawingCoords(Vector2 gameWorldCoords)
        {
            return new Vector2(LeftRenderingEdge + gameToScreen(gameWorldCoords.X), TopRenderingEdge + gameToScreen(gameWorldCoords.Y));
        }

        public float screenToGame(int c)
        {
            return c * ScreenToGame;
        }

        #endregion

        public void drawPrimitiveElements(PrimitiveDrawingElement[] elements, float alpha)
        {
            endBatch();
            foreach (var element in elements)
            {
                drawPrimitiveElement(element, alpha);
            }
            beginBatch();
        }

        public void drawPrimitiveElements(PrimitiveDrawingElement[] elements)
        {
            drawPrimitiveElements(elements, 1.0f);
        }

        public void drawSinglePrimitiveElement(PrimitiveDrawingElement p, float alpha)
        {
            endBatch();
            drawPrimitiveElement(p);
            beginBatch();
        }

        public void drawSinglePrimitiveElement(PrimitiveDrawingElement p)
        {
            drawSinglePrimitiveElement(p, 1.0f);
        }

        private void drawPrimitiveElement(PrimitiveDrawingElement p)
        {
            //drawPrimitiveElement(p, 1.0f);
        }

        private void drawPrimitiveElement(PrimitiveDrawingElement p, float alpha)
        {
            // float temp = effect.Alpha;
            //effect.Alpha = alpha;
            //effect.World = p.transform;
            //foreach (EffectPass ep in effect.CurrentTechnique.Passes)
            //{
            //    ep.Apply();
            //    if (p.primitiveCount > 0)
            //    {
            //        gdm.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(p.type, p.vertices, p.vertexOffset, p.primitiveCount);
            //    }
            //}
            //effect.Alpha = temp;
        }//
        public void drawLoadingText()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            DrawText(spriteBatch, 50, 50, "Loading " + game.perfectComplete + "%", 1,game.splashFadeOut);
            Sprite spalshArt = game.getSprite("c801");
            spriteBatch.Draw(spalshArt.index, new Rectangle((int)(ActualScreenPixelWidth*0.05f),
                           (int)(ActualScreenPixelHeight*0.85f),
                           (int)(spalshArt.index.Width*0.15f*game.scale), (int)(spalshArt.index.Height*0.15f*game.scale)), null, Color.White*game.splashFadeOut, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            
            spriteBatch.End();

        }
        public void endBatch()
        {
            spriteBatch.End();
        }

        public Vector2 oldPosition=Vector2.Zero;
        private void followPlayer()
        {
            float widthRatio = 0.85f;
            float heightRatio = 0.85f;//
            float heightRatio2 = 0.70f;//
            float width = widthRatio * cam.ViewportWidth;// *zoomRatio;
            float height = heightRatio * cam.ViewportHeight;// *zoomRatio;
            float height2 = heightRatio2 * cam.ViewportHeight;// *zoomRatio;
            //This allows the camera to follow the player
            PlayableCharacter p1 = game.Arena.player1;
            game.moveBackGround = new Vector2(0, 0);
            oldPosition = cam.Pos;
            bool hasCamMoved = false;
            if (!(game.Arena.maxLeft > p1.Position.X || game.Arena.maxRight < p1.Position.X))
            {
                if (p1.Position.X + width > cam._pos.X + cam.ViewportWidth / 1)
                {
                    hasCamMoved = true;
                    cam.Move(new Vector2((p1.Position.X + width) - (cam._pos.X + cam.ViewportWidth / 1), 0));
                    //cam.Move(new Vector2(10, 0));
                    if (Math.Abs(game.Arena.player1.body.LinearVelocity.X) > 0.1f)
                    {
                        float moveAmount = (p1.Position.X + width) - (cam._pos.X + cam.ViewportWidth / 1) / 10000f;
                        //game.moveBackGround -= new Vector2(moveAmount, 0);
                    }
                }
                if (p1.Position.X - width < cam._pos.X - cam.ViewportWidth / 1)
                {
                    hasCamMoved = true;
                    cam.Move(new Vector2((p1.Position.X - width) - (cam._pos.X - cam.ViewportWidth / 1), 0));
                    //cam.Move(new Vector2(-, 0));
                    if (Math.Abs(game.Arena.player1.body.LinearVelocity.X) > 0.1f)
                    {
                        float moveAmount = (p1.Position.X + width) - (cam._pos.X + cam.ViewportWidth / 1) / 10000f;
                        //game.moveBackGround += new Vector2(moveAmount, 0);
                    }
                }
            }
            if (!(game.Arena.maxTop > p1.Position.Y || game.Arena.maxButtom < p1.Position.Y))
            {
                if (p1.Position.Y + height2 > cam._pos.Y + cam.ViewportHeight / 1)
                {
                    hasCamMoved = true;
                    cam.Move(new Vector2(0, (p1.Position.Y + height2) - (cam._pos.Y + cam.ViewportHeight / 1)));
                    if (Math.Abs(game.Arena.player1.body.LinearVelocity.Y) > 0.1f)
                    {
                        float moveAmount = (p1.Position.Y + height2) - (cam._pos.Y + cam.ViewportHeight / 1);
                        //game.moveBackGround -= new Vector2(0, moveAmount);
                    }
                }
                if (p1.Position.Y - height < cam._pos.Y - cam.ViewportHeight / 1)
                {
                    hasCamMoved = true;
                    cam.Move(new Vector2(0, (p1.Position.Y - height) - (cam._pos.Y - cam.ViewportHeight / 1)));
                    if (Math.Abs(game.Arena.player1.body.LinearVelocity.Y) > 0.1f)
                    {
                        float moveAmount = (p1.Position.Y - height) - (cam._pos.Y - cam.ViewportHeight / 1);
                        //game.moveBackGround += new Vector2(0, moveAmount);
                    }
                }
            }

            //Player dies if they go out of the camera bounds
            float zoomAdjustments =  zoomRatioValue/cam.Zoom ;
            if (p1.Position.X > cam.Pos.X + cam.ViewportWidth * zoomAdjustments || p1.Position.X < cam.Pos.X - cam.ViewportWidth * zoomAdjustments
                || p1.Position.Y > cam.Pos.Y + cam.ViewportHeight * zoomAdjustments || p1.Position.Y < cam.Pos.Y - cam.ViewportHeight * zoomAdjustments)
            {
                game.PlayerDies();
            }
            moveBackgrounCamera();
            if(hasCamMoved)
                game.moveBackGround = (cam.Pos - oldPosition)*5;
            
        }


        public void moveBackgrounCamera()
        {
            Vector2 change = cam.Pos - startingCamPosition;
            game.moveBackGround += change;
            startingCamPosition = cam.Pos;
            
        }

        public void followTitle()
        {
            Vector2 Position = game.Title.posSelectText;

            float widthRatio = 0.55f;
            float heightRatio = 0.55f;
            if (Constants.FULLSCREEN)
            {
                widthRatio = 0.25f;
                heightRatio = 0.15f;
            }
            float width = widthRatio * cam.ViewportWidth;// *zoomRatio;
            float height = heightRatio * cam.ViewportHeight;// *zoomRatio;
            //This allows the camera to follow the player
            //PlayableCharacter p1 = game.Arena.player1;

            if (Position.X + width > cam._pos.X + cam.ViewportWidth / 1)
                cam.Move(new Vector2((Position.X + width) - (cam._pos.X + cam.ViewportWidth / 1), 0));
            if (Position.X - width < cam._pos.X - cam.ViewportWidth / 1)
                cam.Move(new Vector2((Position.X - width) - (cam._pos.X - cam.ViewportWidth / 1), 0));
            if (Position.Y + height > cam._pos.Y + cam.ViewportHeight / 1)
                cam.Move(new Vector2(0, (Position.Y + height) - (cam._pos.Y + cam.ViewportHeight / 1)));
            if (Position.Y - height < cam._pos.Y - cam.ViewportHeight / 1)
                cam.Move(new Vector2(0, (Position.Y - height) - (cam._pos.Y - cam.ViewportHeight / 1)));

        }
        public void controlCamera()
        {
            if (game.testLevel)
                return;
            KeyboardState keyState = Keyboard.GetState();

            XboxInput xbInput = (XboxInput)game.Arena.player1.inputState;
            float xDirection = xbInput.getXDirection();
            float yDirection = xbInput.getYDirection();
            float turboMove = 1;

            if (keyState.IsKeyDown(Keys.LeftShift))
                turboMove = 10;
            if (keyState.IsKeyDown(Keys.D7))
            {
                xDirection = -0.01f * turboMove;
            }
            if (keyState.IsKeyDown(Keys.D8))
            {
                xDirection = 0.01f * turboMove;
            }
            if (keyState.IsKeyDown(Keys.D9))
            {
                yDirection = 0.01f * turboMove;
            }
            if (keyState.IsKeyDown(Keys.D0))
            {
                yDirection = -0.01f * turboMove;
            }

            cam.Move(new Vector2(xDirection * 100f, yDirection * 100f));
            float zoomIn = xbInput.isLeftTriggerPressed();
            float zoomOut = xbInput.isRightTriggerPressed();
            if (keyState.IsKeyDown(Keys.D4))
            {
                zoomIn = 1;
            }
            if (keyState.IsKeyDown(Keys.D5))
            {
                zoomOut = 1;
            }
            cam.Zoom += zoomIn / 100f;
            cam.Zoom -= zoomOut / 100f;
        }

        public void beginBatch()
        {
            if (game.isInCreatorMode)
            {
                //DrawGraph
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
                Sprite graph = game.getSprite("graph");
                for (int x = -10; x < 10; x++)
                {
                    for (int y = -10; y < 10; y++)
                    {
                        float xPos = graph.index.Width * x*2f;
                        float yPos = graph.index.Height * y*2f;
                        spriteBatch.Draw(graph.index, new Rectangle((int)(xPos),
                        (int)(yPos),
                        (int)(graph.index.Width*2f), (int)(graph.index.Height*2f)), null, Color.White, 0, new Vector2(0,0), SpriteEffects.None, 0f);
                

                    }

                }
                endBatch();

            }


            if (game.gMode == 0)
            {
                followPlayer();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));

                /*spriteBatch.Begin(SpriteSortMode.Deferred,
                         BlendState.NonPremultiplied,
                         null,
                         null,
                         null,
                         null,
                         cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/
                //));

            }
            else
            {
                if (game.isInCreatorMode)
                    controlCamera();
                else
                    followTitle();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
            }
        }
        internal void drawEntities(List<Entity> entities, GameTime gameTime)
        {
            beginBatch();
            //Draw all game Entities (sprites)
            foreach (Entity e in game.backGroundImages)
            {
                if(e is BackgroundBlock)
                    e.Draw(gameTime, spriteBatch);
            }
            //DrawShadows
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    //experiment with look --- if (b.drawLevel == 1)
                    if (b.drawLevel == 1)
                    {
                        b.DrawShadow(gameTime, spriteBatch);
                    }
                }
                if (e is SignBlock)
                {
                    e.Draw(gameTime, spriteBatch);
                }

            }

            foreach (Entity e in entities)
            {
                if (e is PlayableCharacter || e is GUI)
                    continue;

                if (e.IsVisible)
                {/*
                    if (game.IsPaused)
                    {
                        float remAlpha = e.alpha;
                        e.alpha *= Constants.PAUSE_FADE;
                        //e.Draw(gameTime, spriteBatch);
                        e.alpha = remAlpha;
                    }
                    else
                  * */
                    e.Draw(gameTime, spriteBatch);
                }
            }
            endBatch();

            SamplerState test = new SamplerState();
            test.MaxMipLevel = 100;

            spriteBatch.Begin(SpriteSortMode.Deferred,
                         BlendState.AlphaBlend,
                         SamplerState.LinearClamp,
                         null,
                         null,
                         null,
                         cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
            //DrawShadows
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    //Testing look --- if (b.drawLevel != 1)
                    if (b.drawLevel != 4)
                    {
                        b.DrawShadow(gameTime, spriteBatch);
                    }
                }
                if (e is DeathBlock)
                {
                    DeathBlock b = (DeathBlock)e;
                    b.DrawShadow(gameTime, spriteBatch);
                }
                if (e is GoalBlock)
                {
                    GoalBlock b = (GoalBlock)e;
                    b.DrawShadow(gameTime, spriteBatch);
                }

            }
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 0)
                    {
                            b.Draw(gameTime, spriteBatch);
                    }
                }

            }
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 1)
                    {
                        String name = b.spriteNumber;
                        if(name.Contains("tree"))
                            b.Draw(gameTime, spriteBatch);
                    }
                }

            }
            foreach (Entity e in entities)
            {
                if (e is PlayableCharacter)
                {
                    e.Draw(gameTime, spriteBatch);
                    break;
                }
            }
            foreach (Entity e in entities)
            {
                if (!e.IsVisible)
                {
                    continue;
                }
                
                if (e is DeathBlock || e is GoalBlock || e is CreaterBlock || e is Arena || e is MovingCycle || e is MovingPath || e is WaterBlock)
                    e.Draw(gameTime, spriteBatch);


            }
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 0)
                    {
                            b.Draw(gameTime, spriteBatch);
                    }
                }

            }
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 3)
                    {
                        b.Draw(gameTime, spriteBatch);
                    }
                }

            }
            foreach (Entity e in entities)
            {
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 2)
                    {
                        if(b.IsVisible)
                            e.Draw(gameTime, spriteBatch);
                    }
                }
                if (e is CreaterBlock|| e is TempBlock || e is LayerBlock)
                {
                    e.Draw(gameTime, spriteBatch);
                }
                if (e is TitleScreen)
                    e.Draw(gameTime, spriteBatch);
            }
            if (game.Arena != null) 
               game.Arena.Draw(gameTime, spriteBatch);

            foreach (Entity e in game.backGroundImages)
            {
                if (e is LayerBlock)
                    e.Draw(gameTime, spriteBatch);
            }

            endBatch();

            //foreach (Block i in topBlocks)
            //game.addEntity(i);

            if (game.gMode == 0 || game.gMode == 2)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                if ((game.gMode == 2 && game.activateButtons && game.cameraWindowValue == 0))
                {
                    drawBorderImageForButtons(spriteBatch);
                }
                foreach (Button b in game.Arena.buttons)
                {
                    b.Draw(gameTime, spriteBatch);
                }
                foreach (Entity e in entities)
                {
                    game.hackyGuiThing = true;
                    if (e is CutSceneText)
                        e.Draw(gameTime, spriteBatch);
                    game.hackyGuiThing = false;

                }
                game.Arena.gui.Draw(gameTime, spriteBatch);
                game.Arena.gui.DrawButtonPress(gameTime, spriteBatch);
                game.Arena.gui.DrawMouse(gameTime, spriteBatch);
                spriteBatch.End();

            }
            if (game.gameInsertValues)
            {
                game.gameDoneLoading = true;
            }


        }
        public void beginBatchGui()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

        }

        public void drawBGGradient(Texture2D bgGradient)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
            spriteBatch.Draw(bgGradient, new Rectangle(0, 0, ActualScreenPixelWidth, ActualScreenPixelHeight), Color.White * 0.65f);
            spriteBatch.End();
        }
        public void drawBorderImageForButtons(SpriteBatch spriteBatch)
        {
            if (!Constants.FULLSCREEN)
            {
                for (int i = 0; i <= 700; i += 50)
                {

                    game.drawingTool.drawBorderImageFromPos(10, i, 30, 30, spriteBatch);
                    game.drawingTool.drawBorderImageFromPos(50, i, 30, 30, spriteBatch);
                    game.drawingTool.drawBorderImageFromPos(90, i, 30, 30, spriteBatch);

                }
            }
            else
            {

                for (int i = 0; i <= 1200; i += 50)
                {
                    game.drawingTool.drawBorderImageFromPos(10, i, 30, 30, spriteBatch);
                    game.drawingTool.drawBorderImageFromPos(50, i, 30, 30, spriteBatch);
                    game.drawingTool.drawBorderImageFromPos(90, i, 30, 30, spriteBatch);
                }

            }



        }

        public void drawBorderImage(float x, float y, int height, int width, SpriteBatch spriteBatch)
        {
            if (Constants.FULLSCREEN)
            {
                x = x * 5;
                y = y + y / 2 + y / 4;
                height = (int)(height * 1.25f);
                width = width * 2;

            }
            Rectangle rect = new Rectangle((int)((game.getWorldSize().X * x)), (int)((game.getWorldSize().Y * y)), width, height);
            Rectangle rect2 = new Rectangle((int)((game.getWorldSize().X * x)) - 2, (int)((game.getWorldSize().Y * y)) - 2, width + 4, height + 4);
            Rectangle rect3 = new Rectangle((int)((game.getWorldSize().X * x)) - 4, (int)((game.getWorldSize().Y * y)) - 4, width + 8, height + 8);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect3, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImageWhite").index, rect2, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect, Color.White);

        }

        public void drawBorderImageFromPos(float x, float y, int height, int width, SpriteBatch spriteBatch)
        {
            if (Constants.FULLSCREEN && !game.isInCreatorMode)
            {

                x = x * 5;
                y = y + y / 2 + y / 4;
                height = (int)(height * 1.25f);
                width = width * 2;

            }
            Rectangle rect = new Rectangle((int)(( x)), (int)(( y)), width, height);
            Rectangle rect2 = new Rectangle((int)(( x)) - 2, (int)(( y)) - 2, width + 4, height + 4);
            Rectangle rect3 = new Rectangle((int)((x)) - 4, (int)(( y)) - 4, width + 8, height + 8);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect3, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImageWhite").index, rect2, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect, Color.White);


        }


        //Draws a simple rectangle based on given location and color
        public void drawRectangle(Rectangle rect, Color color)
        {
            spriteBatch.Draw(rectangle, rect, color);
        }

        // Draw a sprite based on a center position and an x and y radius. All parameters are in terms of game world sizes. Does not support sprite rotation
        public void drawSprite(Texture2D sprite, Vector2 center, float radiusX, float radiusY, Color blendColor)
        {
            var topLeft = game.drawingTool.getDrawingCoords(new Vector2(center.X - radiusX, center.Y - radiusY));
            Rectangle drawRect = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)Math.Round(gameToScreen(radiusX) * 2), (int)Math.Round(gameToScreen(radiusY) * 2));
            spriteBatch.Draw(sprite, drawRect, blendColor);
        }

        // Draws a sprite based on a center position, and a rectangle centered at that position. Parameters are in terms of game world sizes. Does not support sprite rotation
        public void drawSprite(Texture2D sprite, Vector2 center, Rectangle rect, Color blendColor)
        {
            var topLeft = game.drawingTool.getDrawingCoords(new Vector2(center.X - (rect.Width / 2), center.Y - (rect.Height / 2)));
            Rectangle drawRect = new Rectangle((int)topLeft.X, (int)topLeft.Y, rect.Width, rect.Height);
            spriteBatch.Draw(sprite, drawRect, blendColor);
        }
        //DrawsLine be careful, to many lines cause lag....
        public void DrawLine(SpriteBatch batch,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            /*
            Texture2D blank = new Texture2D(game.drawingTool.getGraphicsDevice(), 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });
                

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
             * */
        }
        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text, float size)
        {
            DrawText(spriteBatch, x, y, text, size, 1);
        }

        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text, float size,float fadePercent)
        {

            char[] tempstrMulti = text.ToCharArray();
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_PIXEL];

            //drawBorderImage(x - font.MeasureString("A").X * size * game.scale, y - font.MeasureString("A").Y * size * game.scale * 0.5f, 100, (int)(size * game.scale * 0.75f), spriteBatch);

            float drawPosX = 0;
            float drawPosY = 0;
            for (int i = 0; i < tempstrMulti.Length; i += 1)
            {
                if ("{".Equals("" + tempstrMulti[i]))
                {
                    drawPosX = 0;
                    drawPosY += font.MeasureString("A").Y * size * game.scale;
                    continue;
                }
                spriteBatch.DrawString(font, "" + tempstrMulti[i],
                    new Vector2(x + drawPosX, y + drawPosY),
                    Color.White*fadePercent,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    1f * size * game.scale,
                    SpriteEffects.None,
                    0);
                drawPosX += font.MeasureString("" + tempstrMulti[i]).X * size * game.scale;
                
            }

        }
    }
}