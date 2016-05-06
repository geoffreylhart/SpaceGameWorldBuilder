using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<ImageWrapper> imageWrappers = new List<ImageWrapper>();
        ImageWrapper mainMapWrapper;
        float cameraSpeed;
        Vector3 cameraOffset, cameraVelocity;
        double scale = 1;

        Matrix viewMatrix, projectionMatrix, worldMatrix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            int size = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height*2/3;
            // change size
            graphics.PreferredBackBufferWidth = size;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = size;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            // add basic world background
            mainMapWrapper = new ImageWrapper(GraphicsDevice, @"..\..\..\earth\global.png", 0, 0, size, size);
            imageWrappers.Add(mainMapWrapper);

            // camera settings
            cameraOffset = Vector3.Zero;
            cameraVelocity = Vector3.Zero;
            cameraSpeed = 10f;

            // make cursor show
            this.IsMouseVisible = true;

            // initialize view
            viewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, (float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
            worldMatrix = Matrix.Identity;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        private static int prevScrollWheelValue;
        private static ButtonState prevLeftState;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            KeyboardState keyState = Keyboard.GetState();
            cameraVelocity = Vector3.Zero;
            if (keyState.IsKeyDown(Keys.Up))
            {
                cameraVelocity += Vector3.UnitY * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                cameraVelocity += Vector3.UnitX * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                cameraVelocity -= Vector3.UnitY * cameraSpeed;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                cameraVelocity -= Vector3.UnitX * cameraSpeed;
            }
            cameraOffset += Vector3.Multiply(cameraVelocity, (float)(1/scale));
            MouseState mouseState = Mouse.GetState();
            double multiplier = Math.Pow(1.001, mouseState.ScrollWheelValue - prevScrollWheelValue);
            prevScrollWheelValue = mouseState.ScrollWheelValue;
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)scale);
            scale *= multiplier;


            Vector3 pivotPoint = new Vector3(mouseState.X, mouseState.Y, 0);
            Vector3 diff = GraphicsDevice.Viewport.Unproject(pivotPoint, projectionMatrix, viewMatrix, worldMatrix);
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)scale);
            diff -= GraphicsDevice.Viewport.Unproject(pivotPoint, projectionMatrix, viewMatrix, worldMatrix);
            cameraOffset -= diff;
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)scale); // redo this step after the change

            if(mouseState.LeftButton == ButtonState.Pressed && prevLeftState == ButtonState.Released)
            {
                Vector3 imageCoord = GraphicsDevice.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 0), projectionMatrix, viewMatrix, worldMatrix); // position according to image coordinates
                double longitude = (imageCoord.X/mainMapWrapper.Width - 0.5)* 360;
                double latitude = ToLat(imageCoord.Y / mainMapWrapper.Height)*-180/Math.PI;
                int zoom = (int)Math.Max(Math.Floor(Math.Log(scale)/Math.Log(2)+2),0);
                String fileName = @"..\..\..\earth\"+longitude+"+"+latitude+"+"+zoom+".png";
                GoogleMaps.DownloadMap(latitude, longitude, zoom, fileName);
                float newWidth = (float)(mainMapWrapper.Width/Math.Pow(2,zoom));
                float newHeight = (float)(mainMapWrapper.Height/Math.Pow(2,zoom));
                imageWrappers.Add(new ImageWrapper(GraphicsDevice, fileName, imageCoord.X-newWidth/2, imageCoord.Y-newHeight/2, newWidth, newHeight));
            }
            prevLeftState = mouseState.LeftButton;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.World = worldMatrix;
            basicEffect.VertexColorEnabled = true;
            foreach (ImageWrapper image in imageWrappers)
            {
                image.Draw(GraphicsDevice, basicEffect);
            }
            base.Draw(gameTime);
        }

        private static double ToLat(double y)
        {
            return 2 * Math.Atan(Math.Pow(Math.E, (y - 0.5) * 2 * Math.PI)) - Math.PI / 2;
        }
    }
}
