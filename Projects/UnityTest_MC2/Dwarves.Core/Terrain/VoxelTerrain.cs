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
    using Dwarves.Core.Terrain.Engine;

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
        /// The terrain factory.
        /// </summary>
        private TerrainEngineFactory factory;

        /// <summary>
        /// Initialises a new instance of the VoxelTerrain class.
        /// </summary>
        /// <param name="engine">The type of terrain engine.</param>
        /// <param name="chunkWidth">The chunk width.</param>
        /// <param name="chunkHeight">The chunk height.</param>
        /// <param name="chunkDepth">The chunk depth.</param>
        /// <param name="worldDepth">The depth level at which the game simulation takes place.</param>
        /// <param name="scale">The scaling ratio.</param>
        public VoxelTerrain(
            TerrainEngineType engine,
            int chunkWidth,
            int chunkHeight,
            int chunkDepth,
            int worldDepth,
            int scale)
        {
            this.Engine = engine;
            this.ChunkWidth = chunkWidth;
            this.ChunkHeight = chunkHeight;
            this.ChunkDepth = chunkDepth;
            this.WorldDepth = worldDepth;
            this.Scale = scale;
            this.Voxels = new Dictionary<Vector2I, IVoxels>();
            this.Meshes = new Dictionary<Vector2I, MeshData>();
            this.SurfaceHeights = new Dictionary<int, float[]>();

            this.factory = new TerrainEngineFactory(this.Engine);
        }

        /// <summary>
        /// Gets the type of terrain engine.
        /// </summary>
        public TerrainEngineType Engine { get; private set; }

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
        /// Gets the depth level at which the game simulation takes place.
        /// </summary>
        public int WorldDepth { get; private set; }

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
        /// Creates a new chunk at the given chunk index.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        public void NewChunk(Vector2I chunkIndex)
        {
            this.Voxels.Add(chunkIndex, this.factory.CreateVoxels(this.ChunkWidth, this.ChunkHeight, this.ChunkDepth));
        }

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