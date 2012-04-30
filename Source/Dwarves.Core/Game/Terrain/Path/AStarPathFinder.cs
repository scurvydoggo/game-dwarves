// ----------------------------------------------------------------------------
// <copyright file="AStarPathFinder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain.Path
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Common;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for determining paths between points in the game terrain using the A* algorithm.
    /// </summary>
    public class AStarPathFinder : PathFinder
    {
        /// <summary>
        /// The cost for a normal up/down/left/right step.
        /// </summary>
        private const int SquareStepCost = 10;

        /// <summary>
        /// The cost for a normal diagonal step.
        /// </summary>
        private const int DiagonalStepCost = 14;

        /// <summary>
        /// The open list.
        /// </summary>
        private List<Node> open;

        /// <summary>
        /// The closed list.
        /// </summary>
        private List<Node> closed;

        /// <summary>
        /// The goal node.
        /// </summary>
        private Point goal;

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
        /// <param name="nodeWidth">The width of each node along the path in terrain units.</param>
        /// <param name="nodeHeight">The height in of each node along the path in terrain units.</param>
        /// <param name="path">The array of points.</param>
        /// <returns>True if a path was established.</returns>
        public override bool FindPath(Point start, Point goal, int nodeWidth, int nodeHeight, out Point[] path)
        {
            // Reset open/closed lists
            this.open = new List<Node>();
            this.closed = new List<Node>();
            this.goal = goal;

            // Check that the start and goal nodes are valid
            if (!IsOpenSpace(new Rectangle(start.X, start.Y, nodeWidth, nodeHeight)) ||
                !IsOpenSpace(new Rectangle(goal.X, goal.Y, nodeWidth, nodeHeight)))
            {
                path = new Point[0];
                return false;
            }

            // Add the start point to the open list
            this.open.Add(new Node(start, this.CalculateH(start)));

            // Keep searching the open list
            while (this.open.Count > 0)
            {
                // Take the node from the open list with the lowest cost
                Node currentNode = this.open[0];
                for (int i = 1; i < this.open.Count; i++)
                {
                    if (this.open[i].F < currentNode.F)
                    {
                        currentNode = this.open[i];
                    }
                }

            }

            path = new Point[0];
            return false;
        }

        /// <summary>
        /// Determine whether the the given rectangle is free of obstructing terrain.
        /// </summary>
        /// <param name="rect">The rectangle to test.</param>
        /// <returns>True if the rectangle doesn't contain any terrain; False if the rectangle contains obstructing
        /// terrain or the rectangle is outside the bounds of the terrain quad tree.</returns>
        private bool IsOpenSpace(Rectangle rect)
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

        /// <summary>
        /// Calculate the heuristic distance estimate from the given point to the end point.
        /// </summary>
        /// <param name="point">The point for which to calculate H.</param>
        /// <returns>The heuristic distance estimate from the given point to the end point.</returns>
        private int CalculateH(Point point)
        {
            return Math.Abs(point.X - this.goal.X) + Math.Abs(point.Y - this.goal.Y);
        }

        /// <summary>
        /// Represents a node in the A-star algorithm.
        /// </summary>
        public class Node
        {
            /// <summary>
            /// Initializes a new instance of the Node class.
            /// </summary>
            /// <param name="node">The point for this node.</param>
            /// <param name="h">The heuristic estimate of the distance to the goal from this node.</param>
            public Node(Point node, int h)
            {
                this.Point = node;
                this.H = h;
            }

            /// <summary>
            /// Gets the point for this node.
            /// </summary>
            public Point Point { get; private set; }

            /// <summary>
            /// Gets or sets the parent node towards the start.
            /// </summary>
            public Node Parent { get; set; }

            /// <summary>
            /// Gets the estimated total cost from the starting node to the goal through of the node.
            /// </summary>
            public int F
            {
                get
                {
                    return this.G + this.H;
                }
            }

            /// <summary>
            /// Gets the heuristic estimate of the distance to the goal from this node.
            /// </summary>
            public int H { get; private set; }

            /// <summary>
            /// Gets or sets the cost from the starting node to the this node.
            /// </summary>
            public int G { get; set; }
        }
    }
}