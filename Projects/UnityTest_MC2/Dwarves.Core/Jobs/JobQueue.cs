// ----------------------------------------------------------------------------
// <copyright file="JobQueue.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The job queue for a chunk.
    /// </summary>
    public class JobQueue
    {
        /// <summary>
        /// The job that is currently pending or executing.
        /// </summary>
        private Job pendingJob;

        /// <summary>
        /// The queue of jobs to be executed.
        /// </summary>
        private Queue<Job> queue;

        /// <summary>
        /// The queue lock.
        /// </summary>
        private SpinLock queueLock;

        /// <summary>
        /// Initialises a new instance of the JobQueue class.
        /// </summary>
        public JobQueue()
        {
            this.queue = new Queue<Job>();
            this.queueLock = new SpinLock(10);
        }

        /// <summary>
        /// Initialises a new instance of the JobQueue class.
        /// </summary>
        /// <param name="masterJobs">The current master jobs which must be immediately queued.</param>
        public JobQueue(List<Job> masterJobs)
        {
            this.queue = new Queue<Job>();
            this.queueLock = new SpinLock(10);

            if (masterJobs.Count > 0)
            {
                foreach (Job job in masterJobs)
                {
                    this.queue.Enqueue(job);
                    job.AddOwners(this);
                }

                this.MoveNext();
            }
        }

        /// <summary>
        /// The queue has become idle.
        /// </summary>
        public event EventHandler Idle;

        /// <summary>
        /// Gets a value indicating whether the queue is currently idle with no items queued.
        /// </summary>
        public bool IsIdle
        {
            get { return this.pendingJob == null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the queue is flagged for removal. The queue can still accept jobs
        /// (since jobs may expect this queue to pend on them), however once the queue is empty it will be removed.
        /// </summary>
        public bool FlaggedForRemoval { get; set; }

        /// <summary>
        /// Enqueues a job.
        /// </summary>
        /// <param name="job">The jobs.</param>
        public void Enqueue(Job job)
        {
            bool jobIsPending;
            this.queueLock.Enter();
            try
            {
                if (this.pendingJob != null)
                {
                    this.queue.Enqueue(job);
                    jobIsPending = false;
                }
                else
                {
                    this.pendingJob = job;
                    jobIsPending = true;
                }
            }
            finally
            {
                this.queueLock.Exit();
            }

            // Indicate to the job that this queue is pending. If this queue is the last to pend, enqueue for execution
            if (jobIsPending)
            {
                job.IncrementPendingQueues(this.FlaggedForRemoval);
            }
        }

        /// <summary>
        /// Moves to the next job in the queue.
        /// </summary>
        public void MoveNext()
        {
            Job nextJob;
            this.queueLock.Enter();
            try
            {
                if (this.queue.Count > 0)
                {
                    nextJob = this.pendingJob = this.queue.Dequeue();
                }
                else
                {
                    nextJob = this.pendingJob = null;
                }
            }
            finally
            {
                this.queueLock.Exit();
            }

            if (nextJob != null)
            {
                // Indicate to the job that this queue is pending
                nextJob.IncrementPendingQueues(this.FlaggedForRemoval);
            }
            else
            {
                // The queue is now idle
                if (this.Idle != null)
                {
                    this.Idle(this, EventArgs.Empty);
                }
            }
        }
    }
}