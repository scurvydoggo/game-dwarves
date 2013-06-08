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
        /// The chunks being removed.
        /// </summary>
        private HashSet<Vector2I> chunksToRemove;

        /// <summary>
        /// Initialises a new instance of the MasterJobQueueState class.
        /// </summary>
        public MasterJobQueueState()
        {
            this.chunksToRemove = new HashSet<Vector2I>();
        }

        /// <summary>
        /// Add the given chunks to the set to be removed.
        /// </summary>
        /// <param name="chunks">The chunks to remove.</param>
        /// <returns>True if at least one of the given chunks was added.</returns>
        public bool AddChunksToRemove(List<Vector2I> chunks)
        {
            bool added = false;
            lock ((this.chunksToRemove as ICollection).SyncRoot)
            {
                foreach (Vector2I chunk in chunks)
                {
                    added |= this.chunksToRemove.Add(chunk);
                }
            }

            return added;
        }
    }
}