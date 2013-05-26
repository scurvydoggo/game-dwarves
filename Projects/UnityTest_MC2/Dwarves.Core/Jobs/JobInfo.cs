// ----------------------------------------------------------------------------
// <copyright file="JobInfo.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// Represents the information and access requirements of a job.
    /// </summary>
    public class JobInfo
    {
        /// <summary>
        /// The chunks that the job accesses.
        /// </summary>
        private HashSet<Vector2I> chunks;

        /// <summary>
        /// Initialises a new instance of the JobInfo class.
        /// </summary>
        /// <param name="behaviour">The job behaviour.</param>
        /// <param name="write">The access requirements for job's write operation.</param>
        /// <param name="writeOrder">The write order in relation to other jobs with the same write access.</param>
        /// <param name="read">The access requirements for job's read operations.</param>
        public JobInfo(JobBehaviour behaviour, JobAccess write, JobWriteOrder writeOrder, params JobAccess[] read)
        {
            this.Behaviour = behaviour;
            this.Write = write;
            this.WriteOrder = writeOrder;
            this.Reads = read;
            this.chunks = new HashSet<Vector2I>();

            // Add the chunks that are accessed by the write task
            if (this.Write != null)
            {
                foreach (Vector2I chunk in this.Write.Chunks)
                {
                    this.chunks.Add(chunk);
                }
            }

            // Add the chunks that are accessed by the read tasks
            foreach (JobAccess access in this.Reads)
            {
                foreach (Vector2I chunk in access.Chunks)
                {
                    this.chunks.Add(chunk);
                }
            }
        }

        /// <summary>
        /// Gets the job behaviour.
        /// </summary>
        public JobBehaviour Behaviour { get; private set; }

        /// <summary>
        /// Gets the access requirements for job's write operation.
        /// </summary>
        public JobAccess Write { get; private set; }

        /// <summary>
        /// Gets the write order in relation to other jobs with the same write access.
        /// </summary>
        public JobWriteOrder WriteOrder { get; private set; }

        /// <summary>
        /// Gets the access requirements for job's read operations.
        /// </summary>
        public JobAccess[] Reads { get; private set; }

        /// <summary>
        /// Determine whether this uses the given chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if this uses the given chunk.</returns>
        public bool HasChunk(Vector2I chunk)
        {
            return this.chunks.Contains(chunk);
        }

        /// <summary>
        /// Determine whether this uses any of the same chunks.
        /// </summary>
        /// <param name="other">The other to compare.</param>
        /// <returns>True if this uses one or more of the same chunks.</returns>
        public bool HasAnyChunks(JobInfo other)
        {
            foreach (Vector2I chunk in this.chunks)
            {
                if (other.chunks.Contains(chunk))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determine whether this uses all of the same chunks.
        /// </summary>
        /// <param name="other">The other to compare.</param>
        /// <returns>True if this uses all of the same chunks.</returns>
        public bool HasAllChunks(JobInfo other)
        {
            foreach (Vector2I chunk in this.chunks)
            {
                if (!other.chunks.Contains(chunk))
                {
                    return false;
                }
            }

            return true;
        }
    }
}