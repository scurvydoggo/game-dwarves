// ----------------------------------------------------------------------------
// <copyright file="ChunkJob.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;

    /// <summary>
    /// The ChunkJob class.
    /// </summary>
    public class ChunkJob : Job
    {
        /// <summary>
        /// The finalisation to be executed on each owner queue on completion.
        /// </summary>
        private Action<ChunkJobQueue> finalise;

        /// <summary>
        /// Initialises a new instance of the ChunkJob class.
        /// </summary>
        /// <param name="work">The work to be executed by the job.</param>
        /// <param name="finalise">The finalisation to be executed on each owner on completion.</param>
        /// <param name="canSkip">Indicates whether the job can be skipped.</param>
        /// <param name="ownerCapacity">The initial capacity for the owners list.</param>
        public ChunkJob(Action work, Action<ChunkJobQueue> finalise, bool canSkip, int ownerCapacity)
            : base(work, canSkip, ownerCapacity)
        {
            this.finalise = finalise;
        }

        /// <summary>
        /// Finalises the owner queue(s) on the completion of the job.
        /// </summary>
        protected override void FinaliseOwners()
        {
            if (this.finalise != null)
            {
                foreach (JobQueue owner in this.Owners)
                {
                    this.finalise(owner as ChunkJobQueue);
                }
            }
        }
    }
}