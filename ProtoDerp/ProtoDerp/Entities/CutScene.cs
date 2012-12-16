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
        public CutScene(Game g, int cutSceneNumber)
            : base(g)
        {
            this.cutSceneNumber = cutSceneNumber;
            addCurrentPictures();
            player1 = game.playerOneInput;
            LoadContent();
            isVisible = true;
            keyInput = new KeyboardInput();

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
            
            if (isVisible)
            {
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
                        game.gMode = 0;
                        game.isInLevelSelect = false;
                        //game.playSong("Music//ForrestSounds");
                        //game.playRandonSong();
                        game.populateWorld();
                        game.drawingTool.cam.Zoom = 0.55f * game.drawingTool.zoomRatio;
                        this.isVisible = false;
                        this.dispose = true;
                    }


                }
            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //TODO add Draw so that it goes to fullScreen :)
            if(isVisible)
                spriteBatch.Draw(currentImage.index, game.drawingTool.getDrawingCoords(new Vector2(0, 0)), null, Color.White * alpha * 1f, 0, currentImage.origin, game.drawingTool.gameToScreen(1.0f), SpriteEffects.None, 0);
                
        }

    }
}
