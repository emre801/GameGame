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


            posSelectText = new Vector2(Constants.GAME_WORLD_WIDTH * 0.475f, Constants.GAME_WORLD_HEIGHT * 0.634f);

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

            keyInput = new KeyboardInput();
        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            keyInput.Update(gameTime);
            if (IsVisible)
            {
                game.drawingTool.resetCamera();


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

                posLogoText = new Vector2(Constants.GAME_WORLD_WIDTH * 0.3f, Constants.GAME_WORLD_HEIGHT * (0.301f));
                posLogoGear = new Vector2(Constants.GAME_WORLD_WIDTH * 0.036f, Constants.GAME_WORLD_HEIGHT * (0.079f + offsetGear));
                posLogoHandShort = new Vector2(Constants.GAME_WORLD_WIDTH * 0.127f, Constants.GAME_WORLD_HEIGHT * (0.457f + offsetGear));
                posLogoHandLong = new Vector2(Constants.GAME_WORLD_WIDTH * 0.309f, Constants.GAME_WORLD_HEIGHT * (0.148f + offsetGear));

                //Menu Control
                if ((this.player1.IsUpPressed() || keyInput.IsNewKeyPressed(Keys.Up)) && alpha >= 1)
                {
                    //game.getSound("Audio\\Waves\\menuNavigate").Play();
                    select -= 1;
                    if (select < 0)
                        select = numOptions - 1;
                   
                }
                else if ((this.player1.IsDownPressed() || keyInput.IsNewKeyPressed(Keys.Down)) && alpha >= 1)
                {
                   // game.getSound("Audio\\Waves\\menuNavigate").Play();
                    select += 1;
                    if (select > numOptions - 1)
                        select = 0;
                }
                else if ((this.player1.IsSelectPressed() || keyInput.IsNewKeyPressed(Keys.Space)) && alpha >= 1)
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
                posSelectInner = new Vector2(posSelectText.X, (posSelectText.Y - (Constants.GAME_WORLD_HEIGHT * 0.01f)) + ((float)select * (Constants.GAME_WORLD_HEIGHT * 0.08f)));
                posSelectOuter = new Vector2(posSelectText.X, (posSelectText.Y - (Constants.GAME_WORLD_HEIGHT * 0.01f)) + ((float)select * (Constants.GAME_WORLD_HEIGHT * 0.08f)));

                //Fade out effect
                if (decreaseAlpha != -1)
                {
                    alpha -= Constants.TITLE_FADEOUT_SPEED*.22f;

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
                        //game.playSong("Audio\\Mp3s\\BulletMusicInGame");
                        /*game.gMode = 0;
                        game.isInLevelSelect = false;
                        game.playSong("Music//ForrestSounds");
                        game.populateWorld();
                        game.drawingTool.cam.Zoom = 0.55f * game.drawingTool.zoomRatio;
                        */
                        IsVisible = false;
                        CutScene ct = new CutScene(game, 1);
                        game.addEntity(ct);


                    }
                    else if (decreaseAlpha == 1)
                    {
                        decreaseAlpha = -1;
                        game.isInLevelSelect = true;
                        game.levelSelect = new LevelSelect(game);
                        game.addEntity(game.levelSelect);
                        game.levelSelect.IsVisible = true;
                    }
                    else if (this.decreaseAlpha == 2)
                    {
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
                spriteBatch.Draw(sprSelectText.index, game.drawingTool.getDrawingCoords(posSelectText), null, Color.White * alpha * 1f, 0, sprSelectText.origin, game.drawingTool.gameToScreen(1.0f * scaleFactor), SpriteEffects.None, 0);
                game.GUI.DrawRectangle(spriteBatch, new Rectangle((int)game.drawingTool.gameToScreen(0), (int)game.drawingTool.gameToScreen(posSelectInner.Y), (int)game.drawingTool.gameXCoordToScreenCoordX(game.getWorldSize().X), (int)game.drawingTool.gameToScreen(sprSelectInner.index.Height * scaleFactor)), Color.Black, .75f);
                
            }
        }

        public void DrawCredits(GameTime gameTime, SpriteBatch spriteBatch)
        {
            String str;
            switch (this.creditDev % 4)
            {
                case 0:
                    str = "Andrew Marrero: Team Lead, Graphics, Audio";
                    break;
                case 1:
                    str = "John Erdogan: Graphics/Level Design";
                    break;
                case 2:
                    str = "Jeremy Schiff: Graphics Programming, Engine";
                    break;
                default:
                    str = "Vaibhav Verma: Gameplay Programming, Tutorial";
                    break;
            }
            String[] tempstrMulti = str.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            tempstrMulti = str.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    game.drawingTool.getDrawingCoords(new Vector2(game.getWorldSize().X * 0.065f, (game.getWorldSize().Y * 0.05f) + (font.MeasureString("A").Y * i))),
                    Color.Black * 0.75f * alpha * this.devAlpha,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);
            if (this.increaseDevAlpha)
                devAlpha += 0.0002f;
            else
                devAlpha -= 0.0002f;
            if (devAlpha >= 0.64f)
            {

                this.increaseDevAlpha = false;
            }
            if (devAlpha <= 0)
            {
                this.creditDev++;
                this.increaseDevAlpha = true;
            }
        }
    }
}
