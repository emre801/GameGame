using System;
using System.Diagnostics;
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
using FarseerPhysics.Dynamics.Contacts;


namespace ProtoDerp
{
    class CutSceneMovingItem: CutSceneItem
    {
        float xPosEnd, yPosEnd,xPosStart,yPosStart;
        bool direction = true;
        Vector2 goalPoint;
        
        public CutSceneMovingItem(Game g, Arena a, float xPos, float yPos, float xPosEnd,float yPosEnd,int playerNum, String spriteNumber, float startTime, float endTime)
            : base(g,a,xPos,yPos,playerNum,spriteNumber,startTime,endTime)
        {
            this.xPosEnd = game.drawingTool.ActualScreenPixelWidth*xPosEnd;
            this.yPosEnd = game.drawingTool.ActualScreenPixelHeight * yPosEnd;
            this.xPosStart = game.drawingTool.ActualScreenPixelWidth * xPos;
            this.yPosStart = yPos;
        }
        public CutSceneMovingItem(Game g, Arena a, float xPos, float yPos, float xPosEnd, float yPosEnd, int playerNum, String spriteNumber, float aValue, bool loadFromCT)
            : base(g,a,xPos,yPos,playerNum,spriteNumber,aValue,loadFromCT)
        {
            this.xPosEnd = game.drawingTool.ActualScreenPixelWidth * xPosEnd;
            this.yPosEnd = game.drawingTool.ActualScreenPixelHeight * yPosEnd;
            this.xPosStart = game.drawingTool.ActualScreenPixelWidth * xPos;
            this.yPosStart = game.drawingTool.ActualScreenPixelHeight * yPos;
        }
        public void movePosition()
        {
            Vector2 currentPosition = pos;

            double angel = Math.Atan2((double)(goalPoint.Y - currentPosition.Y), (double)(goalPoint.X - currentPosition.X)) * 180 / Math.PI;
            float x = (float)Math.Cos(angel * Math.PI / 180f);
            float y = (float)Math.Sin(angel * Math.PI / 180f);

            Vector2 dir = new Vector2(x, y);
            dir = goalPoint - currentPosition;
            dir.Normalize();
            pos += dir*0.25f;
        }
        public double distance()
        {
            return Math.Sqrt((goalPoint.X - pos.X) * (goalPoint.X - pos.X)
                + (goalPoint.Y - pos.Y) * (goalPoint.Y - pos.Y));

        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            base.Update(gameTime, worldSpeed);
           
            if (!direction)
            {
                goalPoint = new Vector2(xPosStart, yPosStart);
            }
            else
            {
                goalPoint = new Vector2(xPosEnd, yPosEnd);
            }
            if (distance() < 1)
            {
                direction = !direction;
            }
            movePosition();
            
        }
    }
}
