// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Load;
    using UnityEngine;

    /// <summary>
    /// Core component for terrain entity.
    /// </summary>
    public class TerrainComponent : MonoBehaviour
    {
        /// <summary>
        /// The seed for generating terrain.
        /// </summary>
        public float Seed;

        /// <summary>
        /// The isolevel for the rendered terrain. This is the density at which the surface is rendered.
        /// </summary>
        public byte IsoLevel;

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.Terrain = new VoxelTerrain();
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }
}