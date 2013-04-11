// ----------------------------------------------------------------------------
// <copyright file="DwarfTerrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// A chunk event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    public delegate void ChunkEvent(object sender, Vector2I chunkIndex);

    /// <summary>
    /// Represents the terrain.
    /// </summary>
    public class DwarfTerrain
    {
        /// <summary>
        /// The active chunks.
        /// </summary>
        private Dictionary<Vector2I, TerrainChunk> chunks;

        /// <summary>
        /// The power-of-2 height of the chunk for quickly determining chunk index.
        /// </summary>
        private int chunkWidthLog;

        /// <summary>
        /// The power-of-2 width of the chunk for quickly determining chunk index.
        /// </summary>
        private int chunkHeightLog;

        /// <summary>
        /// Initialises a new instance of the DwarfTerrain class.
        /// </summary>
        /// <param name="chunkWidthLog">The power-of-2 width of the chunk.</param>
        /// <param name="chunkHeightLog">The power-of-2 height of the chunk.</param>
        /// <param name="chunkDepth">The depth of the chunk.</param>
        /// <param name="scale">The scaling ratio.</param>
        public DwarfTerrain(int chunkWidthLog, int chunkHeightLog, int chunkDepth, int scale)
        {
            this.chunkWidthLog = chunkWidthLog;
            this.chunkHeightLog = chunkHeightLog;
            this.ChunkDepth = chunkDepth;
            this.Scale = scale;
            this.ChunkWidth = 1 << chunkWidthLog;
            this.ChunkHeight = 1 << chunkHeightLog;
            this.chunks = new Dictionary<Vector2I, TerrainChunk>();
            this.SurfaceHeights = new Dictionary<int, float[]>();
        }

        /// <summary>
        /// Indicates that a chunk was added.
        /// </summary>
        public event ChunkEvent ChunkAdded;

        /// <summary>
        /// Indicates that a chunk was removed.
        /// </summary>
        public event ChunkEvent ChunkRemoved;

        /// <summary>
        /// Gets the surface heights for each chunk x-position.
        /// </summary>
        public Dictionary<int, float[]> SurfaceHeights { get; private set; }

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
        /// Gets the scaling ratio for terrain coordinates to world coordinates (essentially the Level of Detail).
        /// </summary>
        public int Scale { get; private set; }

        /// <summary>
        /// Gets the enumerable set of chunks that currently exist.
        /// </summary>
        public IEnumerable<Vector2I> Chunks
        {
            get { return this.chunks.Keys; }
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
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The origin of the chunk in world coordinates.</returns>
        public Vector2I GetChunkOrigin(Vector2I chunkIndex)
        {
            return new Vector2I(chunkIndex.X * this.ChunkWidth, chunkIndex.Y * this.ChunkHeight);
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
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>True if the chunk requires a mesh rebuild.</returns>
        public bool RebuildRequired(Vector2I chunkIndex)
        {
            TerrainChunk chunk;
            if (this.chunks.TryGetValue(chunkIndex, out chunk))
            {
                return chunk.RebuildRequired;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void AddChunk(TerrainChunk chunk, Vector2I chunkIndex)
        {
            this.chunks.Add(chunkIndex, chunk);

            // Flag this chunk mesh for rebuild
            this.FlagRebuildRequired(chunkIndex, true);

            // Notify listeners of chunk addition
            this.OnChunkAdded(chunkIndex);
        }

        /// <summary>
        /// Remove the given chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        public void RemoveChunk(Vector2I chunkIndex)
        {
            this.chunks.Remove(chunkIndex);

            // Notify listeners of chunk removal
            this.OnChunkRemoved(chunkIndex);
        }

        /// <summary>
        /// Get the chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk.</returns>
        public TerrainChunk GetChunk(Vector2I chunkIndex)
        {
            return this.chunks[chunkIndex];
        }

        /// <summary>
        /// Try to get the chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if the chunk exists.</returns>
        public bool TryGetChunk(Vector2I chunkIndex, out TerrainChunk chunk)
        {
            return this.chunks.TryGetValue(chunkIndex, out chunk);
        }

        /// <summary>
        /// Determine if the given chunk exists.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>True if the chunk exists.</returns>
        public bool ContainsChunk(Vector2I chunkIndex)
        {
            return this.chunks.ContainsKey(chunkIndex);
        }

        #endregion Chunk Related

        #region Voxel Related

        /// <summary>
        /// Gets or sets the voxel at the given world position.
        /// </summary>
        /// <param name="pos">The world position.</param>
        /// <returns>The voxel.</returns>
        public TerrainVoxel GetVoxel(Vector3I pos)
        {
            TerrainChunk chunk;
            if (this.chunks.TryGetValue(this.ChunkIndex(pos.X, pos.Y), out chunk))
            {
                return chunk.GetVoxel(this.WorldToChunk(pos));
            }
            else
            {
                return TerrainVoxel.CreateEmpty();
            }
        }

        #endregion Voxel Related

        #region Geometry Related

        /// <summary>
        /// Indicate that a chunk requires its mesh to be rebuilt.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="flagNeighbours">Indicates whether the neighbouring chunks should also be flagged for rebuild.
        /// </param>
        public void FlagRebuildRequired(Vector2I chunkIndex, bool flagNeighbours)
        {
            TerrainChunk chunk;
            if (this.chunks.TryGetValue(chunkIndex, out chunk))
            {
                chunk.RebuildRequired = true;
            }

            if (flagNeighbours)
            {
                if (this.chunks.TryGetValue(new Vector2I(chunkIndex.X, chunkIndex.Y + 1), out chunk))
                {
                    chunk.RebuildRequired = true;
                }

                if (this.chunks.TryGetValue(new Vector2I(chunkIndex.X + 1, chunkIndex.Y), out chunk))
                {
                    chunk.RebuildRequired = true;
                }

                if (this.chunks.TryGetValue(new Vector2I(chunkIndex.X, chunkIndex.Y - 1), out chunk))
                {
                    chunk.RebuildRequired = true;
                }

                if (this.chunks.TryGetValue(new Vector2I(chunkIndex.X - 1, chunkIndex.Y), out chunk))
                {
                    chunk.RebuildRequired = true;
                }
            }
        }

        #endregion Geometry Related

        #region Protected Methods

        /// <summary>
        /// Fire the ChunkAdded event.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        protected void OnChunkAdded(Vector2I chunkIndex)
        {
            if (this.ChunkAdded != null)
            {
                this.ChunkAdded(this, chunkIndex);
            }
        }

        /// <summary>
        /// Fire the ChunkRemoved event.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        protected void OnChunkRemoved(Vector2I chunkIndex)
        {
            if (this.ChunkRemoved != null)
            {
                this.ChunkRemoved(this, chunkIndex);
            }
        }

        #endregion Protected Methods
    }
}