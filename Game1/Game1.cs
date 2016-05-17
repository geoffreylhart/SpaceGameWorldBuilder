using Game1.Drawables;
using Game1.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game1
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        static FractalDrawer geometry = FractalDrawer.Load(@"..\..\..\Data\geometry.data");
        IModule[] modules = new IModule[] { new GoogleMapSampler(), geometry };
        IconCycler mainCycler;
        int moduleSelected = 1;
        float cameraSpeed;
        Vector3 cameraOffset, cameraVelocity;
        double scale = 1;
        int size;

        Matrix viewMatrix, projectionMatrix, worldMatrix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            GlobalTextures.LoadContent(Content);
            foreach (IModule module in modules)
            {
                module.Initialize(Content, GraphicsDevice);
            }
            mainCycler = new IconCycler(Content, 0, modules.Select(m => m.GetIconName()));
            size = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height*2/3;
            // change size
            graphics.PreferredBackBufferWidth = size;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = size;   // set this value to the desired height of your window
            graphics.ApplyChanges();

            // camera settings
            cameraOffset = Vector3.Zero;
            cameraVelocity = Vector3.Zero;
            cameraSpeed = 0.01f;

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
        private static KeyboardState prevKeyState;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                geometry.Save(@"..\..\..\Data\geometry.data");
                Exit();
            }


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
            cameraOffset += Vector3.Multiply(cameraVelocity, (float)(1 / (scale)));
            MouseState mouseState = Mouse.GetState();
            double multiplier = Math.Pow(1.001, mouseState.ScrollWheelValue - prevScrollWheelValue);
            prevScrollWheelValue = mouseState.ScrollWheelValue;
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)(scale*size));
            scale *= multiplier;


            Vector3 pivotPoint = new Vector3(mouseState.X, mouseState.Y, 0);
            Vector3 diff = GraphicsDevice.Viewport.Unproject(pivotPoint, projectionMatrix, viewMatrix, worldMatrix);
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)(scale * size));
            diff -= GraphicsDevice.Viewport.Unproject(pivotPoint, projectionMatrix, viewMatrix, worldMatrix);
            cameraOffset -= diff;
            worldMatrix = Matrix.CreateTranslation(cameraOffset) * Matrix.CreateScale((float)(scale * size)); // redo this step after the change

            Vector3 imageCoord = GraphicsDevice.Viewport.Unproject(new Vector3(mouseState.X, mouseState.Y, 0), projectionMatrix, viewMatrix, worldMatrix); // position according to image coordinates
            modules[moduleSelected].Update(imageCoord, scale);

            if (prevKeyState.IsKeyDown(Keys.Tab) && !keyState.IsKeyDown(Keys.Tab))
            {
                moduleSelected++;
                moduleSelected %= modules.Length;
            }
            prevKeyState = keyState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
            basicEffect.World = worldMatrix;
            foreach (IModule module in modules)
            {
                module.Draw(GraphicsDevice, basicEffect);
            }
            mainCycler.Draw(GraphicsDevice, moduleSelected);
            base.Draw(gameTime);
        }

        private static double ToLat(double y)
        {
            return 2 * Math.Atan(Math.Pow(Math.E, (y-0.5)*2*Math.PI)) - Math.PI / 2;
        }

        private static double ToY(double lat)
        {
            return Math.Log(Math.Tan(lat / 2 + Math.PI / 4)) / (Math.PI * 2) + 0.5;
        }
    }
}
