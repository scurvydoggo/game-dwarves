// ----------------------------------------------------------------------------
// <copyright file="Terrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;
    using Dwarves.Core.VoxelTerrain.Engine;

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Mask for converting from world coordinates to chunk coordinates.
        /// </summary>
        private const int ChunkHeightMask = TerrainConst.ChunkHeight - 1;

        /// <summary>
        /// Initialises a new instance of the Terrain class.
        /// </summary>
        /// <param name="engine">The type of terrain engine to use.</param>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="scale">The scaling ratio.</param>
        public Terrain(TerrainEngineType engine, int chunkWidth, int chunkHeight, int chunkDepth, int scale)
        {
            this.ChunkWidth = chunkWidth;
            this.ChunkHeight = chunkHeight;
            this.ChunkDepth = chunkDepth;
            this.Scale = scale;
            this.Voxels = new Dictionary<Vector2I, IVoxels>();
            this.Meshes = new Dictionary<Vector2I, MeshData>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
        }

        /// <summary>
        /// Gets the chunk width.
        /// </summary>
        public int ChunkWidth { get; private set; }

        /// <summary>
        /// Gets the chunk height.
        /// </summary>
        public int ChunkHeight { get; private set; }

        /// <summary>
        /// Gets the chunk depth.
        /// </summary>
        public int ChunkDepth { get; private set; }

        /// <summary>
        /// Gets the scaling ratio for voxel coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        public int Scale { get; private set; }

        /// <summary>
        /// Gets the voxel data organised by chunk.
        /// </summary>
        public Dictionary<Vector2I, IVoxels> Voxels { get; private set; }

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