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
        /// The master job queue.
        /// </summary>
        private MasterJobQueue masterQueue;

        /// <summary>
        /// All the jobs in the master queue.
        /// </summary>
        private List<Job> masterQueueJobs;

        /// <summary>
        /// The job queue for each chunk.
        /// </summary>
        private Dictionary<Vector2I, ChunkJobQueue> chunkQueues;

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
            this.masterQueue = new MasterJobQueue();
            this.masterQueueJobs = new List<Job>();
            this.chunkQueues = new Dictionary<Vector2I, ChunkJobQueue>();
            this.jobPool = new JobPool(threadCount);
            this.queuesLock = new SpinLock(10);
        }

        /// <summary>
        /// Gets the master queue state.
        /// </summary>
        public MasterJobQueueState MasterQueueState
        {
            get { return this.masterQueue.State; }
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
        /// <param name="completionHandler">The completion handler to register with the job.</param>
        /// <param name="chunks">The chunks to which this job belongs.</param>
        public void Enqueue(Action action, bool canSkip, JobEvent completionHandler, params Vector2I[] chunks)
        {
            // Create the job
            bool isMasterJob = chunks.Length == 0;
            int ownerCapacity = isMasterJob ? this.chunkQueues.Count + 10 : chunks.Length;
            var job = new Job(action, canSkip, isMasterJob, ownerCapacity);
            job.IsPendingChanged += this.Job_IsPendingChanged;
            job.Completed += this.Job_Completed;
            if (completionHandler != null)
            {
                job.Completed += completionHandler;
            }

            if (!job.IsMasterJob)
            {
                // Get the owners of the job, creating the owner queues if necessary
                JobQueue[] owners = new JobQueue[chunks.Length];
                this.queuesLock.Enter();
                try
                {
                    for (int i = 0; i < chunks.Length; i++)
                    {
                        Vector2I chunk = chunks[i];
                        ChunkJobQueue queue;
                        if (!this.chunkQueues.TryGetValue(chunk, out queue))
                        {
                            queue = new ChunkJobQueue(chunk, this.masterQueueJobs);
                            queue.Idle += this.ChunkJobs_QueueIdle;
                            this.chunkQueues.Add(chunk, queue);
                        }

                        owners[i] = queue;
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
                // Add this as a master job and get the owner queues
                JobQueue[] owners;
                this.queuesLock.Enter();
                try
                {
                    // Add this to the master queue job list
                    this.masterQueueJobs.Add(job);

                    // Get the owners which is *all* chunk queues plus the master queue itself
                    int i = 0;
                    owners = new JobQueue[this.chunkQueues.Count + 1];
                    owners[i++] = this.masterQueue;
                    foreach (ChunkJobQueue jobs in this.chunkQueues.Values)
                    {
                        owners[i++] = jobs;
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
            List<Vector2I> removeNow = null;
            this.queuesLock.Enter();
            try
            {
                foreach (ChunkJobQueue queue in this.chunkQueues.Values)
                {
                    bool remove = !activeChunks.ContainsKey(queue.Chunk);
                    if (remove && queue.IsIdle)
                    {
                        if (removeNow == null)
                        {
                            removeNow = new List<Vector2I>();
                        }

                        removeNow.Add(queue.Chunk);
                    }
                    else
                    {
                        queue.FlaggedForRemoval = remove;
                    }
                }

                if (removeNow != null)
                {
                    foreach (Vector2I chunk in removeNow)
                    {
                        this.chunkQueues.Remove(chunk);
                    }
                }
            }
            finally
            {
                this.queuesLock.Exit();
            }
        }

        /// <summary>
        /// Gets the chunk queue state.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The queue state.</returns>
        public ChunkJobQueueState GetChunkQueueState(Vector2I chunk)
        {
            ChunkJobQueue queue;
            this.queuesLock.Enter();
            try
            {
                this.chunkQueues.TryGetValue(chunk, out queue);
            }
            finally
            {
                this.queuesLock.Exit();
            }

            return queue != null ? queue.State : null;
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
                    this.masterQueueJobs.Remove(job);
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
        /// Handle a chunk queue becoming idle.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="queue">The job queue.</param>
        private void ChunkJobs_QueueIdle(object sender, JobQueue queue)
        {
            this.queuesLock.Enter();
            try
            {
                if (queue.IsIdle && queue.FlaggedForRemoval)
                {
                    this.chunkQueues.Remove((queue as ChunkJobQueue).Chunk);
                }
            }
            finally
            {
                this.queuesLock.Exit();
            }
        }
    }
}