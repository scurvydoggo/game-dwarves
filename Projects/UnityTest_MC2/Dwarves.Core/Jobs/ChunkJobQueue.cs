// ----------------------------------------------------------------------------
// <copyright file="ChunkJobQueue.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The job queue for a chunk.
    /// </summary>
    public class ChunkJobQueue : JobQueue
    {
        /// <summary>
        /// Initialises a new instance of the ChunkJobQueue class.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="masterJobs">The current master jobs which must be immediately queued.</param>
        public ChunkJobQueue(Vector2I chunk, List<Job> masterJobs)
        {
            this.Chunk = chunk;
            this.State = new ChunkJobQueueState();

            // Enqueue the existing master jobs
            if (masterJobs.Count > 0)
            {
                foreach (Job job in masterJobs)
                {
                    job.AddOwners(this);
                    this.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Gets the chunk.
        /// </summary>
        public Vector2I Chunk { get; private set; }

        /// <summary>
        /// Gets the queue state.
        /// </summary>
        public ChunkJobQueueState State { get; private set; }
    }
}