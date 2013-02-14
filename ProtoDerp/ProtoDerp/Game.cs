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
using System.IO;
using System.Net;
using System.Net.Mail;



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
        List<string> playedSongs = new List<string>();

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
        public LinkedList<Entity> toBeAdded = new LinkedList<Entity>();

        TimeSpan pauseAdjustment = TimeSpan.Zero;

        public Stopwatch stopWatch = new Stopwatch();
        public Stopwatch stopWatchLagTimer = new Stopwatch();

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

        public enum BlockType { Normal, Death, Moving, Goal, Magnet,Path,Cycle,WaterBlock };
        public BlockType blockType = BlockType.Normal;

        public float cXLocation=0, cYLocation=0;
        public int currentWriteLevel = Constants.WRITE_LEVEL;
        public float saveAlpha = 0;

        public bool inDeleteMode = false;
        public int gameTemplateLevel = Constants.WRITE_LEVEL;

        public bool loadFromLevel = true;
        public int drawLevel = 0;

        public bool testLevel = false;
        public int cameraWindowValue = 0;

        public Vector2 magnetPulse= new Vector2(0, 0);

        public bool pause = false;
        public float pauseAlpha = 1f;
        public bool backToTitleScreen = false;

        public bool isPausePressed = false;
        public int isUpMenuSelect = 0;

        public Vector2 moveBackGround= new Vector2(0,0);

        public int spriteBlockCounter = 0;

        public Dictionary<Rectangle, Texture2D> cacheOfDirt = new Dictionary<Rectangle, Texture2D>();

        
        public bool isButtonSelect = false;
        public bool isSelectingBlock = false;
        public int blockCounter = 9;
        public String songName= "",songArtist="";

        public int ballPosition = 0;
        public bool activateButtons = true;

        public int pathSpeed = 2;
        public Vector2 moveSpeed = new Vector2(0, 0);

        public int gameTitleValue = 0;
        public bool isCollidingWithButton = false;
        public float fadeAlpha = 1f;

        public string writeDirectory="";

        public Color backGroundColor = Color.SkyBlue;

        public SortedSet<Entity> backGroundImages = new SortedSet<Entity>();

        public int backGroundNum = 0;

        public bool inTransition=false;
        public bool justDied = false;

        public float camZoomValue = -1;
        public Vector2 camPosSet= new Vector2(0,0);

        public bool deathBlockGoalCollision = false;

        public bool inWater = false;
        public int waterCollisionTime = -1;
        public int[] xValues = new int[1000];
        public int[] yValues = new int[1000];
        public int xycounter = 0;

        public bool gameDoneLoading = false;
        public bool gameInsertValues = false;

        public float respawnFadeValue = -1f;

        public float currentWorld = 1;
        public bool worldFinished = false;

        public bool preloadLevelOnly = false;

        public bool inCutScene = false;

        public bool loadCutScene = false;

        
        public Game()
        {
            WorldSpeed = 1.0f;
            world = new World(new Vector2(0, 5.0f));
            world2 = new World(new Vector2(0, 0));
            world3 = new World(new Vector2(0, 0));
            ConvertUnits.SetDisplayUnitToSimUnitRatio(30);
            //graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
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

        public void preLoadEachLevel()
        {
            //JumpPoint
            LevelEditor le = new LevelEditor(this);
            preloadLevelOnly = true;
            for(int i =0; i<20;i++)
            {
                le.readFile(i);
            }
            preloadLevelOnly = false;
            backToTitleScreen = false;
            pause = false;
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
            Title.IsVisible = true;
            addEntity(Title);
            isInCreatorMode = false;

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

            //Load uploaded sprites
            if(Constants.IS_IN_DEBUG_MODE)
                loadImageFromContent();

            //Load Sprites            
            /*sprites.Add("fire0", new Sprite(Content, "fire0"));
            sprites.Add("fire1", new Sprite(Content, "fire1"));
            sprites.Add("fire2", new Sprite(Content, "fire2"));
             */
            sprites.Add("cloud", new Sprite(Content, "cloud"));
            blockList.AddLast("cloud");
            //sprites.Add("DeathTime", new Sprite(Content, "DeathTime"));
            sprites.Add("black", new Sprite(Content,"black"));
            //sprites.Add("rage", new Sprite(Content, "rage"));
            //sprites.Add("dirtBlock", new Sprite(Content, "dirtBlock"));
            sprites.Add("rC1", new Sprite(Content, "rC1"));
            sprites.Add("rC2", new Sprite(Content, "rC2"));
            sprites.Add("rC3", new Sprite(Content, "rC3"));


            sprites.Add("lC1", new Sprite(Content, "lC1"));
            sprites.Add("lC2", new Sprite(Content, "lC2"));
            sprites.Add("lC3", new Sprite(Content, "lC3"));

            sprites.Add("graph", new Sprite(Content, "graph"));
            sprites.Add("Water", new Sprite(Content, "Water"));
            blockList.AddLast("Water");
            sprites.Add("dirtyBlock", new Sprite(Content, "dirtyBlock"));
            blockList.AddLast("dirtyBlock");
            sprites.Add("dirtyBlock2", new Sprite(Content, "dirtyBlock2"));
            blockList.AddLast("dirtyBlock2");
            sprites.Add("skyScrapper", new Sprite(Content,"skyScrapper"));
            blockList.AddLast("skyScrapper");
            sprites.Add("simplePix", new Sprite(Content, "simplePix"));
            blockList.AddLast("simplePix");
            sprites.Add("mtPix", new Sprite(Content, "mtPix"));
            blockList.AddLast("mtPix");
            sprites.Add("GrassHill", new Sprite(Content, "GrassHill"));
            blockList.AddLast("GrassHill");
            sprites.Add("BorderImage", new Sprite(Content, "BorderImage"));
            sprites.Add("BorderImageWhite", new Sprite(Content, "BorderImageWhite"));
            //blockList.AddLast("BorderImage");
            sprites.Add("backGroundMan", new Sprite(Content, "backGroundMan"));
            blockList.AddLast("backGroundMan");
            sprites.Add("grassTempBack", new Sprite(Content, "grassTempBack"));
            blockList.AddLast("grassTempBack");
            sprites.Add("grassTemplate", new Sprite(Content, "grassTemplate"));
            sprites.Add("blankImage", new Sprite(Content, "blankImage"));
            blockList.AddLast("grassTemplate");
            sprites.Add("groundPat", new Sprite(Content, "groundPat"));
            blockList.AddLast("groundPat");
            sprites.Add("goundBottom", new Sprite(Content, "goundBottom"));
            blockList.AddLast("goundBottom");
            sprites.Add("groundWall", new Sprite(Content, "groundWall"));
            blockList.AddLast("groundWall");
            sprites.Add("backImage", new Sprite(Content, "backImage"));
            blockList.AddLast("backImage");
            sprites.Add("pixSun", new Sprite(Content, "pixSun"));
            blockList.AddLast("pixSun");
            sprites.Add("pixGavin", new Sprite(Content, "pixGavin"));
            blockList.AddLast("pixGavin");
            sprites.Add("pixRay", new Sprite(Content, "pixRay"));
            blockList.AddLast("pixRay");
            sprites.Add("GreenHill", new Sprite(Content, "GreenHill"));
            blockList.AddLast("GreenHill");
            sprites.Add("BlueBackground", new Sprite(Content, "BlueBackground"));
            blockList.AddLast("BlueBackground");
            sprites.Add("bullet.0", new Sprite(Content, "bullet.0"));
            blockList.AddLast("bullet.0");
            sprites.Add("magnet1", new Sprite(Content, "magnet1"));
            blockList.AddLast("magnet1");
            sprites.Add("GreenHill2", new Sprite(Content, "GreenHill2"));
            blockList.AddLast("GreenHill2");
            sprites.Add("CloudBack", new Sprite(Content, "CloudBack"));
            blockList.AddLast("CloudBack");
            sprites.Add("bigBlock", new Sprite(Content, "bigBlock"));
            blockList.AddLast("bigBlock");
            sprites.Add("bigGround", new Sprite(Content, "bigGround"));
            blockList.AddLast("bigGround");
            sprites.Add("LongGround", new Sprite(Content, "LongGround"));
            blockList.AddLast("LongGround");
            sprites.Add("Background", new Sprite(Content, "Background"));
            blockList.AddLast("Background");
            sprites.Add("BigBackGround", new Sprite(Content, "BigBackGround"));
            blockList.AddLast("BigBackGround");
            sprites.Add("ground", new Sprite(Content, "ground"));
            blockList.AddLast("ground");
            sprites.Add("groundTemplate", new Sprite(Content, "groundTemplate"));
            blockList.AddLast("groundTemplate");
            sprites.Add("outPut", new Sprite(Content, "outPut"));
            blockList.AddLast("outPut");
            sprites.Add("pixelWood", new Sprite(Content, "pixelWood"));
            blockList.AddLast("pixelWood");
            sprites.Add("deathSpikes", new Sprite(Content, "deathSpikes"));
            blockList.AddLast("deathSpikes");

            sprites.Add("deathSpikes2", new Sprite(Content, "deathSpikes2"));
            blockList.AddLast("deathSpikes2");

            sprites.Add("deathSpikes3", new Sprite(Content, "deathSpikes3"));
            blockList.AddLast("deathSpikes3");

            sprites.Add("deathSpikes4", new Sprite(Content, "deathSpikes4"));
            blockList.AddLast("deathSpikes4");

            sprites.Add("deathBall", new Sprite(Content, "deathBall"));
            blockList.AddLast("deathBall");

            sprites.Add("Error", new Sprite(Content, "Error"));
            blockList.AddLast("Error");
            sprites.Add("RageQuit", new Sprite(Content, "RageQuit"));
            sprites.Add("Space", new Sprite(Content, "Space"));
            sprites.Add("SpaceAir", new Sprite(Content, "SpaceAir"));
            sprites.Add("SpaceDERP", new Sprite(Content, "SpaceDERP"));
            
            sprites.Add("arena", new Sprite(Content, "arena")); 
            /*
            sprites.Add("titleScreenElement.0", new Sprite(Content, "titleScreenElement.0")); //Logo Text
            sprites.Add("titleScreenElement.1", new Sprite(Content, "titleScreenElement.1")); //Logo Gear
            sprites.Add("titleScreenElement.2", new Sprite(Content, "titleScreenElement.2", new Vector2(547, 0))); //Selection Inside
            */
            //sprites.Add("titleScreenElement.3", new Sprite(Content, "titleScreenElement.3", new Vector2(547, 0))); //Selection Border
            sprites.Add("titleScreenElement.4", new Sprite(Content, "titleScreenElement.4", new Vector2(335, 0))); //Selection Text
            //sprites.Add("titleScreenElement.5", new Sprite(Content, "titleScreenElement.5", new Vector2(75, 70))); //Long Clock Hand
            //sprites.Add("titleScreenElement.6", new Sprite(Content, "titleScreenElement.6", new Vector2(40, 70))); //Short Clock Hand
            
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

            sprites.Add("start", new Sprite(Content, "start"));
            sprites.Add("SelectLevel", new Sprite(Content, "SelectLevel"));
            sprites.Add("CreativeMode", new Sprite(Content, "CreativeMode"));
            sprites.Add("Exit", new Sprite(Content, "Exit"));

            sprites.Add("clouds", new Sprite(Content, "clouds"));

            sprites.Add("Tree", new Sprite(Content, "Tree"));
            blockList.AddLast("Tree");
            sprites.Add("Tree2", new Sprite(Content, "Tree2"));
            blockList.AddLast("Tree2");
            sprites.Add("Tree3", new Sprite(Content, "Tree3"));
            blockList.AddLast("Tree3");
            sprites.Add("Tree4", new Sprite(Content, "Tree4"));
            blockList.AddLast("Tree4");

            sprites.Add("leftCurtain", new Sprite(Content, "leftCurtain"));
            sprites.Add("rightCurtain", new Sprite(Content, "rightCurtain"));
            sprites.Add("TopCurtain", new Sprite(Content, "TopCurtain"));

            sprites.Add("MouseImage", new Sprite(Content, "MouseImage"));
            sprites.Add("MouseClick", new Sprite(Content, "MouseClick"));

            //Load Fonts
            fonts[(int)Fonts.FT_MEDIUM] = Content.Load<SpriteFont>("Font\\share_20px_reg");
            fonts[(int)Fonts.FT_HEADER] = Content.Load<SpriteFont>("Font\\share_48px_bold");
            fonts[(int)Fonts.FT_TITLE] = Content.Load<SpriteFont>("Font\\ghost_42px_bold");

            //Title Screen
            //populateWorld();
            addSound("Rage//Wave//jump");
            addSound("Rage//Wave//death");
            addSound("Rage//Wave//Opening");
            addSound("Rage//Wave//explosion");
            addSound("Rage//Wave//menu");
            addSound("Rage//Wave//running");
            
            spriteAnimation.Add("player_strip12", new SpriteStripAnimationHandler(new Sprite(Content, "player_strip12")
                , 12,60));//Player Standing

            spriteAnimation.Add("fan", new SpriteStripAnimationHandler(new Sprite(Content, "fan")
                , 4,120));//fan
            sprites.Add("fan", new Sprite(new SpriteStripAnimationHandler(new Sprite(Content, "fan"), 4, 120).getIndex(), "fan"));
            blockList.AddLast("fan");

            spriteAnimation.Add("spriteStripFlower", new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripFlower")
                , 8, 120));//fan
            sprites.Add("spriteStripFlower", new Sprite(new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripFlower"), 4, 120).getIndex(), "spriteStripFlower"));
            blockList.AddLast("spriteStripFlower");

            /////////////////////
            spriteAnimation.Add("spriteStripBlueFlowers", new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripBlueFlowers")
                , 8, 120));//fan
            sprites.Add("spriteStripBlueFlowers", new Sprite(new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripBlueFlowers"), 4, 120).getIndex(), "spriteStripBlueFlowers"));
            blockList.AddLast("spriteStripBlueFlowers");
            /////////////////////

            /////////////////////
            spriteAnimation.Add("spriteStripPurpleFlower", new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripPurpleFlower")
                , 8, 120));//fan
            sprites.Add("spriteStripPurpleFlower", new Sprite(new SpriteStripAnimationHandler(new Sprite(Content, "spriteStripPurpleFlower"), 4, 120).getIndex(), "spriteStripPurpleFlower"));
            blockList.AddLast("spriteStripPurpleFlower");
            /////////////////////



            spriteAnimation.Add("missile_strip_strip4", new SpriteStripAnimationHandler(new Sprite(Content, "missile_strip_strip4")
                , 4, 45));//missle
            spriteAnimation.Add("sprite14_strip9", new SpriteStripAnimationHandler(new Sprite(Content, "sprite14_strip9")
                , 9, 60));//Wall Hang
            spriteAnimation.Add("sprite15_strip4", new SpriteStripAnimationHandler(new Sprite(Content, "sprite15_strip4")
                , 4, 60));//walking
            spriteAnimation.Add("sprite16_strip6", new SpriteStripAnimationHandler(new Sprite(Content, "sprite16_strip6")
                , 6, 120));//Running
            spriteAnimation.Add("sprite17", new SpriteStripAnimationHandler(new Sprite(Content, "sprite17")
                , 1, 60));//jumping up
            spriteAnimation.Add("sprite17-2", new SpriteStripAnimationHandler(new Sprite(Content, "sprite17-2")
                , 1, 60));//jumping up
            spriteAnimation.Add("sprite18_strip4", new SpriteStripAnimationHandler(new Sprite(Content, "sprite18_strip4")
                , 4, 45));//WallJump
            blockCounter=9;
            foreach (String i in blockList)
            {
                if(!spriteAnimation.ContainsKey(i))
                    spriteAnimation.Add(i, new SpriteStripAnimationHandler(getSprite(i),1,10));
                blockCounter++;
            }
            int bCounter=0;
            

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
            preloadSongs();
            if(Constants.PRELOAD_LEVELS)
                preLoadEachLevel();
             
        
        }
        //Gets a Sprite object from the database, based on its file name (without the extension!)
        public Sprite getSprite(String fName)
        {
            if (sprites.ContainsKey(fName))
                return sprites[fName];
            else
                return sprites["Error"];
        }

        public SpriteStripAnimationHandler getSpriteAnimation(String fName)
        {
            if (this.spriteAnimation.ContainsKey(fName))
                return spriteAnimation[fName];
            else
                return getSpriteAnimation("Error");
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

        public void addButtons()
        {
            
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
            if (level == -1)
            {

                if (currentWorld < Constants.TOTAL_NUMBER_OF_WORLDS)
                {

                    //entities.Clear();
                    //toBeAdded.Clear();
                    //cachedEntityLists = new Dictionary<Type, object>();
                    //backGroundImages.Clear();

                    //CutScene ct = new CutScene(this, (int)currentWorld);
                    //addEntity(ct);
                    //inCutScene = true;
                    camZoomValue = -1;
                    currentLevel = level;
                    winningAnimation = true;
                    ///backToTitleScreen = true;
                    //backGroundImages.Clear();
                    //JUMP Point


                }

            }
            else
            {
                currentLevel = level;
                winningAnimation = true;
                animationTime = new Stopwatch();
                //animationTime.Reset();
                animationTime.Start();
                gameDoneLoading = false;
                gameInsertValues = false;
            }
            //loadNewLevel = true;
        }

        public void playSong(String songName)
        {
            //MediaPlayer.Stop();

            if (MediaPlayer.State != MediaState.Playing)
            {
                Song song = Content.Load<Song>(songName);
                this.songArtist=song.Artist.Name;
                this.songName = songName.Substring(songName.IndexOf("\\")+1);
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = 0.2f;
            }
        }
        public void preloadSongs()
        {
            DirectoryInfo di = new DirectoryInfo(Content.RootDirectory + "\\Music");
            FileInfo[] fi = di.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo fInfo in fi)
            {
                String songName = fInfo.Name;
                songName = songName.Substring(0, songName.IndexOf("."));

                Song song = Content.Load<Song>("Music\\"+songName);
                MediaPlayer.Play(song);
                MediaPlayer.Pause();
            }

        }

        public void playRandonSong()
        {
            if (!Constants.PLAY_MUSIC)
                return;
            if (MediaPlayer.State != MediaState.Playing)
            {
                DirectoryInfo di = new DirectoryInfo(Content.RootDirectory + "\\Music");
                FileInfo[] fi = di.GetFiles("*", SearchOption.AllDirectories);

                String songName = fi[ran.Next(fi.Length)].Name;
                songName = songName.Substring(0, songName.IndexOf("."));
                if(playedSongs.Contains(songName)&& playedSongs.Count!=Constants.NUMBER_OF_SONGS)
                {
                    playRandonSong();
                }
                else
                {
                    if (playedSongs.Count == Constants.NUMBER_OF_SONGS)
                    {
                        playedSongs.Clear();
                    }
                    else
                    {
                        playedSongs.Add(songName);
                    }
                    playSong("Music\\" + songName);
                }
            }
        
        }

        public void writeLevel(int templateNum)
        {
            String path ="";
            int pathIndex = path.IndexOf("bin");
            //Need to change this.....
            DirectoryInfo di = new DirectoryInfo(@"Content\" + "World"+currentWorld);
            String fullName = di.FullName;
            if (Constants.IS_IN_DEBUG_MODE)
            {
                path = @"C:\Users\John\Documents\visual studio 2010\Projects\ProtoDerp\ProtoDerp\ProtoDerpContent\World"+currentWorld+@"\Level" + templateNum + @".txt";
            }
            else
            {
                //path = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LevelEditor2").FullName;
                path = this.writeDirectory + @"\Level" + templateNum + @".txt";
            }//path = fullName + @"\Level" + templateNum + @".txt";
            LinkedList<String> lines = new LinkedList<String>();
            LinkedList<Block> blocks = getEntitiesOfType<Block>();
            foreach (Block b in blocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " Block" +" "+b.height +" "+b.width+" "+b.drawLevel +" "+ b.rotationAngle);

            }

            LinkedList<DeathBlock> deathBlocks = getEntitiesOfType<DeathBlock>();
            foreach (DeathBlock b in deathBlocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " DeathBlock" +" " +b.rotationAngle);

            }

            LinkedList<MagnetBlock> magnetBlocks = getEntitiesOfType<MagnetBlock>();
            foreach (MagnetBlock b in magnetBlocks)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " MagnetBlock" + " " + b.magnetPulse.X + " "+ b.magnetPulse.Y + " "+ b.height +" "+ b.width  );

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
            LinkedList<MovingPath> movingPathBlock = getEntitiesOfType<MovingPath>();
            foreach (MovingPath b in movingPathBlock)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " MovingPath " + b.velObj + " "
                    + (int)b.point1.X + " " + (int)b.point1.Y + " " + (int)b.point2.X + " " + (int)b.point2.Y + " ");

            }

            LinkedList<MovingCycle> movingCycle = getEntitiesOfType<MovingCycle>();
            foreach (MovingCycle b in movingCycle)
            {
                 if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                String count = ""+b.getSizeOfPaths();
                String paths = b.getVectorsAsString();
                lines.AddLast(x + " " + y + " " + spriteName + " MovingCycle " + b.velObj + " " +count + paths);

            }
            foreach (BackgroundBlock b in backGroundImages)
            {
                if (!b.IsVisible)
                    continue;
                int x = (int)b.startPos.X;
                int y = (int)b.startPos.Y;
                String spriteName=b.spriteName;
                lines.AddLast(x+ " " + y + " " + spriteName + " " + "SuperBack" + " " + b.width +" "+b.height);
            }
            LinkedList<WaterBlock> waterBlocks = getEntitiesOfType<WaterBlock>();
            foreach (WaterBlock b in waterBlocks)
            {

                if (!b.IsVisible)
                    continue;
                int x = (int)b.origPos.X;
                int y = (int)b.origPos.Y;
                String spriteName = b.spriteNumber;
                lines.AddLast(x + " " + y + " " + spriteName + " WaterBlock" + " " + b.rotationAngle + " "+b.width+ " "+ b.height);

            }

            lines.AddLast("Demi " + (int)maxLeft + " " + (int)maxRight + " " + (int)maxTop + " " + (int)maxButtom);
            //lines.AddLast(path);
            Vector3 vecColor= backGroundColor.ToVector3();
            lines.AddLast("BGCOLOR " + (int)(vecColor.X * 252) + " " + (int)(vecColor.Y * 252) + " " + (int)(vecColor.Z * 252) + " ");

            lines.AddLast("CAM " + camZoomValue/drawingTool.zoomRatio);

            lines.AddLast("CPOS " + camPosSet.X + " " + camPosSet.Y);
            System.IO.File.WriteAllLines(path, lines);
                
            if (Constants.SEND_EMAIL_DATA)
            {
                string emailBody = "";
                foreach (String l in lines)
                {
                    emailBody += System.Environment.NewLine + l;
                }

                //sendEmail(emailBody);
            }
            if (!Constants.IS_IN_DEBUG_MODE)
            {
                DirectoryInfo di2 = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LevelEditor");
                System.IO.File.WriteAllLines(di2.FullName+"\\Level" + templateNum + @".txt", lines);
            }

        }

        public void sendEmail(string body)
        {
            var fromAddress = new MailAddress("c801studios@gmail.com", "EmailBot");
            var toAddress = new MailAddress("c801studios@gmail.com", "EmailBot");
            const string fromPassword = "Ercan801";
            const string subject = "LevelData";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }

        public void sendEmailWithAttachments()
        {

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("c801studios@gmail.com");
                mail.To.Add("c801studios@gmail.com");
                mail.Subject = "Levels";
                mail.Body = "mail with attachment";


                DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\LevelEditor");
                foreach (FileInfo fi in di.GetFiles())
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(fi.FullName);
                    mail.Attachments.Add(attachment);

                }
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("c801studios@gmail.com", "Ercan801");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
               
            }

        }

        protected override void Update(GameTime gameTime)
        {
            inWater = false;
            bool reloadButtons = false;
            xycounter = 0;
            if (inTransition)
                return;
            
            playerOneInput.Update(gameTime);
            if (camZoomValue != -1 && isInCreatorMode)//&& !isInCreatorMode)
            {
                //drawingTool.cam.Zoom = camZoomValue;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (((XboxInput)playerOneInput).IsNewButtonPressed(Buttons.Start) && !pause)
            {
                if (gMode == 0)
                    restart = true;
            }

            if (loadNewLevel||restart)
            {
                restart = false;
                loadNewLevel = false;
                cachedEntityLists.Clear();
                deathAnimation = false;
                winningAnimation = false;
                ballPosition = 0;
                entities.Clear();
                newLevel();
                drawingTool.resetCamera();
                Arena.setUpDemensions(maxLeft, maxRight, maxTop, maxButtom);
                if (gMode == 2)
                    reloadButtons = true;
                animationTime.Reset();
                return;
            }
            if (((XboxInput)playerOneInput).IsNewButtonPressed(Buttons.Back) || isPausePressed)
            {
                isPausePressed = false;
               
                if (gMode == 0 && !deathAnimation && !winningAnimation)
                {
                    
                    if (pause)
                        stopWatch.Stop();
                    pause = !pause;
                    pauseMusic();
                    return;
                }
                
            }
            pauseAlpha = 1f;
            if (pause)
           { 
                pauseAlpha = 0.25f;
                return;
            }

            if (backToTitleScreen)
            {
                backToTitleScreen = false;
                pause = false;
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
                Title.IsVisible = true;
                addEntity(Title);
                isInCreatorMode = false;

            }
            if (inCutScene)
            {
                
                pause = false;
                restart = false;
                world = new World(new Vector2(0, 5.0f));
                entities.Clear();
                toBeAdded.Clear();
                cachedEntityLists = new Dictionary<Type, object>();
                //drawingTool.initialize();
                //drawingTool.resetCamera();
                gMode = 6;
                cachedEntityLists = new Dictionary<Type, object>();
                CutScene ct= new CutScene(this,(int)currentWorld);
                backGroundImages.Clear();
                //Title.IsVisible = true;
                addEntity(ct);
                isInCreatorMode = false;
                currentWorld++;
                inCutScene = false;
                winningAnimation = false;
                drawingTool.cam.Zoom = 0.95f * drawingTool.zoomRatio;
                drawingTool.cam.Pos = new Vector2(400, 360);
                
                //worldFinished = true;


            }

            

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

            toBeAdded.Clear();

            foreach (Entity e in entities)
            {
                if (e.IsUpdateable && !e.dispose)
                {
                    

                    if (e.updatesWhenPaused)
                    {
                        // Entities which continue to update when the game is paused receive the unmodified gametime
                        if (e is CreaterBlock || e is Button)
                            e.Update(gameTime, WorldSpeed);
                    }
                    else if (!isPaused)
                    {
                        // All others receive the modified one.
                        if (gMode == 2)
                        {
                            //if (!(e is CreaterBlock || e is Button))
                                e.Update(pauseAdjustedGameTime, WorldSpeed);
                        }
                        else
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
                ballPosition += 30;
                if (ts.CompareTo(new TimeSpan(0, 0, 0,0,500)) > 0)
                {
                    if (currentLevel > 0)
                        inTransition = true;
                    else
                    {
                        if(!inCutScene)
                            inCutScene = true;
                    }
                    
                    

                    //loadNewLevel = true;
                }
            }
            if (deathAnimation == true)
            {
                animationTime.Stop();
                TimeSpan ts = animationTime.Elapsed;
                animationTime.Start();
                count++;
                ballPosition+=50;
                if (ts.CompareTo(new TimeSpan(0, 0, 0,0,300)) > 0|| playerOneInput.isAPressed())
                {
                    //deathAnimation = false;
                    numDeath++;
                    restart = true;
                    //animationTime.Restart();
                    //return;
                }
            }
            if (inWater)
            {
                world.Gravity = new Vector2(0, 5);
            }

            if (reloadButtons)
                addButtons();
            base.Update(gameTime);
            if (xycounter > 0)
            {
                int k121 = xValues[0]+yValues[0];
            }

        }
        public void pauseMusic()
        {
            if (Constants.PLAY_MUSIC)
            {
                if (MediaPlayer.State == MediaState.Playing && pause)
                    MediaPlayer.Pause();
                else if (MediaPlayer.State == MediaState.Paused && !pause)
                    MediaPlayer.Resume();
            }

        }


        protected override void Draw(GameTime gameTime)
        {
            if(gMode==0 || gMode==2)
                GraphicsDevice.Clear(backGroundColor);
            else
                GraphicsDevice.Clear(Color.Black);

            drawingTool.drawEntities(entities, gameTime);
            drawingTool.drawLetterBox();

            base.Draw(gameTime);
        }
        public void PlayerDies()
        {
            //newLevel();
            //numDeath++;
            deathAnimation = true;
            respawnFadeValue = 0f;
            //restart = true;
        }
        public Texture2D getCachedDirt(Rectangle key)
        {
            Texture2D textCached = null;
            cacheOfDirt.TryGetValue(key, out textCached);
            return textCached;

        }
        public void addCachedDirt(Rectangle key, Texture2D value)
        {
            cacheOfDirt.Add(key, value);
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

        public void loadImageFromContent()
        {


            try
            {
                if (Constants.IS_IN_REALSE_MODE)
                    return;
                DirectoryInfo di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\UploadedImages");
                FileInfo[] fi = di.GetFiles("*", SearchOption.AllDirectories);
                GraphicsDevice gd = drawingTool.getGraphicsDevice();
                foreach (FileInfo fileInfo in fi)
                {
                    Texture2D file;
                    RenderTarget2D result;

                    String nameOfFile = fileInfo.FullName;
                    using (FileStream sourceStream = new FileStream(nameOfFile, FileMode.Open))
                    {
                        file = Texture2D.FromStream(gd, sourceStream);
                    }
                    //Setup a render target to hold our final texture which will have premulitplied alpha values
                    result = new RenderTarget2D(gd, file.Width, file.Height);

                    gd.SetRenderTarget(result);
                    gd.Clear(Color.Black);

                    //Multiply each color by the source alpha, and write in just the color values into the final texture
                    var blendColor = new BlendState
                    {
                        ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
                        AlphaDestinationBlend = Blend.Zero,
                        ColorDestinationBlend = Blend.Zero,
                        AlphaSourceBlend = Blend.SourceAlpha,
                        ColorSourceBlend = Blend.SourceAlpha
                    };

                    var spriteBatch = new SpriteBatch(gd);
                    spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
                    spriteBatch.Draw(file, file.Bounds, Color.White);
                    spriteBatch.End();

                    //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
                    var blendAlpha = new BlendState
                    {
                        ColorWriteChannels = ColorWriteChannels.Alpha,
                        AlphaDestinationBlend = Blend.Zero,
                        ColorDestinationBlend = Blend.Zero,
                        AlphaSourceBlend = Blend.One,
                        ColorSourceBlend = Blend.One
                    };

                    spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
                    spriteBatch.Draw(file, file.Bounds, Color.White);
                    spriteBatch.End();

                    //Release the GPU back to drawing to the screen
                    gd.SetRenderTarget(null);

                    Texture2D finalFile = result as Texture2D;

                    sprites.Add(fileInfo.Name, new Sprite(finalFile, fileInfo.Name));
                    blockList.AddLast(fileInfo.Name);

                }
            }
            catch
            {

            }


        }
    }
}
