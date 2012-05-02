// ----------------------------------------------------------------------------
// <copyright file="PathBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using System.Collections.Generic;
    using Dwarves.Common;
    using Dwarves.Game.Terrain;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for creating movement paths.
    /// </summary>
    public class PathBuilder
    {
        /// <summary>
        /// Populate the path nodes dictionary from the given terrain quad tree.
        /// </summary>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <returns>The set of path nodes.</returns>
        public Dictionary<Point, PathNode> BuildPathNodes(ClipQuadTree<TerrainType> quadTree)
        {
            var pathNodes = new Dictionary<Point, PathNode>();

            // Populate all horizontal-terrain nodes
            this.PopulateFlatNodes(pathNodes, quadTree);

            // Connect diagonal adjacent nodes
            this.ConnectAdjacentNodes(pathNodes);

            return pathNodes;
        }

        /// <summary>
        /// Populate the path nodes set with all flat-terrain nodes. These are any horizontal segments of terrain.
        /// </summary>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        private void PopulateFlatNodes(Dictionary<Point, PathNode> pathNodes, ClipQuadTree<TerrainType> quadTree)
        {
            foreach (QuadTreeData<TerrainType> data in quadTree)
            {
                // Ignore non-walkable terrain
                if (data.Data == TerrainType.None)
                {
                    continue;
                }

                // Test each point 1 pixel above the top of this quad
                for (int x = data.Bounds.X; x < data.Bounds.Right; x++)
                {
                    // Get the next point above this one
                    var pointAbove = new Point(x, data.Bounds.Y - 1);

                    bool isWalkable = false;
                    TerrainType terrainAbove;
                    if (quadTree.GetData(pointAbove, out terrainAbove))
                    {
                        if (terrainAbove == TerrainType.None)
                        {
                            isWalkable = true;
                        }
                    }
                    else
                    {
                        // GetData returns false if point is outside the quad tree, which we treat as walkable points
                        isWalkable = true;
                    }

                    // If this is a walkable point, add it to the set of path nodes
                    if (isWalkable)
                    {
                        // Create and add the path node
                        pathNodes.Add(pointAbove, new PathNode(pointAbove, PathNodeType.Normal));
                    }
                }
            }
        }

        /// <summary>
        /// Connect nodes that are directly adjacent to one another.
        /// </summary>
        /// <param name="pathNodes">The set of path nodes.</param>
        private void ConnectAdjacentNodes(Dictionary<Point, PathNode> pathNodes)
        {
            // For each node, test the adjacent points top-right, right and bottom-right of its position
            foreach (PathNode node in pathNodes.Values)
            {
                PathNode topRight;
                if (pathNodes.TryGetValue(new Point(node.Point.X + 1, node.Point.Y + 1), out topRight))
                {
                    topRight.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(topRight);
                }

                PathNode right;
                if (pathNodes.TryGetValue(new Point(node.Point.X + 1, node.Point.Y), out right))
                {
                    right.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(right);
                }

                PathNode bottomRight;
                if (pathNodes.TryGetValue(new Point(node.Point.X + 1, node.Point.Y - 1), out bottomRight))
                {
                    bottomRight.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(bottomRight);
                }
            }
        }
    }
}