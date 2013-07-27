// ----------------------------------------------------------------------------
// <copyright file="JobSystem.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
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
        /// Prevents a default instance of the JobSystem class from being created.
        /// </summary>
        private JobSystem()
        {
            this.Scheduler = new JobScheduler(Environment.ProcessorCount);
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
    }
}