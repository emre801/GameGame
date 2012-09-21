using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProtoDerp
{
    class LevelEditor:Entity
    {
        float height, width;
        public LevelEditor(Game g)
            : base(g)
        {
            this.game = game;
            initLevels();
        }
        public PlayableCharacter player1 = null;
        public PlayableCharacter player2 = null;
        LinkedList<Block> blocks = new LinkedList<Block>();
        LinkedList<DeathBlock> deathBlocks = new LinkedList<DeathBlock>();
        LinkedList<GoalBlock> goalBlocks = new LinkedList<GoalBlock>();
        LinkedList<MovingDeath> moveDeathBlocks = new LinkedList<MovingDeath>();
        public void readFile(int templateNum)
        {
            String path = Directory.GetCurrentDirectory();
            int pathIndex = path.IndexOf("bin");
            path = @"Level" + templateNum + @".txt";

            string file;
            if (!templates.TryGetValue(path, out file))
            {
                file = templates.Values.First<string>();
                Console.WriteLine("Level " + path + " not found.");
            }

            height = Constants.GAME_WORLD_HEIGHT;
            width = Constants.GAME_WORLD_WIDTH;

            createLevel(file);
        }

        public void createLevel(String file)
        {
            StringReader sr = new StringReader(file);
            String line;
            char[] delimiterChars = { ' ', ',', ':', '\t' };
            int c = 0;
            if(game.loadNewLevel)
            game.clearEntities();
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
                float x=Convert.ToInt32(words[0]);
                float y = Convert.ToInt32(words[1]);
                String spriteName = words[2];
                if(words[3].Equals("Block"))
                    blocks.AddLast(new Block(game, game.Arena, new Vector2(x, y), 1, spriteName, Convert.ToInt32(words[4]), Convert.ToInt32(words[5])));
                if (words[3].Equals("DeathBlock"))
                   deathBlocks.AddLast(new DeathBlock(game, game.Arena, new Vector2(x, y), 1, spriteName));
                if (words[3].Equals("GoalBlock"))
                    goalBlocks.AddLast(new GoalBlock(game, game.Arena, new Vector2(x, y), 1, spriteName, Convert.ToInt32(words[4])));
                if (words[3].Equals("MovingDeath"))
                {
                    Vector2 shootAngle = new Vector2(Convert.ToInt32(words[5]), Convert.ToInt32(words[6]));
                    moveDeathBlocks.AddLast(new MovingDeath(game, game.Arena, new Vector2(x, y), 1, spriteName, shootAngle, Convert.ToInt32(words[4])));
                }
                if (words[0].Equals("Demi"))
                {
                    game.maxLeft = Convert.ToInt32(words[1]);
                    game.maxRight = Convert.ToInt32(words[2]);
                    game.maxTop = Convert.ToInt32(words[3]);
                    game.maxButtom= Convert.ToInt32(words[4]);
                }
                
            }
            
           foreach (Block i in blocks)
                game.addEntity(i);
           foreach (DeathBlock i in deathBlocks)
               game.addEntity(i);
           foreach (GoalBlock i in goalBlocks)
               game.addEntity(i);
           foreach (MovingDeath i in moveDeathBlocks)
               game.addEntity(i);

            

           foreach(MovingDeath i in moveDeathBlocks)
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
                   i.fixture.CollisionFilter.IgnoreCollisionWith(j.fixture);
                   j.fixture.CollisionFilter.IgnoreCollisionWith(i.fixture);
               }
           }
           SortedSet<Entity> omg=game.entities;

        }
        

        private static Dictionary<string, string> templates = new Dictionary<string, string>();
        public static int templateCount = 0;

        private static bool isInitialized = false;
        private static void initLevels()
        {
            if (!isInitialized)
            {
                DirectoryInfo di = new DirectoryInfo(@"Content\World1");
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
            }
        }

    }
}
