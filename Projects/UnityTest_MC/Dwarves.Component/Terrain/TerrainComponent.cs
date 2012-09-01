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