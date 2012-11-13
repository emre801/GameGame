using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProtoDerp
{
    public class LevelSelect : Entity
    {

        //BattleOptions battleOptions;

        Sprite sprLevelSelect;
        Vector2 posLevelSelect;     
        float scaleFactor = 0.8f;
        int select = 0;
        const int numOptions = 4;
        Input player1;
        Vector2 origin;

        int levelNum = 1;
        //Input player1 = null;
        public LevelSelect(Game g)
            : base(g)
        {
            sprLevelSelect = game.getSprite("LeverSelect");
            posLevelSelect = new Vector2(Constants.GAME_WORLD_WIDTH * 0.11f, Constants.GAME_WORLD_HEIGHT * 0.41f);
            IsVisible = false;
            
            alpha = 1;
            this.scale = (float)Constants.GAME_WORLD_WIDTH / (float)sprLevelSelect.index.Width;
            player1 = game.playerOneInput;
            origin = new Vector2(sprLevelSelect.index.Width / 2, sprLevelSelect.index.Height / 2);
            
        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            if (IsVisible)
            {
                if (this.player1.IsDownPressed())
                {
                    if (levelNum != 1)
                        levelNum--;
                }
                if (this.player1.IsUpPressed())
                {
                    if (levelNum != Constants.MAX_WRITE_LEVEL)
                        levelNum++;
                }
                if (player1.isAPressed())
                {
                    game.gMode = 0;
                    game.isInLevelSelect = false;
                    game.playSong("Music//ForrestSounds");
                    game.currentLevel = levelNum;
                    game.populateWorld();                    
                    game.drawingTool.cam.Zoom = 0.55f * game.drawingTool.zoomRatio;
                    this.IsVisible = false;
                    this.dispose = true;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                DrawText(spriteBatch, 0.5f, 0.5f, "Current Level " + levelNum);
            }
        }

        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text)
        {
            String[] tempstrMulti = text.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = text.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * x, (game.getWorldSize().Y * y) + (font.MeasureString("A").Y * i))),
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
