// ----------------------------------------------------------------------------
// <copyright file="Terrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using Dwarves.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents terrain in the game world.
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Initializes a new instance of the Terrain class.
        /// </summary>
        /// <param name="position">The top left position in world coordinates.</param>
        /// <param name="scale">The ratio for scaling quad tree coordinates to world coordinates.</param>
        /// <param name="bounds">The bounds of the terrain.</param>
        public Terrain(Vector2 position, float scale, Square bounds)
        {
            this.Position = position;
            this.Scale = scale;
            this.QuadTree = new ClipQuadTree<TerrainType>(bounds);
            this.QuadTree.Data = TerrainType.None;
        }

        /// <summary>
        /// Gets or sets the top left position of the terrain in world coordinates.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the scaling ratio from quad-tree points to world coordinates.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Gets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainType> QuadTree { get; private set; }
    }
}