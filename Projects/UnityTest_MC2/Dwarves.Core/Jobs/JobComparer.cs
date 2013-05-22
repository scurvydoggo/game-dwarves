// ----------------------------------------------------------------------------
// <copyright file="JobComparer.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections.Generic;

    /// <summary>
    /// Performs comparisons on jobs to determine dependencies.
    /// </summary>
    public class JobComparer : IComparer<Job>
    {
        /// <summary>
        /// Compares two jobs and returns a value indicating whether one is dependent on the other or if they are
        /// independent.
        /// </summary>
        /// <param name="x">The first job.</param>
        /// <param name="y">The second job.</param>
        /// <returns>The comparison result. Less than 0: x depends on y; Zero: x and y are independent; Greater than
        /// 0: y depends on x.</returns>
        public int Compare(Job x, Job y)
        {
            if (x.Info.Write != null && y.Info.Write != null && x.Info.Write.Type == y.Info.Write.Type)
            {
                // Chunk loading is a special case which has precedence over other jobs, even with no chunks in common
                if (x.Info.Write.Type == JobAccessType.ChunkLoading || x.Info.Write.HasAnyChunks(y.Info.Write))
                {
                    // Check if there is a specified write order; if not, order by the respective job IDs
                    int writeOrderDiff = x.Info.WriteOrder - y.Info.WriteOrder;
                    if (writeOrderDiff != 0)
                    {
                        return writeOrderDiff;
                    }
                    else
                    {
                        return x.Id - y.Id;
                    }
                }

                // The jobs are independent (we know neither job can have a read task that occurs after this write)
                return 0;
            }
            else if (x.Info.Write != null && this.WriteHasDependents(x, y))
            {
                // Job y depends on x
                return 1;
            }
            else if (y.Info.Write != null)
            {
                // Job x depends on y
                return -1;
            }
            else
            {
                // The jobs are independent
                return 0;
            }
        }

        /// <summary>
        /// Determine whether a job depends on the completion of the write task of the given write job.
        /// </summary>
        /// <param name="writeJob">The job performing the write.</param>
        /// <param name="other">The other job.</param>
        /// <returns>True if the job depends on the completion of the write task.</returns>
        private bool WriteHasDependents(Job writeJob, Job other)
        {
            // Chunk loading is a special case which has precedence over other jobs, even with no chunks in common
            if (writeJob.Info.Write.Type == JobAccessType.ChunkLoading)
            {
                return true;
            }

            // Check if any of the read tasks require the same access type as the write job and have chunks in common
            foreach (JobAccess read in other.Info.Reads)
            {
                if (read.Type == writeJob.Info.Write.Type && read.HasAnyChunks(writeJob.Info.Write))
                {
                    return true;
                }
            }

            return false;
        }
    }
}