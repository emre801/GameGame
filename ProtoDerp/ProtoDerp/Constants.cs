using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProtoDerp
{
    class Constants
    {
        public const int WRITE_LEVEL = 1;
        public const int STARTING_WORLD = 1;
        public const int MAX_WRITE_LEVEL = 29;
        public const int READ_LEVEL = 2;

        public const bool IS_IN_REALSE_MODE = false; // set to true to make sure no debug info is set

        public const int MAX_TEMPLATE_LEVEL = 10;

        public const bool IS_IN_DEBUG_MODE = true; //set this to false when release

        public const bool ENABLE_CREATIVE_MODE = true; //set this to false if you want to give access to creative mode

        public const bool SEND_EMAIL_DATA = false; // set to true when working it friends
        
        public const double GAMEWORLD_ASPECT_RATIO = 4f / 3f; // Width of game world / height of game world

        public const bool PRELOAD_LEVELS = true; // Set this to true when release

        public const bool DO_FADE_OUT = false; // set this to true for release

        public const bool DO_CUT_SCENE = true;

        public const bool BLOCK_EFFECT=true;

        public const bool FIND_LAG = false;
        // The size of the screen in pixels
        public const int DESIRED_GAME_RESOLUTION_WIDTH = 800;
        public const int DESIRED_GAME_RESOLUTION_HEIGHT = (int)(DESIRED_GAME_RESOLUTION_WIDTH / GAMEWORLD_ASPECT_RATIO);

        // The size of the game world in imaginary units
        public const float GAME_WORLD_WIDTH = 800f;  // Changing this constant will require changing lots of other constants, unless we cahnge them to be relative to this
        public const float GAME_WORLD_HEIGHT = (float)(GAME_WORLD_WIDTH / GAMEWORLD_ASPECT_RATIO);

        public const int NUMBER_OF_SONGS = 8;

        public const int TEMPLATE_COUNT = 8;

        public const int TOTALNUMBEROFMAPS = 4;

        public const bool FULLSCREEN = false;

        public const bool PLAY_MUSIC = true;

        public const float SHADOW_VALUE = 0.25f;

        public const int TOTAL_NUM_BLOCK_TYPES = 9;

        public const int TOTAL_NUMBER_OF_WORLDS = 2;

        public const bool PLAY_RAGE_SOUNDS = true;

        public const bool OVERRIDE_FULLSCREEN_RES = false; // If this is true, we will attempt to override the system's default fullscreen resolution.  Not recommended

        public static readonly Buttons[] EXPAND_BUTTONS = { Buttons.LeftShoulder, Buttons.LeftTrigger };
        public static readonly Buttons[] RELEASE_BUTTONS = { Buttons.RightShoulder, Buttons.RightTrigger };
        public const Buttons PRECISION_BUTTON = Buttons.A;
        public const Keys QUIT_BUTTON = Keys.Escape;

        public const Keys PAUSE_KEY = Keys.P;
        public const Keys UNPAUSE_KEY = Keys.U;
        public const float PAUSE_FADE = 0.15f;

        //Title Screen
        public const bool ENABLE_TITLE_SCREEN = true;
        public const float TITLE_FADEOUT_SPEED = 0.02f;
        public const float GAME_FADEIN_SPEED = 0.02f;

        //are we in joystick mode. default false
        public const bool JOYSTICK_MODE = true;

        public const int CAPTURE_REGION_BORDER_SEGMENT_COUNT = 74;
        public static readonly TimeSpan JOYSTICK_DELAY = TimeSpan.FromMilliseconds(500);
        
        #region colors
        
        

        public static readonly Color BACKGROUND_COL = Color.Black;
        public static readonly Color PREVIEW_EDGE_COL = Color.Black;
        public static readonly Color ENVIRONMENT_BULLET_COL = new Color(0, 0, 0, 128);
        public static readonly Color LETTERBOX_COLOR = Color.Black;
        

        #endregion

        #region object locations

        public static readonly Vector2 player1SpawnLocation = new Vector2(Constants.GAME_WORLD_WIDTH * .01f, Constants.GAME_WORLD_HEIGHT*0.85f);
        public static readonly Vector2 player2SpawnLocation = new Vector2(Constants.GAME_WORLD_WIDTH * .9f, Constants.GAME_WORLD_HEIGHT / 2);

        #endregion

        #region Drawing Priorities
        public const int ENEMY_DRAWPRI = 2;
        public const int PLAYER_DRAWPRI = 3;
        public const int GUI_DRAWPRI = 6;
        public const int PARTICLE_DRAWPRI = 1;
        public const int BULLET_DRAWPRI = 4;
        public const int PREVIEW_DRAWPRI = 5;
        #endregion

        #region Entity Default Scales
        #endregion

        #region Strings

        public const string PLAYER1WINS_STR = "Player 1|Wins!";
        public const string PLAYER2WINS_STR = "Player 2|Wins!";
        public const string WINDOW_TITLE = "Mirror Mechanism";

        #endregion
    }
}
