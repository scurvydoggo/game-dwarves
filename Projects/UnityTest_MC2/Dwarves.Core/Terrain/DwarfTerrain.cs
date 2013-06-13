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
        /// The surface heights for each chunk x-position.
        /// </summary>
        private Dictionary<int, float[]> surfaceHeights;

        /// <summary>
        /// Initialises a new instance of the DwarfTerrain class.
        /// </summary>
        public DwarfTerrain()
        {
            this.chunks = new Dictionary<Vector2I, TerrainChunk>();
            this.surfaceHeights = new Dictionary<int, float[]>();
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
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk to add.</param>
        public void AddChunk(Vector2I chunkIndex, TerrainChunk chunk)
        {
            lock (this.chunksLock)
            {
                this.chunks.Add(chunkIndex, chunk);
            }

            if (this.ChunkAdded != null)
            {
                this.ChunkAdded(this, chunkIndex);
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
        /// Adds the surface heights.
        /// </summary>
        /// <param name="x">The x position of the surface.</param>
        /// <param name="heights">The heights.</param>
        public void AddSurfaceHeights(int x, float[] heights)
        {
            this.surfaceHeights.Add(x, heights);
        }

        /// <summary>
        /// Removes the surface heights.
        /// </summary>
        /// <param name="x">The x position of the surface.</param>
        public void RemoveSurfaceHeights(int x)
        {
            this.surfaceHeights.Remove(x);
        }

        /// <summary>
        /// Get the surface heights.
        /// </summary>
        /// <param name="x">The x position of the surface.</param>
        /// <returns>The heights.</returns>
        public float[] GetSurfaceHeights(int x)
        {
            return this.surfaceHeights[x];
        }

        /// <summary>
        /// Determine if the given surface heights exist.
        /// </summary>
        /// <param name="x">The x position of the surface.</param>
        /// <returns>True if the surface heights exists.</returns>
        public bool HasSurfaceHeights(int x)
        {
            return this.surfaceHeights.ContainsKey(x);
        }

        /// <summary>
        /// Determines whether the given surface heights can be removed.
        /// </summary>
        /// <param name="x">The x position of the surface.</param>
        /// <returns>True if the surface can be removed.</returns>
        public bool CanRemoveSurfaceHeights(int x)
        {
            foreach (Vector2I chunkIndex in this.chunks.Keys)
            {
                if (chunkIndex.X == x)
                {
                    return false;
                }
            }

            return true;
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