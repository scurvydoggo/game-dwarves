// ----------------------------------------------------------------------------
// <copyright file="PathNode.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using Microsoft.Xna.Framework;

    public enum PathNodeType : byte
    {
        /// <summary>
        /// The node is standard terrain which the character walks on.
        /// </summary>
        Normal,

        /// <summary>
        /// The node is only accessible by jumping/falling.
        /// </summary>
        Jump,

        /// <summary>
        /// The node is only accessible by climbing/descending a ladder/rope.
        /// </summary>
        ClimbRope,

        /// <summary>
        /// The node is only accessible by climbing/descending a wall.
        /// </summary>
        ClimbWall
    }

    /// <summary>
    /// A pathfinding node which can be navigated to.
    /// </summary>
    public struct PathNode
    {
        /// <summary>
        /// Initializes a new instance of the Node class.
        /// </summary>
        /// <param name="point">The point for this node.</param>
        /// <param name="type">The type of this node.</param>
        public PathNode(Point point, PathNodeType type)
            : this()
        {
            this.Point = point;
            this.Type = type;
        }

        /// <summary>
        /// Gets the point for this node.
        /// </summary>
        public Point Point { get; private set; }

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public PathNodeType Type { get; private set; }
    }
}