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
        double life = 0;
        Sprite gameOverGUI;
        public bool gameOver = false;
        float gameOverAlpha = 0;
        int gameOverSelect = 0; //0 - TRY AGAIN, 1 - BACK TO TITLE
        int gameOverSelectCount = 2;
        int pauseSelect = 0; //0 - Resume Game, 1 - Back to Title
        int pauseSelectCount = 2;
        GameTime startTime;
        
        public GUI(Game g)
            : base(g)
        {
            this.drawPriority = Constants.GUI_DRAWPRI;
            game = g;

            pix = game.getSprite("MikeIcon");
            cir = game.getSprite("Circle");
            gameOverGUI = game.getSprite("gameOver");
            this.updatesWhenPaused = true;
        }

        public override void Update(GameTime gameTime, float worldSpeed)
        {

            

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(pix.index, new Rectangle(5,5,100,100), Color.White);
            if (game.gMode == 0)
            {
                DrawBackThing(gameTime, spriteBatch);
                DrawCredits(gameTime, spriteBatch);
            }
            if (game.gMode == 2)
            {
                if (!game.inDeleteMode)
                {
                    DrawCreatorInformation(gameTime, spriteBatch);
                    DrawPositionInformation(gameTime, spriteBatch);
                    DrawCurrentLevelInfo(gameTime, spriteBatch);
                    DrawSaveText(gameTime, spriteBatch);
                    DrawMouse(gameTime, spriteBatch);
                }
            }
            
        }

        public void DrawMouse(GameTime gameTime, SpriteBatch spriteBatch)
        {
            MouseState ms = Mouse.GetState();
            Rectangle rect= new Rectangle(ms.X,ms.Y,10,15);
            spriteBatch.Draw(game.getSprite("MouseImage").index, rect,Color.White);
        
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
            game.stopWatch.Stop();
            TimeSpan ts = game.stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
             ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            String str = "Time: " + elapsedTime + " Death: " + game.numDeath; 
            if(!game.winningAnimation)
            game.stopWatch.Start();

            String[] tempstrMulti = str.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = str.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.065f, (game.getWorldSize().Y * 0.05f) + (font.MeasureString("A").Y * i))),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);
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
                    break;
                case Game.BlockType.Goal:
                    blockInfo += "Goal";
                    break;

            }

            String[] tempstrMulti = blockInfo.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = blockInfo.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.065f, (game.getWorldSize().Y * 0.85f) + (font.MeasureString("A").Y * i))),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

        }

        public void DrawPositionInformation(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String positionInfo = "X: " + (int)game.cXLocation + " Y: " + (int)game.cYLocation ;

            String[] tempstrMulti = positionInfo.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = positionInfo.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.465f, (game.getWorldSize().Y * 0.85f) + (font.MeasureString("A").Y * i))),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

        }

        public void DrawCurrentLevelInfo(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String currentLevel = "Current Level: " + game.currentWriteLevel;

            String[] tempstrMulti = currentLevel.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = currentLevel.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.465f, (game.getWorldSize().Y * 0.90f) + (font.MeasureString("A").Y * i))),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

        }

        public void DrawSaveText(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String currentLevel = "Saved file to: Level"+game.currentWriteLevel+".txt";
            if (game.saveAlpha >= 0)
                game.saveAlpha -= 0.01f;
            String[] tempstrMulti = currentLevel.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = currentLevel.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.465f, (game.getWorldSize().Y * 0.95f) + (font.MeasureString("A").Y * i))),
                    Color.White*game.saveAlpha,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

        }

    }
}
