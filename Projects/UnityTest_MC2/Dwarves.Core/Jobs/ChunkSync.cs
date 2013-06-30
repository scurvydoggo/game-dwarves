// ----------------------------------------------------------------------------
// <copyright file="ChunkSync.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using Dwarves.Core.Math;
    using System.Collections.Generic;

    /// <summary>
    /// Used to synchronise job execution across multiple chunks such that all chunks are in the same state.
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
        /// Indicates whether the chunks have already been synchronised.
        /// </summary>
        private bool isComplete;

        /// <summary>
        /// The queues lock.
        /// </summary>
        private SpinLock chunksLock;

        /// <summary>
        /// Initialises a new instance of the ChunkSync class.
        /// </summary>
        public ChunkSync()
        {
            this.chunks = new Dictionary<Vector2I, bool>();
            this.chunksLock = new SpinLock(10);
        }

        /// <summary>
        /// Adds a chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if the chunk was added; False if the chunks have already been synchronised.</returns>
        public bool AddChunk(Vector2I chunk)
        {
            this.chunksLock.Enter();
            try
            {
                if (!this.isComplete)
                {
                    if (!this.chunks.ContainsKey(chunk))
                    {
                        this.chunks.Add(chunk, false);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                this.chunksLock.Exit();
            }
        }

        /// <summary>
        /// Indicate that a chunk is ready.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if all chunks are now synchronised.</returns>
        public bool SetChunkReady(Vector2I chunk)
        {
            this.chunksLock.Enter();
            try
            {
                if (!this.chunks[chunk])
                {
                    this.chunks[chunk] = true;
                    if (++this.readyCount == this.chunks.Count)
                    {
                        this.isComplete = true;
                    }
                }
            }
            finally
            {
                this.chunksLock.Exit();
            }

            return this.isComplete;
        }
    }
}