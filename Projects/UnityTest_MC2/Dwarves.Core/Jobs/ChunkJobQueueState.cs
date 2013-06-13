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
        /// The locking object for mesh related jobs.
        /// </summary>
        private readonly object meshLock = new object();

        /// <summary>
        /// Indicates whether the mesh data will be newer than the mesh filter once all queued jobs are complete.
        /// </summary>
        private bool meshFilterUpdateRequired;

        /// <summary>
        /// Indicates whether a job is queued to rebuild the chunk mesh.
        /// </summary>
        private bool rebuildMesh;

        /// <summary>
        /// Indicates whether a job is queued to update the mesh filter.
        /// </summary>
        private bool updateMeshFilter;

        /// <summary>
        /// Initialises a new instance of the ChunkJobQueueState class.
        /// </summary>
        public ChunkJobQueueState()
        {
        }

        /// <summary>
        /// Check whether a LoadPoints job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanLoadPoints()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Completes a LoadPoints job.
        /// </summary>
        public void CompleteLoadPoints()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Check whether a RebuildMesh job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanRebuildMesh()
        {
            lock (this.meshLock)
            {
                if (!this.rebuildMesh)
                {
                    this.rebuildMesh = true;
                    this.meshFilterUpdateRequired = true;
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
            lock (this.meshLock)
            {
                this.rebuildMesh = false;
            }
        }

        /// <summary>
        /// Check whether a UpdateMeshFilter job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanUpdateMeshFilter()
        {
            lock (this.meshLock)
            {
                if (!this.updateMeshFilter && this.meshFilterUpdateRequired)
                {
                    this.updateMeshFilter = true;
                    this.meshFilterUpdateRequired = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Completes a UpdateMeshFilter job.
        /// </summary>
        public void CompleteUpdateMeshFilter()
        {
            lock (this.meshLock)
            {
                this.updateMeshFilter = false;
            }
        }
    }
}