﻿using System;
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
        public Button[] buttons;

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
            buttons = new Button[g.blockCounter-9];
            int bCounter = 0;
            
            foreach (String i in g.blockList)
            {
                Button b ;
                if(bCounter%3==0)
                    b = new Button(g, new Vector2(10, (5 + 50 * bCounter)/3), bCounter, i);
                else if(bCounter%3==1)
                    b = new Button(g, new Vector2(50, (5 + 50 * (bCounter-1))/3), bCounter, i);
                else
                    b = new Button(g, new Vector2(90, (5 + 50 * (bCounter - 2)) / 3), bCounter, i);
                //gamaddEntity(b);
                buttons[bCounter] = b;
                bCounter++;
            }
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
                le.readFile(game.gameTemplateLevel); 
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
            if (game.gMode == 2)
            {
                int count = 0;
                foreach (Button b in buttons)
                {
                    b.Update(gameTime, worldFactor);
                    count++;
                }

            }

            base.Update(gameTime, worldFactor);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(background.index, game.drawingTool.getDrawingCoords(pos), null, blend, MathHelper.ToRadians(angle), background.origin, game.drawingTool.gameToScreen(scale), SpriteEffects.None, 0);
            Vector2 origin = new Vector2(1000, 1000);
            
            //spriteBatch.Draw(background.index, new Rectangle(0, 0, 8000, 8000), null, Color.White, 0, origin, SpriteEffects.None, 0f);
            if (game.isInCreatorMode)
            {
                for (int i = (int)game.Arena.maxLeft; i < game.Arena.maxRight; i++)
                {
                    DrawText(spriteBatch, i, game.Arena.maxTop, "*");
                    DrawText(spriteBatch, i, game.Arena.maxButtom, "*");
                }
                for (int i = (int)game.Arena.maxTop; i < game.Arena.maxButtom; i++)
                {
                    DrawText(spriteBatch, game.Arena.maxLeft, i, "*");
                    DrawText(spriteBatch, game.Arena.maxRight, i, "*");
                }
            }
            base.Draw(gameTime, spriteBatch);
        }

        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text)
        {
            String[] tempstrMulti = text.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = text.Split("|".ToCharArray());
            Color colo = Color.Red;
            PlayableCharacter p1 = game.Arena.player1;

            if (game.Arena.maxLeft < p1.Position.X && game.Arena.maxRight > p1.Position.X
                && game.Arena.maxTop < p1.Position.Y && game.Arena.maxButtom > p1.Position.Y)
            {
                colo = Color.Green;
            }

            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    new Vector2((x), (y) + (font.MeasureString("A").Y * i)),
                    colo,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

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
