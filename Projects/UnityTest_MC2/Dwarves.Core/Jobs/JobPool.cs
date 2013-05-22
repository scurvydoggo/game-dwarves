// ----------------------------------------------------------------------------
// <copyright file="JobPool.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Dwarves.Core.Math;

    /// <summary>
    /// Performs asynchronous execution of jobs by a pool of worker threads.
    /// </summary>
    public class JobPool : IDisposable
    {
        /// <summary>
        /// The lock for the queue.
        /// </summary>
        private readonly object queueLock = new object();

        /// <summary>
        /// The job queue.
        /// </summary>
        private LinkedList<Job> queue;

        /// <summary>
        /// The last node in the queue with a prioritised job.
        /// </summary>
        private LinkedListNode<Job> lastPriorityNode;

        /// <summary>
        /// The priority chunks.
        /// </summary>
        private Vector2I[] priorityChunks;

        /// <summary>
        /// The threads.
        /// </summary>
        private Thread[] threads;

        /// <summary>
        /// Wait handle for the item queued event.
        /// </summary>
        private EventWaitHandle itemQueuedEvent;

        /// <summary>
        /// Wait handle for the exit queue event.
        /// </summary>
        private EventWaitHandle exitQueueEvent;

        /// <summary>
        /// The queue events.
        /// </summary>
        private EventWaitHandle[] queueEvents;

        /// <summary>
        /// Indicates whether the instance has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initialises a new instance of the JobPool class.
        /// </summary>
        /// <param name="threadCount">The number of threads to spawn.</param>
        public JobPool(int threadCount)
        {
            this.queue = new LinkedList<Job>();
            this.priorityChunks = new Vector2I[0];
            this.itemQueuedEvent = new AutoResetEvent(false);
            this.exitQueueEvent = new ManualResetEvent(false);
            this.queueEvents = new EventWaitHandle[] { this.itemQueuedEvent, this.exitQueueEvent };

            // Fire up the worker threads
            this.threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                this.threads[i] = new Thread(this.Run);
                this.threads[i].Start();
            }
        }

        /// <summary>
        /// Gets or sets the chunks with priority.
        /// </summary>
        public Vector2I[] PriorityChunks
        {
            get
            {
                return this.priorityChunks;
            }

            set
            {
                if (this.priorityChunks != value)
                {
                    this.priorityChunks = value;
                    this.SortQueue();
                }
            }
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                // Exit all threads
                this.exitQueueEvent.Set();
                for (int i = 0; i < this.threads.Length; i++)
                {
                    this.threads[i].Join();
                }

                this.exitQueueEvent.Close();
                this.itemQueuedEvent.Close();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Adds the job to the end of the queue.
        /// </summary>
        /// <param name="job">The job.</param>
        public void Enqueue(Job job)
        {
            lock (this.queueLock)
            {
                // Check if this is a priority job, in which case it is queued towards the front
                if (this.IsPriorityJob(job))
                {
                    if (this.lastPriorityNode != null)
                    {
                        this.lastPriorityNode = this.queue.AddAfter(this.lastPriorityNode, job);
                    }
                    else
                    {
                        this.lastPriorityNode = this.queue.AddFirst(job);
                    }
                }
                else
                {
                    this.queue.AddLast(job);
                }

                this.itemQueuedEvent.Set();
            }
        }

        /// <summary>
        /// Removes the given job from the queue.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if the job was removed.</returns>
        public bool Remove(Job job)
        {
            lock (this.queueLock)
            {
                LinkedListNode<Job> node = this.queue.Find(job);
                if (node != null)
                {
                    // If this is the last priority node, update the reference
                    if (node == this.lastPriorityNode)
                    {
                        this.lastPriorityNode = node.Previous;
                    }

                    this.queue.Remove(node);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The worker execution method.
        /// </summary>
        private void Run()
        {
            Job job;
            while (WaitHandle.WaitAny(this.queueEvents) != 1)
            {
                while ((job = this.Dequeue()) != null)
                {
                    job.Execute();
                }
            }
        }

        /// <summary>
        /// Takes a job from the queue.
        /// </summary>
        /// <returns>The job.</returns>
        private Job Dequeue()
        {
            lock (this.queueLock)
            {
                if (this.queue.Count > 0)
                {
                    // Take the first node
                    LinkedListNode<Job> node = this.queue.First;
                    this.queue.RemoveFirst();

                    // Check if this is the last priority node, in which case clear the reference
                    if (this.lastPriorityNode == node)
                    {
                        this.lastPriorityNode = null;
                    }

                    return node.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Sorts the queue.
        /// </summary>
        private void SortQueue()
        {
            lock (this.queueLock)
            {
                if (this.queue.Count > 0)
                {
                    // Clear the last priority node reference
                    this.lastPriorityNode = null;

                    // Step through each node in the list and move it towards the front if it is a priority
                    LinkedListNode<Job> node = this.queue.First;
                    while (node.Next != null)
                    {
                        LinkedListNode<Job> next = node.Next;

                        // If this is a priority job, move it after the last priority node
                        if (this.IsPriorityJob(node.Value))
                        {
                            if (this.lastPriorityNode != null)
                            {
                                if (node.Previous != this.lastPriorityNode)
                                {
                                    this.queue.Remove(node);
                                    this.queue.AddAfter(this.lastPriorityNode, node);
                                }
                            }
                            else
                            {
                                if (node != this.queue.First)
                                {
                                    this.queue.Remove(node);
                                    this.queue.AddFirst(node);
                                }
                            }

                            this.lastPriorityNode = node;
                        }

                        node = next;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the given job is a priority.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if the job is a priority.</returns>
        private bool IsPriorityJob(Job job)
        {
            return this.PriorityChunks.Any((c) => job.Info.HasChunk(c));
        }
    }
}