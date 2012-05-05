// ----------------------------------------------------------------------------
// <copyright file="PathFinder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Common;
    using Dwarves.Component.Game;
    using Dwarves.Game.Terrain;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Responsible for determining paths between points in the game terrain using the A* algorithm.
    /// </summary>
    public class PathFinder
    {
        #region Private Variables

        /// <summary>
        /// The ordered open list.
        /// </summary>
        private PriorityQueue<AStarNode> openSetOrdered;

        /// <summary>
        /// The open list mapped by point coordinates.
        /// </summary>
        private Dictionary<LinkedPathNode, AStarNode> openSet;

        /// <summary>
        /// The closed list mapped by point coordinates.
        /// </summary>
        private Dictionary<LinkedPathNode, AStarNode> closedSet;

        /// <summary>
        /// The goal node.
        /// </summary>
        private LinkedPathNode goal;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PathFinder class.
        /// </summary>
        /// <param name="terrain">The terrain to be traversed.</param>
        public PathFinder(TerrainComponent terrain)
        {
            this.Terrain = terrain;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the terrain to be traversed.
        /// </summary>
        public TerrainComponent Terrain { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find the path between the two given points.
        /// </summary>
        /// <param name="start">The start point of the path to search from.</param>
        /// <param name="goal">The goal point of the path.</param>
        /// <param name="nodeWidth">The width of each node along the path in terrain units.</param>
        /// <param name="nodeHeight">The height in of each node along the path in terrain units.</param>
        /// <param name="path">The array of points.</param>
        /// <returns>True if a path was established.</returns>
        public bool FindPath(Point start, Point goal, int nodeWidth, int nodeHeight, out PathNode[] path)
        {
            // Reset open/closed lists
            this.openSetOrdered = new PriorityQueue<AStarNode>(Comparer<AStarNode>.Default);
            this.openSet = new Dictionary<LinkedPathNode, AStarNode>();
            this.closedSet = new Dictionary<LinkedPathNode, AStarNode>();

            // Get the path node for the start and goal points. Also test that the start and goal points have valid
            // rectangles
            LinkedPathNode startNode;
            if (!this.Terrain.PathNodes.TryGetValue(start, out startNode) ||
                !this.Terrain.PathNodes.TryGetValue(goal, out this.goal) ||
                !this.IsOpenSpace(this.GetNodeRectangle(start.X, start.Y, nodeWidth, nodeHeight)) ||
                !this.IsOpenSpace(this.GetNodeRectangle(goal.X, goal.Y, nodeWidth, nodeHeight)))
            {
                path = new PathNode[0];
                return false;
            }

            // Add the start node to the open list
            this.AddOpenNode(new AStarNode(startNode, this.CalculateH(startNode.Node), 0));

            // Search for the path to the goal node
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            AStarNode goalNode = this.FindGoalNode(nodeWidth, nodeHeight);
            watch.Stop();
            System.Console.Write("Test path took " + watch.ElapsedMilliseconds + "ms.");

            // If the goal node was found, iterate backwards through the parent nodes for the path
            if (goalNode != null)
            {
                var pathList = new List<PathNode>();

                AStarNode current = goalNode;
                while (current != null)
                {
                    pathList.Add(current.PathNode.Node);
                    current = current.Parent;
                }

                path = pathList.ToArray();
                return true;
            }
            else
            {
                path = new PathNode[0];
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Find the goal node from the existing open set.
        /// </summary>
        /// <param name="nodeWidth">The width of each node along the path in terrain units.</param>
        /// <param name="nodeHeight">The height in of each node along the path in terrain units.</param>
        /// <returns>The goal node.</returns>
        private AStarNode FindGoalNode(int nodeWidth, int nodeHeight)
        {
            AStarNode goalNode = null;

            // Keep searching the open list
            while (this.openSetOrdered.Count > 0)
            {
                // Take the node from the open list with the lowest cost and move to the closed list
                AStarNode current = this.openSetOrdered.Pop();
                this.openSet.Remove(current.PathNode);
                this.closedSet.Add(current.PathNode, current);

                // The goal node was just added to the closed list so the search is complete
                if (current.PathNode.Node.Equals(this.goal.Node))
                {
                    goalNode = current;
                    break;
                }

                // Add the linked nodes to the open list
                foreach (PathLink link in current.PathNode.GetLinks())
                {
                    // Add the left node (if it isn't already in the closed list or blocked)
                    if (!this.closedSet.ContainsKey(link.Node))
                    {
                        if (this.IsOpenSpace(
                            this.GetNodeRectangle(link.Node.Node.X, link.Node.Node.Y, nodeWidth, nodeHeight)))
                        {
                            // Calculate the G and H path cost for the adjacent node
                            int g = current.G + link.Cost;
                            int h = this.CalculateH(link.Node.Node);

                            // Add the adjacent node to the open list
                            this.AddOpenNode(new AStarNode(link.Node, h, g, current));
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
        private void AddOpenNode(AStarNode node)
        {
            // Check if this node exists
            AStarNode existing;
            if (this.openSet.TryGetValue(node.PathNode, out existing))
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
                this.openSet.Add(node.PathNode, node);
            }
        }

        /// <summary>
        /// Calculate the heuristic distance estimate from the given point to the end point.
        /// </summary>
        /// <param name="point">The node for which to calculate H.</param>
        /// <returns>The heuristic distance estimate from the given point to the end point.</returns>
        private int CalculateH(PathNode point)
        {
            // Multiply by 10 so as to use the same scale as G values
            return (Math.Abs(point.X - this.goal.Node.X) + Math.Abs(point.Y - this.goal.Node.Y)) * 10;
        }

        /// <summary>
        /// Gets a rectangle for the given node with the node point at the middle of the bottom edge.
        /// </summary>
        /// <param name="x">The center x position of the node.</param>
        /// <param name="y">The bottom y position of the node.</param>
        /// <param name="width">The width of the node's bounds.</param>
        /// <param name="height">The height of the node's bounds.</param>
        /// <returns>The node rectangle.</returns>
        private Rectangle GetNodeRectangle(int x, int y, int width, int height)
        {
            return new Rectangle(x - (width / 2), y - height, width, height);
        }

        /// <summary>
        /// Determine whether the the given rectangle is free of obstructing terrain.
        /// </summary>
        /// <param name="rect">The rectangle to test.</param>
        /// <returns>True if the rectangle doesn't contain any terrain; False if the rectangle contains obstructing
        /// terrain or the rectangle is outside the bounds of the terrain quad tree.</returns>
        private bool IsOpenSpace(Rectangle rect)
        {
            QuadTreeData<TerrainData>[] terrainDataArray;
            if (this.Terrain.QuadTree.GetDataIntersecting(rect, out terrainDataArray))
            {
                foreach (QuadTreeData<TerrainData> terrainData in terrainDataArray)
                {
                    if (terrainData.Data.Type != TerrainType.None)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Represents a node in the A-star algorithm.
        /// </summary>
        private class AStarNode : IIndexedObject, IComparable<AStarNode>
        {
            /// <summary>
            /// Initializes a new instance of the AStarNode class.
            /// </summary>
            /// <param name="node">The path node.</param>
            /// <param name="h">The heuristic estimate of the distance to the goal from this node.</param>
            /// <param name="g">The cost from the starting node to the this node.</param>
            public AStarNode(LinkedPathNode node, int h, int g)
                : this(node, h, g, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the AStarNode class.
            /// </summary>
            /// <param name="node">The path node.</param>
            /// <param name="h">The heuristic estimate of the distance to the goal from this node.</param>
            /// <param name="g">The cost from the starting node to the this node.</param>
            /// <param name="parent">The parent node in the direction of the start node.</param>
            public AStarNode(LinkedPathNode node, int h, int g, AStarNode parent)
            {
                this.PathNode = node;
                this.H = h;
                this.G = g;
                this.Parent = parent;
            }

            /// <summary>
            /// Gets the path node.
            /// </summary>
            public LinkedPathNode PathNode { get; private set; }

            /// <summary>
            /// Gets or sets the parent node in the direction of the start node.
            /// </summary>
            public AStarNode Parent { get; set; }

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
            public int CompareTo(AStarNode other)
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

        #endregion
    }
}