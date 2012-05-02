// ----------------------------------------------------------------------------
// <copyright file="PathNode.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Indicates the type of node.
    /// </summary>
    public enum PathNodeType : byte
    {
        /// <summary>
        /// The node is standard terrain which the character walks on.
        /// </summary>
        Normal,

        /// <summary>
        /// The node is only accessible by jumping.
        /// </summary>
        Jump,

        /// <summary>
        /// The node is only accessible by climbing/descending a wall.
        /// </summary>
        ClimbWall
    }

    /// <summary>
    /// A pathfinding node which can be navigated to.
    /// </summary>
    public class PathNode
    {
        /// <summary>
        /// Initializes a new instance of the PathNode class.
        /// </summary>
        /// <param name="point">The point for this node.</param>
        /// <param name="type">The type of this node.</param>
        public PathNode(Point point, PathNodeType type)
        {
            this.Point = point;
            this.Type = type;
            this.AdjacentNodes = new List<PathNode>();
        }

        /// <summary>
        /// Gets the point for this node.
        /// </summary>
        public Point Point { get; private set; }

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public PathNodeType Type { get; private set; }

        /// <summary>
        /// Gets the list of adjacent path nodes.
        /// </summary>
        public List<PathNode> AdjacentNodes { get; private set; }
    }
}