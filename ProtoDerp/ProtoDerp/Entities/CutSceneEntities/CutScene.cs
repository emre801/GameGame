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
        bool isFirstUpdate = true;
        public CutScene(Game g, int cutSceneNumber)
            : base(g)
        {
            this.cutSceneNumber = cutSceneNumber;
            game.cutScene = cutSceneNumber;
            if (this.cutSceneNumber > Constants.TOTAL_NUMBER_OF_WORLDS)
                this.cutSceneNumber = Constants.TOTAL_NUMBER_OF_WORLDS;
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
            game.aButtonValue = 0;
            createCutScene();
            game.orderNumber = 0;
            game.hackyGuiThing = true;
            

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
#if WINDOWS
                    images.Add(fi.Name, File.ReadAllText(fi.FullName));
#endif
                }
                catch
                {
                    int i = 0;
                }
            }


        }

        public void createCutScene()
        {
            if (!Constants.DO_CUT_SCENE || game.currentLevel > 1)
            {
                loadLevelInfo();
                return;
            }
//#if WINDOWS
            Stream stream = TitleContainer.OpenStream("Content\\CutScene" + cutSceneNumber + "\\info.txt");
            System.IO.StreamReader sReader = new System.IO.StreamReader(stream);
            String textInfo = sReader.ReadToEnd();

            StringReader sr = new StringReader(textInfo);
                
            String line;
            char[] delimiterChars = { ' ', ',', ':', '\t' };
            while ((line = sr.ReadLine()) != null)
            {
                    
                string[] words = line.Split(delimiterChars);
                float x = System.Convert.ToSingle(words[1]);
                float y = System.Convert.ToSingle(words[2]);
               
                if (words[0].Equals("Normal"))
                {
                    String spriteName = words[3];
                    CutSceneItem csi = new CutSceneItem(game, game.Arena, x, y, 1, spriteName,
                        System.Convert.ToSingle(words[4]), System.Convert.ToSingle(words[5]));
                    game.addEntity(csi);
                }
                if (words[0].Equals("AButt"))
                {
                    String spriteName = words[3];
                    bool butt=Boolean.Parse(words[5]);
                    CutSceneItem csi = new CutSceneItem(game, game.Arena, x, y, 1, spriteName,
                        System.Convert.ToSingle(words[4]), butt);
                    game.addEntity(csi);
                }
                if (words[0].Equals("AButtMoveCycle"))
                {
                    String spriteName = words[3];
                    bool butt = Boolean.Parse(words[5]);
                    CutSceneMovingItem csi = new CutSceneMovingItem(game, game.Arena, x, y, 
                        System.Convert.ToSingle(words[6]), 
                        System.Convert.ToSingle(words[7]), 1, spriteName,
                        System.Convert.ToSingle(words[4]), butt);
                    game.addEntity(csi);
                }
                if (words[0].Equals("NormalMoveCycle"))
                {
                    String spriteName = words[3];
                    CutSceneMovingItem csi = new CutSceneMovingItem(game, game.Arena, x, y,
                        System.Convert.ToSingle(words[6]),
                        System.Convert.ToSingle(words[7]), 1, spriteName,
                        System.Convert.ToSingle(words[4]), System.Convert.ToSingle(words[5]));
                    game.addEntity(csi);
                }
                if (words[0].Equals("MultipleMoves"))
                {
                    String spriteName = words[3];
                    float totalCount = x;
                    int sizeOfArray = (int)y;
                     bool butt = Boolean.Parse(words[4]);
                    Vector2[] paths = new Vector2[sizeOfArray];
                    int count = 0;
                    for (int i = 0; i < x;i=i+2 )
                    {
                        float xVal = System.Convert.ToSingle(words[i + 5]);
                        float yVal = System.Convert.ToSingle(words[i + 6]);
                        paths[count]=(new Vector2(xVal,yVal));
                        count++;
                    }
                    CutSceneMultipleMoves csmm = new CutSceneMultipleMoves(game, game.Arena, paths, 1, spriteName, butt);
                    game.addEntity(csmm);
                }
                if (words[0].Equals("Text"))
                {
                    //float numberOfLines=System.Convert.ToSingle(words[3]);
                    float orderNumber = System.Convert.ToSingle(words[3]);
                    //String[] text = new String[(int)numberOfLines];
                    List<String> textItems = new List<String>();
                    //for (int i = 0; i < numberOfLines; i++)
                    String lineText;
                    int i=0;
                    while(!(lineText=sr.ReadLine()).Contains("$"))
                    {
                        //text[i] = lineText;
                        textItems.Add(lineText);
                        i++;
                    }
                    String[] text=textItems.ToArray();
                    CutSceneText cst = new CutSceneText(game, game.Arena, new Vector2(x, y), 1, text,orderNumber);
                    game.addEntity(cst);
                }
            }
//#endif
        }

        public override void Update(GameTime gameTime, float worldFactor)
        {
            if (!Constants.DO_CUT_SCENE || game.currentLevel > 1)
            {
                loadLevelInfo();
                return;
            }

            if (isVisible)
            {
                game.pauseMusic();
                if (game.currentWorld > Constants.STARTING_WORLD)
                    game.doNotLoadLevel = true;
                if (game.currentWorld > Constants.STARTING_WORLD)
                    gui.Update(gameTime, worldFactor);
                if ((this.player1.isAPressed() || keyInput.IsNewKeyPressed(Keys.Enter)) && !game.ignoreAInputs)
                {
                    game.aButtonValue = game.aButtonValue + 1;

                }
                LinkedList<CutSceneItem> cutScenes = game.getEntitiesOfType<CutSceneItem>();
                bool skippToMain = true;
                if (!isFirstUpdate)
                {
                    foreach (CutSceneItem csi in cutScenes)
                    {
                        if (csi.IsVisible)
                        {
                            skippToMain = false;
                            break;
                        }
                    }
                    if (skippToMain)
                    {
                        loadLevelInfo();
                    }
                }
                isFirstUpdate = false;
            }
            //if(Constants.FULLSCREEN)
                //game.drawingTool.cam.Pos = new Vector2(800, 540);
            /*
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
                    game.aButtonValue = game.aButtonValue + 1;
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
             * */
            if (gui.isDoneClosing)
                loadLevelInfo();

        }

        public void loadLevelInfo()
        {
            //game.preLoadEachLevelWithOutGoingToTitle();
            game.hackyGuiThing = false;
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
            /*
            if (isVisible)
            {
                
                //spriteBatch.Draw(currentImage.index, game.drawingTool.getDrawingCoords(new Vector2(0, 0)), null, Color.White * alpha * 1f, 0, currentImage.origin, game.drawingTool.gameToScreen(1.0f), SpriteEffects.None, 0);
                spriteBatch.Draw(currentImage.index, new Rectangle((int)(curWidth/2),
                        (int)(curHeight/4),
                        (int)(currentImage.index.Width/2), (int)(currentImage.index.Height/2)), null, Color.White, 0,
                        new Vector2(currentImage.index.Width / 2, currentImage.index.Height/2), SpriteEffects.None, 0f);
                
                if (game.currentWorld > Constants.STARTING_WORLD)
                    gui.Draw(gameTime, spriteBatch);
            }   */
            if(isVisible)
                if (game.currentWorld > Constants.STARTING_WORLD)
                gui.Draw(gameTime, spriteBatch);
        }

    }
}
