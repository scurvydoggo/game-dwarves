// ----------------------------------------------------------------------------
// <copyright file="AStarPathFinder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain.Path
{
    using Dwarves.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for determining paths between points in the game terrain using the A* algorithm.
    /// </summary>
    public class AStarPathFinder : PathFinder
    {
        /// <summary>
        /// Initializes a new instance of the AStarPathFinder class.
        /// </summary>
        /// <param name="terrain">The terrain quad tree to be traversed.</param>
        public AStarPathFinder(ClipQuadTree<TerrainType> terrain)
            : base(terrain)
        {
        }

        /// <summary>
        /// Find the path between the two given points.
        /// </summary>
        /// <param name="start">The start point of the path to search from.</param>
        /// <param name="goal">The goal point of the path.</param>
        /// <returns>The path; Null if a path not be established.</returns>
        public override Point[] FindPath(Point start, Point goal)
        {
            // TODO
            return new Point[0];
        }
    }
}
