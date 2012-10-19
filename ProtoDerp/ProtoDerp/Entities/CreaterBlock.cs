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
        public float blockWidth, blockHeight;
        int drawLevel = 0;
        //bool inDeleteMode = false; 
        bool isSelectedBlockChanged = false;
        int blockIterater = 0;

        Entity selected = null;

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
            blockHeight = playerSprite.index.Height;
            blockWidth = playerSprite.index.Width;
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
           // KeyboardState keyState = Keyboard.GetState();
            keyInput.Update(gameTime);
            if (keyInput.IsNewKeyPressed(Keys.M))
            {
                blockIterater = 0;
                game.inDeleteMode = !game.inDeleteMode;
                isSelectedBlockChanged = true;
                if (selected != null)
                    selected.isSelected = !selected.isSelected;
            }
            if (game.inDeleteMode)
            {
                DeleteBlockSelector();
                return;
            }
            moveBlock();
            if (keyInput.IsNewKeyPressed(Keys.A))
            {
                game.blockType =Game.BlockType.Normal;
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.S))
            {
                game.blockType = Game.BlockType.Death;
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 1;
            }
            if (keyInput.IsNewKeyPressed(Keys.Z))
            {
                game.saveAlpha = 1;
                game.writeLevel(game.currentWriteLevel);
            }
            if (keyInput.IsNewKeyPressed(Keys.D)) 
            {
                game.blockType = Game.BlockType.Moving;
                blockPressed = 2;
            }
            if (keyInput.IsNewKeyPressed(Keys.F))
            {
                game.blockType = Game.BlockType.Goal;
                game.cachedEntityLists = new Dictionary<Type, object>();
                blockPressed = 3;
            }
            if (keyInput.IsNewKeyPressed(Keys.O))
            {
                if(game.currentWriteLevel>0)
                    game.currentWriteLevel--;
            }
            if (keyInput.IsNewKeyPressed(Keys.P))
            {
                if(game.currentWriteLevel<Constants.MAX_WRITE_LEVEL)
                    game.currentWriteLevel++;
            }
            
            if (keyInput.IsNewKeyPressed(Keys.Enter))
            {
                switch (game.blockType)
                {
                    case Game.BlockType.Normal:
                        addBlock();
                        break;
                    case Game.BlockType.Death:
                        addDeathBlock();
                        break;
                    case Game.BlockType.Moving:
                        addMovingDeathBlock();
                        break;
                    case Game.BlockType.Goal:
                        addGoalBlock();
                        game.cachedEntityLists = new Dictionary<Type, object>();
                        break;

                }
                
            }

            if (keyInput.IsNewKeyPressed(Keys.I) || (keyInput.IsKeyPressed(Keys.I)&&keyInput.IsKeyPressed(Keys.LeftShift)))
            {
                blockHeight += 5;
            }
            if (keyInput.IsNewKeyPressed(Keys.K) || (keyInput.IsKeyPressed(Keys.K) && keyInput.IsKeyPressed(Keys.LeftShift)))
            {
                blockHeight -= 5;
            }
            if (keyInput.IsNewKeyPressed(Keys.J) || (keyInput.IsKeyPressed(Keys.J) && keyInput.IsKeyPressed(Keys.LeftShift)))
            {
                blockWidth -= 5;
            }
            if (keyInput.IsNewKeyPressed(Keys.L) || (keyInput.IsKeyPressed(Keys.L) && keyInput.IsKeyPressed(Keys.LeftShift)))
            {
                blockWidth += 5;
            }

            game.cXLocation = pos.X;
            game.cYLocation = pos.Y;
            chooseNextSprite();
            updateDrawLevel();
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
        public void updateDrawLevel()
        {
            if (keyInput.IsNewKeyPressed(Keys.E))
            {
                drawLevel = 1;
            }
            if (keyInput.IsNewKeyPressed(Keys.R))
            {
                drawLevel = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.T))
            {
                drawLevel = 2;
            }
        }

        public void DeleteBlockSelector()
        {
            SortedSet<Entity> block= game.entities;
            int count = 0;
            if (isSelectedBlockChanged)
            {
                isSelectedBlockChanged = false;
                foreach (Entity e in game.entities)
                {
                    if (e is DeathBlock || e is Block || e is MovingDeath || e is GoalBlock)
                    {
                        if (count == blockIterater)
                        {
                            selected = e;
                            e.isSelected = true;
                        }
                        count++;
                    }
                }
            }
            moveSelectedBlock();
            if (keyInput.IsNewKeyPressed(Keys.Q))
            {
                if (blockIterater != 0)
                    blockIterater--;
                isSelectedBlockChanged=true;
                selected.isSelected = false;
            }
            if (keyInput.IsNewKeyPressed(Keys.W))
            {
                blockIterater++;
                isSelectedBlockChanged = true;
                selected.isSelected = false;

            }
            


        }

        public void moveSelectedBlock()
        {
            XboxInput xbi = (XboxInput)game.playerOneInput;
            float turbo = 1;
            if (xbi.isTriggerPressed())
                turbo = 5;
            float x = xbi.HorizontalMovementRight();
            float y = xbi.VerticalMovementRight();
            KeyboardState keyState = Keyboard.GetState();
            float xturbo = 1;
            if (keyState.IsKeyDown(Keys.LeftShift))
                xturbo = 5;
            if (keyState.IsKeyDown(Keys.Up))
            {
                y += 5;// 1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                y -= 5;// -1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                x -= 5;// -1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                x += 5;// 1 * xturbo;
            }

            if (selected is Block)
            {
                ((Block)selected).body.Position += new Vector2(ConvertUnits.ToSimUnits(x * turbo), ConvertUnits.ToSimUnits(y * turbo));
                ((Block)selected).origPos += new Vector2(x * turbo, y * turbo);
            }
            if (selected is DeathBlock)
            {
                ((DeathBlock)selected).body.Position += new Vector2(ConvertUnits.ToSimUnits(x * turbo), ConvertUnits.ToSimUnits(y * turbo));
                ((DeathBlock)selected).origPos += new Vector2(x * turbo, y * turbo);
            }
            if (selected is MovingDeath)
            {
                ((MovingDeath)selected).body.Position += new Vector2(ConvertUnits.ToSimUnits(x * turbo), ConvertUnits.ToSimUnits(y * turbo));
                ((MovingDeath)selected).origPos += new Vector2(x * turbo, y * turbo);
            }
            if (selected is GoalBlock)
            {
                ((GoalBlock)selected).body.Position += new Vector2(ConvertUnits.ToSimUnits(x * turbo), ConvertUnits.ToSimUnits(y * turbo));
                ((GoalBlock)selected).origPos += new Vector2(x * turbo, y * turbo);
            }
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
                y += 5;// 1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                y -= 5;// -1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                x -= 5;// -1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                x += 5;// 1 * xturbo;
            }
            
           
            pos += new Vector2(x * turbo, y * turbo);
            origPos += new Vector2(x*turbo, y*turbo);
        }
        public void addBlock()
        {

            game.addEntity(new Block(game, game.Arena, origPos, 1, blockArray[counter], blockHeight, blockWidth,drawLevel));
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
            if(!game.inDeleteMode)
                spriteBatch.Draw(playerSprite.index, new Rectangle((int)pos.X, (int)pos.Y, (int)blockWidth, (int)blockHeight), null, Color.White, 0, origin, SpriteEffects.None, 0f);

        }

        private void drawHealthBar()
        {

        }
    }
}
