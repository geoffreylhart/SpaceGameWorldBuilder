using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    class GlobalTextures
    {
        public static Texture2D pixelTexture;

        public static void LoadContent(ContentManager content)
        {
            pixelTexture = content.Load<Texture2D>("icons/point");
        }
    }
}
