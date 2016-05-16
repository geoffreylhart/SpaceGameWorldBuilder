using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Geom
{
    class Point2D
    {
        public Vector3 pos;

        public Point2D(Vector3 pos)
        {
            this.pos = pos;
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
