// ----------------------------------------------------------------------------
// <copyright file="JobSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Jobs;

    /// <summary>
    /// The job sub-system.
    /// </summary>
    public class JobSystem
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static JobSystem instance = new JobSystem();

        /// <summary>
        /// The jobs to wait on this frame.
        /// </summary>
        private List<Job> frameJobs;

        /// <summary>
        /// The merge actions delegates that are registered for job behaviours.
        /// </summary>
        private Dictionary<JobBehaviour, JobMergeAction> mergeActions;

        /// <summary>
        /// Prevents a default instance of the JobSystem class from being created.
        /// </summary>
        private JobSystem()
        {
            this.frameJobs = new List<Job>();
            this.mergeActions = new Dictionary<JobBehaviour, JobMergeAction>();
            this.Scheduler = new JobScheduler();

            // Register the merge behaviours
            this.RegisterMergeActions();
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static JobSystem Instance
        {
            get { return JobSystem.instance; }
        }

        /// <summary>
        /// Gets the job scheduler.
        /// </summary>
        public JobScheduler Scheduler { get; private set; }

        /// <summary>
        /// Adds a job to be completed this frame.
        /// </summary>
        /// <param name="job">The job.</param>
        public void AddFrameJob(Job job)
        {
            lock ((this.frameJobs as ICollection).SyncRoot)
            {
                this.frameJobs.Add(job);
            }
        }

        /// <summary>
        /// Wait for the the jobs to be completed this frame.
        /// </summary>
        public void WaitForFrameJobs()
        {
            Job[] toWait;
            lock ((this.frameJobs as ICollection).SyncRoot)
            {
                toWait = this.frameJobs.ToArray();
                this.frameJobs.Clear();
            }

            Job.WaitAll(toWait);
        }

        /// <summary>
        /// Register a merge action for the given job behaviour.
        /// </summary>
        /// <param name="behaviour">The job behaviour.</param>
        /// <param name="mergeAction">The delegate for merging the input parameters of one job to another.</param>
        public void RegisterMergeAction(JobBehaviour behaviour, JobMergeAction mergeAction)
        {
            this.mergeActions.Add(behaviour, mergeAction);
        }

        /// <summary>
        /// Gets the merge action delegate for the given job behaviour.
        /// </summary>
        /// <param name="behaviour">The job behaviour.</param>
        /// <returns>The merge action.</returns>
        public JobMergeAction GetMergeAction(JobBehaviour behaviour)
        {
            JobMergeAction mergeAction;

            if (!this.mergeActions.TryGetValue(behaviour, out mergeAction))
            {
                throw new InvalidOperationException("Merge action does not exist.");
            }

            return mergeAction;
        }

        /// <summary>
        /// Register the merge action delegates.
        /// </summary>
        private void RegisterMergeActions()
        {
            // Add surface heights
            JobMergeAction addSurfaceHeights = (target, source) =>
                {
                    foreach (int x in source.WorkParameter as HashSet<int>)
                    {
                        (target.WorkParameter as HashSet<int>).Add(x);
                    }
                };
            this.RegisterMergeAction(JobBehaviour.AddSurfaceHeights, addSurfaceHeights);
        }
    }
}