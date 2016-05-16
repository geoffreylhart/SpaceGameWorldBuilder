using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Geom
{
    class Point2D
    {
        public Vector3 pos;
        private Vector2 pos2d;

        public Point2D(Vector3 pos)
        {
            this.pos = pos;
            this.pos2d = new Vector2(pos.X, pos.Y);
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect, Color color)
        {
            Vector3 relToScreen = graphicsDevice.Viewport.Project(pos, basicEffect.Projection, basicEffect.View, basicEffect.World);
            BasicEffect screenSpace = new BasicEffect(graphicsDevice);
            screenSpace.TextureEnabled = true;
            screenSpace.VertexColorEnabled = true;
            screenSpace.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            screenSpace.Projection = Matrix.CreateOrthographicOffCenter(0, (float)graphicsDevice.Viewport.Width, (float)graphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            screenSpace.World = Matrix.Identity;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, screenSpace);
                batch.Draw(GlobalTextures.pixelTexture, new Vector2(relToScreen.X-5, relToScreen.Y-5), null, null, null, 0,new Vector2(2,2), color);
                batch.End();
            }
        }
    }
}
