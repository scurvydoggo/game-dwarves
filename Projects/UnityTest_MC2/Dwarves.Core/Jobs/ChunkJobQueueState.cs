// ----------------------------------------------------------------------------
// <copyright file="ChunkJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The state of a chunk job queue.
    /// </summary>
    public class ChunkJobQueueState
    {
        /// <summary>
        /// The locking object for jobs.
        /// </summary>
        private readonly object jobsLock = new object();

        /// <summary>
        /// Indicates whether the point data will be newer than the mesh data once all queued jobs are complete.
        /// </summary>
        private bool rebuildMeshRequired;

        /// <summary>
        /// Indicates whether the mesh data will be newer than the mesh filter once all queued jobs are complete.
        /// </summary>
        private bool meshFilterUpdateRequired;

        /// <summary>
        /// Indicates whether a job is queued to load the points.
        /// </summary>
        private bool loadPoints;

        /// <summary>
        /// Indicates whether a job is queued to rebuild the chunk mesh.
        /// </summary>
        private bool rebuildingMesh;

        /// <summary>
        /// Indicates whether a job is queued to update the mesh filter.
        /// </summary>
        private bool updatingMeshFilter;

        /// <summary>
        /// The dictionary tracking the current dig circle jobs.
        /// </summary>
        private Dictionary<Vector2I, int> digCircle;

        /// <summary>
        /// Initialises a new instance of the ChunkJobQueueState class.
        /// </summary>
        public ChunkJobQueueState()
        {
            this.digCircle = new Dictionary<Vector2I, int>();
        }

        /// <summary>
        /// Check whether a LoadPoints job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanLoadPoints()
        {
            lock (this.jobsLock)
            {
                if (!this.loadPoints)
                {
                    this.loadPoints = true;
                    this.rebuildMeshRequired = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Completes a LoadPoints job.
        /// </summary>
        public void CompleteLoadPoints()
        {
            lock (this.jobsLock)
            {
                this.loadPoints = false;
            }
        }

        /// <summary>
        /// Check whether a DigCircle job can execute.
        /// </summary>
        /// <param name="origin">The circle origin.</param>
        /// <param name="radius">The circle radius.</param>
        /// <returns>True if the job can be executed.</returns>
        public bool CanDigCircle(Vector2I origin, int radius)
        {
            lock (this.jobsLock)
            {
                int existing;
                if (this.digCircle.TryGetValue(origin, out existing))
                {
                    if (radius > existing)
                    {
                        this.digCircle[origin] = radius;
                        this.rebuildMeshRequired = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    this.digCircle.Add(origin, radius);
                    this.rebuildMeshRequired = true;
                    return true;
                }
            }
        }

        /// <summary>
        /// Completes a DigCircle job.
        /// </summary>
        /// <param name="origin">The circle origin.</param>
        /// <param name="radius">The circle radius.</param>
        public void CompleteDigCircle(Vector2I origin, int radius)
        {
            lock (this.jobsLock)
            {
                if (this.digCircle[origin] == radius)
                {
                    this.digCircle.Remove(origin);
                }
            }
        }

        /// <summary>
        /// Check whether a RebuildMesh job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanRebuildMesh()
        {
            lock (this.jobsLock)
            {
                if (!this.rebuildingMesh && this.rebuildMeshRequired)
                {
                    this.rebuildingMesh = true;
                    this.rebuildMeshRequired = false;
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
            lock (this.jobsLock)
            {
                this.rebuildingMesh = false;
            }
        }

        /// <summary>
        /// Check whether a UpdateMeshFilter job can execute.
        /// </summary>
        /// <returns>True if the job can be executed.</returns>
        public bool CanUpdateMeshFilter()
        {
            lock (this.jobsLock)
            {
                if (!this.updatingMeshFilter && this.meshFilterUpdateRequired)
                {
                    this.updatingMeshFilter = true;
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
            lock (this.jobsLock)
            {
                this.updatingMeshFilter = false;
            }
        }
    }
}