using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game1
{
    class ImageWrapper
    {
        private Texture2D texture;
        Rectangle rect;

        public ImageWrapper(GraphicsDevice graphicsDevice, string file, int x, int y, int w, int h)
        {
            rect = new Rectangle(x, y, w, h);
            using (var reader = File.OpenRead(file))
            {
                texture = Texture2D.FromStream(graphicsDevice, reader);
            }
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            basicEffect.TextureEnabled = true;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, basicEffect);
                batch.Draw(texture, rect, Color.White);
                batch.End();
            }

            basicEffect.TextureEnabled = false;
        }
    }
}
