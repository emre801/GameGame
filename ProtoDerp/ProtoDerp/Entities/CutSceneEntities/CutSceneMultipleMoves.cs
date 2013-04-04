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
    class CutSceneMultipleMoves : CutSceneItem
    {
        bool direction = true;
        Vector2 goalPoint;
        int currentAValue = 0;
        Vector2[] points;
        KeyboardInput ki = new KeyboardInput();
        XboxInput xi;
        public CutSceneMultipleMoves(Game g, Arena a, Vector2[] points, int playerNum, String spriteNumber, bool loadFromCT)
            : base(g, a, points[0].X, points[0].Y, playerNum, spriteNumber, -1, loadFromCT)
        {
            xi = (XboxInput)game.playerOneInput;
            this.points = points;
            goalPoint = new Vector2(points[0].X * game.drawingTool.ActualScreenPixelWidth,
                points[0].Y * game.drawingTool.ActualScreenPixelHeight);
            this.IsVisible = true;
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
            pos += dir * 1f;
        }
        public double distance()
        {
            return Math.Sqrt((goalPoint.X - pos.X) * (goalPoint.X - pos.X)
                + (goalPoint.Y - pos.Y) * (goalPoint.Y - pos.Y));

        }
        public override void Update(GameTime gameTime, float worldSpeed)
        {
            ani.Update();
            if (currentAValue >= points.Length)
                this.IsVisible = false;
            ki.Update(gameTime);
            double dist = distance();
            if (distance() < 1)
            {
                if (xi.isAPressed())
                {
                    currentAValue++;
                    if(currentAValue<points.Length)
                        goalPoint = new Vector2(points[currentAValue].X * (float)game.drawingTool.ActualScreenPixelWidth,
                            points[currentAValue].Y * (float)game.drawingTool.ActualScreenPixelHeight);
                }
            }
            else
            {
                movePosition();

            }
        }
    }
}
