// ----------------------------------------------------------------------------
// <copyright file="Job.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// A job event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="job">The job.</param>
    public delegate void JobEvent(object sender, Job job);

    /// <summary>
    /// The Job class.
    /// </summary>
    public abstract class Job
    {
        /// <summary>
        /// The work to be executed by the job.
        /// </summary>
        private Action work;

        /// <summary>
        /// Indicates whether the job can be skipped.
        /// </summary>
        private bool canSkip;

        /// <summary>
        /// The number of queues that are pending this job.
        /// </summary>
        private int pendingCount;

        /// <summary>
        /// The number of queues that wish to skip this job.
        /// </summary>
        private int skipCount;

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="ownerCapacity">The initial capacity for the owners list.</param>
        public Job(Action work, bool canSkip, int ownerCapacity)
        {
            this.work = work;
            this.canSkip = canSkip;
            this.Owners = new List<JobQueue>(ownerCapacity);
        }

        /// <summary>
        /// The job is pending execution.
        /// </summary>
        public event JobEvent IsPendingChanged;

        /// <summary>
        /// The job completed execution.
        /// </summary>
        public event JobEvent Completed;

        /// <summary>
        /// Gets a value indicating whether all owner queues are pending this job.
        /// </summary>
        public bool IsPending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all owner queues have requested to skip this job.
        /// </summary>
        public bool IsSkipRequested
        {
            get { return this.canSkip && this.skipCount >= this.Owners.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the job completed execution.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets the owners of the job.
        /// </summary>
        protected List<JobQueue> Owners { get; private set; }

        /// <summary>
        /// Execute the job.
        /// </summary>
        public void Execute()
        {
            try
            {
                // Perform the work
                this.work();
            }
            finally
            {
                try
                {
                    this.FinaliseOwners();
                }
                catch
                {
                }
            }

            this.IsCompleted = true;
            if (this.Completed != null)
            {
                this.Completed(this, this);
            }
        }

        /// <summary>
        /// Increments the number of owner queues that are pending this job.
        /// </summary>
        /// <param name="requestSkip">Indicates that the owner queue wishes to skip this job.</param>
        public void IncrementPendingQueues(bool requestSkip)
        {
            if (!this.IsPending)
            {
                int pendingCount = Interlocked.Increment(ref this.pendingCount);
                if (requestSkip)
                {
                    Interlocked.Increment(ref this.skipCount);
                }

                if (pendingCount == this.Owners.Count)
                {
                    this.IsPending = true;
                    this.IsPendingChanged(this, this);
                }
            }
        }

        /// <summary>
        /// Gets the owners.
        /// </summary>
        /// <returns>The owners.</returns>
        public IEnumerable<JobQueue> GetOwners()
        {
            return this.Owners;
        }

        /// <summary>
        /// Adds owner queues.
        /// </summary>
        /// <param name="owners">The owners.</param>
        public virtual void AddOwners(params JobQueue[] owners)
        {
            this.Owners.AddRange(owners);
        }

        /// <summary>
        /// Finalises the owner queue(s) on the completion of the job.
        /// </summary>
        protected abstract void FinaliseOwners();
    }
}