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
        Jump
    }

    /// <summary>
    /// A navigatable node.
    /// </summary>
    public struct PathNode
    {
        /// <summary>
        /// Initializes a new instance of the PathNode struct.
        /// </summary>
        /// <param name="point">The position for this node.</param>
        /// <param name="type">The type of this node.</param>
        public PathNode(Point point, PathNodeType type)
            : this(point.X, point.Y, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PathNode struct.
        /// </summary>
        /// <param name="x">The x position for this node.</param>
        /// <param name="y">The y position for this node.</param>
        /// <param name="type">The type of this node.</param>
        public PathNode(int x, int y, PathNodeType type)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
        }

        /// <summary>
        /// Gets the x position for this node.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Gets the y position for this node.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public PathNodeType Type { get; private set; }
    }

    /// <summary>
    /// A path node linked to adjacent nodes.
    /// </summary>
    public class LinkedPathNode
    {
        /// <summary>
        /// Initializes a new instance of the LinkedPathNode class.
        /// </summary>
        /// <param name="point">The position for this node.</param>
        /// <param name="type">The type of this node.</param>
        public LinkedPathNode(Point point, PathNodeType type)
            : this(new PathNode(point, type))
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinkedPathNode class.
        /// </summary>
        /// <param name="x">The x position for this node.</param>
        /// <param name="y">The y position for this node.</param>
        /// <param name="type">The type of this node.</param>
        public LinkedPathNode(int x, int y, PathNodeType type)
            : this(new PathNode(x, y, type))
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinkedPathNode class.
        /// </summary>
        /// <param name="node">The path node.</param>
        public LinkedPathNode(PathNode node)
        {
            this.Node = node;
            this.AdjacentNodes = new List<LinkedPathNode>();
        }

        /// <summary>
        /// Gets the path node.
        /// </summary>
        public PathNode Node { get; private set; }

        /// <summary>
        /// Gets the list of adjacent path nodes.
        /// </summary>
        public List<LinkedPathNode> AdjacentNodes { get; private set; }
    }
}