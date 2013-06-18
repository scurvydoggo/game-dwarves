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
    }
}