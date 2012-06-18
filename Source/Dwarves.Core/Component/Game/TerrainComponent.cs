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
    using FarseerPhysics.Dynamics;
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
            ClipQuadTree<TerrainData> terrain,
            bool isCollidable,
            Dictionary<Point, LinkedPathNode> pathNodes)
        {
            this.Terrain = terrain;
            this.IsCollidable = isCollidable;
            this.PathNodes = pathNodes;
            this.Fixtures = new Dictionary<Square, Fixture>();
        }

        /// <summary>
        /// Gets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainData> Terrain { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this terrain can be collided with.
        /// </summary>
        public bool IsCollidable { get; set; }

        /// <summary>
        /// Gets or sets the pathfinding nodes of the terrain.
        /// </summary>
        public Dictionary<Point, LinkedPathNode> PathNodes { get; set; }

        /// <summary>
        /// Gets or sets the fixtures which currently exist for the terrain's physics body.
        /// </summary>
        public Dictionary<Square, Fixture> Fixtures { get; set; }
    }
}