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


namespace ProtoDerp
{
    class CutSceneText : CutSceneItem
    {
        KeyboardInput ki = new KeyboardInput();
        XboxInput xi;
        String[] text;
        int textValue = 0;
        int textSpeed = 2;
        int textSpeedValue = 0;
        int currentTextValue = 0;
        bool stopSound = false;
        float boxLength = 1000;
        bool IsDoneOpening = false,isClosing=false;
        float orderNumber = 0;
        
        float openValue = 0;
        public CutSceneText(Game g, Arena a, Vector2 points, int playerNum, String[] text, float orderNumber)
            : base(g, a, points.X, points.Y, playerNum, "Error", -1, false)
        {
            xi = (XboxInput)game.playerOneInput;
            this.IsVisible = true;
            this.text = text;
            this.orderNumber = orderNumber;
        }


        public override void Update(GameTime gameTime, float worldSpeed)
        {
            if (IsVisible&& orderNumber==game.orderNumber)
            {

                if (IsDoneOpening)
                {
                    if (xi.isAPressed())
                    {
                        textValue++;
                        currentTextValue = 0;
                        stopSound = false;

                    }
                    if (textSpeedValue >= textSpeed)
                    {
                        currentTextValue++;
                        textSpeedValue = 0;
                        if (!stopSound)
                            game.sounds["Rage//Wave//menu"].Play();
                    }
                    textSpeedValue++;
                    if (textValue >= text.Length)
                    {
                        //IsVisible = false;
                        IsDoneOpening = false;
                        isClosing = true;
                    }
                }
                else
                {
                    if (isClosing)
                    {
                        openValue -= 10;
                        if (openValue == 0)
                        {
                            IsVisible = false;
                            game.orderNumber = game.orderNumber + 1;
                        }
                    }
                    else
                    {
                        openValue += 5;
                        if (openValue == 100)
                        {
                            IsDoneOpening = true;
                        }
                    }
                    


                }
            }
          
        }
        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text, float size)
        {

            char[] tempstrMulti = text.ToCharArray(); 
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_PIXEL];

            drawBorderImage(x - font.MeasureString("A").X * size, y - font.MeasureString("A").Y*size*0.5f, 100, (int)(boxLength * size * 0.75f), spriteBatch);
            
            if (currentTextValue > tempstrMulti.Length)
            {
                currentTextValue = tempstrMulti.Length;
                stopSound = true;
            }
            float numOfCharsInLine = boxLength*size / (font.MeasureString("A").Y * size);
            int counter = 0;
            while (counter + numOfCharsInLine < tempstrMulti.Length)
            {
                float pos = counter + numOfCharsInLine;
                while (!tempstrMulti[(int)(pos)].Equals(' '))
                {
                    pos--;
                }
                tempstrMulti[(int)(pos)] = '{';
                counter = (int)pos;
            }
            float drawPosX = 0;
            float drawPosY = 0;
            for (int i = 0; i < currentTextValue; i += 1)
            {
                if ("{".Equals("" + tempstrMulti[i]))
                {
                    drawPosX = 0;
                    drawPosY += font.MeasureString("A").Y * size;
                    continue;
                }
                spriteBatch.DrawString(font, ""+tempstrMulti[i],
                    new Vector2(x + drawPosX, y+drawPosY),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    1f*size,
                    SpriteEffects.None,
                    0);
                drawPosX += font.MeasureString(""+tempstrMulti[i]).X*size;
                if (drawPosX > boxLength*size)
                {
                    drawPosX = 0;
                    drawPosY += font.MeasureString("A").Y * size;
                }
            }

        }
        public void drawBorderImage(float x, float y, int height, int width, SpriteBatch spriteBatch)
        {
            
            Rectangle rect = new Rectangle((int)((x)), (int)((y)), width, height);
            Rectangle rect2 = new Rectangle((int)((x)) - 2, (int)((y)) - 2, width + 4, height + 4);
            Rectangle rect3 = new Rectangle((int)((x)) - 4, (int)((y)) - 4, width + 8, height + 8);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect3, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImageWhite").index, rect2, Color.White);

            spriteBatch.Draw(game.getSprite("BorderImage").index, rect, Color.White);

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible && game.orderNumber==orderNumber)
            {
                if (IsDoneOpening)
                {
                    DrawText(spriteBatch, pos.X, pos.Y, text[textValue], 0.5f);
                }
                else
                {
                    SpriteFont font = game.fonts[(int)Game.Fonts.FT_PIXEL];

                    drawBorderImage(pos.X - font.MeasureString("A").X * 0.5f, pos.Y - font.MeasureString("A").Y * 0.5f * 0.5f, (int)(100 * (openValue / 100f)), (int)((boxLength * 0.5f * 0.75f) * (openValue / 100f)), spriteBatch);
            


                }
            }
            
        }
    }
}
