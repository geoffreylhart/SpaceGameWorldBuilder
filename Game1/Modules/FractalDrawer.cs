using Game1.Geom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    class FractalDrawer : IModule
    {
        List<Line> lines = new List<Line>();
        HashSet<Point2D> points = new HashSet<Point2D>();
        Point2D creating = null;
        Point2D lastMousePos = null;

        internal override string GetIconName()
        {
            return "icons/geom";
        }

        internal override void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            base.Initialize(content, graphicsDevice);
        }

        internal override void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            basicEffect.VertexColorEnabled = true;
            foreach(Line line in lines){
                line.Draw(graphicsDevice, basicEffect, Color.White);
            }
            foreach (Point2D point in points)
            {
                point.Draw(graphicsDevice, basicEffect, (point == lastMousePos)?Color.Red:Color.White);
            }
            if (creating != null) creating.Draw(graphicsDevice, basicEffect, Color.Red);
            if (creating != null && lastMousePos != null)
            {
                new Line(creating, lastMousePos).Draw(graphicsDevice, basicEffect, Color.Red);
            }
            basicEffect.VertexColorEnabled = false;
        }

        private static ButtonState prevLeftState;
        internal override void Update(Vector3 relMousePos, double scale)
        {
            MouseState mouseState = Mouse.GetState();
            if (prevLeftState.Equals(ButtonState.Pressed) && mouseState.LeftButton.Equals(ButtonState.Released))
            {
                if (creating == null)
                {
                    creating = lastMousePos;
                }
                else
                {
                    points.Add(creating);
                    points.Add(lastMousePos);
                    lines.Add(new Line(creating, lastMousePos));
                    creating = null;
                }
            }
            prevLeftState = mouseState.LeftButton;
            lastMousePos = new Point2D(relMousePos);
            double minDist = double.MaxValue;
            foreach (Point2D point in points)
            {
                double dist = Vector3.Distance(lastMousePos.pos, point.pos) * scale;
                if (dist < 0.03 && dist < minDist)
                {
                    minDist = dist;
                    lastMousePos = point;
                }
            }
        }
    }
}
