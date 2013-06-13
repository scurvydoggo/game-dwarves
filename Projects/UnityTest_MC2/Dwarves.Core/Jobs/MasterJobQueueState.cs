// ----------------------------------------------------------------------------
// <copyright file="MasterJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using Dwarves.Core.Math;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The state of the master job queue.
    /// </summary>
    public class MasterJobQueueState
    {
        /// <summary>
        /// Initialises a new instance of the MasterJobQueueState class.
        /// </summary>
        public MasterJobQueueState()
        {
        }

        /// <summary>
        /// Check whether a AddChunks job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanAddChunks(List<Vector2I> chunks)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Completes a AddChunks job.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        public void CompleteAddChunks(List<Vector2I> chunks)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether a RemoveChunks job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanRemoveChunks(List<Vector2I> chunks)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Completes a RemoveChunks job.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        public void CompleteRemoveChunks(List<Vector2I> chunks)
        {
            throw new NotImplementedException();
        }
    }
}