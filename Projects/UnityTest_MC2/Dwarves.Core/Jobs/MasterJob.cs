// ----------------------------------------------------------------------------
// <copyright file="MasterJob.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;

    /// <summary>
    /// The MasterJob class.
    /// </summary>
    public class MasterJob : Job
    {
        /// <summary>
        /// The finalisation to be executed on the master queue on completion.
        /// </summary>
        private Action<MasterJobQueue> finalise;

        /// <summary>
        /// The master queue.
        /// </summary>
        private MasterJobQueue masterQueue;

        /// <summary>
        /// Initialises a new instance of the MasterJob class.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="finalise">The finalisation to be executed on the master queue on completion.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="ownerCapacity">The initial capacity for the owners list.</param>
        public MasterJob(Action work, Action<MasterJobQueue> finalise, bool canSkip, int ownerCapacity)
            : base(work, canSkip, ownerCapacity)
        {
            this.finalise = finalise;
        }

        /// <summary>
        /// Adds owner queues.
        /// </summary>
        /// <param name="owners">The owners.</param>
        public override void AddOwners(params JobQueue[] owners)
        {
            base.AddOwners(owners);
            foreach (JobQueue owner in owners)
            {
                if (owner is MasterJobQueue)
                {
                    this.masterQueue = (MasterJobQueue)owner;
                }
            }
        }

        /// <summary>
        /// Finalises the owner queue(s) on the completion of the job.
        /// </summary>
        protected override void FinaliseOwners()
        {
            if (this.finalise != null && this.masterQueue != null)
            {
                this.finalise(this.masterQueue);
            }
        }
    }
}