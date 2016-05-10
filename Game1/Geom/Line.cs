using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Geom
{
    class Line
    {
        private Point2D p1;
        private Point2D p2;

        public Line(Point2D p1, Point2D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect, Color color)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                VertexPositionColor[] points = new VertexPositionColor[] { new VertexPositionColor(p1.pos, color), new VertexPositionColor(p2.pos, color) };
                graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, points, 0, 1);
            }
        }
    }
}
