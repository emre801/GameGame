using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoDerp
{
    public class TitleScreen : Entity
    {

        //BattleOptions battleOptions;

        Sprite sprBackground;
        Sprite sprLogoText;
        Sprite sprLogoGear;
        Sprite sprLogoHandShort;
        Sprite sprLogoHandLong;
        Sprite sprSelectInner;
        Sprite sprSelectOuter;
        Sprite sprSelectText;

        Vector2 posLogoText;
        Vector2 posLogoGear;
        Vector2 posLogoHandShort;
        Vector2 posLogoHandLong;
        Vector2 posSelectInner;
        Vector2 posSelectOuter;
        public Vector2 posSelectText;

        float angleHandShort = 0;
        float angleHandLong = 0;
        float offsetGear = 0;
        float offsetLogo = 0;
        float selectionAlpha = 0;
        int creditDev = 0;
        float devAlpha = 0.0f;
        bool increaseDevAlpha = true;
        float scaleFactor = 0.41f;

        double life = 0;
        Input player1 = null;

        int select = 0;
        const int numOptions = 4;
        int decreaseAlpha = -1; //If set to something other than -1, fades the title screen out, and performs
        //the parallel option (that matches select)

        KeyboardInput keyInput;
        Button start, selectLevel, creativeMode, exit;
        bool moveDirection;
        int moveTimer = 0;

        public TitleScreen(Game g)
            : base(g)
        {

            sprBackground = game.getSprite("arena");
            sprLogoText = game.getSprite("RageQuit");
            sprLogoGear = game.getSprite("titleScreenElement.1");
            sprSelectInner = game.getSprite("titleScreenElement.2");
            sprSelectOuter = game.getSprite("titleScreenElement.3");
            sprSelectText = game.getSprite("titleScreenElement.4");
            sprLogoHandLong = game.getSprite("titleScreenElement.5");
            sprLogoHandShort = game.getSprite("titleScreenElement.6");


            posSelectText = new Vector2(Constants.GAME_WORLD_WIDTH * 0.5f, Constants.GAME_WORLD_HEIGHT * 0.634f);
            if (Constants.FULLSCREEN)
            {
                posSelectText = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.634f);

            }
            angleHandShort = 135;
            angleHandLong = 350;
            alpha = 1;

            //game.playSong("Audio\\Mp3s\\BulletMusicTitle");

            this.scale = (float)Constants.GAME_WORLD_WIDTH / (float)sprBackground.index.Width;

            player1 = game.playerOneInput;

            //game.player1Score = 0;
            //game.player2Score = 0;

            //battleOptions = new BattleOptions(g, this);
            //game.addEntity(battleOptions);
            //battleOptions.IsVisible = false;
            Sprite startSP=game.getSprite("start");
            Sprite selectLevelSP = game.getSprite("SelectLevel");
            Sprite creativeSP = game.getSprite("CreativeMode");
            Sprite exitSP = game.getSprite("Exit");
            float spaceValue = 0.45f;
            if (Constants.FULLSCREEN)
                spaceValue = 0.65f;

            Vector2 v1 = new Vector2(posSelectText.X, (posSelectText.Y - (Constants.GAME_WORLD_HEIGHT * 0.01f)));
            Vector2 v2 = new Vector2(posSelectText.X, v1.Y + startSP.index.Height * spaceValue * game.drawingTool.zoomRatio + 20 * game.drawingTool.zoomRatio);
            Vector2 v3 = new Vector2(posSelectText.X, v2.Y + selectLevelSP.index.Height * spaceValue * game.drawingTool.zoomRatio + 20 * game.drawingTool.zoomRatio);
            Vector2 v4 = new Vector2(posSelectText.X, v3.Y + creativeSP.index.Height * spaceValue * game.drawingTool.zoomRatio + 20 * game.drawingTool.zoomRatio);

            start = new Button(g, v1, 0, "start");
            start.setTitleValuee(0);
            selectLevel = new Button(g, v2, 0, "SelectLevel");
            selectLevel.setTitleValuee(1);
            creativeMode = new Button(g, v3, 0, "CreativeMode");
            creativeMode.setTitleValuee(2);
            exit = new Button(g, v4, 0, "Exit");
            exit.setTitleValuee(3);

            game.fadeAlpha = 1f;
            game.drawingTool.resetCamera();
            keyInput = new KeyboardInput();

            posLogoText = new Vector2(Constants.GAME_WORLD_WIDTH * 0.3f, Constants.GAME_WORLD_HEIGHT * (0.301f));
                
        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            keyInput.Update(gameTime);
            if (IsVisible)
            {
                //game.drawingTool.resetCamera();
                
                //Update menu buttons
                start.Update(gameTime, worldFactor);
                selectLevel.Update(gameTime, worldFactor);
                creativeMode.Update(gameTime, worldFactor);
                exit.Update(gameTime, worldFactor);

                //Move buttons up and down
                /*
                if (moveTimer % 30 == 0)
                {
                    if (moveDirection)
                    {
                        posLogoText += game.drawingTool.getDrawingCoords(new Vector2(0, 1f));
                        start.moveButton(new Vector2(0, 1f));
                        selectLevel.moveButton(new Vector2(0, 1f));
                        creativeMode.moveButton(new Vector2(0, 1f));
                        exit.moveButton(new Vector2(0, 1f));
                    }
                    else
                    {
                        posLogoText += game.drawingTool.getDrawingCoords(new Vector2(0, -1f));
                        start.moveButton(new Vector2(0, -1f));
                        selectLevel.moveButton(new Vector2(0, -1f));
                        creativeMode.moveButton(new Vector2(0, -1f));
                        exit.moveButton(new Vector2(0, -1f));
                    }
                }

                moveTimer++;
                if (moveTimer == 120)
                {
                    moveTimer = 0;
                    moveDirection = !moveDirection;
                }
                 * */
                 

                life += gameTime.ElapsedGameTime.TotalMilliseconds;
                game.drawingTool.cam.Zoom = 0.95f*game.drawingTool.zoomRatio;
                //Clock hand ticking
                angleHandShort -= 0.25f;
                if (angleHandShort < 135 - 27)
                    angleHandShort = 135;
                angleHandLong -= 0.25f;
                if (angleHandLong < 350 - 27)
                    angleHandLong = 350;

                //Logo oscillation
                offsetGear = (float)Math.Sin(life * 0.001) * 0.01f;
                offsetLogo = (float)Math.Cos(life * 0.001) * 0.01f;
                selectionAlpha = (float)(Math.Abs(Math.Cos(life * 0.00075)) + 0.15f) * 0.3f;

                //posLogoText = new Vector2(Constants.GAME_WORLD_WIDTH * 0.3f, Constants.GAME_WORLD_HEIGHT * (0.301f));
                //posLogoGear = new Vector2(Constants.GAME_WORLD_WIDTH * 0.036f, Constants.GAME_WORLD_HEIGHT * (0.079f + offsetGear));
                //posLogoHandShort = new Vector2(Constants.GAME_WORLD_WIDTH * 0.127f, Constants.GAME_WORLD_HEIGHT * (0.457f + offsetGear));
                //posLogoHandLong = new Vector2(Constants.GAME_WORLD_WIDTH * 0.309f, Constants.GAME_WORLD_HEIGHT * (0.148f + offsetGear));

                //Menu Control
                if ((this.player1.IsUpPressed() || keyInput.IsNewKeyPressed(Keys.Up)) && alpha >= 1)
                {
                    //game.getSound("Audio\\Waves\\menuNavigate").Play();
                    game.sounds["Rage//Wave//menu"].Play();
                    select -= 1;
                    if (select < 0)
                        select = numOptions - 1;
                   
                }
                else if ((this.player1.IsDownPressed() || keyInput.IsNewKeyPressed(Keys.Down)) && alpha >= 1)
                {
                    game.sounds["Rage//Wave//menu"].Play();
                   // game.getSound("Audio\\Waves\\menuNavigate").Play();
                    select += 1;
                    if (select > numOptions - 1)
                        select = 0;
                }
                else if ((this.player1.IsSelectPressed() || keyInput.IsNewKeyPressed(Keys.Enter)) && alpha >= 1)
                {
                    //game.getSound("Audio\\Waves\\menuSelect").Play();
                    game.sounds["Rage//Wave//Opening"].Play();
                    if (select == 0) //Start Game
                        decreaseAlpha = 0;
                    else if (select == 1) //Battle Options
                        decreaseAlpha = 1;
                    else if (select == 2) //Tutoria;
                        decreaseAlpha = 2;
                    else if (select == 3) //End Game
                        decreaseAlpha = 3;
                }
                if (game.isCollidingWithButton)
                {
                    game.isCollidingWithButton = false;
                    select = game.gameTitleValue;
                }
                else
                {
                    game.gameTitleValue = select;
                }
                
                //posSelectInner = new Vector2(posSelectText.X, (posSelectText.Y - (Constants.GAME_WORLD_HEIGHT * 0.01f)) + ((float)select * (Constants.GAME_WORLD_HEIGHT * 0.08f)));
                //posSelectOuter = new Vector2(posSelectText.X, (posSelectText.Y - (Constants.GAME_WORLD_HEIGHT * 0.01f)) + ((float)select * (Constants.GAME_WORLD_HEIGHT * 0.08f)));

                //Fade out effect
                if (decreaseAlpha != -1)
                {
                    alpha -= Constants.TITLE_FADEOUT_SPEED*.22f;
                    game.fadeAlpha = alpha;

                    if (decreaseAlpha == 1)
                    {
                       // battleOptions.alpha = 1 - alpha;
                        //battleOptions.IsVisible = true;
                    }
                }
               // else if (alpha < 1 && battleOptions.IsVisible == false)
                   // alpha += Constants.TITLE_FADEOUT_SPEED;
                else if (alpha >= 1)
                    alpha = 1;

                if (alpha <= 0)
                {
                    if (decreaseAlpha == 0)
                    {
                        //Start Game
                        IsVisible = false;
                        CutScene ct = new CutScene(game, 1);
                        game.addEntity(ct);


                    }
                    else if (decreaseAlpha == 1)
                    {
                        //Level Select
                        decreaseAlpha = -1;
                        IsVisible = false;
                        game.isInLevelSelect = true;
                        game.levelSelect = new LevelSelect(game);
                        game.addEntity(game.levelSelect);
                        game.levelSelect.IsVisible = true;
                    }
                    else if (this.decreaseAlpha == 2)
                    {
                        //Creative Mode
                        game.isInLevelSelect = false;
                        game.drawingTool.cam.Zoom = 0.35f * game.drawingTool.zoomRatio;
                        game.populateWorldCreatorMode();
                        game.gMode = 2;
                        IsVisible = false;
                    }
                    else if (this.decreaseAlpha == 3)
                        Environment.Exit(0);

                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                base.Draw(gameTime, spriteBatch);
                spriteBatch.Draw(sprLogoText.index, game.drawingTool.getDrawingCoords(posLogoText), null, Color.White * alpha * 1f, 0, sprLogoText.origin, game.drawingTool.gameToScreen(1.0f * scaleFactor), SpriteEffects.None, 0);
                //spriteBatch.Draw(sprSelectText.index, game.drawingTool.getDrawingCoords(posSelectText), null, Color.White * alpha * 1f, 0, sprSelectText.origin, game.drawingTool.gameToScreen(1.0f * scaleFactor), SpriteEffects.None, 0);
                //game.GUI.DrawRectangle(spriteBatch, new Rectangle((int)game.drawingTool.gameToScreen(0), (int)game.drawingTool.gameToScreen(posSelectInner.Y), (int)game.drawingTool.gameXCoordToScreenCoordX(game.getWorldSize().X), (int)game.drawingTool.gameToScreen(sprSelectInner.index.Height * scaleFactor)), Color.Black, .75f);
                start.Draw(gameTime, spriteBatch);
                selectLevel.Draw(gameTime, spriteBatch);
                creativeMode.Draw(gameTime, spriteBatch);
                exit.Draw(gameTime, spriteBatch);
                game.GUI.DrawMouse(gameTime, spriteBatch);    
            
            }
        }
    }
}
