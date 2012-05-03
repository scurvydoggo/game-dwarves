// ----------------------------------------------------------------------------
// <copyright file="LinkedPathNode.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

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

        /// <summary>
        /// Gets the string representation of this instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Node.ToString();
        }
    }
}
