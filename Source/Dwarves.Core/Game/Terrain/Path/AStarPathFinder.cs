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
        private Dictionary<Point, Node> closed;

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
            this.closed = new Dictionary<Point, Node>();
            this.goal = goal;

            // Check that the start and goal nodes are valid
            if (!IsOpenSpace(this.GetNodeRectangle(start.X, start.Y, nodeWidth, nodeHeight)) ||
                !IsOpenSpace(this.GetNodeRectangle(goal.X, goal.Y, nodeWidth, nodeHeight)))
            {
                path = new Point[0];
                return false;
            }

            // Add the start point to the open list
            this.open.Add(new Node(start, this.CalculateH(start), 0));

            // Keep searching the open list
            Node goalNode = null;
            while (this.open.Count > 0)
            {
                // Take the node from the open list with the lowest cost
                Node current = this.open[0];
                for (int i = 1; i < this.open.Count; i++)
                {
                    if (this.open[i].F < current.F)
                    {
                        current = this.open[i];
                    }
                }

                // Move node to the closed list
                this.open.Remove(current);
                this.closed.Add(current.Point, current);

                // The goal node was just added to the closed list so the search is complete
                if (current.Point.Equals(goal))
                {
                    goalNode = current;
                    break;
                }

                // Add the adjacent nodes to the open list
                for (int y = current.Point.Y - 1; y < current.Point.Y + 1; y++)
                {
                    // The G value for this node
                    int g = current.G + this.CalculateGIncrement(current.Point.Y - y);

                    // Add the left node (if it isn't already in the closed list or blocked)
                    Point left = new Point(current.Point.X - 1, y);
                    if (!this.closed.ContainsKey(left))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(left.X, y, nodeWidth, nodeHeight)))
                        {
                            this.UpdateOpenNode(new Node(left, this.CalculateH(left), g, current));
                        }
                    }

                    // Add the right node (if it isn't already in the closed list or blocked)
                    var right = new Point(current.Point.X + nodeWidth, y);
                    if (!this.closed.ContainsKey(right))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(right.X, y, nodeWidth, nodeHeight)))
                        {
                            this.UpdateOpenNode(new Node(right, this.CalculateH(right), g, current));
                        }
                    }
                }
            }

            // If the goal node was found, iterate backwards through the parent nodes for the path
            if (goalNode != null)
            {
                var pathList = new List<Point>();

                Node current = goalNode;
                while (current != null)
                {
                    pathList.Add(current.Point);
                    current = current.Parent;
                }

                path = pathList.ToArray();
                return true;
            }
            else
            {
                path = new Point[0];
                return false;
            }
        }

        /// <summary>
        /// Add the node to the open list; if the point already exists in the open list and this node has a better G
        /// cost, update the parent and G cost of the existing node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void UpdateOpenNode(Node node)
        {
            // Check if this node exists
            bool exists = false;
            foreach (Node existing in this.open)
            {
                if (existing.Point.Equals(node.Point))
                {
                    if (node.G < existing.G)
                    {
                        existing.Parent = node;
                        existing.G = node.G;
                    }

                    exists = true;
                    break;
                }
            }

            // If the node doesn't already exist, add it to the open list
            if (!exists)
            {
                this.open.Add(node);
            }
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
        /// Calculate the amount to increment G for a single step with the given X or Y offset.
        /// </summary>
        /// <param name="offset">The distance that the node is offset from the origin by X or Y.</param>
        /// <returns>The amount to increment G.</returns>
        private int CalculateGIncrement(int offset)
        {
            return (int)Math.Sqrt(1 + (offset * offset));
        }

        /// <summary>
        /// Gets a rectangle for the given node with the node point at the middle of the bottom edge.
        /// </summary>
        /// <param name="x">The x position of the node.</param>
        /// <param name="y">The y position of the node.</param>
        /// <param name="width">The width of the node's bounds.</param>
        /// <param name="height">The height of the node's bounds.</param>
        /// <returns>The node rectangle.</returns>
        private Rectangle GetNodeRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(x - (width / 2), y - height, width, height);
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
            /// <param name="g">The cost from the starting node to the this node.</param>
            public Node(Point node, int h, int g)
                : this(node, h, g, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the Node class.
            /// </summary>
            /// <param name="node">The point for this node.</param>
            /// <param name="h">The heuristic estimate of the distance to the goal from this node.</param>
            /// <param name="g">The cost from the starting node to the this node.</param>
            /// <param name="parent">The parent node in the direction of the start node.</param>
            public Node(Point node, int h, int g, Node parent)
            {
                this.Point = node;
                this.H = h;
                this.G = g;
                this.Parent = parent;
            }

            /// <summary>
            /// Gets the point for this node.
            /// </summary>
            public Point Point { get; private set; }

            /// <summary>
            /// Gets or sets the parent node in the direction of the start node.
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