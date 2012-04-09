// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Physics
{
    using EntitySystem.Component;
    using FarseerPhysics.Common;

    /// <summary>
    /// Destructible terrain.
    /// </summary>
    public class TerrainComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the TerrainComponent class.
        /// </summary>
        /// <param name="terrain">The terrain object.</param>
        public TerrainComponent(MSTerrain terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets the terrain object.
        /// </summary>
        public MSTerrain Terrain { get; private set; }
    }
}