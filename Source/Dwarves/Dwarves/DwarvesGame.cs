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
    public class DwarvesGame : Game
    {
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
    }
}
