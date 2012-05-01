// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Game
{
    using System.Collections.Generic;
    using Dwarves.Common;
    using Dwarves.Game.Path;
    using Dwarves.Game.Terrain;
    using EntitySystem.Component;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Destructible terrain.
    /// </summary>
    public class TerrainComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the TerrainComponent class.
        /// </summary>
        /// <param name="terrain">The terrain quad tree.</param>
        /// <param name="isCollidable">Indicates whether the terrain can be collided with.</param>
        /// <param name="pathNodes">The pathfinding nodes of the terrain.</param>
        public TerrainComponent(
            ClipQuadTree<TerrainType> terrain,
            bool isCollidable,
            Dictionary<Point, PathNode> pathNodes)
        {
            this.QuadTree = terrain;
            this.IsCollidable = isCollidable;
            this.PathNodes = pathNodes;
        }

        /// <summary>
        /// Gets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainType> QuadTree { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this terrain can be collided with.
        /// </summary>
        public bool IsCollidable { get; set; }

        /// <summary>
        /// Gets or sets the pathfinding nodes of the terrain.
        /// </summary>
        private Dictionary<Point, PathNode> PathNodes { get; set; }
    }
}