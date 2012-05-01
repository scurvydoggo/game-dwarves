// ----------------------------------------------------------------------------
// <copyright file="AStarPathFinder.cs" company="Acidwashed Games">
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
        /// The ordered open list.
        /// </summary>
        private PriorityQueue<Node> openSetOrdered;

        /// <summary>
        /// The open list mapped by point coordinates.
        /// </summary>
        private Dictionary<Point, Node> openSet;

        /// <summary>
        /// The closed list mapped by point coordinates.
        /// </summary>
        private Dictionary<Point, Node> closedSet;

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
            this.openSetOrdered = new PriorityQueue<Node>(Comparer<Node>.Default);
            this.openSet = new Dictionary<Point, Node>();
            this.closedSet = new Dictionary<Point, Node>();
            this.goal = goal;

            // Check that the start and goal nodes are valid
            if (!IsOpenSpace(this.GetNodeRectangle(start.X, start.Y, nodeWidth, nodeHeight)) ||
                !IsOpenSpace(this.GetNodeRectangle(goal.X, goal.Y, nodeWidth, nodeHeight)))
            {
                path = new Point[0];
                return false;
            }

            // Add the start node to the open list
            this.AddOpenNode(new Node(start, this.CalculateH(start), 0));

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            // Search for the path to the goal node
            watch.Start();
            Node goalNode = this.FindGoalNode(nodeWidth, nodeHeight);
            watch.Stop();
            System.Console.Write("Test path took " + watch.ElapsedMilliseconds + "ms.");

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
        /// Find the goal node from the existing open set.
        /// </summary>
        /// <param name="nodeWidth">The width of each node along the path in terrain units.</param>
        /// <param name="nodeHeight">The height in of each node along the path in terrain units.</param>
        /// <returns>The goal node.</returns>
        private Node FindGoalNode(int nodeWidth, int nodeHeight)
        {
            Node goalNode = null;

            // Keep searching the open list
            while (this.openSetOrdered.Count > 0)
            {
                // Take the node from the open list with the lowest cost and move to the closed list
                Node current = this.openSetOrdered.Pop();
                this.openSet.Remove(current.Point);
                this.closedSet.Add(current.Point, current);

                // The goal node was just added to the closed list so the search is complete
                if (current.Point.Equals(this.goal))
                {
                    goalNode = current;
                    break;
                }

                // Add the adjacent Y-nodes to the open list
                for (int y = current.Point.Y - 1; y < current.Point.Y + 1; y++)
                {
                    // The G value for this node
                    int g = current.G + this.CalculateGIncrement(current.Point.Y - y);

                    // Add the left node (if it isn't already in the closed list or blocked)
                    Point left = new Point(current.Point.X - 1, y);
                    if (!this.closedSet.ContainsKey(left))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(left.X, y, nodeWidth, nodeHeight)))
                        {
                            this.AddOpenNode(new Node(left, this.CalculateH(left), g, current));
                        }
                    }

                    // Add the right node (if it isn't already in the closed list or blocked)
                    var right = new Point(current.Point.X + nodeWidth, y);
                    if (!this.closedSet.ContainsKey(right))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(right.X, y, nodeWidth, nodeHeight)))
                        {
                            this.AddOpenNode(new Node(right, this.CalculateH(right), g, current));
                        }
                    }
                }

                // Add the adjacent X-nodes to the open list
                for (int x = current.Point.X - 1; x < current.Point.X + 1; x++)
                {
                    // The G value for this node
                    int g = current.G + this.CalculateGIncrement(current.Point.X - x);

                    // Add the left node (if it isn't already in the closed list or blocked)
                    Point top = new Point(x, current.Point.Y - 1);
                    if (!this.closedSet.ContainsKey(top))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(x, top.Y, nodeWidth, nodeHeight)))
                        {
                            this.AddOpenNode(new Node(top, this.CalculateH(top), g, current));
                        }
                    }

                    // Add the right node (if it isn't already in the closed list or blocked)
                    var bottom = new Point(x, current.Point.Y + nodeHeight);
                    if (!this.closedSet.ContainsKey(bottom))
                    {
                        if (this.IsOpenSpace(this.GetNodeRectangle(x, bottom.Y, nodeWidth, nodeHeight)))
                        {
                            this.AddOpenNode(new Node(bottom, this.CalculateH(bottom), g, current));
                        }
                    }
                }
            }

            return goalNode;
        }

        /// <summary>
        /// Add the node to the open list; if the point already exists in the open list and this node has a better G
        /// cost, update the parent and G cost of the existing node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void AddOpenNode(Node node)
        {
            // Check if this node exists
            Node existing;
            if (this.openSet.TryGetValue(node.Point, out existing))
            {
                // If this node has a better cost than the existing node, update the existing node's parent and G score
                if (node.G < existing.G)
                {
                    existing.Parent = node;
                    existing.G = node.G;
                }
            }
            else
            {
                // If the node doesn't already exist, add it to the open list
                this.openSetOrdered.Push(node);
                this.openSet.Add(node.Point, node);
            }
        }

        /// <summary>
        /// Calculate the heuristic distance estimate from the given point to the end point.
        /// </summary>
        /// <param name="point">The point for which to calculate H.</param>
        /// <returns>The heuristic distance estimate from the given point to the end point.</returns>
        private int CalculateH(Point point)
        {
            // Multiply by 10 so as to use the same scale as G values
            return (Math.Abs(point.X - this.goal.X) + Math.Abs(point.Y - this.goal.Y)) * 10;
        }

        /// <summary>
        /// Calculate the amount to increment G for a single step with the given X or Y offset.
        /// </summary>
        /// <param name="offset">The distance that the node is offset from the origin by X or Y.</param>
        /// <returns>The amount to increment G.</returns>
        private int CalculateGIncrement(int offset)
        {
            // Multiply by 10 such that the first sqrt decimal isn't rounded off
            offset *= 10;
            return (int)Math.Sqrt(100 + (offset * offset));
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
        public class Node : IIndexedObject, IComparable<Node>
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

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Compare the F value of this node to another node.
            /// </summary>
            /// <param name="other">The other node.</param>
            /// <returns>The relative comparative value.</returns>
            public int CompareTo(Node other)
            {
                if (this.F < other.F)
                {
                    return -1;
                }
                else if (this.F > other.F)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}