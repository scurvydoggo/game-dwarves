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
        /// The adjacent nodes that are linked to this node.
        /// </summary>
        private List<PathLink> links;

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
            this.links = new List<PathLink>();
        }

        /// <summary>
        /// Gets the path node.
        /// </summary>
        public PathNode Node { get; internal set; }

        /// <summary>
        /// Add a linked path node.
        /// </summary>
        /// <param name="link">The link.</param>
        public void AddLink(PathLink link)
        {
            this.links.Add(link);
        }

        /// <summary>
        /// Add a linked path node.
        /// </summary>
        /// <param name="node">The linked node.</param>
        /// <param name="cost">The movement cost of this link.</param>
        public void AddLink(LinkedPathNode node, int cost)
        {
            this.links.Add(new PathLink(node, cost));
        }

        /// <summary>
        /// Remove a linked path node.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>True if the link was removed.</returns>
        public bool RemoveLink(PathLink link)
        {
            return this.links.Remove(link);
        }

        /// <summary>
        /// Gets the list of linked nodes.
        /// </summary>
        /// <returns>The list of linked nodes.</returns>
        public List<PathLink> GetLinks()
        {
            return this.links;
        }

        /// <summary>
        /// Indicates whether this node contains a link to the given node. 
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>True if the given node is linked to this node.</returns>
        public bool HasLinkedNode(LinkedPathNode node)
        {
            foreach (PathLink link in this.links)
            {
                if (link.Node.Equals(node))
                {
                    return true;
                }
            }

            return false;
        }

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