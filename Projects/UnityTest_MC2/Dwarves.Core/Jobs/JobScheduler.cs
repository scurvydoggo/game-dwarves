// ----------------------------------------------------------------------------
// <copyright file="JobScheduler.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The job scheduler.
    /// </summary>
    public class JobScheduler : IDisposable
    {
        /// <summary>
        /// The jobs for each chunk.
        /// </summary>
        private Dictionary<Vector2I, ChunkJobQueue> queues;

        /// <summary>
        /// The jobs which require all queues.
        /// </summary>
        private List<Job> masterJobs;

        /// <summary>
        /// The queue of master jobs.
        /// </summary>
        private JobQueue masterQueue;

        /// <summary>
        /// The pool of worker threads to execute jobs.
        /// </summary>
        private JobPool jobPool;

        /// <summary>
        /// The queues lock.
        /// </summary>
        private SpinLock queuesLock;

        /// <summary>
        /// Indicates whether the instance has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initialises a new instance of the JobScheduler class.
        /// </summary>
        /// <param name="threadCount">The number of threads to spawn.</param>
        public JobScheduler(int threadCount)
        {
            this.queues = new Dictionary<Vector2I, ChunkJobQueue>();
            this.queuesLock = new SpinLock(10);
            this.masterQueue = new JobQueue();
            this.masterJobs = new List<Job>();
            this.jobPool = new JobPool(threadCount);
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.jobPool.Dispose();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Enqueue a job.
        /// </summary>
        /// <param name="action">The action delegate.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="chunks">The chunks to which this job belongs.</param>
        public void Enqueue(Action action, bool canSkip, params Vector2I[] chunks)
        {
            if (chunks.Length > 0)
            {
                // Create the job
                var job = new Job(action, canSkip, false, chunks.Length);
                job.IsPendingChanged += this.Job_IsPendingChanged;
                job.Completed += this.Job_Completed;

                // Get the owners of the job, creating the owner queues if necessary
                JobQueue[] owners = new JobQueue[chunks.Length];
                this.queuesLock.Enter();
                try
                {
                    for (int i = 0; i < chunks.Length; i++)
                    {
                        Vector2I chunk = chunks[i];
                        ChunkJobQueue queue;
                        if (!this.queues.TryGetValue(chunk, out queue))
                        {
                            queue = new ChunkJobQueue(chunk, this.masterJobs);
                            queue.QueueIdle += this.ChunkJobs_QueueIdle;
                            this.queues.Add(chunk, queue);
                        }

                        owners[i] = queue.Queue;
                    }
                }
                finally
                {
                    this.queuesLock.Exit();
                }

                // Add the owners and enqueue the job
                job.AddOwners(owners);
                foreach (JobQueue queue in owners)
                {
                    queue.Enqueue(job);
                }
            }
            else
            {
                // Create the job
                var job = new Job(action, canSkip, true, this.queues.Count + 10);
                job.IsPendingChanged += this.Job_IsPendingChanged;
                job.Completed += this.Job_Completed;

                // Add this as a master job and get the owner queues
                JobQueue[] owners;
                this.queuesLock.Enter();
                try
                {
                    // Add this as a master job
                    this.masterJobs.Add(job);

                    owners = new JobQueue[this.queues.Count + 1];
                    owners[0] = this.masterQueue;

                    int i = 1;
                    foreach (ChunkJobQueue jobs in this.queues.Values)
                    {
                        owners[i++] = jobs.Queue;
                    }
                }
                finally
                {
                    this.queuesLock.Exit();
                }

                // Enqueue the job
                job.AddOwners(owners);
                foreach (JobQueue queue in owners)
                {
                    queue.Enqueue(job);
                }
            }
        }

        /// <summary>
        /// Update the chunks that are currently active.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks.</param>
        public void UpdateActiveChunks(Dictionary<Vector2I, bool> activeChunks)
        {
            this.queuesLock.Enter();
            try
            {
                foreach (ChunkJobQueue jobs in this.queues.Values)
                {
                    jobs.Queue.FlaggedForRemoval = !activeChunks.ContainsKey(jobs.Chunk);
                }
            }
            finally
            {
                this.queuesLock.Exit();
            }
        }

        /// <summary>
        /// Moves the owner queues of a job forward. This means each queue is stepped to the job after this one.
        /// </summary>
        /// <param name="job">The job.</param>
        private void MoveToNextJob(Job job)
        {
            if (job.IsMasterJob)
            {
                this.queuesLock.Enter();
                try
                {
                    this.masterJobs.Remove(job);
                }
                finally
                {
                    this.queuesLock.Exit();
                }
            }

            foreach (JobQueue owner in job.GetOwners())
            {
                owner.MoveNext();
            }
        }

        /// <summary>
        /// Handle a job pending execution.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="job">The job.</param>
        private void Job_IsPendingChanged(object sender, Job job)
        {
            if (job.IsPending)
            {
                if (!job.IsSkipRequested)
                {
                    // Enqueue this job to be executed
                    this.jobPool.Enqueue(job);
                }
                else
                {
                    // Skip the job by moving each owner queue forward
                    this.MoveToNextJob(job);
                }
            }
        }

        /// <summary>
        /// Handle a job entering the completed state.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="job">The job.</param>
        private void Job_Completed(object sender, Job job)
        {
            this.MoveToNextJob(job);
        }

        /// <summary>
        /// Handle a job queue becoming idle.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="jobs">The chunk jobs.</param>
        private void ChunkJobs_QueueIdle(object sender, ChunkJobQueue jobs)
        {
            this.queuesLock.Enter();
            try
            {
                if (jobs.Queue.IsIdle && jobs.Queue.FlaggedForRemoval)
                {
                    this.queues.Remove(jobs.Chunk);
                }
            }
            finally
            {
                this.queuesLock.Exit();
            }
        }
    }
}