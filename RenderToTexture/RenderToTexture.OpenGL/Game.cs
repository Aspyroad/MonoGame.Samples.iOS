using System;
//using System.Collections.Generic;
//using System.Linq;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;

namespace RenderToTexture
{
    public enum Mode { Object, Camera };
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1024f / 768f, 0.1f, 100f);
        float angle = 0;
        float distance = 10;
        float topbottom = 0;
        Vector3 viewVector;
        float objectAngle = 0;

        // Create a new render target
        RenderTarget2D renderTarget;

        Model model;
        Mode currentMode = Mode.Camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            base.Initialize();

            //renderTarget = new RenderTarget2D(
                            //GraphicsDevice,
                            //GraphicsDevice.PresentationParameters.BackBufferWidth,
                            //GraphicsDevice.PresentationParameters.BackBufferHeight,
                            //false,
                            //GraphicsDevice.PresentationParameters.BackBufferFormat,
                            //DepthFormat.Depth24);

            renderTarget = new RenderTarget2D(
                            GraphicsDevice,
                            1024,
                            768,
                            false,
                            GraphicsDevice.PresentationParameters.BackBufferFormat,
                            DepthFormat.Depth24);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //model = Content.Load<Model>("Models/Helicopter");

            model = Content.Load<Model>("Horse_full");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.C))
            {
                currentMode = Mode.Object;
            }
            if (keyboardState.IsKeyDown(Keys.V))
            {
                currentMode = Mode.Camera;
            }

            if (currentMode == Mode.Camera)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    distance -= 0.09f;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    distance += 0.09f;
                }
            }
            if (currentMode == Mode.Object)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    objectAngle -= 0.09f;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    objectAngle += 0.09f;
                }

                world = Matrix.CreateRotationY(objectAngle);
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                topbottom -= 0.06f;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                topbottom += 0.06f;
            }

            Vector3 cameraLocation = distance * new Vector3((float)Math.Sin(angle), topbottom, (float)Math.Cos(angle));
            Vector3 cameraTarget = new Vector3(0, 1, 0);
            viewVector = Vector3.Transform(cameraTarget - cameraLocation, Matrix.CreateRotationY(0));
            viewVector.Normalize();
            view = Matrix.CreateLookAt(cameraLocation, cameraTarget, new Vector3(0, 1, 0));

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the entire scene in the given render target.
        /// </summary>
        /// <returns>A texture2D with the scene drawn in it.</returns>
        protected void DrawSceneToTexture(RenderTarget2D renderTarget)
        {
            // Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawModel(model, world, view, projection);

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);
        }


        protected override void Draw(GameTime gameTime)
        {
            DrawSceneToTexture(renderTarget);

            GraphicsDevice.Clear(Color.Pink);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);

            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 1024, 768), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)part.Effect;

                    effect.EmissiveColor = new Vector3(1,2,4);
                    effect.PreferPerPixelLighting = true;
                    effect.World = mesh.ParentBone.Transform * world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
