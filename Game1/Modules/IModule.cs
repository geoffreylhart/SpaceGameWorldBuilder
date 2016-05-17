using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    abstract class IModule
    {
        internal abstract String GetIconName();

        private Texture2D iconTexture;

        internal virtual void Initialize(ContentManager content, GraphicsDevice graphicsDevice) { }

        internal abstract void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect);

        internal abstract void Update(Vector3 relMousePos, double scale);

        internal Texture2D GetIconTexture()
        {
            return iconTexture;
        }
    }
}
