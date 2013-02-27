using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProtoDerp
{
    
    class CutScene : Entity
    {
        LinkedList<Sprite> pictures = new LinkedList<Sprite>();
        int cutSceneNumber;
        int currentPic = 0;
        Sprite currentImage;
        Input player1 = null;
        private static Dictionary<string, string> images = new Dictionary<string, string>();
        public bool isVisible = false;
        KeyboardInput keyInput;
        GUI gui = null;
        float curHeight, curWidth;
        public CutScene(Game g, int cutSceneNumber)
            : base(g)
        {
            this.cutSceneNumber = cutSceneNumber;
            addCurrentPictures();
            player1 = game.playerOneInput;
            LoadContent();
            isVisible = true;
            keyInput = new KeyboardInput();

            if (!Constants.FULLSCREEN)
            {
                curHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                curWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.5f;
               
            }
            else
            {
                curHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height + 10f;
                curWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width+40;
                

            }
            if (cutSceneNumber > Constants.TOTAL_NUMBER_OF_WORLDS)
            {

                cutSceneNumber = Constants.TOTAL_NUMBER_OF_WORLDS;
            }
            
            gui = new GUI(game);

        }
        public void LoadContent()
        {
            currentImage = new Sprite(game.Content, "CutScene" + cutSceneNumber + "\\" + currentPic);
        }
        public void addCurrentPictures()
        {
            DirectoryInfo di = new DirectoryInfo(@"Content\CutScene"+cutSceneNumber);
            images = new Dictionary<string, string>();
            foreach (FileInfo fi in di.GetFiles())
            {
                try
                {
                    images.Add(fi.Name, File.ReadAllText(fi.FullName));
                }
                catch
                {
                    int i = 0;
                }
            }


        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            //if(Constants.FULLSCREEN)
                //game.drawingTool.cam.Pos = new Vector2(800, 540);

            if (game.currentLevel == -1)
            {
                game.currentLevel = 1;
            }

            if (game.currentLevel != 1)
            {
                loadLevelInfo();
                return;

            }
            if (isVisible)
            {
                game.pauseMusic();
                if(game.currentWorld>Constants.STARTING_WORLD)
                    game.doNotLoadLevel = true;
                if (game.currentWorld > Constants.STARTING_WORLD) 
                    gui.Update(gameTime, worldFactor);
                if (!Constants.DO_CUT_SCENE)
                {
                    loadLevelInfo();
                }

                keyInput.Update(gameTime);
                if (this.player1.isAPressed()|| keyInput.IsNewKeyPressed(Keys.Enter))
                {
                    if (images.Count > currentPic+1)
                    {
                        currentPic++;
                        LoadContent();
                    }
                    else
                    {
                        if (game.currentWorld == Constants.STARTING_WORLD)
                            loadLevelInfo();
                        else
                        {
                            game.inTransition = true;
                        }
                    }


                }
            }
            if (gui.isDoneClosing)
                loadLevelInfo();

        }

        public void loadLevelInfo()
        {
            game.gMode = 0;
            game.isInLevelSelect = false;
            //game.playSong("Music//ForrestSounds");
            game.playRandonSong();
            game.populateWorld();
           // game.drawingTool.cam.Zoom = 0.55f * game.drawingTool.zoomRatio;
            this.isVisible = false;
            this.dispose = true;
            if (game.currentWorld>Constants.STARTING_WORLD)
            {
                //game.inTransition = false;
                game.gameDoneLoading = true;
                game.inCutScene = false;
                game.worldFinished = true;
                game.doNotLoadLevel = false;
                game.playRandonSong();
                game.ballPosition = 0;
                //game.camZoomValue = 0.55f;
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //TODO add Draw so that it goes to fullScreen :)
            if (isVisible)
            {
                
                //spriteBatch.Draw(currentImage.index, game.drawingTool.getDrawingCoords(new Vector2(0, 0)), null, Color.White * alpha * 1f, 0, currentImage.origin, game.drawingTool.gameToScreen(1.0f), SpriteEffects.None, 0);
                spriteBatch.Draw(currentImage.index, new Rectangle((int)(curWidth/2),
                        (int)(curHeight/4),
                        (int)(currentImage.index.Width/2), (int)(currentImage.index.Height/2)), null, Color.White, 0,
                        new Vector2(currentImage.index.Width / 2, currentImage.index.Height/2), SpriteEffects.None, 0f);
                
                if (game.currentWorld > Constants.STARTING_WORLD)
                    gui.Draw(gameTime, spriteBatch);
            }   
        }

    }
}
