// ----------------------------------------------------------------------------
// <copyright file="PathBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using System;
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
        /// Initializes a new instance of the PathBuilder class.
        /// </summary>
        /// <param name="quadTree">The terrain quad tree.</param>
        /// <param name="maxJumpLength">The maximum length (in adjacent nodes) of a jump.</param>
        public PathBuilder(ClipQuadTree<TerrainType> quadTree, int maxJumpLength)
        {
            this.QuadTree = quadTree;
            this.MaxSpanLength = maxJumpLength;
        }

        /// <summary>
        /// Gets or sets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainType> QuadTree { get; set; }

        /// <summary>
        /// Gets or sets the maximum length (in adjacent nodes) of a span.
        /// </summary>
        public int MaxSpanLength { get; set; }

        #region Public Methods

        /// <summary>
        /// Populate the path nodes dictionary from the given terrain quad tree.
        /// </summary>
        /// <returns>The set of path nodes along the ground.</returns>
        public Dictionary<Point, LinkedPathNode> BuildPathNodes()
        {
            // Build the set of ground nodes
            var groundNodes = this.BuildGroundNodes();

            // Attach the nodes which span between ground platforms
            this.AttachSpanNodes(groundNodes);

            return groundNodes;
        }

        #endregion

        #region Build Flat-Terrain Nodes

        /// <summary>
        /// Build the set of ground nodes. These are any horizontal segments of terrain which can be walked on by a
        /// character.
        /// </summary>
        /// <returns>The set of path nodes along the ground.</returns>
        private Dictionary<Point, LinkedPathNode> BuildGroundNodes()
        {
            var groundNodes = new Dictionary<Point, LinkedPathNode>();

            // Add all ground nodes
            foreach (QuadTreeData<TerrainType> data in this.QuadTree)
            {
                // Ignore non-walkable terrain
                if (data.Data == TerrainType.None)
                {
                    continue;
                }

                // Test each point 1 pixel above the top of this quad
                for (int x = data.Bounds.X; x < data.Bounds.Right; x++)
                {
                    var pointAbove = new Point(x, data.Bounds.Y - 1);
                    if (this.IsPassableTerrain(pointAbove))
                    {
                        // The point above is a passable node so add it to the set
                        groundNodes.Add(pointAbove, new LinkedPathNode(pointAbove, PathNodeType.Normal));
                    }
                }
            }

            // Connect any adjacent nodes horizontally or diagonally
            foreach (LinkedPathNode node in groundNodes.Values)
            {
                LinkedPathNode rightUp;
                if (groundNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y - 1), out rightUp))
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
                if (groundNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y), out right))
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
                if (groundNodes.TryGetValue(new Point(node.Node.X + 1, node.Node.Y + 1), out rightDown))
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

            return groundNodes;
        }

        #endregion

        #region Build Mid-Air Nodes

        /// <summary>
        /// Find the nodes which span between ground nodes. These are 'mid-air' nodes that are traversed by the
        /// character jumping, climbing and such.
        /// </summary>
        /// <param name="groundNodes">The set of path nodes along the ground.</param>
        private void AttachSpanNodes(Dictionary<Point, LinkedPathNode> groundNodes)
        {
            // Sanity check. A span less than 2 is considered a normal step between adjacent nodes
            if (this.MaxSpanLength < 2)
            {
                return;
            }

            // Check if each node is an 'edge' node, in which case try to 'jump' over to another platform
            foreach (LinkedPathNode node in groundNodes.Values)
            {
                // Check if the points to the left of the node are passable
                if (this.IsPassableTerrain(new Point(node.Node.X - 1, node.Node.Y - 1)) &&
                    this.IsPassableTerrain(new Point(node.Node.X - 1, node.Node.Y)) &&
                    this.IsPassableTerrain(new Point(node.Node.X - 1, node.Node.Y + 1)))
                {
                    this.AttachSpanNodes(node, true, groundNodes);
                }

                // Check if the points to the right of the node are passable
                if (this.IsPassableTerrain(new Point(node.Node.X + 1, node.Node.Y - 1)) &&
                    this.IsPassableTerrain(new Point(node.Node.X + 1, node.Node.Y)) &&
                    this.IsPassableTerrain(new Point(node.Node.X + 1, node.Node.Y + 1)))
                {
                    this.AttachSpanNodes(node, false, groundNodes);
                }
            }
        }

        /// <summary>
        /// Attach nodes which span from the given node to another ground node.</summary>
        /// <param name="origin">The point being searched from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="groundNodes">The set of path nodes along the ground.</param>
        private void AttachSpanNodes(LinkedPathNode origin, bool left, Dictionary<Point, LinkedPathNode> groundNodes)
        {
            // Attach span for a direct vertical 'pin drop' downwards
            this.AttachPinDropJump(origin, left, groundNodes);

            // Attach spans for parabolic jumps
            this.AttachParabolicJump(origin, left, groundNodes, 0.5f, 5);
            this.AttachParabolicJump(origin, left, groundNodes, 1, 2);
            this.AttachParabolicJump(origin, left, groundNodes, 1.5f, 1);
        }

        /// <summary>
        /// Attach nodes which span from the given node as a vertical 'pin drop' downwards to another ground node.
        /// </summary>
        /// <param name="origin">The point being jumped down from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="groundNodes">The set of path nodes along the ground.</param>
        private void AttachPinDropJump(LinkedPathNode origin, bool left, Dictionary<Point, LinkedPathNode> groundNodes)
        {
            var spanNodes = new List<LinkedPathNode>();

            // Set the x coordinate of the pin drop
            int x = left ? origin.Node.X - 1 : origin.Node.X + 1;

            // Add the first 2 points as they have already been tested
            spanNodes.Add(new LinkedPathNode(x, origin.Node.Y, PathNodeType.Jump));
            spanNodes.Add(new LinkedPathNode(x, origin.Node.Y + 1, PathNodeType.Jump));
            spanNodes[0].AdjacentNodes.Add(spanNodes[1]);
            spanNodes[1].AdjacentNodes.Add(spanNodes[0]);

            // Test the remaining points until ground is hit or max span length is reached
            LinkedPathNode groundBelow = null;
            LinkedPathNode prevNode = spanNodes[1];
            for (int y = origin.Node.Y + 2; y <= origin.Node.Y + this.MaxSpanLength; y++)
            {
                var point = new Point(x, y);

                // Check that this point is passable
                if (!this.IsPassableTerrain(point))
                {
                    // This is a non-passable point so the path fails
                    break;
                }

                // If the point below this one is non-passable terrain then the ground has been hit
                if (!this.IsPassableTerrain(new Point(point.X, point.Y + 1)))
                {
                    // A ground node has been hit, so set this and break
                    groundBelow = groundNodes[point];
                    break;
                }

                // The point is mid-air, so create the node and connect it to the previous node
                var node = new LinkedPathNode(point, PathNodeType.Jump);
                prevNode.AdjacentNodes.Add(node);
                node.AdjacentNodes.Add(prevNode);

                // Add the node to the list
                spanNodes.Add(node);

                // Set the previous node
                prevNode = node;
            }

            // If the path is complete, join this span to the ground nodes
            if (groundBelow != null && spanNodes.Count > 0)
            {
                // Connect the first jump node to the origin
                origin.AdjacentNodes.Add(spanNodes[0]);
                spanNodes[0].AdjacentNodes.Add(origin);

                // Connect the last jump node to the ground
                groundBelow.AdjacentNodes.Add(spanNodes[spanNodes.Count - 1]);
                spanNodes[spanNodes.Count - 1].AdjacentNodes.Add(groundBelow);
            }
        }

        /// <summary>
        /// Attach nodes which span from the given node as a parabolic jump to another ground node.
        /// </summary>
        /// <param name="origin">The point being jumped down from.</param>
        /// <param name="left">Indicates whether the path is to the left of origin; False indicates to right.</param>
        /// <param name="groundNodes">The set of path nodes along the ground.</param>
        /// <param name="coefficient">The parabolic coefficient.</param>
        /// <param name="peakOffset">The distance to peak upwards before spanning downwards.</param>
        private void AttachParabolicJump(
            LinkedPathNode origin,
            bool left,
            Dictionary<Point, LinkedPathNode> groundNodes,
            float coefficient,
            uint peakOffset)
        {
            var spanNodes = new List<LinkedPathNode>();

            LinkedPathNode groundBelow = null;
            LinkedPathNode prevNode = null;
            int deltaX = 0;
            int jumpNodeCount = 0;
            bool terrainHit = false;
            while (groundBelow == null && jumpNodeCount < this.MaxSpanLength && !terrainHit)
            {
                // Increment/decrement the x delta
                deltaX = left ? deltaX - 1 : deltaX + 1;

                // Calculate the x and y point for this step
                int x = origin.Node.X + deltaX;
                double square = (coefficient * (left ? -1 : 1) * deltaX - Math.Sqrt(peakOffset));
                double y = origin.Node.Y - (-(square * square) + peakOffset);

                // Calculate how many points of interpolation are required and the direction
                PathNode prevPoint = prevNode == null ? origin.Node : prevNode.Node;
                int interpolateLength = (int)Math.Abs(prevPoint.Y - y);
                bool up = prevPoint.Y > y;

                int deltaY = 0;
                while (Math.Abs(deltaY) <= (interpolateLength))
                {
                    // Calculate the interpolated y value
                    deltaY = up ? deltaY - 1 : deltaY + 1;
                    int innerY = prevPoint.Y + deltaY;

                    var point = new Point(x, innerY);

                    // Check that this point is passable
                    if (!this.IsPassableTerrain(point))
                    {
                        // This is a non-passable point so the path fails
                        terrainHit = true;
                        break;
                    }

                    // If the point below this one is non-passable terrain then the ground has been hit
                    if (!this.IsPassableTerrain(new Point(point.X, point.Y + 1)))
                    {
                        // A ground node has been hit, so set this and break
                        groundBelow = groundNodes[point];
                        break;
                    }

                    // The point is mid-air, so create the node and connect it to the previous node
                    var node = new LinkedPathNode(point, PathNodeType.Jump);
                    if (prevNode != null)
                    {
                        prevNode.AdjacentNodes.Add(node);
                        node.AdjacentNodes.Add(prevNode);
                    }

                    // Add the node to the list
                    spanNodes.Add(node);

                    // Increment the jump nodes count and check whether the limit has been reached
                    if (++jumpNodeCount >= this.MaxSpanLength)
                    {
                        break;
                    }

                    // Set the previous node
                    prevNode = node;
                }
            }

            // If this path is complete, join this jump-segment to the dictionary nodes
            if (groundBelow != null && spanNodes.Count > 4)
            {
                // Connect the first jump node to the origin
                origin.AdjacentNodes.Add(spanNodes[0]);
                spanNodes[0].AdjacentNodes.Add(origin);

                // Connect the last jump node to the ground
                groundBelow.AdjacentNodes.Add(spanNodes[spanNodes.Count - 1]);
                spanNodes[spanNodes.Count - 1].AdjacentNodes.Add(groundBelow);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns a value indicating whether the given point is passable terrain.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>True if the point is passable terrain.</returns>
        private bool IsPassableTerrain(Point point)
        {
            TerrainType terrain;
            if (this.QuadTree.GetData(point, out terrain))
            {
                return terrain == TerrainType.None;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}