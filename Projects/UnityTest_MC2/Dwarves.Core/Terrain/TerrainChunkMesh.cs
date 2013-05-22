// ----------------------------------------------------------------------------
// <copyright file="TerrainChunkMesh.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using Dwarves.Core.Geometry;

    /// <summary>
    /// The mesh for a terrain chunk.
    /// </summary>
    public class TerrainChunkMesh
    {
        /// <summary>
        /// Initialises a new instance of the TerrainChunkMesh class.
        /// </summary>
        public TerrainChunkMesh()
        {
            this.Data = new MeshData();
        }

        /// <summary>
        /// Gets or sets the mesh data.
        /// </summary>
        public MeshData Data { get; set; }

        /// <summary>
        /// Gets a value indicating whether the mesh data has changed.
        /// </summary>
        public bool MeshDataChanged { get; private set; }

        /// <summary>
        /// Set the mesh data changed flag.
        /// </summary>
        public void SetMeshDataChanged()
        {
            this.MeshDataChanged = true;
        }

        /// <summary>
        /// Reset the mesh data changed flag.
        /// </summary>
        public void ResetMeshDataChanged()
        {
            this.MeshDataChanged = false;
        }
    }
}