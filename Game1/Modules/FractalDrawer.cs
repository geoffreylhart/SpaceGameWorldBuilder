using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    class FractalDrawer : IModule
    {
        internal override string GetIconName()
        {
            return "icons/geom";
        }

        internal override void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            base.Initialize(content, graphicsDevice);
        }

        internal override void Draw(GraphicsDevice GraphicsDevice, BasicEffect basicEffect)
        {
        }

        internal override void Update(Vector3 relMousePos, double scale)
        {
        }
    }
}
