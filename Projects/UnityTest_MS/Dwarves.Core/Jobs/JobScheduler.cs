// ----------------------------------------------------------------------------
// <copyright file="JobScheduler.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// Indicates how a non-existing chunk queue is handled when performing an operation on a set of chunks.
    /// </summary>
    public enum MissingQueue
    {
        /// <summary>
        /// Create the missing queue and evaluate the selector.
        /// </summary>
        CreateAndEvaluate,

        /// <summary>
        /// Skip the missing queue with a success result.
        /// </summary>
        Skip,

        /// <summary>
        /// Fail the operation.
        /// </summary>
        Fail,
    }

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
        /// Enqueue a master job. A master job requires exclusive access to all chunks.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="reserveQueue">Evaluates whether the job can be enqueued. If so, the master queue is reserved
        /// to indicate that this job is currently queued or executing.</param>
        /// <param name="unreserveQueue">Un-reserve the master queue to indicate that this job is no longer queued or
        /// executing.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <returns>True if the job was enqueued.</returns>
        public bool EnqueueMaster(
            Action work,
            Predicate<MasterJobQueue> reserveQueue,
            Action<MasterJobQueue> unreserveQueue,
            bool canSkip)
        {
            // Create the job
            var job = new MasterJob(work, unreserveQueue, canSkip, this.chunkQueues.Count + 10);
            job.IsPendingChanged += this.Job_IsPendingChanged;
            job.Completed += this.Job_Completed;

            // Enqueue the job if it can be reserved
            this.queuesLock.Enter();
            try
            {
                // Reserve the master queue
                if (!reserveQueue(this.masterQueue))
                {
                    return false;
                }

                // Build the set of owners
                int i = 0;
                ChunkJobQueue[] chunkQueues = new ChunkJobQueue[this.chunkQueues.Count];
                foreach (ChunkJobQueue queue in this.chunkQueues.Values)
                {
                    chunkQueues[i++] = queue;
                }

                // Add the owners and enqueue the job
                job.AddOwners(this.masterQueue);
                job.AddOwners(chunkQueues);
                this.masterQueue.Enqueue(job);
                foreach (ChunkJobQueue queue in chunkQueues)
                {
                    queue.Enqueue(job);
                }

                // Retain a reference to this master job
                this.masterQueueJobs.Add(job);
            }
            finally
            {
                this.queuesLock.Exit();
            }

            return true;
        }

        /// <summary>
        /// Begin a job enqueue. EndEnqueueChunks should be called as soon as possible to minimise lock duration.
        /// </summary>
        public void BeginEnqueueChunks()
        {
            this.queuesLock.Enter();
        }

        /// <summary>
        /// Finished a job enqueue.
        /// </summary>
        public void EndEnqueueChunks()
        {
            this.queuesLock.Exit();
        }

        /// <summary>
        /// Enqueue a job which requires exclusive access to a chunk. This must be called between BeginEnqueueChunks
        /// and EndEnqueueChunks.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="reserveQueue">Reserve each queue to indicate that this job is currently queued or executing.
        /// </param>
        /// <param name="unreserveQueue">Un-reserve each queue to indicate that this job is no longer queued or
        /// executing.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="chunk">The chunk to which this job requires exclusive access.</param>
        public void EnqueueChunks(
            Action work,
            Action<ChunkJobQueue> reserveQueue,
            Action<ChunkJobQueue> unreserveQueue,
            bool canSkip,
            Vector2I chunk)
        {
            this.EnqueueChunks(work, reserveQueue, unreserveQueue, canSkip, new Vector2I[] { chunk });
        }

        /// <summary>
        /// Enqueue a job which requires exclusive access to one or more chunks. This must be called between
        /// BeginEnqueueChunks and EndEnqueueChunks.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="reserveQueue">Reserve each queue to indicate that this job is currently queued or executing.
        /// </param>
        /// <param name="unreserveQueue">Un-reserve each queue to indicate that this job is no longer queued or
        /// executing.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="chunks">The chunks to which this job requires exclusive access.</param>
        public void EnqueueChunks(
            Action work,
            Action<ChunkJobQueue> reserveQueue,
            Action<ChunkJobQueue> unreserveQueue,
            bool canSkip,
            Vector2I[] chunks)
        {
            // Create the job
            var job = new ChunkJob(work, unreserveQueue, canSkip, chunks.Length);
            job.IsPendingChanged += this.Job_IsPendingChanged;
            job.Completed += this.Job_Completed;

            // Build the set of owners, checking whether the job can be enqueued
            ChunkJobQueue[] chunkQueues = new ChunkJobQueue[chunks.Length];
            for (int i = 0; i < chunks.Length; i++)
            {
                chunkQueues[i] = this.GetOrInitialiseQueue(chunks[i]);
            }

            // Reserve the owner queues
            foreach (ChunkJobQueue queue in chunkQueues)
            {
                reserveQueue(queue);
            }

            // Add the owners and enqueue the job
            job.AddOwners(chunkQueues);
            foreach (ChunkJobQueue queue in chunkQueues)
            {
                queue.Enqueue(job);
            }
        }

        /// <summary>
        /// Update the chunks that are currently active.
        /// </summary>
        /// <param name="activeChunks">The currently active chunks.</param>
        public void UpdateActiveChunks(HashSet<Vector2I> activeChunks)
        {
            List<Vector2I> removeNow = null;
            this.queuesLock.Enter();
            try
            {
                foreach (ChunkJobQueue queue in this.chunkQueues.Values)
                {
                    bool remove = !activeChunks.Contains(queue.Chunk);
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
        /// Execute the selector on each chunk queue and return true if all evaluate true. The predicate can safely
        /// remove the current item from the input list during iteration if desired.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="selector">The chunk queue selector.</param>
        /// <param name="queueMissingAction">Indicates how a non-existing chunk queue is handled when performing an
        /// operation on a chunk.</param>
        /// <returns>True if all chunk queues evaluate true.</returns>
        public bool ForAllChunks(Vector2I chunk, Predicate<ChunkJobQueue> selector, MissingQueue queueMissingAction)
        {
            return this.ForAllChunks(new Vector2I[] { chunk }, selector, queueMissingAction);
        }

        /// <summary>
        /// Execute the selector on each chunk queue and return true if all evaluate true. The predicate can safely
        /// remove the current item from the input list during iteration if desired.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="selector">The chunk queue selector.</param>
        /// <param name="queueMissingAction">Indicates how a non-existing chunk queue is handled when performing an
        /// operation on a chunk.</param>
        /// <returns>True if all chunk queues evaluate true.</returns>
        public bool ForAllChunks(IList chunks, Predicate<ChunkJobQueue> selector, MissingQueue queueMissingAction)
        {
            // Iterate backwards in case the predicate removes the current item from the collection
            for (int i = chunks.Count - 1; i >= 0; i--)
            {
                var chunk = (Vector2I)chunks[i];
                ChunkJobQueue queue;
                if (!this.chunkQueues.TryGetValue(chunk, out queue))
                {
                    switch (queueMissingAction)
                    {
                        case MissingQueue.CreateAndEvaluate:
                            queue = this.InitialiseQueue(chunk);
                            break;

                        case MissingQueue.Skip:
                            continue;

                        case MissingQueue.Fail:
                            return false;
                    }
                }

                if (!selector(queue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Remove chunks from the list that do not satisfy the selector condition.
        /// </summary>
        /// <param name="chunks">The chunks to trim from.</param>
        /// <param name="selector">The selector.</param>
        public void TrimChunks(List<Vector2I> chunks, Predicate<ChunkJobQueue> selector)
        {
            this.queuesLock.Enter();
            try
            {
                for (int i = chunks.Count - 1; i >= 0; i--)
                {
                    ChunkJobQueue queue;
                    if (this.chunkQueues.TryGetValue(chunks[i], out queue))
                    {
                        if (!selector(queue))
                        {
                            chunks.RemoveAt(i);
                        }
                    }
                }
            }
            finally
            {
                this.queuesLock.Exit();
            }
        }

        /// <summary>
        /// Gets the job queue for the given chunk, initialising the queue if one doesn't exist.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job queue.</returns>
        private ChunkJobQueue GetOrInitialiseQueue(Vector2I chunk)
        {
            ChunkJobQueue queue;
            if (!this.chunkQueues.TryGetValue(chunk, out queue))
            {
                queue = this.InitialiseQueue(chunk);
            }

            return queue;
        }

        /// <summary>
        /// Initialise the job queue for the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job queue.</returns>
        private ChunkJobQueue InitialiseQueue(Vector2I chunk)
        {
            var queue = new ChunkJobQueue(chunk, this.masterQueueJobs);
            queue.Idle += this.ChunkJobs_QueueIdle;
            this.chunkQueues.Add(chunk, queue);
            return queue;
        }

        /// <summary>
        /// Moves the owner queues of a job forward. This means each queue is stepped to the job after this one.
        /// </summary>
        /// <param name="job">The job.</param>
        private void MoveToNextJob(Job job)
        {
            if (job is MasterJob)
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