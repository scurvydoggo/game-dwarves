// ----------------------------------------------------------------------------
// <copyright file="JobAccess.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Linq;
    using Dwarves.Core.Math;

    /// <summary>
    /// An access requirement for a job.
    /// </summary>
    public class JobAccess
    {
        /// <summary>
        /// Initialises a new instance of the JobAccess class.
        /// </summary>
        /// <param name="type">The type of access required.</param>
        /// <param name="chunks">The chunks being accessed.</param>
        public JobAccess(JobAccessType type, params Vector2I[] chunks)
        {
            this.Type = type;
            this.Chunks = chunks;
        }

        /// <summary>
        /// Gets the type of access required.
        /// </summary>
        public JobAccessType Type { get; private set; }

        /// <summary>
        /// Gets the chunks being accessed.
        /// </summary>
        public Vector2I[] Chunks { get; private set; }

        /// <summary>
        /// Determine whether this uses any of the same chunks.
        /// </summary>
        /// <param name="other">The other to compare.</param>
        /// <returns>True if this uses one or more of the same chunks.</returns>
        public bool HasAnyChunks(JobAccess other)
        {
            return this.Chunks.Any(c => other.Chunks.Contains(c));
        }

        /// <summary>
        /// Determine whether this uses all of the same chunks.
        /// </summary>
        /// <param name="other">The other to compare.</param>
        /// <returns>True if this uses all of the same chunks.</returns>
        public bool HasAllChunks(JobAccess other)
        {
            return this.Chunks.All(c => other.Chunks.Contains(c));
        }
    }
}