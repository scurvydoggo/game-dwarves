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
        /// Locking object for adding/removing terrain.
        /// </summary>
        private readonly object chunksLock = new object();

        /// <summary>
        /// The chunks.
        /// </summary>
        private Dictionary<Vector2I, TerrainChunk> chunks;

        /// <summary>
        /// Initialises a new instance of the DwarfTerrain class.
        /// </summary>
        public DwarfTerrain()
        {
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
        /// Gets the chunks. This is a thread-safe operation to be used when accessing the terrain from outside of a
        /// scheduled job.
        /// </summary>
        /// <returns>The chunks.</returns>
        public Vector2I[] GetChunksThreadSafe()
        {
            lock (this.chunksLock)
            {
                var chunks = new Vector2I[this.chunks.Count];
                this.chunks.Keys.CopyTo(chunks, 0);
                return chunks;
            }
        }

        /// <summary>
        /// Add a chunk.
        /// </summary>
        /// <param name="chunk">The chunk to add.</param>
        public void AddChunk(TerrainChunk chunk)
        {
            lock (this.chunksLock)
            {
                this.chunks.Add(chunk.Index, chunk);
            }

            if (this.ChunkAdded != null)
            {
                this.ChunkAdded(this, chunk.Index);
            }
        }

        /// <summary>
        /// Remove a chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        public void RemoveChunk(Vector2I chunkIndex)
        {
            bool removed;
            lock (this.chunksLock)
            {
                removed = this.chunks.Remove(chunkIndex);
            }

            if (removed && this.ChunkRemoved != null)
            {
                this.ChunkRemoved(this, chunkIndex);
            }
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
        public bool HasChunk(Vector2I chunkIndex)
        {
            return this.chunks.ContainsKey(chunkIndex);
        }

        /// <summary>
        /// Determines whether a chunk exists at the given x position.
        /// </summary>
        /// <param name="x">The x position in chunk coordinates.</param>
        /// <returns>True if a chunk exists.</returns>
        public bool HasChunkAtX(int x)
        {
            foreach (Vector2I chunkIndex in this.chunks.Keys)
            {
                if (chunkIndex.X == x)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the point at the given world position.
        /// </summary>
        /// <param name="pos">The world position.</param>
        /// <returns>The point.</returns>
        public TerrainPoint GetPoint(Vector3I pos)
        {
            TerrainChunk chunk;
            if (this.chunks.TryGetValue(Metrics.ChunkIndex(pos.X, pos.Y), out chunk))
            {
                return chunk.GetPoint(Metrics.WorldToChunk(pos));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the voxel at the given world position.
        /// </summary>
        /// <param name="pos">The world position.</param>
        /// <returns>The voxel.</returns>
        public TerrainVoxel GetVoxel(Vector3I pos)
        {
            TerrainChunk chunk;
            if (this.chunks.TryGetValue(Metrics.ChunkIndex(pos.X, pos.Y), out chunk))
            {
                return chunk.GetVoxel(Metrics.WorldToChunk(pos));
            }
            else
            {
                return TerrainVoxel.CreateEmpty();
            }
        }
    }
}