// ----------------------------------------------------------------------------
// <copyright file="DwarvesGame.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DwarvesGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// The game world.
        /// </summary>
        private DwarvesWorld gameWorld;

        /// <summary>
        /// The graphics device manager.
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Initializes a new instance of the DwarvesGame class.
        /// </summary>
        public DwarvesGame()
        {
            try
            {
                this.graphics = new GraphicsDeviceManager(this);
                this.graphics.PreferMultiSampling = true;
                this.graphics.SynchronizeWithVerticalRetrace = false;

                this.Content.RootDirectory = "Content";

                this.IsMouseVisible = true;

                this.IsFixedTimeStep = true;
            }
            catch (Exception ex)
            {
                // TODO: Log exception. Throw exception until this is implemented.
                throw ex;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run. This is where it can
        /// query for any required services and load any non-graphic related content. Calling base.Initialize will
        /// enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                // Allow window resizing
                this.Window.AllowUserResizing = true;
                this.Window.ClientSizeChanged += this.WindowClientSizeChanged;

                this.gameWorld = new DwarvesWorld(this.GraphicsDevice, this.Content);
            }
            catch (Exception ex)
            {
                // TODO: Log exception. Throw exception until this is implemented.
                throw ex;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world, hecking for collisions, gathering input, and
        /// playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            try
            {
                int delta = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Step the world
                this.gameWorld.Step(delta);

                base.Update(gameTime);
            }
            catch (Exception ex)
            {
                // TODO: Log exception. Throw exception until this is implemented.
                throw ex;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            try
            {
                this.GraphicsDevice.Clear(Color.White);

                int delta = (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Draw the world
                this.gameWorld.Draw(delta);

                base.Draw(gameTime);
            }
            catch (Exception ex)
            {
                // TODO: Log exception. Throw exception until this is implemented.
                throw ex;
            }
        }

        /// <summary>
        /// Handle the window resize event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void WindowClientSizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Window.ClientBounds.Width > 0 && this.Window.ClientBounds.Height > 0)
                {
                    this.graphics.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
                    this.graphics.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
                }
            }
            catch (Exception ex)
            {
                // TODO: Log exception. Throw exception until this is implemented.
                throw ex;
            }
        }
    }
}