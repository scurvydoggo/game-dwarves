// ----------------------------------------------------------------------------
// <copyright file="JobPool.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// The pool of worker threads for executing jobs.
    /// </summary>
    public class JobPool : IDisposable
    {
        /// <summary>
        /// Locking object.
        /// </summary>
        private readonly object queueLock = new object();

        /// <summary>
        /// The queue of jobs ready to be executed.
        /// </summary>
        private Queue<Job> queue;

        /// <summary>
        /// The threads.
        /// </summary>
        private Thread[] threads;

        /// <summary>
        /// Event handle indicating a job being queued.
        /// </summary>
        private AutoResetEvent jobQueuedEvent;

        /// <summary>
        /// Event handle indicating a job being queued.
        /// </summary>
        private ManualResetEvent exitThreadEvent;

        /// <summary>
        /// The events to wait on.
        /// </summary>
        private WaitHandle[] events;

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
            this.queue = new Queue<Job>();
            this.jobQueuedEvent = new AutoResetEvent(false);
            this.exitThreadEvent = new ManualResetEvent(false);
            this.events = new WaitHandle[] { this.jobQueuedEvent, this.exitThreadEvent };

            // Fire up the worker threads
            this.threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                this.threads[i] = new Thread(this.Run);
                this.threads[i].Start();
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
                this.exitThreadEvent.Set();
                for (int i = 0; i < this.threads.Length; i++)
                {
                    this.threads[i].Join();
                }

                this.exitThreadEvent.Close();
                this.jobQueuedEvent.Close();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Enqueue a job for execution.
        /// </summary>
        /// <param name="job">The job.</param>
        public void Enqueue(Job job)
        {
            lock (this.queueLock)
            {
                this.queue.Enqueue(job);
            }

            this.jobQueuedEvent.Set();
        }

        /// <summary>
        /// The worker execution method.
        /// </summary>
        private void Run()
        {
            while (WaitHandle.WaitAny(this.events) == 0)
            {
                Job job;
                while ((job = this.GetJob()) != null)
                {
                    job.Execute();
                }
            }
        }

        /// <summary>
        /// Gets the next job on the queue.
        /// </summary>
        /// <returns>The job; Null if the queue was empty.</returns>
        private Job GetJob()
        {
            lock (this.queueLock)
            {
                if (this.queue.Count > 0)
                {
                    return this.queue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}