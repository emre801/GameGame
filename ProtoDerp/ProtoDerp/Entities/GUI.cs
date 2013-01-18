using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

/**
 * 
 * The game graphical HUD that displays things such as the bullet reserves, player's score, etc.
 * 
 */
namespace ProtoDerp
{
    public class GUI : Entity
    {
        Sprite pix; //Used to draw rectangles
        Sprite cir; //Used to draw circles
        Sprite gameOverGUI;
        public bool gameOver = false;
        float pauseSelection = 0.475f;

        KeyboardInput keyInput;
        Sprite topCur, leftCur, leftCur2, leftCur3, rightCur, rightCur2, rightCur3;
        float curHeight, curWidth, topCurHeight, topCurWidth;
        float curTopMove = 0,curLeftMove=0,curRightMove=0;
        float curLeftMove2 = 0, curRightMove2 = 0;
        bool firstTransition = true;
        int transitionRate = 20;
        public Button[] buttons;
        public GUI(Game g)
            : base(g)
        {
            this.drawPriority = Constants.GUI_DRAWPRI;
            game = g;

            pix = game.getSprite("MikeIcon");
            cir = game.getSprite("Circle");
            gameOverGUI = game.getSprite("gameOver");
            this.updatesWhenPaused = true;
            keyInput = new KeyboardInput();
            topCur = game.getSprite("TopCurtain");

            leftCur = game.getSprite("leftCurtain");
            leftCur2 = game.getSprite("lC2");
            leftCur3 = game.getSprite("lC3");

            rightCur = game.getSprite("rightCurtain");
            rightCur2 = game.getSprite("rC2");
            rightCur3 = game.getSprite("rC3");
            if (!Constants.FULLSCREEN)
            {
                curHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                curWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f;
                topCurHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.1f;
                topCurWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
            else
            {
                curHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height+10f;
                curWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                topCurHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.1f;
                topCurWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            }
            curTopMove = -topCurHeight;
            curLeftMove = -curWidth/2f;
            curRightMove = curWidth;
            curLeftMove2 = 0;
            curRightMove2 = curWidth/2f;
            

        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {
            //No Need to update anything here
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float zoom=game.drawingTool.cam.Zoom;
            keyInput.Update(gameTime);
            if (firstTransition&& game.numDeath==0)
            {
                DrawTransitionOpening(spriteBatch);
                return;
            }
            if (game.numDeath > 0)
                firstTransition = false;
            if (game.inTransition)
            {
                
                    DrawTransitionClosing(spriteBatch);
                
                return;

            }

            //spriteBatch.Draw(pix.index, new Rectangle(5,5,100,100), Color.White);
            if (game.pause)
            {
                DrawText(spriteBatch, 0.45f, 0.40f, "Paused",1.5f);
                DrawText(spriteBatch, 0.45f, 0.45f, "Rage Quit!!", 1.0f);
                DrawText(spriteBatch, 0.45f, 0.475f, "Return to Game", 1.0f);
                if (game.playerOneInput.VerticalMovement() > 0)
                    pauseSelection = 0.45f;
                else if (game.playerOneInput.VerticalMovement() < 0 )
                    pauseSelection = 0.475f;
                
               if (keyInput.IsKeyPressed(Keys.Up))
                        pauseSelection = 0.45f;
               else if (keyInput.IsKeyPressed(Keys.Down))
                        pauseSelection = 0.475f;
                
                DrawText(spriteBatch, 0.425f, pauseSelection, ">", 1.0f);
                if (game.playerOneInput.IsNewButtonPressed(Buttons.A)|| keyInput.IsNewKeyPressed(Keys.Enter))
                {
                    if (pauseSelection == 0.45f)
                    {
                        game.backToTitleScreen = true;
                        game.backGroundImages.Clear();
                        MediaPlayer.Stop();
                        game.camZoomValue = -1;
                    }
                    game.stopWatch.Start();
                    game.pause = false;
                    game.pauseMusic();
                }
                    game.drawingTool.drawRectangle(new Rectangle(0, 0, game.drawingTool.ActualScreenPixelWidth,
                        (int)(game.drawingTool.ActualScreenPixelHeight*0.10f)), Color.Black);
                    game.drawingTool.drawRectangle(new Rectangle(0, (int)(game.drawingTool.ActualScreenPixelHeight*0.90f),
                        game.drawingTool.ActualScreenPixelWidth, (int)(game.drawingTool.ActualScreenPixelHeight * 0.10f+12)), Color.Black);
                
               
            }

            if (game.gMode == 0)
            {
                //DrawBackThing(gameTime, spriteBatch);
                DrawCredits(gameTime, spriteBatch);
                if (game.pause)
                {
                    DrawText(spriteBatch, 0.05f, 0.9f, game.songName);
                    if (!game.songArtist.Equals(""))
                        DrawText(spriteBatch, 0.05f, 0.925f, "by " + game.songArtist);
                    else
                        DrawText(spriteBatch, 0.05f, 0.925f, "by __twc");
                }
                 

            }
            if (game.gMode == 2)
            {
                if (!game.inDeleteMode && !game.testLevel && game.backGroundNum==0)
                {
                    DrawCreatorInformation(gameTime, spriteBatch);
                    DrawPositionInformation(gameTime, spriteBatch);
                    DrawCurrentLevelInfo(gameTime, spriteBatch);
                    DrawCurrentTemplateLevel(gameTime, spriteBatch);
                    DrawSaveText(gameTime, spriteBatch);
                    DrawControlsInfo(gameTime, spriteBatch);
                    DrawMouse(gameTime, spriteBatch);
                    DrawCameraText(gameTime, spriteBatch);
                    DrawMouseCameraValue(gameTime, spriteBatch);
                }
                else if (game.backGroundNum != 0)
                {
                    DrawBackGroundColorInfo(spriteBatch);
                }
                else
                {
                    DrawControlsInfoEditMode(gameTime, spriteBatch);
                }
            }
            
        }
        public void DrawTransitionOpening(SpriteBatch spriteBatch)
        {

            //game.inTransition = false;
            if (curLeftMove2 > curLeftMove)
            {
                curRightMove2 += transitionRate;
                curLeftMove2 -= transitionRate;
            }
            else
            {

                firstTransition = false;
            }

            spriteBatch.Draw(leftCur.index, new Rectangle((int)(curLeftMove2),
                        (int)(0),
                        (int)(curWidth / 2f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            /*spriteBatch.Draw(leftCur2.index, new Rectangle((int)(curLeftMove2+(curWidth / 2f / 3f)),
                        (int)(0),
                        (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            spriteBatch.Draw(leftCur3.index, new Rectangle((int)(curLeftMove2 + (curWidth / 2f / 3f) + (curWidth / 2f / 3f)),
                        (int)(0),
                        (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            */
            
            spriteBatch.Draw(rightCur.index, new Rectangle((int)(curRightMove2),
                        (int)(0),
                        (int)(curWidth / 2f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            /*spriteBatch.Draw(rightCur2.index, new Rectangle((int)(curRightMove2 + curRightMove2 * 2f / 3f),
                       (int)(0),
                       (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            spriteBatch.Draw(rightCur3.index, new Rectangle((int)(curRightMove2 + curRightMove2 / 3f),
                        (int)(0),
                        (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            */
            

        }
        public void DrawTransitionClosing(SpriteBatch spriteBatch)
        {
            if (curTopMove < 0)
                curTopMove+=3;
            if (curLeftMove < 0)
            {
                curLeftMove += transitionRate;
                curRightMove -= transitionRate;
            }
            else
            {

                game.inTransition = false;
                game.loadNewLevel = true;
            }

            spriteBatch.Draw(leftCur.index, new Rectangle((int)(curLeftMove),
                        (int)(0),
                        (int)(curWidth/2f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            spriteBatch.Draw(rightCur.index, new Rectangle((int)(curRightMove),
                        (int)(0),
                        (int)(curWidth / 2f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            /*spriteBatch.Draw(rightCur2.index, new Rectangle((int)(curRightMove+curRightMove*2f/3f),
                        (int)(0),
                        (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            spriteBatch.Draw(rightCur3.index, new Rectangle((int)(curRightMove+curRightMove/3f),
                        (int)(0),
                        (int)(curWidth / 2f / 3f), (int)(curHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            */
            //spriteBatch.Draw(topCur.index, new Rectangle((int)(0),
            //        (int)(curTopMove),
            //        (int)(topCurWidth), (int)(topCurHeight)), null, Color.White, 0, new Vector2(1, 1), SpriteEffects.None, 0f);
            
        }

        public void DrawBackGroundColorInfo(SpriteBatch spriteBatch)
        {
            Vector3 colorVector = game.backGroundColor.ToVector3();
            DrawText(spriteBatch, 0.2f, 0.8f, "R:"+(int)(colorVector.X*252));
            DrawText(spriteBatch, 0.2f, 0.85f, "G:" + (int)(colorVector.Y * 252));
            DrawText(spriteBatch, 0.2f, 0.9f, "B:" + (int)(colorVector.Z * 252));
        
        }
        public void DrawControlsInfo(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float currentPos = 0;
            float increment = 0.025f;
            float position = 0.90f;
            if (!Constants.FULLSCREEN)
            {
                position = 0.75f;
            }
            DrawText(spriteBatch, position, currentPos += increment, "Controls Info");
            DrawText(spriteBatch, position, currentPos += increment, "Q and W Change sprite");
            DrawText(spriteBatch, position, currentPos += increment, "A: Normal Block");
            DrawText(spriteBatch, position, currentPos += increment, "S: Death Block");
            DrawText(spriteBatch, position, currentPos += increment, "D: Moving Block");
            DrawText(spriteBatch, position, currentPos += increment, "F: Goal Block");
            DrawText(spriteBatch, position, currentPos += increment, "Z: Save");
            DrawText(spriteBatch, position, currentPos += increment, "J and L Change Sprite Length");
            DrawText(spriteBatch, position, currentPos += increment, "I and K Change Sprite Height");
            DrawText(spriteBatch, position, currentPos += increment, "B and N Change Template Level");
            DrawText(spriteBatch, position, currentPos += increment, "V: load new template");
            DrawText(spriteBatch, position, currentPos += increment, "O and P change Save Location");
            DrawText(spriteBatch, position, currentPos += increment, "C: Change Mouse to Select");
            DrawText(spriteBatch, position, currentPos += increment, "V: New Template");
            DrawText(spriteBatch, position, currentPos += increment, "Shift to change loadLocation");
            DrawText(spriteBatch, position, currentPos += increment, "Tab to enter test mode");
            DrawText(spriteBatch, position, currentPos += increment, "M to change into delete mode");
            DrawText(spriteBatch, position, currentPos += increment, "L to change Background");
            DrawText(spriteBatch, position, currentPos += increment, "1,2 Rotate Block");
            DrawText(spriteBatch, position, currentPos += increment, "6 set cameraPosition");
        }

        public void DrawMouseCameraValue(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String position = "";
            switch (game.cameraWindowValue)
            {
                case 0:
                    return;
                case 1:
                    position = "Buttom";
                    break;
                case 2:
                    position = "Top";
                    break;
                case 3:
                    position = "Right";
                    break;
                case 4:
                    position = "Left";
                    break;
            }
            
            DrawText(spriteBatch, 0.065f, 0.65f, "Camera = " + position);
        }

        public void DrawControlsInfoEditMode(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawText(spriteBatch, 0.90f, 0.05f, "Controls Info");
            DrawText(spriteBatch, 0.90f, 0.075f, "Q and W Shift Block");
            DrawText(spriteBatch, 0.90f, 0.10f, "Delete, Remove Block");           
        }

        public void DrawMouse(GameTime gameTime, SpriteBatch spriteBatch)
        {
            MouseState ms = Mouse.GetState();
            Rectangle rect= new Rectangle(ms.X,ms.Y,10,15);
            if (ms.LeftButton == ButtonState.Pressed)
            {
                spriteBatch.Draw(game.getSprite("MouseClick").index, rect, Color.White);
            }
            else
            {
                spriteBatch.Draw(game.getSprite("MouseImage").index, rect, Color.White);
            }
        
        }


        public void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, float alpha)
        {
            spriteBatch.Draw(pix.index, rect, new Color(color.R, color.G, color.B, alpha));
        }
        public void DrawBackThing(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(game.getSprite("DeathTime").index.Width / 2, game.getSprite("DeathTime").index.Height / 2);

            spriteBatch.Draw(game.getSprite("DeathTime").index, new Rectangle((int)(game.getWorldSize().X * 0.140f),                
                (int)(game.getWorldSize().Y * 0.06f),
                (int)game.getSprite("DeathTime").index.Width, (int)game.getSprite("DeathTime").index.Height), null, Color.White, 0, origin, SpriteEffects.FlipHorizontally, 0f);

        }

        public void DrawCredits(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (game.pause)
                return;
            game.stopWatch.Stop();
            TimeSpan ts = game.stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
             ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            String str = "Time: " + elapsedTime + " Death: " + game.numDeath; 
            if(!game.winningAnimation)
            game.stopWatch.Start();

            DrawText(spriteBatch, 0.065f, 0.05f, str);            
        }
        public void DrawCreatorInformation(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String blockInfo = "Current Block Type = ";

            switch (game.blockType)
            {
                case Game.BlockType.Normal:
                    blockInfo += "Normal";
                    break;
                case Game.BlockType.Death:
                    blockInfo += "Death";
                    break;
                case Game.BlockType.Moving:
                    blockInfo += "Moving";
                    DrawText(spriteBatch, 0.065f, 0.875f, "X:" + game.moveSpeed.X+ "Y:" + game.moveSpeed.Y);
                    break;
                case Game.BlockType.Goal:
                    blockInfo += "Goal";
                    break;
                case Game.BlockType.Magnet:
                    blockInfo += "Magnet";
                    DrawText(spriteBatch, 0.065f, 0.875f, "X:"+game.magnetPulse.X + "Y:" + game.magnetPulse.Y);
                    break;
                case Game.BlockType.Path:
                    blockInfo += "PathBlock";
                    DrawText(spriteBatch, 0.065f, 0.875f, "Vel: "+game.pathSpeed);
                    break;
                case Game.BlockType.Cycle:
                    blockInfo += "Cycle Block";
                    DrawText(spriteBatch, 0.065f, 0.875f, "Vel: " + game.pathSpeed);
                    break;

            }
            DrawText(spriteBatch, 0.065f, 0.85f, blockInfo);

            String drawLevel = "";
            if (game.drawLevel == 1)
            {
                drawLevel = "Background";
            }
            else if (game.drawLevel == 2)
            {
                drawLevel = "Forground";
            }
            else if (game.drawLevel == -1)
            {
                drawLevel = "Super Background";
            }
            else
            {
                drawLevel = "Normal";
            }

            DrawText(spriteBatch, 0.065f, 0.90f, "DrawLevel = "+drawLevel);
        }

        public void DrawPositionInformation(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String positionInfo = "X: " + (int)game.cXLocation + " Y: " + (int)game.cYLocation ;
            DrawText(spriteBatch, 0.465f, 0.85f, positionInfo);
        }

        public void DrawCurrentLevelInfo(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String currentLevel = "Current Level: " + game.currentWriteLevel;
            DrawText(spriteBatch, 0.465f, 0.90f, currentLevel);           

        }
        public void DrawCurrentTemplateLevel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String currentLevel = "Template Level: " + game.gameTemplateLevel +" from Templates";
            if(game.loadFromLevel)
                currentLevel = "Template Level: " + game.gameTemplateLevel + " from Levels";
            DrawText(spriteBatch, 0.605f, 0.90f, currentLevel);

        }

        public void DrawSaveText(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String currentLevel = "Saved file to: Level"+game.currentWriteLevel+".txt";
            if (game.saveAlpha >= 0)
                game.saveAlpha -= 0.01f;
            DrawText(spriteBatch, 0.465f, 0.95f, currentLevel); 
        }
        public void DrawCameraText(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String camera = "MaxRight= " + game.maxRight + " MaxLeft= " + game.maxLeft + "  MaxTop= " + game.maxLeft + " MaxButtom= " + game.maxButtom;
            DrawText(spriteBatch, 0.605f, 0.85f, camera);
        }
        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text)
        {
            DrawText(spriteBatch, x, y, text, 1);
        }

        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text, float fontSize)
        {
            String[] tempstrMulti = text.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = text.Split("|".ToCharArray());
            float fontRatio = 1;
            if (!Constants.FULLSCREEN)
            {
                fontRatio = 0.85f;
            }
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * x + 1.5f, (game.getWorldSize().Y * y+2.0f) + (font.MeasureString("A").Y * i))),
                    Color.Black,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f * fontSize * fontRatio,
                    SpriteEffects.None,
                    0);

            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * x, (game.getWorldSize().Y * y) + (font.MeasureString("A").Y * i))),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f * fontSize * fontRatio,
                    SpriteEffects.None,
                    0);

            

        }

    }
}
