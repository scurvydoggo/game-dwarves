// ----------------------------------------------------------------------------
// <copyright file="PathComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Game
{
    using EntitySystem.Component;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a path entity.
    /// </summary>
    public class PathComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the PathComponent class.
        /// </summary>
        /// <param name="nodes">The path nodes.</param>
        public PathComponent(Point[] nodes)
        {
            this.Nodes = nodes;
        }

        /// <summary>
        /// Gets or sets the path nodes.
        /// </summary>
        public Point[] Nodes { get; set; }
    }
}