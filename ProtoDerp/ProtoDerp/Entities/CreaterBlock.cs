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

        public float centerOffset;
        public Vector2 origin;
        public String spriteNumber;
        public Vector2 origPos;
        LinkedList<String> blocks;
        String[] blockArray;
        //public int game.spriteBlockCounter = 0;
        KeyboardInput keyInput;
        int blockPressed = 0;
        public float blockWidth, blockHeight;
        int drawLevel = 0;
        //bool inDeleteMode = false; 
        bool isSelectedBlockChanged = false;
        int blockIterater = 0;

        Entity selected = null;

        MouseState oldMouse;
        int oldMouseValue;
        //int cameraWindowValue = 0;
        bool mouseInSelectMode = false;
        //public Vector2 magnetPulse;
        public bool incrementXMagnetValue;
        SpriteStripAnimationHandler ani;
        int rotation = 0;
        Vector2 point1 = new Vector2(0, 0), point2 = new Vector2(0, 0);
        bool clickOne = false;
        bool clickTwo = false;
        ArrayList pathPoints= new ArrayList();
        Vector2 drawPoint1 = Vector2.Zero, drawPoint2=Vector2.Zero;
        //Vector2 drawSquarePoint1 = Vector2.Zero, drawSquarePoint = Vector2.Zero;
        int currentPointValue = 0;
        int currentSquareValue = 0;
        int everyOther = 10;
        int other = 0;

        bool isCapOn = false;

        public SortedSet<Entity> linedUpBlocks = new SortedSet<Entity>();

        

        public CreaterBlock(Game g, Arena a, Vector2 pos, int playerNum, String spriteNumber)
            : base(g)
        {
            this.pos = Constants.player1SpawnLocation + pos;
            this.origPos = pos;
            this.drawPriority = Constants.PLAYER_DRAWPRI;
            this.blocks = g.blockList;
            
            blockArray=blocks.ToArray<String>();
            this.spriteNumber = blockArray[game.spriteBlockCounter];
            
            LoadContent();
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
           
            keyInput= new KeyboardInput();
            oldMouse = Mouse.GetState();
            oldMouseValue = Mouse.GetState().ScrollWheelValue;
            //magnetPulse = new Vector2(-20, 0);
            

            
        }

        public void addEnemyPlayer(PlayableCharacter enemy)
        {
            //Option to add enemy character, no plan right now to use
        }

        public void LoadContent()
        {
            playerSprite = game.getSprite(blockArray[game.spriteBlockCounter]);
            origin = new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
            blockHeight = playerSprite.index.Height;
            blockWidth = playerSprite.index.Width;


            ani = game.getSpriteAnimation(blockArray[game.spriteBlockCounter]);
            
                blockHeight = ani.heightOf();
                blockWidth = ani.widthOf();
                origin = new Vector2(blockWidth / 2, blockHeight / 2);
                game.isButtonSelect = false;
            

        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
           // KeyboardState keyState = Keyboard.GetState();
            ani.Update();
            keyInput.Update(gameTime);
            //if (Keyboard.GetState().GetPressedKeys().Length == 0)
                //return;
            if (keyInput.IsNewKeyPressed(Keys.U))
            {
                game.backGroundNum++;
                if (game.backGroundNum > 3)
                    game.backGroundNum = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.X))
            {
                if (Constants.SEND_EMAIL_DATA)
                {
                    game.sendEmailWithAttachments();
                }
            }
            if (game.backGroundNum != 0)
            {
                changeBackgroundColor();
                return;
            }
            if (keyInput.IsNewKeyPressed(Keys.M))
            {
                blockIterater = 0;
                game.inDeleteMode = !game.inDeleteMode;
                isSelectedBlockChanged = true;
                if (selected != null)
                    selected.isSelected = !selected.isSelected;
            }

            if (keyInput.IsNewKeyPressed(Keys.H))
            {
                if (game.cameraWindowValue == 4)
                    game.cameraWindowValue = 0;
                else
                    game.cameraWindowValue++;

            }

            if (game.cameraWindowValue != 0)
            {
                cameraInputPress();
                return;

            }

            if (keyInput.IsNewKeyPressed(Keys.OemPeriod))
            {
                if (game.currentWorld < Constants.TOTAL_NUMBER_OF_WORLDS)
                    game.currentWorld += 1;
                else
                    game.currentWorld = 1;

            }

            if (keyInput.IsNewKeyPressed(Keys.Tab))
            {
                game.testLevel = !game.testLevel;
            }
            if (game.testLevel)
            {
                return;
            }
            
            if (game.inDeleteMode)
            {
                DeleteBlockSelector();
                return;
            }
            moveBlock();

            if (keyInput.IsNewKeyPressed(Keys.A))
            {
                if (blockPressed == 0)
                    blockPressed = Constants.TOTAL_NUM_BLOCK_TYPES;
                else
                    blockPressed--;
                changeBlockType();
            }
            if (keyInput.IsNewKeyPressed(Keys.S))
            {
                if (blockPressed == Constants.TOTAL_NUM_BLOCK_TYPES)
                    blockPressed = 0;
                else
                    blockPressed++;
                changeBlockType();
            }



            /*if (keyInput.IsNewKeyPressed(Keys.A))
            {
                game.blockType =Game.BlockType.Normal;
                game.cachedEntityLists = new Dictionary<Type, object>();
                //blockPressed = 0;
            }
            if (keyInput.IsNewKeyPressed(Keys.S))
            {
                game.blockType = Game.BlockType.Death;
                game.cachedEntityLists = new Dictionary<Type, object>();
                //blockPressed = 1;
            }
             * */
            if (keyInput.IsNewKeyPressed(Keys.Z))
            {
                game.saveAlpha = 1;
                game.writeLevel(game.currentWriteLevel);
            }

            if (keyInput.IsNewKeyPressed(Keys.D2))
            {
                rotation += 90;
            }
            if (keyInput.IsNewKeyPressed(Keys.D1))
            {
                rotation -= 90;
            }
            if (keyInput.IsKeyPressed(Keys.D3) &&  keyInput.keyboardState.IsKeyUp(Keys.LeftShift))
            {
                rotation += 1;
            }
            if (keyInput.IsKeyPressed(Keys.D3) && keyInput.keyboardState.IsKeyDown(Keys.LeftShift))
            {
                rotation -= 1;
            }
            if (keyInput.IsNewKeyPressed(Keys.OemPlus))
            {
                rotation = 0 ;
            }
            if (keyInput.IsNewKeyPressed(Keys.D6))
            {
                game.camZoomValue = game.drawingTool.cam.Zoom;
                game.camPosSet = game.drawingTool.cam.Pos;

            }

            if (game.blockType.Equals(Game.BlockType.Magnet))
            {
                if (incrementXMagnetValue)
                {
                    if (keyInput.IsNewKeyPressed(Keys.F))
                    {
                        game.magnetPulse -= new Vector2(1, 0);
                    }
                    else if (keyInput.IsNewKeyPressed(Keys.D))
                    {
                        game.magnetPulse += new Vector2(1, 0);
                    }
                }
                else
                {
                    if (keyInput.IsNewKeyPressed(Keys.F))
                    {
                        game.magnetPulse -= new Vector2(0, 1);
                    }
                    else if (keyInput.IsNewKeyPressed(Keys.D))
                    {
                        game.magnetPulse += new Vector2(0, 1);
                    }

                }
                if (keyInput.IsNewKeyPressed(Keys.LeftShift))
                {
                    incrementXMagnetValue = !incrementXMagnetValue;
                }
            }
            if (game.blockType.Equals(Game.BlockType.Path))
            {

                if (keyInput.IsNewKeyPressed(Keys.F))
                {
                    game.pathSpeed--;
                }
                if (keyInput.IsNewKeyPressed(Keys.D))
                {
                    game.pathSpeed++;
                }
            }
            if (game.blockType.Equals(Game.BlockType.Moving))
            {
                if (clickOne)
                {
                    Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 500 * game.drawingTool.cam.Zoom);
                    Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));
                    Vector2 direction = worldMousePosition - point1;
                    direction = new Vector2((int)(direction.X * 0.1f), (int)(direction.Y * 0.1f));
                    game.moveSpeed = direction;
                }
            }

            /*
            if (keyInput.IsNewKeyPressed(Keys.D)) 
            {
                game.blockType = Game.BlockType.Moving;
                //blockPressed = 2;
            }
            if (keyInput.IsNewKeyPressed(Keys.F))
            {
                game.blockType = Game.BlockType.Goal;
                game.cachedEntityLists = new Dictionary<Type, object>();
                //blockPressed = 3;
            }
             * */
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
            if (keyInput.IsNewKeyPressed(Keys.V))
            {
                game.loadNewLevel = true;
                //this.dispose = true;
                game.currentWriteLevel = game.gameTemplateLevel;
                //game.clearEntities();
                return;
            }
            if (keyInput.IsNewKeyPressed(Keys.B))
            {
                if(game.gameTemplateLevel!=0)
                    game.gameTemplateLevel--;
            }
            if (keyInput.IsNewKeyPressed(Keys.G))
            {
                game.activateButtons = !game.activateButtons;
            }
            if (keyInput.IsNewKeyPressed(Keys.N))
            {
                if (!game.loadFromLevel)
                {
                    if (game.gameTemplateLevel != Constants.MAX_TEMPLATE_LEVEL)
                        game.gameTemplateLevel++;
                }
                else
                {
                    if (game.gameTemplateLevel != Constants.MAX_WRITE_LEVEL)
                        game.gameTemplateLevel++;

                }
            }
            if (keyInput.IsNewKeyPressed(Keys.RightShift))
            {
                game.loadFromLevel = !game.loadFromLevel;
            }

            if (keyInput.IsNewKeyPressed(Keys.C))
            {
                mouseInSelectMode = !mouseInSelectMode;

                if (mouseInSelectMode)
                {
                    Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 500 * game.drawingTool.cam.Zoom);
                    Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));
                    
                    origPos = worldMousePosition;
                    pos = worldMousePosition+Constants.player1SpawnLocation;
                    
                }
                
            }
            if (keyInput.IsNewKeyPressed(Keys.Enter) && keyInput.keyboardState.IsKeyUp(Keys.LeftShift) && keyInput.keyboardState.IsKeyUp(Keys.LeftControl))
            {
                if (currentPointValue == 1)
                {
                    currentPointValue++;
                    drawPoint2 = origin;

                }
                if (currentSquareValue == 1)
                {
                    currentSquareValue++;
                    drawPoint2 = origin;

                }
                {
                    if (game.drawLevel < 0)
                    {
                        game.backGroundImages.Add(new BackgroundBlock(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], blockHeight, blockWidth));
                        return;
                    }
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
                        case Game.BlockType.Magnet:
                            addMagnetBlock();
                            break;
                        case Game.BlockType.WaterBlock:
                            addWaterBlock();
                            break;
                    }
                }


            }
            if (keyInput.IsNewKeyPressed(Keys.Enter) && keyInput.keyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (currentPointValue == 0)
                {
                    currentSquareValue = 0;
                    drawPoint1 = origPos;
                    currentPointValue++;
                }
                else
                {
                    currentSquareValue = 0;
                    currentPointValue++;
                    drawPoint2 = origPos;
                }



            }
            if (keyInput.IsNewKeyPressed(Keys.Enter) && keyInput.keyboardState.IsKeyDown(Keys.LeftControl))
            {
                if (currentSquareValue == 0)
                {
                    currentPointValue = 0;
                    drawPoint1 = origPos;
                    currentSquareValue++;
                }
                else
                {
                    currentPointValue = 0;
                    currentSquareValue++;
                    drawPoint2 = origPos;
                }

            }

            if (currentPointValue >= 1)
            {
                drawPoint2 = origPos;
            }
            if (currentSquareValue >= 1)
            {
                drawPoint2 = origPos;
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

            //Matrix inverseViewMatrix = Matrix.Invert(game.drawingTool.cam._transform);
            //Vector2 worldMousePosition = Vector2.Transform(mousePosition, inverseViewMatrix);
            //Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //Vector2 worldMousePosition = Vector2.Transform(mousePosition, game.drawingTool.cam._transform);
            if (!mouseInSelectMode)
            {
                Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 500 * game.drawingTool.cam.Zoom);
                Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));

                if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && keyInput.keyboardState.IsKeyUp(Keys.LeftShift) && keyInput.keyboardState.IsKeyUp(Keys.LeftControl))
                {
                    
                    if (currentPointValue == 1)
                    {
                        currentPointValue++;
                        drawPoint2 = worldMousePosition;

                    }
                    else if (currentSquareValue == 1)
                    {
                        currentSquareValue++;
                        drawPoint2 = worldMousePosition;

                    }
                    else
                    {
                        addBlockBasedOnMouse(worldMousePosition);
                    }
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && keyInput.keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    if (currentPointValue == 0)
                    {
                        currentSquareValue = 0;
                        drawPoint1 = worldMousePosition;
                        currentPointValue++;
                    }
                    else
                    {
                        currentSquareValue = 0;
                        currentPointValue++;
                        drawPoint2 = worldMousePosition;
                    }
                    
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && keyInput.keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    if (currentSquareValue == 0)
                    {
                        currentPointValue = 0;
                        drawPoint1 = worldMousePosition;
                        currentSquareValue++;
                    }
                    else
                    {
                        currentPointValue = 0;
                        currentSquareValue++;
                        drawPoint2 = worldMousePosition;
                    }

                }
                else if (Mouse.GetState().RightButton == ButtonState.Pressed && oldMouse.RightButton == ButtonState.Released)
                {
                    if (game.blockType == Game.BlockType.Cycle)
                        pathPoints.Add(worldMousePosition + Constants.player1SpawnLocation);
                    else
                    {
                        currentPointValue = 0;
                        //linedUpBlocks.Clear();
                    }
                }

                pos = worldMousePosition+Constants.player1SpawnLocation;
                blockIterater += Mouse.GetState().ScrollWheelValue;
                oldMouseValue = Mouse.GetState().ScrollWheelValue;
                if (currentPointValue >= 1)
                {
                    drawPoint2 = worldMousePosition;
                }
                if (currentSquareValue >= 1)
                {
                    drawPoint2 = worldMousePosition;
                }
                
            }
            else
            {
                
                    makeMouseSelection();
            }
            game.cXLocation = pos.X;
            game.cYLocation = pos.Y;
            oldMouse = Mouse.GetState();
            chooseNextSprite();
            updateDrawLevel();
            game.isSelectingBlock = false;


            //if (currentPointValue == 2)
           // {

                //currentPointValue = 0;
            if (game.blockType != Game.BlockType.Death && game.blockType != Game.BlockType.Normal && game.blockType != Game.BlockType.WaterBlock || Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                
                    currentPointValue = 0;
                    currentSquareValue = 0;
                    linedUpBlocks.Clear();
                
            }
            if (keyInput.IsNewKeyPressed(Keys.CapsLock))
            {
                isCapOn = !isCapOn;
                currentPointValue = 0;
                currentSquareValue = 0;
                linedUpBlocks.Clear();
            }

            if (!isCapOn)
            {
                if ((currentPointValue >= 1 && other % everyOther == 0) || currentPointValue == 2)
                {
                    drawObjectsInLine(drawPoint1, drawPoint2);
                }
                else if ((currentSquareValue >= 1 && other % everyOther * 1000 == 0) || currentSquareValue == 2)
                {
                    drawObjectsInSquare(drawPoint1, drawPoint2);
                }
                other++;
            }
            else
            {
                if (game.blockType == Game.BlockType.Normal || game.blockType == Game.BlockType.Death || game.blockType == Game.BlockType.WaterBlock)
                    dragAndDrawObjects(drawPoint1, drawPoint2);
                else
                {
                    currentPointValue = 0;
                    currentSquareValue = 0;
                    linedUpBlocks.Clear();
                }

            }
            //}

        }
        public void dragAndDrawObjects(Vector2 p1, Vector2 p2)
        {
            if (currentPointValue == 1)
            {
                linedUpBlocks.Clear();
                Vector2 squareVec = p2 - p1;
                if(squareVec.X>2 && squareVec.Y>2)
                    linedUpBlocks.Add(new Block(game, game.Arena, p1 + new Vector2((int)squareVec.X / 2, (int)squareVec.Y / 2), 1, blockArray[game.spriteBlockCounter], (int)squareVec.Y, (int)squareVec.X, game.drawLevel, (int)rotation));
                

            }
            else if (currentPointValue == 2)
            {
                linedUpBlocks.Clear();
                currentPointValue = 0;
                Vector2 squareVec = p2 - p1;
                float tempHeight = blockHeight, tempWidth = blockWidth;
                this.blockHeight = squareVec.Y;
                this.blockWidth = squareVec.X;
                if (squareVec.X > 2 && squareVec.Y > 2)
                    addBlockBasedOnMouse(p1 + new Vector2((int)squareVec.X / 2, (int)squareVec.Y / 2));
                blockHeight = tempHeight;
                blockWidth = tempWidth;
                //if(game.blockType==Game.BlockType.Normal)
                    //game.addEntity(new Block(game, game.Arena, p1 + new Vector2((int)squareVec.X / 2, (int)squareVec.Y / 2), 1, blockArray[game.spriteBlockCounter], (int)squareVec.Y, (int)squareVec.X, game.drawLevel, (int)rotation));
                
            }

        }

        public void drawObjectsInLine(Vector2 p1, Vector2 p2)
        {
            float angle = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            int blockSpacing = (int)this.blockWidth;
            float angleDegree = angle / (float)Math.PI*180f;
            linedUpBlocks = new SortedSet<Entity>() ;
            //angleDegree = Math.Abs(angleDegree);
            if ((angleDegree > 0 && angleDegree <= 90) || (angleDegree > 180 && angleDegree <= 270))
            {
               // blockWidth = (int)this.blockHeight;
            }
            if (blockSpacing < this.blockHeight)
            {
                blockSpacing = (int)this.blockHeight;
                angleDegree += 90;

            }

            Vector2 lineVec = p2 - p1;
            float tempRotation = rotation;
            this.rotation += (int)angleDegree;
            for (int i = 0; i < lineVec.Length(); i = i + blockSpacing)
            {
                int xAngle = (int)(Math.Cos(angle) * i);
                int yAngle = (int)(Math.Sin(angle) * i);
                Vector2 drawVal = new Vector2(p1.X + xAngle, p1.Y + yAngle);
                if (currentPointValue == 2 && (game.blockType== Game.BlockType.Death ||(game.blockType== Game.BlockType.Normal && game.drawLevel!=0)))
                    addBlockBasedOnMouse(drawVal);
                else
                {
                    linedUpBlocks.Add(new Block(game, game.Arena, drawVal, 1, blockArray[game.spriteBlockCounter], blockHeight, blockWidth, game.drawLevel, (int)rotation));
                }

            }
            if (currentPointValue == 2)
            {
                currentPointValue = 0;
                linedUpBlocks = new SortedSet<Entity>();
            }
            rotation = (int)tempRotation;
        }
        public void drawObjectsInSquare(Vector2 p1, Vector2 p2)
        {
            linedUpBlocks = new SortedSet<Entity>();

            Vector2 squareVec = p2 - p1;
            int xWidth = 0,yWidth=0;
            for (int i = 0; i < Math.Abs(squareVec.X); i=i+(int)blockWidth)
            {
                xWidth++;
                yWidth = 0;
                for (int j = 0; j < Math.Abs(squareVec.Y); j+=(int)blockHeight)
                {
                    yWidth++;
                    int xPosi = (int)p1.X + i;
                    if(squareVec.X<0)
                        xPosi = (int)p1.X - i;
                    int yPosi = (int)p1.Y + j;
                    if (squareVec.Y < 0)
                        yPosi = (int)p1.Y - j;
                    Vector2 drawVal = new Vector2(xPosi, yPosi);
                    int tempDrawValue = game.drawLevel;
                    if (currentSquareValue == 2 && (game.blockType == Game.BlockType.Death || (game.blockType == Game.BlockType.Normal)))
                    {
                       
                            game.drawLevel = 2;
                        addBlockBasedOnMouse(drawVal);
                    }
                    else
                    {
                        game.drawLevel = 2;
                        linedUpBlocks.Add(new Block(game, game.Arena, drawVal, 1, blockArray[game.spriteBlockCounter], blockHeight, blockWidth, game.drawLevel, rotation));
                    }
                    game.drawLevel = tempDrawValue;

                }
            }

            if (currentSquareValue == 2)
            {
                currentSquareValue = 0;
                linedUpBlocks = new SortedSet<Entity>();
                if (game.drawLevel == 0)
                {
                    int actuallyWidth = (int)(xWidth * blockWidth);
                    int actuallyHeight = (int)(yWidth * blockHeight);
                    //Vector2 drawOrig=new Vector2(playerSprite.index.Width / 2, playerSprite.index.Height / 2);
                    game.addEntity(new Block(game, game.Arena, p1 + new Vector2((int)(actuallyWidth / 2) - blockWidth / 2, (int)(actuallyHeight / 2) - blockHeight / 2), 1, "blankImage", (int)actuallyHeight, (int)actuallyWidth, 0, rotation));
                }
            }

        }

        public void changeBlockType()
        {

            switch (blockPressed)
            {

                case 0:
                    game.blockType = Game.BlockType.Normal;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    //blockPressed = 0;
                    break;
                case 1:
                    game.blockType = Game.BlockType.Death;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    //blockPressed = 1;
                    break;
                case 2:
                    game.blockType = Game.BlockType.Moving;
                    //blockPressed = 2;
                    break;
                case 3:
                    game.blockType = Game.BlockType.Goal;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                //blockPressed = 3;
                    break;
                case 4:
                    game.blockType = Game.BlockType.Magnet;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    break;
                case 5:
                    game.blockType = Game.BlockType.Path;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    break;
                case 6:
                    game.blockType = Game.BlockType.Cycle;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    break;
                case 7:
                    game.blockType = Game.BlockType.WaterBlock;
                    game.cachedEntityLists = new Dictionary<Type, object>();
                    break;

            }

        }

        public void chooseNextSprite()
        {
            bool isPressed = false;
            if (keyInput.IsNewKeyPressed(Keys.Q))
            {
                if (game.spriteBlockCounter == 0)
                    game.spriteBlockCounter = blockArray.Length - 1;
                else
                    game.spriteBlockCounter--;
                isPressed = true;
            }
            else if (keyInput.IsNewKeyPressed(Keys.W))
            {
                if (game.spriteBlockCounter == blockArray.Length - 1)
                    game.spriteBlockCounter = 0;
                else
                    game.spriteBlockCounter++;
                isPressed = true;
            }
            if(isPressed|| game.isButtonSelect)
                LoadContent();
        }
        public void updateDrawLevel()
        {
            if (keyInput.IsNewKeyPressed(Keys.E))
            {
                if (game.drawLevel > -1)
                {
                    drawLevel--;
                    game.drawLevel--;
                }
                else
                {
                    drawLevel = 2;
                    game.drawLevel = 2;
                }
            }
            if (keyInput.IsNewKeyPressed(Keys.R))
            {
                if (game.drawLevel < 2)
                {
                    drawLevel++;
                    game.drawLevel++;
                }
                else
                {
                    drawLevel = -1;
                    game.drawLevel = -1;
                }
            }
        }

        public void changeBackgroundColor()
        {
            int numIncrease = 0;
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Q))
            {
                numIncrease = 1;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                numIncrease = -1;
            }
            Vector3 color = game.backGroundColor.ToVector3();
            float rValue = color.X * 252;
            float gValue = color.Y * 252;
            float bValue = color.Z * 252;
            if (game.backGroundNum == 1)
            {
                rValue+=numIncrease;
            }
            if (game.backGroundNum == 2)
            {
                gValue += numIncrease;
            }
            if (game.backGroundNum == 3)
            {
                bValue += numIncrease;
            }
            Color newBGColor = new Color(rValue/252f, gValue/252f, bValue/252f);
            game.backGroundColor = newBGColor;
            
            
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
                    if (e is EntityBlock)/* DeathBlock || e is Block || e is MovingDeath || e is GoalBlock
                        || e is MagnetBlock || e is MovingCycle || e is MovingPath || e is BackgroundBlock)*/
                    {
                        if (count == blockIterater&&e.IsVisible)
                        {
                            selected = e;
                            e.isSelected = true;
                            break;
                        }
                        count++;
                    }
                }
                int backCounter = 0;
                if (blockIterater > game.entities.Count)
                {
                    int backgroundBlockCounter = blockIterater - game.entities.Count-1;
                    foreach (BackgroundBlock i in game.backGroundImages)
                    {
                        if (backCounter == backgroundBlockCounter && i.IsVisible)
                        {
                            selected = i;
                            i.isSelected = true;
                        }
                        backCounter++;

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
            if (keyInput.IsNewKeyPressed(Keys.Delete) || keyInput.IsNewKeyPressed(Keys.Back))
            {
                selected.IsVisible = false;
                isSelectedBlockChanged = true;
                selected.isSelected = false;
                blockIterater++;
            }
            if (keyInput.IsNewKeyPressed(Keys.Z))
            {
                game.saveAlpha = 1;
                game.writeLevel(game.currentWriteLevel);
            }
            if (keyInput.IsNewKeyPressed(Keys.T))
            {
                deleteAllSelectedType<DeathBlock>();
            }
            if (keyInput.IsNewKeyPressed(Keys.Y))
            {
                deleteAllSelectedType<Block>();
            }
            if (keyInput.IsNewKeyPressed(Keys.U))
            {
                deleteAllSelectedType<MovingDeath>();
            }
            if (keyInput.IsNewKeyPressed(Keys.I))
            {
                deleteAllSelectedType<MagnetBlock>();
            }
            if (keyInput.IsNewKeyPressed(Keys.O))
            {
                deleteAllSelectedType<MovingCycle>();
            }
            if (keyInput.IsNewKeyPressed(Keys.G))
            {
                //deleteEverything();
                deleteAllSelectedType<EntityBlock>();
                deleteSuperBackGround();
            }
            if (keyInput.IsNewKeyPressed(Keys.J))
            {
                deleteAllSelectedType<Block>(2);
            }
            if (keyInput.IsNewKeyPressed(Keys.K))
            {
                deleteAllSelectedType<Block>(1);
            }
            if (keyInput.IsNewKeyPressed(Keys.D))
            {
                deleteSuperBackGround();
            }
            if (keyInput.IsNewKeyPressed(Keys.H))
            {
                deleteAllSelectedType<Block>(2);
            }

        }
        public void deleteSuperBackGround()
        {
            foreach (Entity e in game.backGroundImages)
            {
                e.IsVisible = false;
                isSelectedBlockChanged = true;
                e.isSelected = false;
                isSelectedBlockChanged = true;
            }

        }

        public void deleteEverything()
        {
            foreach (Entity e in game.entities)
            {
                if (e is DeathBlock || e is Block || e is MovingDeath || e is GoalBlock
                        || e is MagnetBlock || e is MovingCycle || e is MovingPath)
                {

                    e.IsVisible = false;
                    isSelectedBlockChanged = true;
                    e.isSelected = false;


                }
            }
            isSelectedBlockChanged = true;

        }

        public void deleteAllSelectedType<Type>()
        {
            deleteAllSelectedType<Type>(-1);
        }

        public void deleteAllSelectedType<Type>(int level)
        {
            foreach (Entity e in game.entities)
            {
                if (e is Type)
                {
                    if (level >= 0)
                    {
                        Block i = (Block)e;
                        if (i.drawLevel == level)
                        {
                            e.IsVisible = false;
                            isSelectedBlockChanged = true;
                            e.isSelected = false;
                        }
                    }
                    else
                    {
                        e.IsVisible = false;
                        isSelectedBlockChanged = true;
                        e.isSelected = false;
                    }
                    
                   
                    
                }
            }
            isSelectedBlockChanged = true;



        }

        public void makeMouseSelection()
        {
           
            
        }

        public void cameraInputPress()
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 500 * game.drawingTool.cam.Zoom);
            Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));
            worldMousePosition += Constants.player1SpawnLocation;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
            {

                 switch (game.cameraWindowValue)
                 {
                     case 1:
                         game.maxButtom = worldMousePosition.Y;
                         game.Arena.maxButtom = worldMousePosition.Y;
                        break;
                     case 2:
                        game.maxTop = worldMousePosition.Y;
                        game.Arena.maxTop = worldMousePosition.Y;
                        break;
                     case 3:
                        game.maxRight = worldMousePosition.X;
                        game.Arena.maxRight = worldMousePosition.X;
                        break;
                     case 4:
                        game.maxLeft = worldMousePosition.X;
                        game.Arena.maxLeft = worldMousePosition.X;
                        break;
                    }
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
            if (keyInput.IsNewKeyPressed(Keys.Up)|| (keyState.IsKeyDown(Keys.Up) && keyState.IsKeyDown(Keys.LeftShift)))
            {
                y += 1;// 1 * xturbo;
            }
            if (keyInput.IsNewKeyPressed(Keys.Down) || (keyState.IsKeyDown(Keys.Down) && keyState.IsKeyDown(Keys.LeftShift)))
            {
                y -= 1;// -1 * xturbo;
            }
            if (keyInput.IsNewKeyPressed(Keys.Left) || (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyDown(Keys.LeftShift)))
            {
                x -= 1;// -1 * xturbo;
            }
            if (keyInput.IsNewKeyPressed(Keys.Right) || (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyDown(Keys.LeftShift)))
            {
                x += 1;// 1 * xturbo;
            }
            //Should think of a better way of doing this
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
            if (selected is MagnetBlock)
            {
                ((MagnetBlock)selected).body.Position += new Vector2(ConvertUnits.ToSimUnits(x * turbo), ConvertUnits.ToSimUnits(y * turbo));
                ((MagnetBlock)selected).origPos += new Vector2(x * turbo, y * turbo);
            }
            if (selected is BackgroundBlock)
            {

                ((BackgroundBlock)selected).pos += new Vector2(ConvertUnits.ToSimUnits(x * 200 * turbo), ConvertUnits.ToSimUnits(y * 200 * turbo));
                ((BackgroundBlock)selected).startPos += new Vector2(x * 200 * turbo, y * 200 * turbo);
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
            if (keyState.IsKeyDown(Keys.LeftControl))
                xturbo = xturbo * 10;
            if(keyState.IsKeyDown(Keys.RightShift))
                turbo=0.5f;
            if (keyState.IsKeyDown(Keys.Up))
            {
                y += 1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                y -= 1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                x -= 1 * xturbo;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                x += 1 * xturbo;
            }
            
           
            pos += new Vector2(x * turbo, y * turbo);
            origPos += new Vector2(x*turbo, y*turbo);
        }

        public void addBlockBasedOnMouse(Vector2 origin)
        {
            if (!game.isSelectingBlock)
            {
                if (game.drawLevel < 0)
                {
                    game.backGroundImages.Add(new BackgroundBlock(game,game.Arena,origin,1,blockArray[game.spriteBlockCounter],blockHeight,blockWidth));
                    return;
                }

                switch (game.blockType)
                {
                    case Game.BlockType.Normal:
                        game.addEntity(new Block(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], blockHeight, blockWidth, game.drawLevel, rotation));
                        break;
                    case Game.BlockType.Death:
                        game.addEntity(new DeathBlock(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], rotation));
                        break;
                    case Game.BlockType.Moving:
                        if (!clickOne)
                        {
                            point1 = origin;
                            clickOne = true;
                        }
                        else
                        {
                            clickOne = false;
                            Vector2 direction = origin - point1;
                            direction = new Vector2((int)(direction.X*0.01f), (int)(direction.Y*0.01f));
                            game.addEntity(new MovingDeath(game, game.Arena, point1, 1, blockArray[game.spriteBlockCounter], direction, game.pathSpeed));
                           
                        }
                        break;
                    case Game.BlockType.Goal:
                        game.addEntity(new GoalBlock(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], game.currentLevel));
                        game.cachedEntityLists = new Dictionary<Type, object>();
                        break;
                    case Game.BlockType.Magnet:
                        game.addEntity(new MagnetBlock(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], game.magnetPulse, blockHeight, blockWidth));
                        break;
                    case Game.BlockType.Path:
                        if (!clickOne)
                        {
                            point1 = origin;
                            clickOne = true;
                        }
                        else
                        {
                            clickOne = false;
                            game.addEntity(new MovingPath(game, game.Arena, point1, 1, blockArray[game.spriteBlockCounter], game.pathSpeed, point1 + Constants.player1SpawnLocation, origin + Constants.player1SpawnLocation, false));
                        }
                        break;
                    case Game.BlockType.Cycle:

                        if (pathPoints.Count > 1)
                        {
                            pathPoints.Add(origin+Constants.player1SpawnLocation);
                            game.addEntity(new MovingCycle(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], game.pathSpeed, pathPoints, true));
                            pathPoints = new ArrayList();
                        }
                        break;
                    case Game.BlockType.WaterBlock:
                       game.addEntity(new WaterBlock(game, game.Arena, origin, 1, blockArray[game.spriteBlockCounter], rotation,blockWidth,blockHeight));
                        break;
                }
            }
            
        }

        public void addBlock()
        {

            game.addEntity(new Block(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter], blockHeight, blockWidth,game.drawLevel,rotation));
        }

        public void addDeathBlock()
        {

            game.addEntity(new DeathBlock(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter],rotation));
        }
        public void addMagnetBlock()
        {

            game.addEntity(new MagnetBlock(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter], game.magnetPulse, blockHeight,blockWidth));
        }

        public void addGoalBlock()
        {

            game.addEntity(new GoalBlock(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter],game.currentLevel));
        }

        public void addMovingDeathBlock()
        {
            game.addEntity(new MovingDeath(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter], new Vector2(1,0),2 ));
        
        }

        public void addPathBlock()
        {

            game.addEntity(new MovingPath(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter], 2, point1, point2, false));
        }
        public void addWaterBlock()
        {
            game.addEntity(new WaterBlock(game, game.Arena, origPos, 1, blockArray[game.spriteBlockCounter], rotation,blockWidth,blockHeight));
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            //game.toBeAdded.Clear();
            if (!game.inDeleteMode)
            {
                //spriteBatch.Draw(playerSprite.index, new Rectangle((int)pos.X, (int)pos.Y, (int)blockWidth, (int)blockHeight), null, Color.White, 0, origin, SpriteEffects.None, 0f);
                //if (mouseInSelectMode)
                //{
                if (currentPointValue == 0 && currentSquareValue == 0 && keyInput.keyboardState.IsKeyUp(Keys.LeftShift) || keyInput.keyboardState.IsKeyDown(Keys.D3))
                {

                    if (ani.getStateCount() == 1)
                    {
                        spriteBatch.Draw(playerSprite.index, new Rectangle((int)pos.X, (int)pos.Y, (int)blockWidth, (int)blockHeight), null, Color.White, rotation * (float)Math.PI / 180f, origin, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        ani.drawCurrentState(spriteBatch, this, new Vector2((int)pos.X - blockWidth, (int)pos.Y),
                               origin, null, new Rectangle((int)pos.X,
                                   (int)pos.Y, (int)blockWidth, (int)blockHeight), true, new Vector2(0, 0));
                    }
                }
                
                      foreach (Entity e in linedUpBlocks)
                        {
                            e.Draw(gameTime, spriteBatch);
                        }
                
                    
                    //other++;
                //}
                 
            }
            if (clickOne)
            {

                Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y - 500 * game.drawingTool.cam.Zoom);
                Vector2 worldMousePosition = Vector2.Transform(mousePosition, Matrix.Invert(game.drawingTool.cam._transform));

                //game.drawingTool.DrawLine(spriteBatch, 5, Color.Yellow, point1+Constants.player1SpawnLocation, worldMousePosition+Constants.player1SpawnLocation);

            }
        }

        public void DrawText(SpriteBatch spriteBatch, float x, float y, String text)
        {
            

            String[] tempstrMulti = text.Split("|".ToCharArray());
            SpriteFont font = game.fonts[(int)Game.Fonts.FT_HEADER];
            PlayableCharacter p1 = game.Arena.player1;
            Color colo = Color.Green;
            if (!(game.Arena.maxLeft > p1.Position.X || game.Arena.maxRight < p1.Position.X))
            {
                colo = Color.Red;
            }
            if (!(game.Arena.maxTop > p1.Position.Y || game.Arena.maxButtom < p1.Position.Y))
            {
                colo = Color.Red;
            }

            tempstrMulti = text.Split("|".ToCharArray());
            for (int i = 0; i < tempstrMulti.Length; i += 1)
                spriteBatch.DrawString(font, tempstrMulti[i],
                    new Vector2(ConvertUnits.ToSimUnits(x), ConvertUnits.ToSimUnits(y) + ConvertUnits.ToSimUnits(font.MeasureString("A").Y * i)),
                    colo,
                    0f,
                    Vector2.Zero,
                    //new Vector2(font.MeasureString(tempstrMulti[i]).X / 2, 0), 
                    game.drawingTool.gameToScreen(1f) * 0.25f,
                    SpriteEffects.None,
                    0);

        }
    }
}
