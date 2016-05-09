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
        private Texture2D texture = null;
        float x, y, w, h;
        private String file;

        public ImageWrapper(string file, float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.file = file;
        }

        internal void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            if(texture == null)
            {
                using (var reader = File.OpenRead(file))
                {
                    texture = Texture2D.FromStream(graphicsDevice, reader);
                }
            }
            basicEffect.TextureEnabled = true;
            using (var batch = new SpriteBatch(graphicsDevice))
            {
                batch.Begin(0, null, null, null, null, basicEffect);
                batch.Draw(texture, new Vector2(x, y), null, null, null, 0, new Vector2(w/texture.Width, h/texture.Height), Color.White);
                batch.End();
            }

            basicEffect.TextureEnabled = false;
        }

        public float Width { get { return w; } }
        public float Height { get { return h; } }
    }
}
