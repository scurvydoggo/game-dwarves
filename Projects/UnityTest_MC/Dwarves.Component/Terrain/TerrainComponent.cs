// ----------------------------------------------------------------------------
// <copyright file="TerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Generation.MarchingCubes;
    using UnityEngine;

    /// <summary>
    /// Core component for terrain entity.
    /// </summary>
    public class TerrainComponent : MonoBehaviour
    {
        /// <summary>
        /// The isolevel for the rendered terrain. This is the density at which the surface is rendered.
        /// </summary>
        public int IsoLevel = 127;

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets the mesh generator component.
        /// </summary>
        public ChunkMeshGenerator MeshGenerator { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.Terrain = new VoxelTerrain();
            this.MeshGenerator = new MarchingCubesGenerator((byte)this.IsoLevel);
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }
}