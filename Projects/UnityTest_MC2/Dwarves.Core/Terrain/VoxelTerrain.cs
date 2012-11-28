// ----------------------------------------------------------------------------
// <copyright file="VoxelTerrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class VoxelTerrain
    {
        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        private const int ChunkHeightMask = TerrainConst.ChunkHeight - 1;

        /// <summary>
        /// Initialises a new instance of the VoxelTerrain class.
        /// </summary>
        public VoxelTerrain()
        {
            this.Voxels = new Dictionary<Vector2I, Voxel[]>();
            this.Meshes = new Dictionary<Vector2I, MeshData>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
        }

        /// <summary>
        /// Gets the voxel data organised by chunk.
        /// </summary>
        public Dictionary<Vector2I, Voxel[]> Voxels { get; private set; }

        /// <summary>
        /// Gets the mesh data organised by chunk.
        /// </summary>
        public Dictionary<Vector2I, MeshData> Meshes { get; private set; }

        /// <summary>
        /// Gets the surface heights for each chunk x-position.
        /// </summary>
        public Dictionary<int, float[]> SurfaceHeights { get; private set; }

        /// <summary>
        /// Remove the data for the given chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        public void RemoveChunkData(Vector2I chunkIndex)
        {
            this.Voxels.Remove(chunkIndex);
            this.Meshes.Remove(chunkIndex);
        }
    }
}