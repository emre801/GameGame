using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

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
            DrawCredits(gameTime, spriteBatch);
        }


        public void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color, float alpha)
        {
            spriteBatch.Draw(pix.index, rect, new Color(color.R, color.G, color.B, alpha));
        }

        public void DrawCredits(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String str = "Time:       Death: " + game.numDeath; ;
                
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





    }
}
