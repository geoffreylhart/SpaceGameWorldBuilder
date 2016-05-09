using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    abstract class IModule
    {
        internal abstract String GetIconName();

        private Texture2D iconTexture;

        internal virtual void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            iconTexture = content.Load<Texture2D>(GetIconName());
        }

        internal abstract void Draw(GraphicsDevice GraphicsDevice, BasicEffect basicEffect);

        internal abstract void Update(Vector3 relMousePos, double scale);
    }
}
