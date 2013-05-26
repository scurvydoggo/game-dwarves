// ----------------------------------------------------------------------------
// <copyright file="Job.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// The work delegate for a job.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="ct">The cancellation token for the job.</param>
    public delegate void JobWorkAction(object parameter, CancellationToken ct);

    /// <summary>
    /// Merge the work parameters of one job with another.
    /// </summary>
    /// <param name="target">The target of the merge.</param>
    /// <param name="source">The source of the merge.</param>
    public delegate void JobMergeAction(Job target, Job source);

    /// <summary>
    /// A job-related event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="job">The job.</param>
    public delegate void JobEvent(object sender, Job job);

    /// <summary>
    /// A job.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// The locking object for commencing job execution.
        /// </summary>
        private readonly object executionLock = new object();

        /// <summary>
        /// The work to be executed.
        /// </summary>
        private JobWorkAction work;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        private CancellationToken cancellation;

        /// <summary>
        /// The completion wait handle.
        /// </summary>
        private EventWaitHandle waitHandle;

        /// <summary>
        /// Initialises a new instance of the Job class.
        /// </summary>
        /// <param name="work">The work to be executed.</param>
        /// <param name="parameter">The input parameter to the work delegate.</param>
        /// <param name="info">The job information and access requirements.</param>
        public Job(JobWorkAction work, object parameter, JobInfo info)
        {
            this.work = work;
            this.cancellation = new CancellationToken();
            this.Id = JobId.GenerateId();
            this.WorkParameter = parameter;
            this.Info = info;
            this.Dependencies = new List<Job>();
            this.Dependents = new List<Job>();
        }

        /// <summary>
        /// Indicates that the job has completed, for better or for worse.
        /// </summary>
        public event JobEvent Completed;

        /// <summary>
        /// Gets the ID of the job.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the input parameter to the work delegate.
        /// </summary>
        public object WorkParameter { get; set; }

        /// <summary>
        /// Gets the information and access requirements.
        /// </summary>
        public JobInfo Info { get; private set; }

        /// <summary>
        /// Gets the jobs that this is dependent on.
        /// </summary>
        public List<Job> Dependencies { get; private set; }

        /// <summary>
        /// Gets the jobs that are dependent on this.
        /// </summary>
        public List<Job> Dependents { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the job is queued for execution.
        /// </summary>
        public bool IsQueuedForExecution { get; set; }

        /// <summary>
        /// Gets a value indicating whether the job has commenced execution.
        /// </summary>
        public bool IsCommenced { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the job has completed, for better or for worse.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the job faulted.
        /// </summary>
        public bool IsFaulted
        {
            get { return this.Error != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the job was cancelled.
        /// </summary>
        public bool IsCancelled
        {
            get { return this.cancellation.IsCancelled; }
        }

        /// <summary>
        /// Gets the error that occurred during execution.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Waits for all the jobs to complete.
        /// </summary>
        /// <param name="jobs">The jobs to wait on.</param>
        public static void WaitAll(IEnumerable<Job> jobs)
        {
            var toWait = new List<EventWaitHandle>();
            foreach (Job job in jobs)
            {
                if (job.TryInitialiseWaitHandle())
                {
                    toWait.Add(job.waitHandle);
                }
            }

            WaitHandle.WaitAll(toWait.ToArray());
        }

        /// <summary>
        /// Execute the job.
        /// </summary>
        public void Execute()
        {
            // Check whether the job has already started
            lock (this.executionLock)
            {
                if (this.IsCommenced)
                {
                    return;
                }

                this.IsCommenced = true;
            }

            // Execute the work
            if (!this.cancellation.IsCancelled)
            {
                try
                {
                    this.work.Invoke(this.WorkParameter, this.cancellation);
                }
                catch (Exception ex)
                {
                    if (!(ex is CancellationToken.CancelledException) ||
                        (ex as CancellationToken.CancelledException).Token != this.cancellation)
                    {
                        this.Error = ex;
                    }
                }
            }

            // Complete the job
            this.CompleteJob();
        }

        /// <summary>
        /// Cancel the job.
        /// </summary>
        public void Cancel()
        {
            // Set the cancellation flag
            this.cancellation.Cancel();

            // Prevent the job from starting
            bool forceCompletion = false;
            lock (this.executionLock)
            {
                if (!this.IsCommenced)
                {
                    this.IsCommenced = true;
                    forceCompletion = true;
                }
            }

            // Force complete the job if it hadn't started
            if (forceCompletion)
            {
                this.CompleteJob();
            }
        }

        /// <summary>
        /// Waits for the job to complete.
        /// </summary>
        public void Wait()
        {
            if (this.TryInitialiseWaitHandle())
            {
                this.waitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Attempt to merge the input parameters of the given job.
        /// </summary>
        /// <param name="job">The job to merge.</param>
        /// <returns>True if the merge succeeded; False if the job is executing and cannot be merged.</returns>
        public bool TryMerge(Job job)
        {
            // Aquire the lock to prevent commencement of execution while merging
            lock (this.executionLock)
            {
                if (this.IsCommenced)
                {
                    // The job has already started execution so cannot be merged
                    return false;
                }

                JobMergeAction mergeAction = JobSystem.Instance.GetMergeAction(job.Info.Behaviour);
                mergeAction(this, job);
            }

            return true;
        }

        /// <summary>
        /// Determine whether this is considered a reusable substitute for the given job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if the job is a duplicate.</returns>
        public bool IsSubstitute(Job job)
        {
            return
                this.Info.Behaviour != JobBehaviour.General &&
                this.Info.Behaviour == job.Info.Behaviour &&
                this.Info.HasAllChunks(job.Info);
        }

        /// <summary>
        /// Determine whether any dependencies of this are dependent on the given job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if this is dependent.</returns>
        public bool HasSubDependency(Job job)
        {
            foreach (Job dependency in this.Dependencies)
            {
                if (dependency.HasDependency(job))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recursively determine whether the given job is dependent on any dependents of this.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if this has a dependent.</returns>
        public bool HasSubDependent(Job job)
        {
            foreach (Job dependent in this.Dependents)
            {
                if (dependent.HasDependent(job))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recursively enumerates all the dependencies.
        /// </summary>
        /// <returns>The dependencies.</returns>
        public IEnumerable<Job> GetAllDependencies()
        {
            foreach (Job dependency in this.Dependencies)
            {
                foreach (Job subDependency in dependency.GetAllDependencies())
                {
                    yield return subDependency;
                }

                yield return dependency;
            }
        }

        /// <summary>
        /// Completes the job.
        /// </summary>
        private void CompleteJob()
        {
            lock (this.executionLock)
            {
                // Flag the job as completed
                this.IsCompleted = true;
                if (this.waitHandle != null)
                {
                    this.waitHandle.Set();
                }
            }

            // Notify listeners
            if (this.Completed != null)
            {
                this.Completed(this, this);
            }
        }

        /// <summary>
        /// Recursively determine whether this is dependent on the given job.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if this is dependent.</returns>
        private bool HasDependency(Job job)
        {
            foreach (Job dependency in this.Dependencies)
            {
                if (dependency == job || dependency.HasDependency(job))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recursively determine whether the given job is dependent on this.
        /// </summary>
        /// <param name="job">The job.</param>
        /// <returns>True if this has a dependent.</returns>
        private bool HasDependent(Job job)
        {
            foreach (Job dependent in this.Dependents)
            {
                if (dependent == job || dependent.HasDependent(job))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initialises the wait handle for job completion if the job has not yet finished.
        /// </summary>
        /// <returns>True if the wait handle exists.</returns>
        private bool TryInitialiseWaitHandle()
        {
            lock (this.executionLock)
            {
                if (!this.IsCompleted)
                {
                    if (this.waitHandle == null)
                    {
                        this.waitHandle = new ManualResetEvent(false);
                    }

                    return true;
                }
                else
                {
                    // The job has completed - we don't need no wait handles
                    return false;
                }
            }
        }
    }
}