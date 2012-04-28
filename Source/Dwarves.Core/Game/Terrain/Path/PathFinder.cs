// ----------------------------------------------------------------------------
// <copyright file="PathFinder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain.Path
{
    using Dwarves.Common;
    using Dwarves.Game.Terrain;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for determining paths between points in the game terrain.
    /// </summary>
    public abstract class PathFinder
    {
        /// <summary>
        /// Initializes a new instance of the PathFinder class.
        /// </summary>
        /// <param name="terrain">The terrain quad tree to be traversed.</param>
        public PathFinder(ClipQuadTree<TerrainType> terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets or sets the terrain quad tree to be traversed.
        /// </summary>
        public ClipQuadTree<TerrainType> Terrain { get; set; }

        /// <summary>
        /// Find the path between the two given points.
        /// </summary>
        /// <param name="start">The start point of the path to search from.</param>
        /// <param name="goal">The goal point of the path.</param>
        /// <returns>The path; Null if a path not be established.</returns>
        public abstract Point[] FindPath(Point start, Point goal);
    }
}
