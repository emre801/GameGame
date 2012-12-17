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
        public const int WRITE_LEVEL = 10;
        public const int MAX_WRITE_LEVEL = 29;
        public const int READ_LEVEL = 2;

        public const int MAX_TEMPLATE_LEVEL = 3;

        public const double GAMEWORLD_ASPECT_RATIO = 4f / 3f; // Width of game world / height of game world

        // The size of the screen in pixels
        public const int DESIRED_GAME_RESOLUTION_WIDTH = 800;
        public const int DESIRED_GAME_RESOLUTION_HEIGHT = (int)(DESIRED_GAME_RESOLUTION_WIDTH / GAMEWORLD_ASPECT_RATIO);

        // The size of the game world in imaginary units
        public const float GAME_WORLD_WIDTH = 800f;  // Changing this constant will require changing lots of other constants, unless we cahnge them to be relative to this
        public const float GAME_WORLD_HEIGHT = (float)(GAME_WORLD_WIDTH / GAMEWORLD_ASPECT_RATIO);


        public const int TEMPLATE_COUNT = 8;

        public const int TOTALNUMBEROFMAPS = 4;

        public const bool FULLSCREEN = false;

        public const bool OVERRIDE_FULLSCREEN_RES = false; // If this is true, we will attempt to override the system's default fullscreen resolution.  Not recommended

        //Does getting hit by an enemy bullet result in your preview being destroyed?
        public const bool PREVIEW_DESTROY_MODE = true;

        //Does getting hit by an enemy bullet heal the other player?
        public const bool OPPONENT_HEAL_MODE = false;
        public const float OPPONENT_HEAL_AMOUNT = 2f;

        //don't give any fucks about collisions
        public const bool GOD_MODE = false;

        // Do player bullets collide with each other?
        public const bool PLAYER_BULLETS_CANCEL = false;

        //If this mode is on, having less bullets in your capture region will make them move faster, and having
        //more bullets in your capture region will make them move slower.
        public const bool DENSITY_BALANCE_MODE = true;

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

        //align all bullets when you're rotated. default true
        public const bool ALIGNED_BULLETS_COMPUTE = true;

        //default true
        public const bool ALIGNED_BULLETS_DRAW = true;

        //default true.
        public const bool ROTATE_POSITIONS = false;

        public const bool USE_CENTER_BULLET_MASS = true;

        public const int CAPTURE_REGION_BORDER_SEGMENT_COUNT = 74;

        public const float PLAYER_MAX_SPEED = 5f;
        public const float HEALTH_BAR_RADIUS = 71f;
        public const float HEALTH_BAR_THICKNESS = 16f;
        public const int HEALTH_BAR_SEGMENT_COUNT = 53; // Works best if this is (a multiple of the number of hits it takes to kill a player + 1)

        public const float PLAYER_SPRITE_RADIUS = 25f; // This is purely visual, except that the player movement bounds should be based on it
        public const float PLAYER_HITBOX_RADIUS = 5f; // This is visual and also the size of the hitbox
        public const float PRECISION_HITBOX_GROWTH = .5f; // This is the amount by which the hitbox grows when in precision mode
        public const float EXPANSION_HITBOX_GROWTH_FACTOR = 4f; // This is the factor at which the hitbox growth is tied to the expansion size

        // These are hitboxes for each bullet type
        public static readonly Rectangle BULLET_0_RECT = new Rectangle(-1, -1, 3, 3);
        public static readonly Rectangle BULLET_1_RECT = new Rectangle(-6, -6, 13, 13);
        public static readonly Rectangle BULLET_2_RECT = Rectangle.Empty;
        public static readonly Rectangle BULLET_3_RECT = Rectangle.Empty;

        public static readonly Rectangle PLAYERBULL_0_RECT = new Rectangle(-6, -6, 13, 13);
        public static readonly Rectangle PLAYERBULL_1_RECT = new Rectangle(-11, -11, 23, 23);
        public static readonly Rectangle PLAYERBULL_2_RECT = new Rectangle(-11, -11, 23, 23);
        public static readonly Rectangle PLAYERBULL_3_RECT = new Rectangle(-11, -11, 23, 23);


        public static readonly TimeSpan JOYSTICK_DELAY = TimeSpan.FromMilliseconds(500);

        // Preview parameters

        public const float MAX_PREVIEW_RADIUS = 185f;
        public const float MID_PREVIEW_RADIUS = 75f;
        public const float MIN_PREVIEW_RADIUS = 22f;

        public static readonly TimeSpan TEMPLATE_SWITCH_DELAY = TimeSpan.FromSeconds(15); // milliseconds between templates
        public const float PREVIEW_DURATION = 4000;
        public const float EASE_IN_WEIGHT = 0.1f;

        public static readonly TimeSpan PREVIEW_EXPAND_DURATION_IN = TimeSpan.FromMilliseconds(PREVIEW_DURATION * EASE_IN_WEIGHT); //Time to go from Min to Mid
        public static readonly TimeSpan PREVIEW_EXPAND_DURATION_OUT = TimeSpan.FromMilliseconds(PREVIEW_DURATION * (1 - EASE_IN_WEIGHT)); //Time to go from Mid to Max

        #region colors

        public static readonly Color PLAYER_ONE_COL = new Color(0, 0, 255, 128);
        public static readonly Color PLAYER_TWO_COL = new Color(255, 0, 0, 128);
        public static readonly Color PLAYER_ONE_BULLET_COL = Color.Blue;
        public static readonly Color PLAYER_TWO_BULLET_COL = Color.Red;
        public static readonly Color PLAYER_THREE_COL = new Color(0, 255, 0, 128);
        public static readonly Color PLAYER_THREE_BULLET_COL = Color.Green;
        public static readonly Color PLAYER_FOUR_COL = new Color(255, 255, 0, 128);
        public static readonly Color PLAYER_FOUR_BULLET_COL = Color.Yellow;

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
