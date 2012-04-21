// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Game
{
    using Dwarves.Common;
    using Dwarves.Game.Terrain;
    using EntitySystem.Component;

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
        public TerrainComponent(ClipQuadTree<TerrainType> terrain, bool isCollidable)
        {
            this.QuadTree = terrain;
        }

        /// <summary>
        /// Gets the terrain quad tree.
        /// </summary>
        public ClipQuadTree<TerrainType> QuadTree { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this terrain can be collided with.
        /// </summary>
        public bool IsCollidable { get; set; }
    }
}