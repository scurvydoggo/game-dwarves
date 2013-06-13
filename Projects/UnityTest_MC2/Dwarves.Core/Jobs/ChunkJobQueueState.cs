// ----------------------------------------------------------------------------
// <copyright file="ChunkJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// The state of a chunk job queue.
    /// </summary>
    public class ChunkJobQueueState
    {
        /// <summary>
        /// The locking object for RebuildMesh.
        /// </summary>
        private readonly object rebuildMeshLock = new object();

        /// <summary>
        /// Indicates whether the chunk mesh is currently being rebuild.
        /// </summary>
        private bool rebuildMesh;
        
        /// <summary>
        /// Initialises a new instance of the ChunkJobQueueState class.
        /// </summary>
        public ChunkJobQueueState()
        {
        }

        /// <summary>
        /// Prepares a RebuildMesh job.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanRebuildMesh()
        {
            lock (this.rebuildMeshLock)
            {
                if (!this.rebuildMesh)
                {
                    this.rebuildMesh = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Completes a RebuildMesh job.
        /// </summary>
        public void CompleteRebuildMesh()
        {
            lock (this.rebuildMeshLock)
            {
                this.rebuildMesh = false;
            }
        }
    }
}