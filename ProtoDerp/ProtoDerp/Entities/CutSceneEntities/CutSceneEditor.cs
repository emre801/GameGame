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
    class CutSceneEditor
    {
        public int currentFrame = 0;
        List<CutSceneObj> cutSceneObjects;
        KeyboardInput keyInput;
        MouseState oldMouse;
        Game game;
        String fileName="CutScene0";
        bool runEditor = false;
        public enum CutSceneItemMode { NORMAL, TEXT };
        CutSceneItemMode csim = CutSceneItemMode.TEXT;
        public Stopwatch stopWatch = new Stopwatch();
        public CutSceneEditor(Game game,bool runEditor)
        {
            cutSceneObjects = new List<CutSceneObj>();
            keyInput = new KeyboardInput();
            this.game = game;
            initCutScenes();
            String file = "";
            if (!templates.TryGetValue(fileName, out file))
            {
                try
                {
                    file = templates.Values.First<string>();
                }
                catch
                {

                }
            }
            readCutScene(file);
            this.runEditor = runEditor;
            stopWatch.Start();

        }
        private static Dictionary<string, string> templates = new Dictionary<string, string>();

        public void initCutScenes()
        {
            DirectoryInfo di = new DirectoryInfo(@"Content\CutSceneInfo");
            String fullname = di.FullName;
            foreach (FileInfo fi in di.GetFiles())
            {

                templates.Add(fi.Name, File.ReadAllText(fi.FullName));
            }


        }

        public void readCutScene(String file)
        {
            StringReader sr = new StringReader(file);
            String line;
            char[] delimiterChars = { ' ', ',', ':', '\t' };
            while ((line = sr.ReadLine()) != null)
            {
                string[] words = line.Split(delimiterChars);
                if(words[0].Equals("cso"))
                {
                    String imageName = words[1];
                    float xPos = System.Convert.ToSingle(words[2]) * game.drawingTool.worldDemension.X;
                    float yPos = System.Convert.ToSingle(words[3]) * game.drawingTool.worldDemension.Y;
                    int startFrame = (int)System.Convert.ToSingle(words[4]);
                    int endFrame = (int)System.Convert.ToSingle(words[5]);
                    CutSceneObj obj = new CutSceneObj(startFrame, endFrame, imageName, new Vector2(xPos, yPos), game);
                    cutSceneObjects.Add(obj);
                }
                else if(words[0].Equals("csot"))
                {
                    float xPos = System.Convert.ToSingle(words[1]) * game.drawingTool.worldDemension.X;
                    float yPos = System.Convert.ToSingle(words[2]) * game.drawingTool.worldDemension.Y;
                    int startFrame = (int)System.Convert.ToSingle(words[3]);
                    List<String> textItems = new List<String>();
                    String lineText;
                    while (!(lineText = sr.ReadLine()).Contains("$"))
                    {
                        //text[i] = lineText;
                        textItems.Add(lineText);
                        
                    }
                    String[] text = textItems.ToArray();
                    CutSceneObjText obj = new CutSceneObjText(startFrame, "", new Vector2(xPos, yPos), game, text);
                    cutSceneObjects.Add(obj);

                }

            }


        }

        public void writeCutScene()
        {
            String path = "";
            int pathIndex = path.IndexOf("bin");
            //Need to change this.....
            path = @"C:\Users\John\Documents\visual studio 2010\Projects\ProtoDerp\ProtoDerp\ProtoDerpContent\CutSceneInfo\"+fileName+ @".txt";
            LinkedList<String> lines = new LinkedList<String>();
            foreach (CutSceneObj cso in cutSceneObjects)
            {
                if (cso is CutSceneObjText)
                {
                    
                    lines.AddLast("csot " + cso.pos.X / game.drawingTool.worldDemension.X + " " + cso.pos.Y / game.drawingTool.worldDemension.Y
                        + " " + cso.frameNum);
                    foreach (String i in ((CutSceneObjText)cso).text)
                    {
                        lines.AddLast(i);

                    }
                    lines.AddLast("$");
                }
                else
                {
                    lines.AddLast("cso " + cso.itemSprite.fileName + " " + cso.pos.X / game.drawingTool.worldDemension.X + " " + cso.pos.Y / game.drawingTool.worldDemension.Y
                        + " " + cso.frameNum +" "+cso.endFrame);
                }
            }
            
            System.IO.File.WriteAllLines(path, lines);
        }

        public void updateEditor()
        {
            if (keyInput.IsNewKeyPressed(Keys.Q))
            {
                if (currentFrame != 0)
                    currentFrame--;

            }
            if (keyInput.IsNewKeyPressed(Keys.R))
            {
                runEditor = false;
                currentFrame = 0;
                stopWatch.Restart();

            }
            if (keyInput.IsNewKeyPressed(Keys.W))
            {
                currentFrame++;
            }
            if (keyInput.IsNewKeyPressed(Keys.E))
            {
                if (csim == CutSceneItemMode.NORMAL)
                    csim = CutSceneItemMode.TEXT;
                else
                    csim = CutSceneItemMode.NORMAL;

            }
            if (keyInput.IsNewKeyPressed(Keys.Enter) ||
                (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released))
            {
                addCutSceneItem();
            }
            if (keyInput.IsNewKeyPressed(Keys.Z))
            {
                writeCutScene();
            }
        }

        public void addCutSceneItem()
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 20);
            Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));
            if (csim == CutSceneItemMode.NORMAL)
            {
                CutSceneObj obj = new CutSceneObj(currentFrame, currentFrame, "pixGavin", worldMousePosition, game);
                cutSceneObjects.Add(obj);
            }
            if (csim == CutSceneItemMode.TEXT)
            {
                String[] words ={" ........","1234567"};
                CutSceneObjText obj = new CutSceneObjText(currentFrame, "pixGavin", worldMousePosition, game, words);
                cutSceneObjects.Add(obj);

            }

        }
        public void runCutScene(GameTime gameTime)
        {
            stopWatch.Stop();
            foreach (CutSceneObj cso in cutSceneObjects)
            {
                if (cso.frameNum <= currentFrame && cso.endFrame>=currentFrame)
                {
                    cso.Update(gameTime);
                }

            }

            if (keyInput.IsNewKeyPressed(Keys.Enter) || stopWatch.ElapsedMilliseconds>=Constants.WAIT_TIME)
            {
                currentFrame++;
                stopWatch.Restart();
                isDoneRunning();
            }
            else
            {
                stopWatch.Start();
            }


        }
        public void isDoneRunning()
        {
            bool loadLevel = true;
            foreach (CutSceneObj cso in cutSceneObjects)
            {
                if (cso.frameNum <= currentFrame && cso.endFrame>=currentFrame)
                {
                    loadLevel = false;
                    break;
                }
            }
            if (loadLevel)
            {
                game.hackyGuiThing = false;
                game.gMode = 0;
                game.isInLevelSelect = false;
                game.playRandonSong();
                game.populateWorld();
                if (game.currentWorld > Constants.STARTING_WORLD)
                {
                    game.gameDoneLoading = true;
                    game.inCutScene = false;
                    game.worldFinished = true;
                    game.doNotLoadLevel = false;
                    game.playRandonSong();
                    game.ballPosition = 0;
                }
            }




        }
        
        public void Update(GameTime gameTime)
        {
            keyInput.Update(gameTime);

            if (runEditor)
            {
                updateEditor();
            }
            else
            {
                runCutScene(gameTime);

            }


            oldMouse = Mouse.GetState();

            

        }

        public void DrawMouse(SpriteBatch spriteBatch)
        {
             MouseState ms = Mouse.GetState();
                Rectangle rect = new Rectangle(ms.X, ms.Y, 10, 15);
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    spriteBatch.Draw(game.getSprite("MouseClick").index, rect, Color.White);
                }
                else
                {
                    spriteBatch.Draw(game.getSprite("MouseImage").index, rect, Color.White);
                }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            game.drawingTool.beginBatchGui();
            if (runEditor)
            {
                game.drawingTool.DrawText(spriteBatch, 0, 0, "Current frame " + currentFrame, 0.5f);

                String mode = "";
                switch (csim)
                {
                    case CutSceneItemMode.NORMAL:
                        mode = "Normal";
                        break;
                    case CutSceneItemMode.TEXT:
                        mode = "Text";
                        break;

                }

                game.drawingTool.DrawText(spriteBatch, 0, game.drawingTool.worldDemension.Y*0.9f, mode, 0.5f);

            }
            DrawMouse(spriteBatch);
            
            foreach (CutSceneObj cso in cutSceneObjects)
            {
                if (cso.frameNum <= currentFrame && cso.endFrame>=currentFrame)
                {
                    cso.Draw(game.drawingTool.spriteBatch);
                }

            }
            game.drawingTool.endBatch();

        }
    }
    
}
