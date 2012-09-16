using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;


/**
 * 
 * Main class that playable objects will inherit. Shares all common data and operations
 * that player "ships" can perform.
 * 
 */
namespace ProtoDerp
{
    public class CreaterBlock : Entity
    {
        public Sprite playerSprite;

        float playerAngle = 0;
        public float centerOffset;
        public Vector2 origin;
        public String spriteNumber;
        public Vector2 origPos;
        LinkedList<String> blocks;
        String[] blockArray;
        int counter = 0;
        KeyboardInput keyInput;
        int blockPressed = 0;
        public CreaterBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.blocks = g.blockList;
            
            blockArray=blocks.ToArray<String>();
            this.spriteNumber = blockArray[counter];
           
            LoadContent();
            //SetUpPhysics(Constants.player1SpawnLocation + pos);
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
            
            keyInput= new KeyboardInput();

            
        }

        
        //private Vector3[] baseHB = new Vector3[Constants.HEALTH_BAR_SEGMENT_COUNT];
        private void initializePrimitives()
        {

        }

        
        public void addEnemyPlayer(PlayableCharacter enemy)
        {

        }

        public void LoadContent()
        {
            playerSprite = game.getSprite(blockArray[counter]);
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
        }

        public bool isExpanding()
        {
            return false;
        }

        public bool isPreviewing()
        {
            return true;
        }


        public void tutorialReset()
        {

        }

        public void damage(float health, int invinTime)
        {

        }

        public void updateHealthBar()
        {

        }

        public bool isInvincible()
        {
            return false;
        }

        public void expandingSquare(GameTime gameTime)
        {

        }



        public override void Update(GameTime gameTime, float worldSpeed)
        {
            
            moveBlock();
           
            KeyboardState keyState = Keyboard.GetState();
            keyInput.Update(gameTime);
            if (keyInput.IsNewKeyPressed(Keys.A))
            {
                addBlock();
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.S))
            {
                addDeathBlock();
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 1;
            }
            if (keyInput.IsNewKeyPressed(Keys.Z))
            {                
                game.writeLevel(3);
                //blockPressed = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.D)) 
            {
                addMovingDeathBlock();
                blockPressed = 2;
            }
            if (keyInput.IsNewKeyPressed(Keys.F))
            {
                addGoalBlock();
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 3;
            }
            chooseNextSprite();
        }
        public void chooseNextSprite()
        {
            bool isPressed = false;
            if (keyInput.IsNewKeyPressed(Keys.Q))
            {
                if (counter == 0)
                    counter = blockArray.Length - 1;
                else
                    counter--;
                isPressed = true;
            }
            else if (keyInput.IsNewKeyPressed(Keys.W))
            {
                if (counter == blockArray.Length - 1)
                    counter = 0;
                else
                    counter++;
                isPressed = true;
            }
            if(isPressed)
                LoadContent();
        }

        public void moveBlock()
        {
            XboxInput xbi = (XboxInput)game.playerOneInput;
            float turbo = 1;
            if (xbi.isTriggerPressed())
                turbo = 5;
            float x = xbi.HorizontalMovementRight();
            float y = xbi.VerticalMovementRight();
            KeyboardState keyState = Keyboard.GetState();
            float xturbo = 1;
            if(keyState.IsKeyDown(Keys.LeftShift))
                xturbo=5;
            if (keyState.IsKeyDown(Keys.Up))
            {
                y = 1*xturbo;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                y = -1*xturbo;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                x = -1*xturbo;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                x = 1*xturbo;
            }
            
           
            pos += new Vector2(x * turbo, y * turbo);
            origPos += new Vector2(x*turbo, y*turbo);
        }
        public void addBlock()
        {

            game.addEntity(new Block(game, game.Arena, origPos, 1, blockArray[counter], game.getSprite(blockArray[counter]).index.Height, game.getSprite(blockArray[counter]).index.Width));
        }

        public void addDeathBlock()
        {

            game.addEntity(new DeathBlock(game, game.Arena, origPos, 1, blockArray[counter]));
        }

        public void addGoalBlock()
        {

            game.addEntity(new GoalBlock(game, game.Arena, origPos, 1, blockArray[counter],game.currentLevel));
        }

        public void addMovingDeathBlock()
        {
            game.addEntity(new MovingDeath(game, game.Arena, origPos, 1, blockArray[counter], new Vector2(1,0),2 ));
        
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(playerSprite.index, new Rectangle((int)pos.X, (int)pos.Y, (int)playerSprite.index.Width, (int)playerSprite.index.Height), null, Color.White, 0, origin, SpriteEffects.None, 0f);

        }

        private void drawHealthBar()
        {

        }




    }
}
