// ----------------------------------------------------------------------------
// <copyright file="MasterJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The state of the master job queue.
    /// </summary>
    public class MasterJobQueueState
    {
        /// <summary>
        /// The locking object for AddSurfaceHeights.
        /// </summary>
        private readonly object addSurfaceHeightsLock = new object();

        /// <summary>
        /// The currently queued items for AddSurfaceHeights.
        /// </summary>
        private HashSet<int> addSurfaceHeightsSet;

        /// <summary>
        /// Initialises a new instance of the MasterJobQueueState class.
        /// </summary>
        public MasterJobQueueState()
        {
            this.addSurfaceHeightsSet = new HashSet<int>();
        }

        #region AddSurfaceHeights

        /// <summary>
        /// Prepares a AddSurfaceHeights job. Duplicates of existing work are trimmed from the given positions.
        /// </summary>
        /// <param name="positions">The chunk x positions.</param>
        /// <returns>True if the job can be executed.</returns>
        public bool CanAddSurfaceHeights(HashSet<int> positions)
        {
            // Add the new surface heights, checking for duplicates
            var duplicates = new List<int>();
            lock (this.addSurfaceHeightsLock)
            {
                foreach (int x in positions)
                {
                    if (!this.addSurfaceHeightsSet.Add(x))
                    {
                        duplicates.Add(x);
                    }
                }
            }

            // Trim the duplicates from the input set
            foreach (int x in duplicates)
            {
                positions.Remove(x);
            }

            return positions.Count > 0;
        }

        /// <summary>
        /// Completes a AddSurfaceHeights job for the given positions.
        /// </summary>
        /// <param name="positions">The chunk x positions.</param>
        public void CompleteAddSurfaceHeights(HashSet<int> positions)
        {
            lock (this.addSurfaceHeightsLock)
            {
                foreach (int x in positions)
                {
                    this.addSurfaceHeightsSet.Remove(x);
                }
            }
        }

        #endregion
    }
}