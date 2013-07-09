using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace ProtoDerp
{
    class CutSceneObjText: CutSceneObj
    {
        KeyboardInput ki = new KeyboardInput();
        XboxInput xi;
        public String[] text;
        int textValue = 0;
        int textSpeed = 2;
        int textSpeedValue = 0;
        int currentTextValue = 0;
        bool stopSound = false;
        float boxLength = 1000;
        bool IsDoneOpening = false, isClosing = false;
        //public float frameNum;
        KeyboardInput keyInput;
        Stopwatch stopWatch = new Stopwatch();
        public float openValue = 0;

        public bool IsVisible;
        bool firstTime = true;
        public CutSceneObjText(int frameNum, String spriteName, Vector2 pos, Game game, String[] text)
        {
            this.frameNum = frameNum;
            this.endFrame = frameNum + text.Length-1;
            xi = (XboxInput)game.playerOneInput;
            this.text = text;
            this.keyInput = new KeyboardInput();
            this.pos = pos;
            this.game = game;
            this.IsVisible = true;

        }

        public override void Update(GameTime gameTime)
        {
            //if (game.currentLevel > 1)
            // return;
            keyInput.Update(gameTime);
            if (IsVisible)
            {
                if (firstTime)
                {
                    firstTime = false;
                    stopWatch.Start();
                }

                if (IsDoneOpening)
                {
                    stopWatch.Stop();

                    if (xi.isAPressed() || keyInput.IsNewKeyPressed(Keys.Enter) || stopWatch.ElapsedMilliseconds>=Constants.WAIT_TIME)
                    {
                        textValue++;
                        currentTextValue = 0;
                        stopSound = false;
                        stopWatch.Reset();

                    }
                    if (!stopWatch.IsRunning)
                        stopWatch.Start();
                    if (textSpeedValue >= textSpeed)
                    {
                        currentTextValue++;
                        textSpeedValue = 0;
                        if (!stopSound)
                            game.sounds["Rage//Wave//menu"].Play();
                    }
                    textSpeedValue++;
                    if (textValue >= text.Length || xi.IsButtonPressed(Buttons.B))
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
                        if (openValue < 0)
                        {
                            //if (!game.isReadingText)
                            //    this.dispose = true;
                            IsVisible = false;
                            game.orderNumber = game.orderNumber + 1;
                            if (game.isReadingText)
                            {
                                game.isReadingText = false;
                                IsVisible = true;
                            }
                            textValue = 0;
                            IsDoneOpening = false;
                            isClosing = false;
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

            drawBorderImage(x - font.MeasureString("A").X * size * game.scale, y - font.MeasureString("A").Y * size * game.scale * 0.5f, 100,
                (int)(boxLength * size * game.scale * 0.75f), spriteBatch);

            if (currentTextValue > tempstrMulti.Length)
            {
                currentTextValue = tempstrMulti.Length;
                stopSound = true;
            }
            float numOfCharsInLine = boxLength * size * game.scale / (font.MeasureString("A").Y * size * game.scale);
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
                    drawPosY += font.MeasureString("A").Y * size * game.scale;
                    continue;
                }
                spriteBatch.DrawString(font, "" + tempstrMulti[i],
                    new Vector2(x + drawPosX, y + drawPosY),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    1f * size * game.scale,
                    SpriteEffects.None,
                    0);
                drawPosX += font.MeasureString("" + tempstrMulti[i]).X * size * game.scale;
                if (drawPosX > boxLength * size * game.scale)
                {
                    drawPosX = 0;
                    drawPosY += font.MeasureString("A").Y * size * game.scale;
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                if (IsDoneOpening)
                {
                    DrawText(spriteBatch, pos.X, pos.Y, text[textValue], 0.5f);
                }
                else
                {
                    SpriteFont font = game.fonts[(int)Game.Fonts.FT_PIXEL];
                    if (openValue >= 0)
                    {
                        drawBorderImage(pos.X - font.MeasureString("A").X * 0.5f, pos.Y - font.MeasureString("A").Y * 0.5f * 0.5f,
                            (int)(100 * (openValue / 100f)),
                            (int)((boxLength * 0.5f * game.scale * 0.75f) * (openValue / 100f)), spriteBatch);
                    }



                }
            }

        }






    }
}
