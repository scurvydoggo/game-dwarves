// ----------------------------------------------------------------------------
// <copyright file="Terrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game
{
    using Dwarves.Common;

    /// <summary>
    /// Represents terrain in the game world.
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Initializes a new instance of the Terrain class.
        /// </summary>
        /// <param name="bounds">The bounds of the terrain.</param>
        public Terrain(Square bounds)
        {
            this.QuadTree = new ClipQuadTree<TerrainType>(bounds);
            this.QuadTree.Data = TerrainType.None;
        }

        /// <summary>
        /// Gets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainType> QuadTree { get; private set; }
    }
}