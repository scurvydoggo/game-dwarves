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
    public class Job
    {
        /// <summary>
        /// The action delegate.
        /// </summary>
        private Action action;

        /// <summary>
        /// Indicates whether the job can be skipped.
        /// </summary>
        private bool canSkip;

        /// <summary>
        /// The owners of the job.
        /// </summary>
        private List<JobQueue> owners;

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
        /// <param name="action">The action delegate.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="isMasterJob">Indicates whether this is a master job.</param>
        /// <param name="ownerCapacity">The initial capacity for the owners list.</param>
        public Job(Action action, bool canSkip, bool isMasterJob, int ownerCapacity)
        {
            this.action = action;
            this.canSkip = canSkip;
            this.IsMasterJob = isMasterJob;
            this.owners = new List<JobQueue>(ownerCapacity);
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
        /// Gets a value indicating whether this is a master job.
        /// </summary>
        public bool IsMasterJob { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all owner queues are pending this job.
        /// </summary>
        public bool IsPending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether all owner queues have requested to skip this job.
        /// </summary>
        public bool IsSkipRequested
        {
            get { return this.canSkip && this.skipCount >= this.owners.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the job completed execution.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Execute the job.
        /// </summary>
        public void Execute()
        {
            this.action();

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

                if (pendingCount == this.owners.Count)
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
            return this.owners;
        }

        /// <summary>
        /// Adds owner queues.
        /// </summary>
        /// <param name="owners">The owners.</param>
        public void AddOwners(params JobQueue[] owners)
        {
            this.owners.AddRange(owners);
        }
    }
}