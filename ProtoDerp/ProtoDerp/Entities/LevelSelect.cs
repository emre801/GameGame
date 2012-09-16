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

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                base.Draw(gameTime, spriteBatch);
                spriteBatch.Draw(sprLevelSelect.index, new Rectangle((int)400, (int)400, (int)sprLevelSelect.index.Width, (int)sprLevelSelect.index.Height), null, Color.White, 0, origin, SpriteEffects.None, 0f);

            }
        }

    }
}
