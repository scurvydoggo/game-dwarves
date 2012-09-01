// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using Dwarves.Core.Terrain.Generation;
    using Dwarves.Core.Terrain.Generation.MarchingCubes;
    using UnityEngine;

    /// <summary>
    /// Component for the managing the terrain mesh.
    /// </summary>
    [RequireComponent(typeof(TerrainComponent))]
    public class TerrainMeshComponent : MonoBehaviour
    {
        /// <summary>
        /// The isolevel for the rendered terrain. This is the density at which the surface is rendered.
        /// </summary>
        public byte IsoLevel;

        /// <summary>
        /// Gets the mesh generator component.
        /// </summary>
        public ChunkMeshGenerator MeshGenerator { get; private set; }

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.MeshGenerator = new MarchingCubesGenerator(this.IsoLevel);
        }
        
        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
        }
    }
}
