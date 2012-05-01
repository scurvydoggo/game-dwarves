// ----------------------------------------------------------------------------
// <copyright file="PathFinder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
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
        /// <param name="nodeWidth">The width of each node along the path in terrain units.</param>
        /// <param name="nodeHeight">The height in of each node along the path in terrain units.</param>
        /// <param name="path">The array of points in the path.</param>
        /// <returns>True if a path was established.</returns>
        public abstract bool FindPath(Point start, Point goal, int nodeWidth, int nodeHeight, out Point[] path);

        /// <summary>
        /// Determine whether the the given rectangle is free of obstructing terrain.
        /// </summary>
        /// <param name="rect">The rectangle to test.</param>
        /// <returns>True if the rectangle doesn't contain any terrain; False if the rectangle contains obstructing
        /// terrain or the rectangle is outside the bounds of the terrain quad tree.</returns>
        protected virtual bool IsOpenSpace(Rectangle rect)
        {
            QuadTreeData<TerrainType>[] data;
            if (this.Terrain.GetData(rect, out data))
            {
                foreach (QuadTreeData<TerrainType> terrainType in data)
                {
                    if (terrainType.Data != TerrainType.None)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}