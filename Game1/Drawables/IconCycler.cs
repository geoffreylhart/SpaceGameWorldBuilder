using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Drawables
{
    class IconCycler
    {
        private Texture2D[] icons;
        private int level;
        private string p1;
        private string p2;
        private int p3;
        private ContentManager Content;
        private int p;
        private IEnumerable<string> enumerable;

        public IconCycler(IEnumerable<Texture2D> icons, int level)
        {
            icons = icons.ToArray();
            this.level = level;
        }

        public IconCycler(ContentManager content, int level, params String[] iconNames)
        {
            icons = iconNames.Select(x => content.Load<Texture2D>(x)).ToArray();
            this.level = level;
        }

        public IconCycler(ContentManager content, int level, IEnumerable<String> iconNames)
            : this(content, level, iconNames.ToArray())
        {
        }

        internal void Draw(GraphicsDevice GraphicsDevice, int selected)
        {
            int size = Math.Min(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            int iconSize = size / 20;
            int iconSpacing = size / 100;
            BasicEffect iconEffect = new BasicEffect(GraphicsDevice);
            iconEffect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            iconEffect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            iconEffect.World = Matrix.Identity;
            iconEffect.TextureEnabled = true;
            iconEffect.VertexColorEnabled = true;
            using (var batch = new SpriteBatch(GraphicsDevice))
            {
                for (int i = 0; i < icons.Length; i++)
                {
                    batch.Begin(0, null, null, null, null, iconEffect);
                    Rectangle rect = new Rectangle(iconSpacing + (iconSpacing + iconSize) * i, iconSpacing + (iconSpacing + iconSize) * level, iconSize, iconSize);
                    batch.Draw(icons[i], rect, (i == selected) ? new Color(255, 160, 160) : Color.White);
                    batch.End();
                }
            }
        }
    }
}
