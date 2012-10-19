using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtoDerp
{
    public class DrawingTool
    {
        readonly GraphicsDeviceManager gdm;
        //BasicEffect effect;
        SpriteBatch spriteBatch;
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

        

        public DrawingTool(Game game)
        {
            this.game = game;
            instance = this;
            gdm = new GraphicsDeviceManager(game);
            gdm.PreferMultiSampling = true;

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
            if (!Constants.FULLSCREEN)
            {
                cam = new Camera2d(Constants.GAME_WORLD_WIDTH, Constants.GAME_WORLD_HEIGHT);
                cam.Pos = new Vector2(Constants.GAME_WORLD_WIDTH * 0.5f, Constants.GAME_WORLD_HEIGHT * 0.6f);
                h = Constants.GAME_WORLD_HEIGHT;
            }
            else
            {
                cam = new Camera2d(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.6f);
                h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }


            zoomRatio = 1;// h / 600f;
            cam.Zoom = 0.75f;// *zoomRatio;
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
                cam.Pos = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.6f);
                
            }
            cam.Zoom = 0.75f*zoomRatio;
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

        private void endBatch()
        {
            spriteBatch.End();
        }

        private void followPlayer()
        {
            float widthRatio = 0.75f;
            float heightRatio = 0.85f;
            float heightRatio2 = 0.70f;
            float width = widthRatio * cam.ViewportWidth*zoomRatio;
            float height = heightRatio * cam.ViewportHeight*zoomRatio;
            float height2 = heightRatio2 * cam.ViewportHeight * zoomRatio;
            //This allows the camera to follow the player
            PlayableCharacter p1 = game.Arena.player1;

            if (!(game.Arena.maxLeft > p1.Position.X || game.Arena.maxRight < p1.Position.X))
            {
                if (p1.Position.X + width > cam._pos.X + cam.ViewportWidth / 1)
                    cam.Move(new Vector2((p1.Position.X + width) - (cam._pos.X + cam.ViewportWidth / 1), 0));
                if (p1.Position.X - width < cam._pos.X - cam.ViewportWidth / 1)
                    cam.Move(new Vector2((p1.Position.X - width) - (cam._pos.X - cam.ViewportWidth / 1), 0));
            }
            if (!(game.Arena.maxTop > p1.Position.Y || game.Arena.maxButtom < p1.Position.Y))
            {
                if (p1.Position.Y + height2 > cam._pos.Y + cam.ViewportHeight / 1)
                    cam.Move(new Vector2(0, (p1.Position.Y + height2) - (cam._pos.Y + cam.ViewportHeight / 1)));
                if (p1.Position.Y - height < cam._pos.Y - cam.ViewportHeight / 1)
                    cam.Move(new Vector2(0, (p1.Position.Y - height) - (cam._pos.Y - cam.ViewportHeight / 1)));
            }
        }

        public void followTitle()
        {
            Vector2 Position =game.Title.posSelectText;

            float widthRatio = 0.55f;
            float heightRatio = 0.55f;
            if (Constants.FULLSCREEN)
            {
                widthRatio = 0.25f;
                heightRatio = 0.15f;
            }
            float width = widthRatio * cam.ViewportWidth * zoomRatio;
            float height = heightRatio * cam.ViewportHeight * zoomRatio;
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
            XboxInput xbInput=(XboxInput)game.Arena.player1.inputState;
            float xDirection = xbInput.getXDirection();
            float yDirection = xbInput.getYDirection();
            cam.Move(new Vector2(xDirection, yDirection));
            float zoomIn=xbInput.isLeftTriggerPressed();
            float zoomOut = xbInput.isRightTriggerPressed();
            cam.Zoom += zoomIn/100f;
            cam.Zoom -= zoomOut / 100f;
        }

        private void beginBatch()
        {
            if (game.gMode == 0)
            {
                followPlayer();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
                
                /*spriteBatch.Begin(SpriteSortMode.Deferred,
                         BlendState.NonPremultiplied,
                         null,
                         null,
                         null,
                         null,
                         cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*///));
            
            }
            else 
            {
                if (game.isInCreatorMode)
                    controlCamera();
                else
                    followTitle();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
            }
        }
        internal void drawEntities(SortedSet<Entity> entities, GameTime gameTime)
        {
            beginBatch();
            //Draw all game Entities (sprites)
            foreach (Entity e in entities)
            {
                if(e is PlayableCharacter|| e is GUI)
                    continue;

                if (e.IsVisible)
                {
                    if (game.IsPaused)
                    {
                        float remAlpha = e.alpha;
                        e.alpha *= Constants.PAUSE_FADE;
                        e.Draw(gameTime, spriteBatch);
                        e.alpha = remAlpha;
                    }
                    else
                        e.Draw(gameTime, spriteBatch);
                }
            }
            endBatch();

            spriteBatch.Begin(SpriteSortMode.Deferred,
                         BlendState.NonPremultiplied,
                         null,
                         null,
                         null,
                         null,
                         cam.get_transformation(gdm.GraphicsDevice /*Send the variable that has your graphic device here*/));
            
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
                if (e is Block)
                {
                    Block b = (Block)e;
                    if (b.drawLevel == 2|| b.drawLevel==0)
                    {
                        e.Draw(gameTime, spriteBatch);
                    }
                }
                if (e is DeathBlock|| e is GoalBlock)
                    e.Draw(gameTime, spriteBatch);
                
            }
           endBatch();
           
           //foreach (Block i in topBlocks)
           //game.addEntity(i);

           if (game.gMode == 0|| game.gMode==2)
           {
               spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
               game.Arena.gui.Draw(gameTime, spriteBatch);
               spriteBatch.End();
           }
           
 
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
    }
}
