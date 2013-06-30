// ----------------------------------------------------------------------------
// <copyright file="ChunkSync.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>B
    /// Used to synchronise job execution across multiple chunks such that all chunks are in the same state. This class
    /// is not thread safe.
    /// </summary>
    public class ChunkSync
    {
        /// <summary>
        /// The chunks that are being synchronised. A value of true indicates that the chunk is ready.
        /// </summary>
        private Dictionary<Vector2I, bool> chunks;

        /// <summary>
        /// A count of the chunks that are ready (to avoid re-counting the dictionary every time).
        /// </summary>
        private int readyCount;

        /// <summary>
        /// Initialises a new instance of the ChunkSync class.
        /// </summary>
        /// <param name="chunks">The chunks requiring synchronisation.</param>
        public ChunkSync(params Vector2I[] chunks)
        {
            this.chunks = new Dictionary<Vector2I, bool>();
            foreach (Vector2I chunk in chunks)
            {
                this.chunks.Add(chunk, false);
            }
        }

        /// <summary>
        /// Indicates that the chunks are synchronised.
        /// </summary>
        public event EventHandler IsSynchronised;

        /// <summary>
        /// Adds a chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        public void AddChunk(Vector2I chunk)
        {
            if (!this.chunks.ContainsKey(chunk))
            {
                this.chunks.Add(chunk, false);
            }
        }

        /// <summary>
        /// Indicate that a chunk is ready.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        public void SetChunkReady(Vector2I chunk)
        {
            if (!this.chunks[chunk])
            {
                this.chunks[chunk] = true;

                if (++this.readyCount == this.chunks.Count && this.IsSynchronised != null)
                {
                    this.IsSynchronised(this, EventArgs.Empty);
                }
            }
        }
    }
}