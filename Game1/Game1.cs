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
            imageWrappers.Add(new ImageWrapper(GraphicsDevice, @"..\..\..\earth\global.png", 0, 0, size, size));

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
    }
}
