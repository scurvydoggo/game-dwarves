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
            this.ConnectDiagonalAdjacentNodes(pathNodes);

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
                PathNode prevNode = null;
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
                        PathNode node = new PathNode(pointAbove, PathNodeType.Normal);
                        pathNodes.Add(pointAbove, node);

                        // Mark this node and the previous node as adjacent to one another
                        if (prevNode != null)
                        {
                            prevNode.AdjacentNodes.Add(node);
                            node.AdjacentNodes.Add(prevNode);
                        }

                        // Set the previous node reference
                        prevNode = node;
                    }
                    else
                    {
                        // Clear the previous node reference since this point represents an obstruction in the path
                        prevNode = null;
                    }
                }
            }
        }

        /// <summary>
        /// Connect nodes that are diagonally adjacent to one another.
        /// </summary>
        /// <param name="pathNodes">The set of path nodes.</param>
        private void ConnectDiagonalAdjacentNodes(Dictionary<Point, PathNode> pathNodes)
        {
            // For each node, test the adjacent points top-left and top-right of its position
            foreach (PathNode node in pathNodes.Values)
            {
                PathNode topLeft;
                if (pathNodes.TryGetValue(new Point(node.Point.Y + 1, node.Point.X - 1), out topLeft))
                {
                    // There is a node top-left of this one
                    topLeft.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(topLeft);
                }

                PathNode topRight;
                if (pathNodes.TryGetValue(new Point(node.Point.Y + 1, node.Point.X + 1), out topRight))
                {
                    // There is a node top-right of this one
                    topRight.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(topRight);
                }
            }
        }
    }
}