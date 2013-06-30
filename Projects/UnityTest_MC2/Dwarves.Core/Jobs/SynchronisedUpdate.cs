// ----------------------------------------------------------------------------
// <copyright file="SynchronisedUpdate.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// Used to synchronise updates through the chunk pipeline such that chunks are updated on-screen in a single
    /// frame. This is to prevent flickering when two neighbouring meshes are updated in separate frames when a change
    /// in geometry is shared across them.
    /// </summary>
    public class SynchronisedUpdate
    {
        /// <summary>
        /// The chunks that are being synchronised. A value of true indicates that the chunk is ready.
        /// </summary>
        private Dictionary<Vector2I, bool> chunks;

        /// <summary>
        /// A count of the chunks that are ready (to avoid re-counting the dictionary every time).
        /// </summary>
        private int readyCount;

        /// <summary>
        /// Initialises a new instance of the SynchronisedUpdate class.
        /// </summary>
        /// <param name="chunks">The chunks requiring synchronisation.</param>
        public SynchronisedUpdate(params Vector2I[] chunks)
        {
            if (chunks.Length <= 1)
            {
                throw new InvalidOperationException("Two or more chunks are required for SynchronisedUpdate.");
            }

            this.chunks = new Dictionary<Vector2I, bool>();
            foreach (Vector2I chunk in chunks)
            {
                this.chunks.Add(chunk, false);
            }
        }

        /// <summary>
        /// Indicates that the chunks are synchronised.
        /// </summary>
        public event EventHandler IsSynchronised;

        /// <summary>
        /// Indicate that a chunk is ready.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        public void SetChunkReady(Vector2I chunk)
        {
            if (!this.chunks[chunk])
            {
                this.chunks[chunk] = true;

                if (++this.readyCount == this.chunks.Count && this.IsSynchronised != null)
                {
                    this.IsSynchronised(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Determines whether this instance contains all of the chunks of the other set.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>True if this instance contains all of the chunks.</returns>
        public bool Contains(SynchronisedUpdate other)
        {
            foreach (Vector2I chunk in other.chunks.Keys)
            {
                if (!this.chunks.ContainsKey(chunk))
                {
                    return false;
                }
            }

            return true;
        }
    }
}