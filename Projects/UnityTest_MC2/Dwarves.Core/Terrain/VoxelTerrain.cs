// ----------------------------------------------------------------------------
// <copyright file="VoxelTerrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain.Engine;

    /// <summary>
    /// A chunk event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="chunk">The chunk index.</param>
    public delegate void ChunkEvent(object sender, Vector2I chunk);

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class VoxelTerrain
    {
        /// <summary>
        /// The voxel data organised by chunk.
        /// </summary>
        private Dictionary<Vector2I, IVoxels> voxels;

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
        /// <param name="chunkDepth">The depth of the chunk.</param>
        /// <param name="scale">The scaling ratio.</param>
        public VoxelTerrain(
            TerrainEngineType engine,
            int chunkWidthLog,
            int chunkHeightLog,
            int chunkDepth,
            int scale)
        {
            this.Engine = engine;
            this.chunkWidthLog = chunkWidthLog;
            this.chunkHeightLog = chunkHeightLog;
            this.ChunkDepth = chunkDepth;
            this.Scale = scale;
            this.ChunkWidth = 1 << chunkWidthLog;
            this.ChunkHeight = 1 << chunkHeightLog;
            this.voxels = new Dictionary<Vector2I, IVoxels>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
            this.factory = new TerrainEngineFactory(this.Engine);
        }

        /// <summary>
        /// Indicates that a chunk of voxels was added.
        /// </summary>
        public event ChunkEvent ChunkAdded;

        /// <summary>
        /// Indicates that a chunk of voxels was removed.
        /// </summary>
        public event ChunkEvent ChunkRemoved;

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
        /// Gets the scaling ratio for voxel coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        public int Scale { get; private set; }

        /// <summary>
        /// Gets the enumerable set of chunks that currently exist.
        /// </summary>
        public IEnumerable<Vector2I> Chunks
        {
            get { return this.voxels.Keys; }
        }

        #region Indexing and coordinate conversion

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
        /// Get the origin of the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The origin of the chunk in world coordinates.</returns>
        public Vector2I GetChunkOrigin(Vector2I chunk)
        {
            return new Vector2I(chunk.X * this.ChunkWidth, chunk.Y * this.ChunkHeight);
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldPos">The position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public Vector2I WorldToChunk(Vector2I worldPos)
        {
            return new Vector2I(worldPos.X & (this.ChunkWidth - 1), worldPos.Y & (this.ChunkHeight - 1));
        }

        /// <summary>
        /// Convert the world coordinates into chunk coordinates.
        /// </summary>
        /// <param name="worldPos">The position.</param>
        /// <returns>The position in chunk coordinates.</returns>
        public Vector3I WorldToChunk(Vector3I worldPos)
        {
            return new Vector3I(worldPos.X & (this.ChunkWidth - 1), worldPos.Y & (this.ChunkHeight - 1), worldPos.Z);
        }

        #endregion Indexing and coordinate conversion

        #region Chunk Related

        /// <summary>
        /// Gets a value indicating whether the given chunk requires a mesh rebuild.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>True if the chunk requires a mesh rebuild.</returns>
        public bool RebuildRequired(Vector2I chunk)
        {
            IVoxels voxels;
            if (this.voxels.TryGetValue(chunk, out voxels))
            {
                return voxels.RebuildRequired;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new voxel chunk at the given chunk index.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        public void NewChunk(Vector2I chunk)
        {
            this.voxels.Add(chunk, this.factory.CreateVoxels(this.ChunkWidth, this.ChunkHeight, this.ChunkDepth));

            // Notify listeners of chunk creation
            this.OnChunkAdded(chunk);
        }

        /// <summary>
        /// Remove the voxels for the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        public void RemoveChunk(Vector2I chunk)
        {
            this.voxels.Remove(chunk);

            // Notify listeners of chunk removal
            this.OnChunkRemoved(chunk);
        }

        /// <summary>
        /// Get the chunk of voxels.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>The voxels.</returns>
        public IVoxels GetChunk(Vector2I chunk)
        {
            return this.voxels[chunk];
        }

        /// <summary>
        /// Try to get the chunk of voxels.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <param name="voxels">The voxels.</param>
        /// <returns>True if the chunk exists.</returns>
        public bool TryGetChunk(Vector2I chunk, out IVoxels voxels)
        {
            return this.voxels.TryGetValue(chunk, out voxels);
        }

        /// <summary>
        /// Determine if the given chunk exists.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>True if the chunk exists.</returns>
        public bool ContainsChunk(Vector2I chunk)
        {
            return this.voxels.ContainsKey(chunk);
        }

        #endregion Chunk Related

        #region Voxel Related

        /// <summary>
        /// Gets or sets the voxel at the given position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>The voxel.</returns>
        public Voxel GetVoxel(Vector3I pos)
        {
            IVoxels voxels;
            if (pos.Z < this.ChunkDepth &&
                this.voxels.TryGetValue(this.ChunkIndex(pos.X, pos.Y), out voxels))
            {
                return voxels[this.WorldToChunk(pos)];
            }
            else
            {
                return Voxel.Air;
            }
        }

        #endregion Voxel Related

        #region Protected Methods

        /// <summary>
        /// Fire the ChunkAdded event.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        protected void OnChunkAdded(Vector2I chunk)
        {
            if (this.ChunkAdded != null)
            {
                this.ChunkAdded(this, chunk);
            }
        }

        /// <summary>
        /// Fire the ChunkRemoved event.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        protected void OnChunkRemoved(Vector2I chunk)
        {
            if (this.ChunkRemoved != null)
            {
                this.ChunkRemoved(this, chunk);
            }
        }

        #endregion Protected Methods
    }
}