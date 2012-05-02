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
        /// <param name="maxJumpHeight">The maximum height of a jump.</param>
        /// <param name="maxJumpWidth">The maximum width of a jump.</param>
        /// <returns>The set of path nodes.</returns>
        public Dictionary<Point, LinkedPathNode> BuildPathNodes(
            ClipQuadTree<TerrainType> quadTree,
            int maxJumpHeight,
            int maxJumpWidth)
        {
            var pathNodes = new Dictionary<Point, LinkedPathNode>();

            // Populate all terrain nodes
            this.PopulateTerrainNodes(pathNodes, quadTree);

            // Populate all mid-air nodes
            this.PopulateMidAirNodes(pathNodes, quadTree, maxJumpHeight, maxJumpWidth);

            return pathNodes;
        }

        /// <summary>
        /// Populate the terrain nodes. These are any horizontal segments of terrain.
        /// </summary>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        private void PopulateTerrainNodes(Dictionary<Point, LinkedPathNode> pathNodes, ClipQuadTree<TerrainType> quadTree)
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
                        // GetData returns false if point is outside the quad tree, which we treat as a walkable point
                        // since the character would be walking 'on top' of the terrain
                        isWalkable = true;
                    }

                    // If this is a walkable point, add it to the set of path nodes
                    if (isWalkable)
                    {
                        // Create and add the path node
                        pathNodes.Add(pointAbove, new LinkedPathNode(pointAbove, PathNodeType.Normal));
                    }
                }
            }

            // Connect any adjacent nodes horizontally or diagonally
            foreach (LinkedPathNode node in pathNodes.Values)
            {
                LinkedPathNode rightUp;
                if (pathNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y - 1), out rightUp))
                {
                    if (!rightUp.AdjacentNodes.Contains(node))
                    {
                        rightUp.AdjacentNodes.Add(node);
                    }

                    if (!node.AdjacentNodes.Contains(rightUp))
                    {
                        node.AdjacentNodes.Add(rightUp);
                    }
                }

                LinkedPathNode right;
                if (pathNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y), out right))
                {
                    if (!right.AdjacentNodes.Contains(node))
                    {
                        right.AdjacentNodes.Add(node);
                    }

                    if (!node.AdjacentNodes.Contains(right))
                    {
                        node.AdjacentNodes.Add(right);
                    }
                }

                LinkedPathNode rightDown;
                if (pathNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y + 1), out rightDown))
                {
                    if (!rightDown.AdjacentNodes.Contains(node))
                    {
                        rightDown.AdjacentNodes.Add(node);
                    }

                    if (!node.AdjacentNodes.Contains(rightDown))
                    {
                        node.AdjacentNodes.Add(rightDown);
                    }
                }
            }
        }

        /// <summary>
        /// Populate the nodes that join terrain segments. These are 'mid-air' nodes that are traversed by the
        /// character jumping, climbing and such.
        /// </summary>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <param name="maxJumpHeight">The maximum height of a jump.</param>
        /// <param name="maxJumpWidth">The maximum width of a jump.</param>
        private void PopulateMidAirNodes(
            Dictionary<Point, LinkedPathNode> pathNodes,
            ClipQuadTree<TerrainType> quadTree,
            int maxJumpHeight,
            int maxJumpWidth)
        {
            // Sanity check
            if (maxJumpHeight < 2)
            {
                return;
            }

            // Sanity check
            if (maxJumpWidth < 1)
            {
                return;
            }

            // For each node, check if it is an 'edge' node which means it has no adjacent nodes directly to the
            // left/right
            foreach (LinkedPathNode node in pathNodes.Values)
            {
                // Determine if this is an edge point
                bool isLeftEdge = true;
                bool isRightEdge = true;
                Point leftUp = new Point(node.Node.X - 1, node.Node.Y - 1);
                Point left = new Point(node.Node.X - 1, node.Node.Y);
                Point leftDown = new Point(node.Node.X - 1, node.Node.Y + 1);
                Point rightUp = new Point(node.Node.X + 1, node.Node.Y - 1);
                Point right = new Point(node.Node.X + 1, node.Node.Y);
                Point rightDown = new Point(node.Node.X + 1, node.Node.Y + 1);
                foreach (LinkedPathNode adjacent in node.AdjacentNodes)
                {
                    if (adjacent.Node.Equals(leftUp) || adjacent.Node.Equals(left) || adjacent.Node.Equals(leftDown))
                    {
                        isLeftEdge = false;
                    }

                    if (adjacent.Node.Equals(rightUp) || adjacent.Node.Equals(right) || adjacent.Node.Equals(rightDown))
                    {
                        isRightEdge = false;
                    }

                    if (!isLeftEdge && !isRightEdge)
                    {
                        break;
                    }
                }

                // Populate the left-edge jump nodes
                if (isLeftEdge)
                {
                    this.PopulateDownwardMidAirNodes(node, true, pathNodes, quadTree, maxJumpHeight, maxJumpWidth);
                }

                // Populate the right-edge jump nodes
                if (isRightEdge)
                {
                    this.PopulateDownwardMidAirNodes(node, false, pathNodes, quadTree, maxJumpHeight, maxJumpWidth);
                }
            }
        }

        /// <summary>
        /// Populate the nodes that join terrain segments that are accessible downwards from the given origin node.
        /// These are 'mid-air' nodes that are traversed by the character jumping, climbing and such.
        /// </summary>
        /// <param name="origin">The point being jumped down from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <param name="maxJumpHeight">The maximum height of a jump.</param>
        /// <param name="maxJumpWidth">The maximum width of a jump.</param>
        private void PopulateDownwardMidAirNodes(
            LinkedPathNode origin,
            bool left,
            Dictionary<Point, LinkedPathNode> pathNodes,
            ClipQuadTree<TerrainType> quadTree,
            int maxJumpHeight,
            int maxJumpWidth)
        {
            // Populate the nodes for a direct vertical 'pin drop' downwards
            this.PopulatePinDropNodes(origin, left, pathNodes, quadTree, maxJumpHeight, maxJumpWidth);

            // Populate the nodes for parabolic jumps
            ////this.PopulateParabolicDropNodes(origin, toLeft, pathNodes, quadTree, maxJumpHeight, maxJumpWidth);
        }

        /// <summary>
        /// Populate the nodes for a direct vertical 'pin drop' downwards from the given origin node.
        /// </summary>
        /// <param name="origin">The point being jumped down from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <param name="maxJumpHeight">The maximum height of a jump.</param>
        /// <param name="maxJumpWidth">The maximum width of a jump.</param>
        private void PopulatePinDropNodes(
            LinkedPathNode origin,
            bool left,
            Dictionary<Point, LinkedPathNode> pathNodes,
            ClipQuadTree<TerrainType> quadTree,
            int maxJumpHeight,
            int maxJumpWidth)
        {
            var nodes = new List<LinkedPathNode>();

            // Set the x coordinate of the pin drop
            int x = left ? origin.Node.X - 1 : origin.Node.X + 1;

            // Add the first 2 points as they have already been tested
            nodes.Add(new LinkedPathNode(x, origin.Node.Y, PathNodeType.Jump));
            nodes.Add(new LinkedPathNode(x, origin.Node.Y + 1, PathNodeType.Jump));
            nodes[0].AdjacentNodes.Add(nodes[1]);
            nodes[1].AdjacentNodes.Add(nodes[0]);

            // Test the remaining points until ground is hit or max jump height is reached
            LinkedPathNode groundBelow = null;
            LinkedPathNode prevNode = nodes[1];
            for (int y = origin.Node.Y + 2; y <= origin.Node.Y + maxJumpHeight; y++)
            {
                Point point = new Point(x, y);

                // Check if this point is a terrain block. In which case the path is complete, otherwise keep iterating
                LinkedPathNode node;
                if (pathNodes.TryGetValue(point, out node))
                {
                    // This point is a terrain platform
                    groundBelow = node;
                    break;
                }
                else
                {
                    // The point is mid-air, so create the node and connect it to the previous node
                    node = new LinkedPathNode(point, PathNodeType.Jump);
                    prevNode.AdjacentNodes.Add(node);
                    node.AdjacentNodes.Add(prevNode);

                    // Add the node to the list
                    nodes.Add(node);

                    // Set the previous node
                    prevNode = node;
                }
            }

            // If this path is complete, join this jump-segment to the dictionary nodes
            if (groundBelow != null)
            {
                // Connect the first jump node to the origin
                origin.AdjacentNodes.Add(nodes[0]);
                nodes[0].AdjacentNodes.Add(origin);

                // Connect the last jump node to the ground
                groundBelow.AdjacentNodes.Add(nodes[nodes.Count - 1]);
                nodes[nodes.Count - 1].AdjacentNodes.Add(groundBelow);
            }
        }

        /// <summary>
        /// Populate the nodes for a direct vertical 'pin drop' downwards from the given origin node.
        /// </summary>
        /// <param name="origin">The point being jumped down from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="pathNodes">The set of path nodes.</param>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <param name="maxJumpHeight">The maximum height of a jump.</param>
        /// <param name="maxJumpWidth">The maximum width of a jump.</param>
        private void PopulateParabolicDropNodes(
            LinkedPathNode origin,
            bool left,
            Dictionary<Point, LinkedPathNode> pathNodes,
            ClipQuadTree<TerrainType> quadTree,
            int maxJumpHeight,
            int maxJumpWidth)
        {
            var nodes = new List<LinkedPathNode>();

            LinkedPathNode groundBelow = null;
            LinkedPathNode prevNode = null;
            int peakJumpY = int.MaxValue;
            for (int deltaX = 1; deltaX <= maxJumpWidth; deltaX++)
            {
                // Get the x point for this step
                int x = left ? origin.Node.X - deltaX : origin.Node.X + deltaX;

                // Calculate the y point for this step
                int y = origin.Node.Y + (deltaX * deltaX);
                if (y > maxJumpHeight)
                {
                    y = maxJumpHeight;
                }

                if (y < peakJumpY)
                {
                    peakJumpY = y;
                }

                // Compare the y point with the previous point as interpolation may be required
                PathNode prevPoint = prevNode == null ? origin.Node : prevNode.Node;
                bool up = prevPoint.Y > y;

                // Set the start y position
                int interpolateY;
                if (prevPoint.Y == y)
                {
                    interpolateY = prevPoint.Y;
                }
                else if (up)
                {
                    interpolateY = prevPoint.Y - 1;
                }
                else
                {
                    interpolateY = prevPoint.Y + 1;
                }

                while ((up && interpolateY >= y) || (!up && interpolateY <= y))
                {
                    Point point = new Point(x, interpolateY);

                    // Check if this point is a terrain block. In which case the path is complete, otherwise keep iterating
                    LinkedPathNode node;
                    if (pathNodes.TryGetValue(point, out node))
                    {
                        // This point is a terrain platform
                        groundBelow = node;
                        break;
                    }
                    else
                    {
                        // The point is mid-air, so create the node and connect it to the previous node
                        node = new LinkedPathNode(point, PathNodeType.Jump);
                        if (prevNode != null)
                        {
                            prevNode.AdjacentNodes.Add(node);
                            node.AdjacentNodes.Add(prevNode);
                        }

                        // Add the node to the list
                        nodes.Add(node);

                        // Set the previous node
                        prevNode = node;
                    }

                    // Increment/decrement y
                    if (up)
                    {
                        interpolateY--;
                    }
                    else
                    {
                        interpolateY++;
                    }
                }

                // Stop iterating if the ground has been hit or max jump height reached
                if (groundBelow != null || y == maxJumpHeight)
                {
                    break;
                }
            }

            // If this path is complete, join this jump-segment to the dictionary nodes
            if (groundBelow != null)
            {
                // Connect the first jump node to the origin
                origin.AdjacentNodes.Add(nodes[0]);
                nodes[0].AdjacentNodes.Add(origin);

                // Connect the last jump node to the ground
                groundBelow.AdjacentNodes.Add(nodes[nodes.Count - 1]);
                nodes[nodes.Count - 1].AdjacentNodes.Add(groundBelow);
            }
        }
    }
}