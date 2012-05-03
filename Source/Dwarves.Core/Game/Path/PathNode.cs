// ----------------------------------------------------------------------------
// <copyright file="PathNode.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
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

        /// <summary>
        /// Determine if the given object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to test for equality.</param>
        /// <returns>True if the given object is equal to this object.</returns>
        public override bool Equals(object obj)
        {
            // Perform equality checks with Point objects too
            return
                (obj is PathNode &&
                ((PathNode)obj).X == this.X &&
                ((PathNode)obj).Y == this.Y &&
                ((PathNode)obj).Type == this.Type) ||
                (obj is Point &&
                ((Point)obj).X == this.X &&
                ((Point)obj).Y == this.Y);
        }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Type.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}, Type:{2}", this.X, this.Y, this.Type);
        }
    }
}