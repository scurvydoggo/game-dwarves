// ----------------------------------------------------------------------------
// <copyright file="PathLink.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Path
{
    /// <summary>
    /// A link with another path node.
    /// </summary>
    public class PathLink
    {
        /// <summary>
        /// Initializes a new instance of the PathLink class.
        /// </summary>
        /// <param name="node">The linked node.</param>
        /// <param name="cost">The movement cost of this link.</param>
        public PathLink(LinkedPathNode node, int cost)
        {
            this.Node = node;
            this.Cost = cost;
        }

        /// <summary>
        /// Gets the linked node.
        /// </summary>
        public LinkedPathNode Node { get; private set; }

        /// <summary>
        /// Gets the movement cost of this link.
        /// </summary>
        public int Cost { get; private set; }
    }
}
