// ----------------------------------------------------------------------------
// <copyright file="TerrainFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Creates terrain objects.
    /// </summary>
    public class TerrainFactory
    {
        /// <summary>
        /// Create a terrain object from the given terrain bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap defining the terrain.</param>
        /// <param name="scale">The scale ratio for the terrain image.</param>
        /// <returns>The terrain object.</returns>
        public Terrain CreateTerrain(Texture2D bitmap, float scale)
        {
            // Terrain data
            var data = new Color[bitmap.Width * bitmap.Height];
            bitmap.GetData<Color>(data);

            // TODO
            return null;
        }
    }
}