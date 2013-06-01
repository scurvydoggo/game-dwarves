// ----------------------------------------------------------------------------
// <copyright file="ChunkJobQueue.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// A ChunkJobs event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="jobs">The chunk jobs.</param>
    public delegate void ChunkJobsEvent(object sender, ChunkJobQueue jobs);

    /// <summary>
    /// The job queue for a chunk.
    /// </summary>
    public class ChunkJobQueue
    {
        /// <summary>
        /// Initialises a new instance of the ChunkJobQueue class.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="masterJobs">The current master jobs which must be immediately queued.</param>
        public ChunkJobQueue(Vector2I chunk, List<Job> masterJobs)
        {
            this.Chunk = chunk;
            this.Queue = new JobQueue(masterJobs);
            this.Queue.Idle += this.Queue_Idle;
        }

        /// <summary>
        /// The queue has become idle.
        /// </summary>
        public event ChunkJobsEvent QueueIdle;

        /// <summary>
        /// Gets the chunk.
        /// </summary>
        public Vector2I Chunk { get; private set; }

        /// <summary>
        /// Gets the job queue for the chunk.
        /// </summary>
        public JobQueue Queue { get; private set; }

        /// <summary>
        /// Handles the queue becoming idle.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void Queue_Idle(object sender, EventArgs e)
        {
            if (this.QueueIdle != null)
            {
                this.QueueIdle(this, this);
            }
        }
    }
}