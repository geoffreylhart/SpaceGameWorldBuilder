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
                    creating = new Point2D(relMousePos);
                }
                else
                {
                    lines.Add(new Line(creating, new Point2D(relMousePos)));
                    creating = null;
                }
            }
            prevLeftState = mouseState.LeftButton;
            lastMousePos = new Point2D(relMousePos);
        }
    }
}
