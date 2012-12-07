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
        /// The power-of-2 height of the chunk for quickly determining chunk index.
        /// </summary>
        private int chunkWidthLog;

        /// <summary>
        /// The power-of-2 width of the chunk for quickly determining chunk index.
        /// </summary>
        private int chunkHeightLog;

        /// <summary>
        /// The terrain factory.
        /// </summary>
        private TerrainEngineFactory factory;

        /// <summary>
        /// Initialises a new instance of the VoxelTerrain class.
        /// </summary>
        /// <param name="engine">The type of terrain engine.</param>
        /// <param name="chunkWidthLog">The power-of-2 width of the chunk.</param>
        /// <param name="chunkHeightLog">The power-of-2 height of the chunk.</param>
        /// <param name="chunkDepthLog">The power-of-2 depth of the chunk.</param>
        /// <param name="worldDepth">The depth level at which the game simulation takes place.</param>
        /// <param name="scale">The scaling ratio.</param>
        public VoxelTerrain(
            TerrainEngineType engine,
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepthLog,
            int worldDepth,
            int scale)
        {
            this.Engine = engine;
            this.chunkWidthLog = chunkWidthLog;
            this.chunkHeightLog = chunkHeightLog;
            this.WorldDepth = worldDepth;
            this.Scale = scale;
            this.ChunkWidth = 1 << chunkWidthLog;
            this.ChunkHeight = 1 << chunkHeightLog;
            this.ChunkDepth = 1 << chunkDepthLog;
            this.Voxels = new Dictionary<Vector2I, IVoxels>();
            this.Meshes = new Dictionary<Vector2I, MeshData>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
            this.factory = new TerrainEngineFactory(this.Engine);
        }

        /// <summary>
        /// Gets or sets the current terrain instance for the application.
        /// </summary>
        public static VoxelTerrain Instance { get; set; }

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
        /// Get the index of the chunk at the given world coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The chunk index.</returns>
        public Vector2I ChunkIndex(int worldX, int worldY)
        {
            return new Vector2I(worldX >> this.chunkWidthLog, worldY >> this.chunkHeightLog);
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldX">The x position.</param>
        /// <param name="worldY">The y position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public Vector2I WorldToChunk(int worldX, int worldY)
        {
            return new Vector2I(worldX & (this.ChunkWidth - 1), worldY & (this.ChunkHeight - 1));
        }

        /// <summary>
        /// Get the origin of the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The origin of the chunk in world coordinates.</returns>
        public Vector2I GetChunkOrigin(Vector2I chunk)
        {
            return new Vector2I(chunk.X * this.ChunkWidth, chunk.Y * this.ChunkHeight);
        }

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