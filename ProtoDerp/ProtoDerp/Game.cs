using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace ProtoDerp
{
    
    public class Game : Microsoft.Xna.Framework.Game
    {
        public Random ran = new Random();
        public enum Fonts { FT_MEDIUM, FT_HEADER, FT_TITLE };
        public SpriteFont[] fonts = new SpriteFont[4];

        public SortedSet<Entity> entities = new SortedSet<Entity>();
        public float WorldSpeed { get; set; }
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        public LinkedList<String> blockList = new LinkedList<string>();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<string, SpriteStripAnimationHandler> spriteAnimation = new Dictionary<string, SpriteStripAnimationHandler>();


        bool restart = false;

        public Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        Dictionary<string, SoundInstance> soundInstances = new Dictionary<string, SoundInstance>();

        public bool isInLevelSelect = false;
        public LevelSelect levelSelect;
        public World world,world2,world3;

        public bool startGame = false;

        public readonly DrawingTool drawingTool;
        public Input playerOneInput = null;

        private bool isPaused = false;
        public bool IsPaused { get { return isPaused; } }
        public GUI GUI { get; private set; }

        public Arena Arena { get; private set; }
        LinkedList<Entity> toBeAdded = new LinkedList<Entity>();

        TimeSpan pauseAdjustment = TimeSpan.Zero;

        public Stopwatch stopWatch = new Stopwatch();

        public Stopwatch animationTime = new Stopwatch();
        public bool deathAnimation = false,winningAnimation=false;

        LinkedList<Entity> toBeRemoved = new LinkedList<Entity>();
        public bool loadNewLevel = false;
        public TitleScreen Title;
        public int gMode=1;
        public bool isInCreatorMode = false;
        public int currentLevel = 1;
        public float windowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public float windowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public int numDeath;
        public float maxLeft, maxRight, maxTop, maxButtom;
        public int count = 0;

        public enum BlockType { Normal, Death, Moving, Goal };
        public BlockType blockType = BlockType.Normal;

        public float cXLocation=0, cYLocation=0;
        public int currentWriteLevel = Constants.WRITE_LEVEL;
        public float saveAlpha = 0;

        public bool inDeleteMode = false;
        public int gameTemplateLevel = 1;

        public bool loadFromLevel = false;
        public int drawLevel = 0;

        public Game()
        {
            WorldSpeed = 1.0f;
            world = new World(new Vector2(0, 5.0f));
            world2 = new World(new Vector2(0, 0));
            world3 = new World(new Vector2(0, 0));
            ConvertUnits.SetDisplayUnitToSimUnitRatio(30);
            //graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            drawingTool = new DrawingTool(this);
            numDeath = 0;
            
        }

        protected void addSound(String fName)
        {
            SoundEffect s = Content.Load<SoundEffect>(fName);
            sounds.Add(fName, s);
            soundInstances.Add(fName, new SoundInstance(s.CreateInstance()));
        }

        public SoundInstance getSoundInstance(String fName)
        {
            if (soundInstances.ContainsKey(fName))
                return soundInstances[fName];
            else
                return null;
        }

        public void addEntity(Entity e)
        {
            toBeAdded.AddLast(e);
        }
        public void populateWorld()
        {
            drawingTool.cam.Zoom = 0.55f * drawingTool.zoomRatio;
            //Create the in-game arena.
            Arena = new Arena(this, Arena.GameMode.MODE_MULTI_PLAYER);
            addEntity(Arena);
            Arena.setUpDemensions(maxLeft, maxRight, maxTop, maxButtom);
            //playSong("Music//ForrestSounds");
            
            //GUI
            //GUI = new GUI(this);
            //addEntity(GUI);

            this.startGame = true;

            this.isInCreatorMode = false;
        }

        public void populateWorldCreatorMode()
        {
            drawingTool.cam.Zoom = 0.35f * drawingTool.zoomRatio;
            //Create the in-game arena.
            //clearEntities();
            Arena = new Arena(this, Arena.GameMode.MODE_MULTI_PLAYER);
            addEntity(Arena);
            addEntity(new CreaterBlock(this, Arena, new Vector2(100, 100), 1, "block1-2"));

            //GUI
            GUI = new GUI(this);
            addEntity(GUI);

            this.startGame = true;

            this.isInCreatorMode = true;
        }

        /**
        * Get the dimensions of the window
        */
        public Vector2 getWorldSize()
        {
            return new Vector2(Constants.GAME_WORLD_WIDTH, Constants.GAME_WORLD_HEIGHT);
        }

        protected override void Initialize()
        {          
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            //spriteBatch = new SpriteBatch(GraphicsDevice);
           
            drawingTool.initialize();
            playerOneInput = new XboxInput(PlayerIndex.One);

            //Load Sprites            
            sprites.Add("fire0", new Sprite(Content, "fire0"));
            sprites.Add("fire1", new Sprite(Content, "fire1"));
            sprites.Add("fire2", new Sprite(Content, "fire2"));
            sprites.Add("cloud", new Sprite(Content, "cloud"));
            blockList.AddLast("cloud");
            sprites.Add("DeathTime", new Sprite(Content, "DeathTime"));
            sprites.Add("black", new Sprite(Content,"black"));
            sprites.Add("rage", new Sprite(Content, "rage"));
            sprites.Add("bigBlock", new Sprite(Content, "bigBlock"));
            blockList.AddLast("bigBlock");
            sprites.Add("Background", new Sprite(Content, "Background"));
            blockList.AddLast("Background");
            sprites.Add("ground", new Sprite(Content, "ground"));
            blockList.AddLast("ground");
            sprites.Add("grass1", new Sprite(Content, "grass1"));
            blockList.AddLast("grass1");
            sprites.Add("grass2", new Sprite(Content, "grass3"));
            blockList.AddLast("grass2");
            sprites.Add("grass3", new Sprite(Content, "grass3"));
            blockList.AddLast("grass3");
            sprites.Add("outPut", new Sprite(Content, "outPut"));
            blockList.AddLast("outPut");
            sprites.Add("pixelWood", new Sprite(Content, "pixelWood"));
            blockList.AddLast("pixelWood");
            sprites.Add("block1-1", new Sprite(Content, "block1-1"));
            blockList.AddLast("block1-1");
            sprites.Add("block1-2", new Sprite(Content, "block1-2"));
            blockList.AddLast("block1-2");
            sprites.Add("block1-3", new Sprite(Content, "block1-3"));
            blockList.AddLast("block1-3");
            sprites.Add("block1-4", new Sprite(Content, "block1-4"));
            blockList.AddLast("block1-4");
            sprites.Add("pixelGrass", new Sprite(Content, "pixelGrass"));
            blockList.AddLast("pixelGrass");
            sprites.Add("deathSpikes", new Sprite(Content, "deathSpikes"));
            blockList.AddLast("deathSpikes");
            sprites.Add("deathSpikes2", new Sprite(Content, "deathSpikes2"));
            blockList.AddLast("deathSpikes2");
            sprites.Add("deathSpikes3", new Sprite(Content, "deathSpikes3"));
            blockList.AddLast("deathSpikes3");
            sprites.Add("deathSpikes4", new Sprite(Content, "deathSpikes4"));
            blockList.AddLast("deathSpikes4");
            sprites.Add("cannon", new Sprite(Content, "cannon"));
            blockList.AddLast("cannon");
            sprites.Add("mountain", new Sprite(Content, "mountain"));
            blockList.AddLast("mountain");
            sprites.Add("bullet.0", new Sprite(Content, "bullet.0"));
            blockList.AddLast("bullet.0");
            sprites.Add("RageQuit", new Sprite(Content, "RageQuit"));
            sprites.Add("Space", new Sprite(Content, "Space"));
            sprites.Add("SpaceAir", new Sprite(Content, "SpaceAir"));
            sprites.Add("SpaceDERP", new Sprite(Content, "SpaceDERP"));
            sprites.Add("arena", new Sprite(Content, "arena"));            
            sprites.Add("titleScreenElement.0", new Sprite(Content, "titleScreenElement.0")); //Logo Text
            sprites.Add("titleScreenElement.1", new Sprite(Content, "titleScreenElement.1")); //Logo Gear
            sprites.Add("titleScreenElement.2", new Sprite(Content, "titleScreenElement.2", new Vector2(547, 0))); //Selection Inside
            sprites.Add("titleScreenElement.3", new Sprite(Content, "titleScreenElement.3", new Vector2(547, 0))); //Selection Border
            sprites.Add("titleScreenElement.4", new Sprite(Content, "titleScreenElement.4", new Vector2(335, 0))); //Selection Text
            sprites.Add("titleScreenElement.5", new Sprite(Content, "titleScreenElement.5", new Vector2(75, 70))); //Long Clock Hand
            sprites.Add("titleScreenElement.6", new Sprite(Content, "titleScreenElement.6", new Vector2(40, 70))); //Short Clock Hand

            sprites.Add("pix", new Sprite(Content, "pix"));
            sprites.Add("Circle", new Sprite(Content, "Circle"));
            sprites.Add("LeverSelect", new Sprite(Content, "LeverSelect"));

            sprites.Add("MikeIcon", new Sprite(Content, "MikeIcon"));
            //sprites.Add("ahLogo", new Sprite(Content, "ahLogo"));
            sprites.Add("ahLogo0", new Sprite(Content, "ahLogo0"));
            sprites.Add("ahLogo1", new Sprite(Content, "ahLogo1"));
            sprites.Add("ahLogo2", new Sprite(Content, "ahLogo2"));
            sprites.Add("ahLogo3", new Sprite(Content, "ahLogo3"));
            blockList.AddLast("ahLogo0");

            sprites.Add("MikeRun1", new Sprite(Content, "MikeRun1"));
            sprites.Add("MikeRun2", new Sprite(Content, "MikeRun2"));
            sprites.Add("MikeJump1", new Sprite(Content, "MikeJump1"));
            sprites.Add("MikeJump2", new Sprite(Content, "MikeJump2"));
            sprites.Add("MikeStand", new Sprite(Content, "MikeStand"));
            sprites.Add("MikeWall", new Sprite(Content, "MikeWall"));
            
            sprites.Add("clouds", new Sprite(Content, "clouds"));

            sprites.Add("Tree", new Sprite(Content, "Tree"));
            blockList.AddLast("Tree");
            sprites.Add("Tree2", new Sprite(Content, "Tree2"));
            blockList.AddLast("Tree2");
            sprites.Add("Tree3", new Sprite(Content, "Tree3"));
            blockList.AddLast("Tree3");
            sprites.Add("Tree4", new Sprite(Content, "Tree4"));
            blockList.AddLast("Tree4");

            sprites.Add("MouseImage", new Sprite(Content, "MouseImage"));
            sprites.Add("MouseClick", new Sprite(Content, "MouseClick"));

            //Load Fonts
            fonts[(int)Fonts.FT_MEDIUM] = Content.Load<SpriteFont>("Font\\share_20px_reg");
            fonts[(int)Fonts.FT_HEADER] = Content.Load<SpriteFont>("Font\\share_48px_bold");
            fonts[(int)Fonts.FT_TITLE] = Content.Load<SpriteFont>("Font\\ghost_42px_bold");

            //Title Screen
            //populateWorld();

            addSound("Rage//Wave//Rage1");
            addSound("Rage//Wave//Rage2");
            addSound("Rage//Wave//jump");
            addSound("Rage//Wave//death");
            addSound("Rage//Wave//Opening");
            
            spriteAnimation.Add("player_strip12", new SpriteStripAnimationHandler(new Sprite(Content, "player_strip12")
                , 12,60));//Player Standing
            spriteAnimation.Add("missile_strip_strip4", new SpriteStripAnimationHandler(new Sprite(Content, "missile_strip_strip4")
                , 4,10));//missle
            spriteAnimation.Add("sprite14_strip9", new SpriteStripAnimationHandler(new Sprite(Content, "sprite14_strip9")
                , 9, 60));//Wall Hang
            spriteAnimation.Add("sprite15_strip4", new SpriteStripAnimationHandler(new Sprite(Content, "sprite15_strip4")
                , 4, 60));//walking
            spriteAnimation.Add("sprite16_strip6", new SpriteStripAnimationHandler(new Sprite(Content, "sprite16_strip6")
                , 6, 120));//Running
            spriteAnimation.Add("sprite17", new SpriteStripAnimationHandler(new Sprite(Content, "sprite17")
                , 1, 60));//Running
            
            if (Constants.ENABLE_TITLE_SCREEN)
            {
                GUI = new GUI(this);
                Title = new TitleScreen(this);
                addEntity(Title);
            }
            else
            {
                if (gMode == 0)
                    populateWorld();
                else if (gMode == 2)
                    populateWorldCreatorMode();
            }

            levelSelect = new LevelSelect(this);
            levelSelect.IsVisible = false;
            addEntity(levelSelect);
           
            //playSong("Music//GameBeat2");
             
        
        }
        //Gets a Sprite object from the database, based on its file name (without the extension!)
        public Sprite getSprite(String fName)
        {
            if (sprites.ContainsKey(fName))
                return sprites[fName];
            else
                return null;
        }

        public SpriteStripAnimationHandler getSpriteAnimation(String fName)
        {
            if (this.spriteAnimation.ContainsKey(fName))
                return spriteAnimation[fName];
            else
                return null;
        }
        
        protected override void UnloadContent()
        {
            
        }

        public void Pause()
        {
            
        }
        public void clearEntities()
        {
            entities.Clear();
        }

        public void newLevel()
        {
            //writeLevel(2);
            world = new World(new Vector2(0, 5.0f));
            world2 = new World(new Vector2(0, 0));
            world3 = new World(new Vector2(0, 0));
            entities.Clear();
            toBeAdded.Clear();
            cachedEntityLists = new Dictionary<Type, object>();
            drawingTool.initialize();
            cachedEntityLists = new Dictionary<Type, object>();
            stopWatch.Reset();
            stopWatch.Start();
            if (!this.isInCreatorMode)
                populateWorld();
            else
                populateWorldCreatorMode();
        }
        public void updateCreatorCamera()
        {
            
        }

        public void nextLevel(int level)
        {
            /*
            entities.Clear();
            clearEntities();
            cachedEntityLists = new Dictionary<Type, object>();            
            currentLevel = level;
             * */
            //newLevel();
            currentLevel = level;
            winningAnimation = true;
            animationTime.Reset();
            animationTime.Start();
            //loadNewLevel = true;
        }

        public void playSong(String songName)
        {
            MediaPlayer.Stop();

            //if (Options.gameOptions.musicToggled) //Only actually play the song if music is turned on.
            //{
                Song song = Content.Load<Song>(songName);
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            //}
        }

        public void writeLevel(int templateNum)
        {
            String path ="";
            int pathIndex = path.IndexOf("bin");
            path = @"C:\Users\John\Documents\visual studio 2010\Projects\ProtoDerp\ProtoDerp\ProtoDerpContent\World1\Level" + templateNum + @".txt";
            LinkedList<String> lines = new LinkedList<String>();
            LinkedList<Block> blocks = getEntitiesOfType<Block>();
            foreach (Block b in blocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " Block" +" "+b.height +" "+b.width+" "+b.drawLevel);

            }

            LinkedList<DeathBlock> deathBlocks = getEntitiesOfType<DeathBlock>();
            foreach (DeathBlock b in deathBlocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " DeathBlock");

            }

            LinkedList<GoalBlock> goalBlocks = getEntitiesOfType<GoalBlock>();
            foreach (GoalBlock b in goalBlocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " GoalBlock "+ b.nextLevel);

            }

            LinkedList<MovingDeath> movingBlock = getEntitiesOfType<MovingDeath>();
            foreach (MovingDeath b in movingBlock)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " MovingDeath " + b.velObj +" "+b.shootAngle.X + " "+b.shootAngle.Y);

            }
            lines.AddLast("Demi "+ maxLeft + " "+ maxRight +" "+ maxTop+ " " +maxButtom);

            System.IO.File.WriteAllLines(path, lines);

        }

        protected override void Update(GameTime gameTime)
        {         

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (((XboxInput)playerOneInput).IsNewButtonPressed(Buttons.Start))
            {
                if (gMode == 0)
                    restart = true;
            }

            if (loadNewLevel||restart)
            {
                restart = false;
                loadNewLevel = false;
                cachedEntityLists.Clear();
                newLevel();                
            }
            if (((XboxInput)playerOneInput).IsNewButtonPressed(Buttons.Back))
            {
                restart = false;
                world = new World(new Vector2(0, 5.0f));
                entities.Clear();
                toBeAdded.Clear();
                cachedEntityLists = new Dictionary<Type, object>();
                //drawingTool.initialize();
                //drawingTool.resetCamera();
                gMode = 6;
                cachedEntityLists = new Dictionary<Type, object>();
                Title = new TitleScreen(this);
                addEntity(Title);
                isInCreatorMode = false;

            }

            playerOneInput.Update(gameTime);

            if (isInCreatorMode)
            {
                updateCreatorCamera();
            }

            GameTime pauseAdjustedGameTime = new GameTime(
                  gameTime.TotalGameTime - pauseAdjustment,
                  gameTime.ElapsedGameTime,
                  gameTime.IsRunningSlowly);

            foreach (Entity e in toBeAdded)
            {
                entities.Add(e);
                e.OnAddedToGame(e.updatesWhenPaused ? gameTime : pauseAdjustedGameTime);
            }

            foreach (Entity e in entities)
            {
                if (e.IsUpdateable && !e.dispose)
                {
                    if (e.updatesWhenPaused)
                    {
                        // Entities which continue to update when the game is paused receive the unmodified gametime
                        e.Update(gameTime, WorldSpeed);
                    }
                    else if (!isPaused)
                    {
                        // All others receive the modified one.
                        e.Update(pauseAdjustedGameTime, WorldSpeed);
                    }
                }
                if (e.dispose)
                {
                    
                    toBeRemoved.AddLast(e);
                }
            }
            
            foreach (Entity e in toBeRemoved)
            {
                entities.Remove(e);
            }

            if (!winningAnimation && !deathAnimation)
                world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.002));
            if (winningAnimation == true)
            {
                animationTime.Stop();
                TimeSpan ts = animationTime.Elapsed;
                animationTime.Start();
                if (ts.CompareTo(new TimeSpan(0, 0, 2)) > 0)
                {
                    //entities.Clear();
                    //clearEntities();
                    //cachedEntityLists = new Dictionary<Type, object>();
                    //currentLevel = level;
                    winningAnimation = false;
                    loadNewLevel = true;
                    //return;
                }
            }
            if (deathAnimation == true)
            {
                animationTime.Stop();
                TimeSpan ts = animationTime.Elapsed;
                animationTime.Start();
                count++;
                if (ts.CompareTo(new TimeSpan(0, 0, 0)) > 0)
                {
                    deathAnimation = false;
                    restart = true;
                    animationTime.Restart();
                    //return;
                }
            }

            
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            if(gMode==0)
                GraphicsDevice.Clear(Color.SkyBlue);
            else
                GraphicsDevice.Clear(Color.Black);

            drawingTool.drawEntities(entities, gameTime);
            drawingTool.drawLetterBox();

            base.Draw(gameTime);
        }
        public void PlayerDies()
        {
            //newLevel();
            numDeath++;
            deathAnimation = true;
            //restart = true;
        }

        // Caching entity lists so that we don't need to regenerate them every time.
        public Dictionary<Type, object> cachedEntityLists = new Dictionary<Type, object>();
        public LinkedList<T> getEntitiesOfType<T>() where T : Entity
        {
            LinkedList<T> ret;
            object cachedList = null;
            cachedEntityLists.TryGetValue(typeof(T), out cachedList);
            if (cachedList == null)
            {
                ret = new LinkedList<T>();
                foreach (Entity entity in entities)
                {
                    T t = entity as T;
                    if (t != null)
                    {
                        ret.AddLast(t);
                    }
                }
                cachedEntityLists.Add(typeof(T), ret);
            }
            else
            {
                ret = (LinkedList<T>)cachedList;
            }
            return ret;
        }
    }
}
