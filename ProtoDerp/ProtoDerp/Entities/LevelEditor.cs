using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace ProtoDerp
{
    class LevelEditor:Entity
    {
        float height, width;
        public LevelEditor(Game g)
            : base(g)
        {
            this.game = game;
            if (game.gMode == 0)
                initLevels("World"+game.currentWorld);
            else
            {
                if (!game.loadFromLevel)
                {
                    initLevels("Template");
                }
                else
                {
                    initLevels("World"+game.currentWorld);
                }
            }

            DirectoryInfo di = new DirectoryInfo(@"Content\" + "World");
            this.game.writeDirectory = fullLocation;
        }
        public PlayableCharacter player1 = null;
        public PlayableCharacter player2 = null;
        LinkedList<Block> blocks = new LinkedList<Block>();
        LinkedList<Block> topBlocks = new LinkedList<Block>();
        LinkedList<Block> buttomBlocks = new LinkedList<Block>();
        LinkedList<DeathBlock> deathBlocks = new LinkedList<DeathBlock>();
        LinkedList<MagnetBlock> magnetBlocks = new LinkedList<MagnetBlock>();
        LinkedList<GoalBlock> goalBlocks = new LinkedList<GoalBlock>();
        LinkedList<MovingDeath> moveDeathBlocks = new LinkedList<MovingDeath>();
        LinkedList<MovingPath> movePathBlock = new LinkedList<MovingPath>();
        LinkedList<MovingCycle> moveCycleBlock = new LinkedList<MovingCycle>();
        LinkedList<WaterBlock> waterBlocks = new LinkedList<WaterBlock>();
        public void readFile(int templateNum)
        {
            String path = Directory.GetCurrentDirectory();
            game.backGroundImages.Clear();
            int pathIndex = path.IndexOf("bin");
            path = @"Level" + templateNum + @".txt";

            string file;
            if (!templates.TryGetValue(path, out file))
            {
                file = templates.Values.First<string>();
                Console.WriteLine("Level " + path + " not found.");
                game.currentLevel = 1;
            }

            height = Constants.GAME_WORLD_HEIGHT;
            width = Constants.GAME_WORLD_WIDTH;
            
            createLevel(file);
        }

        public void createLevel(String file)
        {
            //if (game.inCutScene)
               // return;
            StringReader sr = new StringReader(file);
            String line;
            char[] delimiterChars = { ' ', ',', ':', '\t' };
            int c = 0;
            if(game.loadNewLevel)
                game.clearEntities();
            game.camZoomValue = -1;
            game.deathBlockGoalCollision = false;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            game.stopWatchLagTimer.Reset();
            game.magValue = 1;
            game.blockNumber = 0;
            while ((line = sr.ReadLine()) != null)
            {
                c++;
                string[] words = line.Split(delimiterChars);
                if (words[0].Equals("Demi"))
                {
                    game.maxLeft = Convert.ToInt32(words[1]);
                    game.maxRight = Convert.ToInt32(words[2]);
                    game.maxTop = Convert.ToInt32(words[3]);
                    game.maxButtom = Convert.ToInt32(words[4]);
                    continue;
                }
                if (words[0].Equals("BGCOLOR"))
                {
                    game.backGroundColor= new Color((float)(Convert.ToInt32(words[1]))/252f,
                        (float)(Convert.ToInt32(words[2]))/252f,(float)(Convert.ToInt32(words[3]))/252f);
                    continue;

                }
                if (words[0].Equals("CAM"))
                {

                    float camValue=System.Convert.ToSingle(words[1]);
                    game.drawingTool.cam.Zoom = camValue;
                    game.camZoomValue = camValue;
                    continue;
                }
                if (words[0].Equals("CPOS"))
                {
                    game.camPosSet = new Vector2(System.Convert.ToSingle(words[1]), System.Convert.ToSingle(words[2]));
                    continue;
                }
                float x=Convert.ToInt32(words[0]);
                float y = Convert.ToInt32(words[1]);
                String spriteName = words[2];
                if (words[3].Equals("Block"))
                {
                   
                    float rotation = 0;
                    if(words.Length==8)
                        rotation =(Convert.ToInt32(words[7]));
                    
                    Block toBeAdded = new Block(game, game.Arena, new Vector2(x, y), 1, spriteName, System.Convert.ToSingle(words[4]), System.Convert.ToSingle(words[5]), Convert.ToInt32(words[6]), rotation);
                    
                    if (words.Length >= 8)
                    {
                        //toBeAdded.setdisAppearTimer(Convert.ToInt32(words[7]));
                    }                   
                    if(Convert.ToInt32(words[6])==0)
                        blocks.AddLast(toBeAdded);
                    else if(Convert.ToInt32(words[6])==1)
                        topBlocks.AddLast(toBeAdded);
                    else
                        buttomBlocks.AddLast(toBeAdded);
                    
                    
                }
                if (words[3].Equals("DeathBlock"))
                {
                    float rotation = 0;
                    if(words.Length==5)
                        rotation = System.Convert.ToSingle(words[4]);
                    deathBlocks.AddLast(new DeathBlock(game, game.Arena, new Vector2(x, y), 1, spriteName,rotation));
                }
                if (words[3].Equals("MagnetBlock"))
                {
                    if (words.Length < 9)
                        magnetBlocks.AddLast(new MagnetBlock(game, game.Arena, new Vector2(x, y), 1, spriteName, new Vector2(Convert.ToInt32(words[4]), Convert.ToInt32(words[5])), Convert.ToInt32(words[6]), Convert.ToInt32(words[7])));
                    else
                    {
                        bool isFan = Convert.ToInt32(words[8]) == 1;
                        magnetBlocks.AddLast(new MagnetBlock(game, game.Arena, new Vector2(x, y), 1, spriteName, new Vector2(Convert.ToInt32(words[4]), Convert.ToInt32(words[5])), Convert.ToInt32(words[6]), Convert.ToInt32(words[7]), isFan));
                    }
                }
                if (words[3].Equals("GoalBlock"))
                    goalBlocks.AddLast(new GoalBlock(game, game.Arena, new Vector2(x, y), 1, spriteName, Convert.ToInt32(words[4])));
                if (words[3].Equals("MovingDeath"))
                {
                    Vector2 shootAngle = new Vector2(Convert.ToInt32(words[5]), Convert.ToInt32(words[6]));
                    moveDeathBlocks.AddLast(new MovingDeath(game, game.Arena, new Vector2(x, y), 1, spriteName, shootAngle, Convert.ToInt32(words[4])));
                }
                if (words[3].Equals("MovingPath"))
                {
                    movePathBlock.AddLast(new MovingPath(game, game.Arena, new Vector2(x, y),1,spriteName,Convert.ToInt32(words[4]),
                        new Vector2(Convert.ToInt32(words[5]),Convert.ToInt32(words[6])),new Vector2(Convert.ToInt32(words[7]),Convert.ToInt32(words[8])),false));
                }

                if (words[3].Equals("MovingCycle"))
                {
                    int count = Convert.ToInt32(words[5]);
                    ArrayList paths = new ArrayList();
                    for (int i = 6; i < 6+count*2; i+=2)
                    {
                        int xPos = Convert.ToInt32(words[i]);
                        int yPos = Convert.ToInt32(words[i+1]);
                        Vector2 vectors = new Vector2(xPos, yPos);
                        paths.Add(vectors);
                    }
                    moveCycleBlock.AddLast(new MovingCycle(game, game.Arena, new Vector2(x, y), 1, spriteName, System.Convert.ToSingle(words[4]), paths, true));
                    

                }

                if (words[0].Equals("Demi"))
                {
                    game.maxLeft = Convert.ToInt32(words[1]);
                    game.maxRight = Convert.ToInt32(words[2]);
                    game.maxTop = Convert.ToInt32(words[3]);
                    game.maxButtom= Convert.ToInt32(words[4]);
                }

                if (words[3].Equals("SuperBack"))
                {
                    game.backGroundImages.Add(new BackgroundBlock(game, game.Arena, new Vector2(x, y), 1, spriteName, System.Convert.ToSingle(words[5]), System.Convert.ToSingle(words[4])));

                }

                if (words[3].Equals("WaterBlock"))
                {
                    waterBlocks.AddLast(new WaterBlock(game,game.Arena,new Vector2(x,y),1,spriteName,System.Convert.ToSingle(words[4]),System.Convert.ToSingle(words[5]),System.Convert.ToSingle(words[6])));
                }
                
            }

            stopWatch.Stop();
            if (game.preloadLevelOnly)
                return;
            foreach (Block i in buttomBlocks)
                game.addEntity(i);            
            foreach (MovingDeath i in moveDeathBlocks)
                game.addEntity(i);
            foreach (MovingCycle i in moveCycleBlock)
                game.addEntity(i);
            foreach (Block i in topBlocks)
                game.addEntity(i);
            foreach (DeathBlock i in deathBlocks)
                game.addEntity(i);
            foreach (MagnetBlock i in magnetBlocks)
                game.addEntity(i);
            foreach (MovingPath i in movePathBlock)
                game.addEntity(i);

            foreach (Block i in blocks)
                game.addEntity(i);
            foreach (GoalBlock i in goalBlocks)
                game.addEntity(i);
            foreach (WaterBlock i in waterBlocks)
                game.addEntity(i);

            

            foreach (MovingDeath i in moveDeathBlocks)
            {
                foreach (MovingDeath j in moveDeathBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);
                }
            }
            

            foreach (Block i in blocks)
            {
                   foreach (Block j in blocks)
                   {
                       if (!(j is LayerBlock || i is LayerBlock))
                       {
                           i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                           j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);
                       }
                   }
            }


            foreach (DeathBlock i in deathBlocks)
            {
                foreach (DeathBlock j in deathBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                        j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }

            foreach (MovingPath i in movePathBlock)
            {
                foreach (MovingPath j in movePathBlock)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }

            foreach (MovingPath i in movePathBlock)
            {
                foreach (DeathBlock j in deathBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }

            foreach (MovingPath i in movePathBlock)
            {
                foreach (GoalBlock j in goalBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }

            foreach (MovingPath i in movePathBlock)
            {
                foreach (Block j in blocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }

            foreach (MovingPath i in movePathBlock)
            {
                foreach (MagnetBlock j in magnetBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }


            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (MagnetBlock j in magnetBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }

            foreach (MovingDeath i in moveDeathBlocks)
            {
                foreach (MovingDeath j in moveDeathBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }

            }
            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (MovingCycle j in moveCycleBlock)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }

            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (GoalBlock j in goalBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }

            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (Block j in blocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }

            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (DeathBlock j in deathBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }

            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (MovingPath j in movePathBlock)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }
            foreach (MovingCycle i in moveCycleBlock)
            {
                foreach (MagnetBlock j in magnetBlocks)
                {
                    i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                    j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);

                }
            }
            if (Constants.FIND_LAG)
            {
                DirectoryInfo di2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DebugInfo");
                String name = di2.FullName + @"\LagInfo.txt";
                LinkedList<String> lines = new LinkedList<String>();
                lines.AddLast("Level"+game.currentLevel);
                lines.AddLast(stopWatch.Elapsed.ToString());
                lines.AddLast(game.stopWatchLagTimer.Elapsed.ToString());
                System.IO.File.WriteAllLines(name, lines);
            }
            game.gameInsertValues = true;

            //game.drawingTool.resetCamera();
            
                game.drawingTool.cam.Zoom = game.camZoomValue;
                game.drawingTool.cam.Pos = game.camPosSet;
            

            SortedSet<Entity> omg=game.entities;

        }
        

        private static Dictionary<string, string> templates = new Dictionary<string, string>();
        public static int templateCount = 0;

        public static string fullLocation="";

        private static bool isInitialized = false;
        private static void initLevels(String path)
        {
            if (!isInitialized)
            {
                DirectoryInfo di = new DirectoryInfo(@"Content\"+path);
                String fullname=di.FullName;
                //This clears the cache templates so that new Maps can be loaded
                templates = new Dictionary<string, string>();
                foreach (FileInfo fi in di.GetFiles())
                {
                    try
                    {
                        templates.Add(fi.Name, File.ReadAllText(fi.FullName));
                    }
                    catch {
                        int i = 0;
                    }
                }
                if (!Constants.IS_IN_REALSE_MODE)
                {
                    DirectoryInfo di2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DebugInfo");
                    String name = di2.FullName + @"\DebugInfo.txt";
                    LinkedList<String> lines = new LinkedList<String>();
                    lines.AddLast(di.FullName);
                    fullLocation = di.FullName;
                    System.IO.File.WriteAllLines(name, lines);
                }
                
            }
        }




    }
}
