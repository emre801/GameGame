using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

/**
 * 
 * The in-game arena where the game is played.
 * 
 */
namespace ProtoDerp
{
    public class Arena : Entity
    {
        public enum GameMode { MODE_SINGLE_PLAYER, MODE_MULTI_PLAYER };
        public GameMode mode;

        public int score = 0;

        //Arena boundaries
        public Rectangle bounds;

        //Important objects in Arena

        public PlayableCharacter player1 = null;
        public PlayableCharacter player2 = null;
        Sprite background;
       
        LinkedList<Block> blocks = new LinkedList<Block>();

        public float maxLeft, maxRight, maxTop, maxButtom;
        public GUI gui;

        /**
         * Constructor
         */
        public Arena(Game g, GameMode mode)
            : base(g)
        {
            this.mode = mode;
            Init();
            bounds = new Rectangle(0, 0, (int)Constants.GAME_WORLD_WIDTH, (int)Constants.GAME_WORLD_HEIGHT);
            this.scale = (float)Constants.GAME_WORLD_WIDTH / (float)background.index.Width;
            player1.LoadContent();
            background = game.getSprite("clouds");
            maxLeft = -100;
            maxRight = 800;
            maxTop = -800;
            maxButtom = 300;
            gui = new GUI(g);
        }

        public void setUpDemensions(float maxLeft, float maxRight, float maxTop, float maxButtom)
        {
            this.maxLeft = maxLeft;
            this.maxRight = maxRight;
            this.maxTop = maxTop;
            this.maxButtom = maxButtom;

        }

        /**
         * Initializes all entities inside of the arena
         */
        public void Init()
        {
            player1 = new PlayableCharacter(game, this, Constants.player1SpawnLocation, 1);
            player1.inputState = game.playerOneInput;
            //TODO: Load Level;
            LevelEditor le = new LevelEditor(game);
            if(game.gMode==0)
                le.readFile(game.currentLevel);  
            else
                le.readFile(8); 
            game.addEntity(player1);
            this.IsVisible = true;

            background = game.getSprite("black");
        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            //Pause Game
            if (!game.GUI.gameOver) //Can't pause on Game Over Screen
            {
                if (!game.IsPaused && (game.playerOneInput.IsPausePressed()))
                    game.Pause();
            }

            base.Update(gameTime, worldFactor);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background.index, game.drawingTool.getDrawingCoords(pos), null, blend, MathHelper.ToRadians(angle), background.origin, game.drawingTool.gameToScreen(scale), SpriteEffects.None, 0);
            Vector2 origin = new Vector2(1000, 1000);
            
            //spriteBatch.Draw(background.index, new Rectangle(0, 0, 8000, 8000), null, Color.White, 0, origin, SpriteEffects.None, 0f);
            
            base.Draw(gameTime, spriteBatch);
        }

        /**
         *  Disposes of all entities in the arena, in preparation for moving to another game state.
         */
        public void End()
        {
            game.clearEntities();
        }
    }
}
