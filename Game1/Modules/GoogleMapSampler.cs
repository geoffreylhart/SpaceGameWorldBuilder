using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Game1.Modules
{
    class GoogleMapSampler : IModule
    {

        ImageWrapper mainMapWrapper;
        List<ImageWrapper> imageWrappers = new List<ImageWrapper>();

        internal override string GetIconName()
        {
            return "icons/gmap";
        }

        internal override void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            base.Initialize(content, graphicsDevice);

            // add basic world background
            mainMapWrapper = new ImageWrapper(@"..\..\..\earth\global.png", 0, 0, 1, 1);
            imageWrappers.Add(mainMapWrapper);

            // load previously saved bgs
            foreach (string file in Directory.EnumerateFiles(@"..\..\..\earth"))
            {
                if (!file.Contains("global"))
                {
                    String[] split = file.Split(new[] { '+' });
                    double longitude = (double.Parse(split[0].Substring(15)) / 360 + 0.5);
                    double latitude = (1 - ToY(double.Parse(split[1]) * Math.PI / 180));
                    int zoom = int.Parse(split[2].Substring(0, split[2].Length - 4));
                    float newWidth = (float)(mainMapWrapper.Width / Math.Pow(2, zoom));
                    float newHeight = (float)(mainMapWrapper.Height / Math.Pow(2, zoom));
                    imageWrappers.Add(new ImageWrapper(file, (float)longitude - newWidth / 2, (float)latitude - newHeight / 2, newWidth, newHeight));
                }
            }
            imageWrappers.Sort((x, y) => y.Width.CompareTo(x.Width));
        }

        private static double ToLat(double y)
        {
            return 2 * Math.Atan(Math.Pow(Math.E, (y - 0.5) * 2 * Math.PI)) - Math.PI / 2;
        }

        private static double ToY(double lat)
        {
            return Math.Log(Math.Tan(lat / 2 + Math.PI / 4)) / (Math.PI * 2) + 0.5;
        }

        internal override void Draw(GraphicsDevice GraphicsDevice, BasicEffect basicEffect)
        {
            foreach (ImageWrapper image in imageWrappers)
            {
                image.Draw(GraphicsDevice, basicEffect);
            }
        }

        private static ButtonState prevLeftState;
        internal override void Update(Vector3 relMousePos, double scale)
        {

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && prevLeftState == ButtonState.Released)
            {
                double longitude = (relMousePos.X / mainMapWrapper.Width - 0.5) * 360;
                double latitude = ToLat(relMousePos.Y / mainMapWrapper.Height) * -180 / Math.PI;
                int zoom = (int)Math.Max(Math.Floor(Math.Log(scale) / Math.Log(2) + 2), 0);
                String fileName = @"..\..\..\earth\" + longitude + "+" + latitude + "+" + zoom + ".png";
                GoogleMaps.DownloadMap(latitude, longitude, zoom, fileName);
                float newWidth = (float)(mainMapWrapper.Width / Math.Pow(2, zoom));
                float newHeight = (float)(mainMapWrapper.Height / Math.Pow(2, zoom));
                imageWrappers.Add(new ImageWrapper(fileName, relMousePos.X - newWidth / 2, relMousePos.Y - newHeight / 2, newWidth, newHeight));
            }
            prevLeftState = mouseState.LeftButton;
        }
    }
}
