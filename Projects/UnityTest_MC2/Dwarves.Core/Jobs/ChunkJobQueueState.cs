// ----------------------------------------------------------------------------
// <copyright file="ChunkJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The state of a chunk job queue.
    /// </summary>
    public class ChunkJobQueueState
    {
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
        /// <param name="chunk">The chunk.</param>
        public ChunkJobQueueState(Vector2I chunk)
        {
            this.Chunk = chunk;
            this.digCircle = new Dictionary<Vector2I, int>();
        }

        /// <summary>
        /// Gets the chunk.
        /// </summary>
        public Vector2I Chunk { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a load points job has completed.
        /// </summary>
        public bool LoadPointsCompleted { get; private set; }

        #region Load Points

        /// <summary>
        /// Check whether a LoadPoints job can execute.
        /// </summary>
        /// <param name="chunk">The chunk being loaded.</param>
        /// <returns>True if the job can be enqueued.</returns>
        public bool CanLoadPoints(Vector2I chunk)
        {
            return this.Chunk != chunk || !this.loadPoints;
        }

        /// <summary>
        /// Reserves a LoadPoints job.
        /// </summary>
        /// <param name="chunk">The chunk being loaded.</param>
        public void ReserveLoadPoints(Vector2I chunk)
        {
            if (this.Chunk == chunk)
            {
                this.loadPoints = true;
            }

            this.rebuildMeshRequired = true;
        }

        /// <summary>
        /// Un-reserves a LoadPoints job.
        /// </summary>
        /// <param name="chunk">The chunk being loaded.</param>
        public void UnreserveLoadPoints(Vector2I chunk)
        {
            if (this.Chunk == chunk)
            {
                this.loadPoints = false;
                this.LoadPointsCompleted = true;
            }
        }

        #endregion

        #region Rebuild Mesh

        /// <summary>
        /// Check whether a RebuildMesh job can execute.
        /// </summary>
        /// <param name="chunk">The chunk being rebuilt.</param>
        /// <returns>True if the job can be enqueued.</returns>
        public bool CanRebuildMesh(Vector2I chunk)
        {
            return this.Chunk != chunk || (!this.rebuildingMesh && this.rebuildMeshRequired);
        }

        /// <summary>
        /// Reserves a RebuildMesh job.
        /// </summary>
        /// <param name="chunk">The chunk being rebuilt.</param>
        public void ReserveRebuildMesh(Vector2I chunk)
        {
            if (this.Chunk == chunk)
            {
                this.rebuildingMesh = true;
                this.rebuildMeshRequired = false;
                this.meshFilterUpdateRequired = true;
            }
        }

        /// <summary>
        /// Un-reserves a RebuildMesh job.
        /// </summary>
        /// <param name="chunk">The chunk being rebuilt.</param>
        public void UnreserveRebuildMesh(Vector2I chunk)
        {
            if (this.Chunk == chunk)
            {
                this.rebuildingMesh = false;
            }
        }

        #endregion

        #region Update Mesh Filter

        /// <summary>
        /// Check whether a UpdateMeshFilter job can execute.
        /// </summary>
        /// <returns>True if the job can be enqueued.</returns>
        public bool CanUpdateMeshFilter()
        {
            return !this.updatingMeshFilter && this.meshFilterUpdateRequired;
        }

        /// <summary>
        /// Reserves a UpdateMeshFilter job.
        /// </summary>
        public void ReserveUpdateMeshFilter()
        {
            this.updatingMeshFilter = true;
            this.meshFilterUpdateRequired = false;
        }

        /// <summary>
        /// Un-reserves a UpdateMeshFilter job.
        /// </summary>
        public void UnreserveUpdateMeshFilter()
        {
            this.updatingMeshFilter = false;
        }

        #endregion

        #region Dig Circle

        /// <summary>
        /// Check whether a DigCircle job can execute.
        /// </summary>
        /// <param name="chunk">The chunk in which the origin lies.</param>
        /// <param name="origin">The circle origin.</param>
        /// <param name="radius">The circle radius.</param>
        /// <returns>True if the job can be enqueued.</returns>
        public bool CanDigCircle(Vector2I chunk, Vector2I origin, int radius)
        {
            if (chunk == this.Chunk)
            {
                bool exists;
                int existing;
                lock ((this.digCircle as ICollection).SyncRoot)
                {
                    exists = this.digCircle.TryGetValue(origin, out existing);
                }

                return !exists || radius > existing;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Reserves a DigCircle job.
        /// </summary>
        /// <param name="chunk">The chunk in which the origin lies.</param>
        /// <param name="origin">The circle origin.</param>
        /// <param name="radius">The circle radius.</param>
        public void ReserveDigCircle(Vector2I chunk, Vector2I origin, int radius)
        {
            if (chunk == this.Chunk)
            {
                lock ((this.digCircle as ICollection).SyncRoot)
                {
                    if (this.digCircle.ContainsKey(origin))
                    {
                        this.digCircle[origin] = radius;
                        this.rebuildMeshRequired = true;
                    }
                    else
                    {
                        this.digCircle.Add(origin, radius);
                        this.rebuildMeshRequired = true;
                    }
                }
            }
        }

        /// <summary>
        /// Un-reserves a DigCircle job.
        /// </summary>
        /// <param name="chunk">The chunk in which the origin lies.</param>
        /// <param name="origin">The circle origin.</param>
        /// <param name="radius">The circle radius.</param>
        public void UnreserveDigCircle(Vector2I chunk, Vector2I origin, int radius)
        {
            if (chunk == this.Chunk)
            {
                lock ((this.digCircle as ICollection).SyncRoot)
                {
                    if (this.digCircle[origin] == radius)
                    {
                        this.digCircle.Remove(origin);
                    }
                }
            }
        }

        #endregion
    }
}