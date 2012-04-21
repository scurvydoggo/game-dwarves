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
        /// <param name="x">The top left X position in world coordinates.</param>
        /// <param name="y">The top-left Y position in world coordinates.</param>
        /// <param name="scale">The ratio for scaling quad tree coordinates to world coordinates.</param>
        /// <param name="bitmap">The bitmap defining the terrain.</param>
        /// <returns>The terrain object.</returns>
        public Terrain CreateTerrain(float x, float y, float scale, Texture2D bitmap)
        {
            // Terrain data
            var data = new Color[bitmap.Width * bitmap.Height];
            bitmap.GetData<Color>(data);

            // TODO
            return null;
        }
    }
}