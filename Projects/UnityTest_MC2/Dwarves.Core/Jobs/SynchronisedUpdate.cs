// ----------------------------------------------------------------------------
// <copyright file="SynchronisedUpdate.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// Ensures that multiple chunks are updated on-screen in the same frame. This is to prevent flickering that occurs
    /// when there is a change in geometry that spans multiple chunks but the meshes are updated on separate frames.
    /// </summary>
    public class SynchronisedUpdate
    {
        /// <summary>
        /// The content.
        /// </summary>
        private Content content;

        /// <summary>
        /// Initialises a new instance of the SynchronisedUpdate class.
        /// </summary>
        /// <param name="chunks">The chunks being synchronised.</param>
        public SynchronisedUpdate(params Vector2I[] chunks)
        {
            this.content = new Content(chunks);
        }

        /// <summary>
        /// Gets a value indicating whether the chunks are synchronised.
        /// </summary>
        public bool IsSynchronised
        {
            get { return this.content.IsSynchronised; }
        }

        /// <summary>
        /// Indicates that a chunk is ready.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        public void SetReady(Vector2I chunk)
        {
            this.content.SetReady(chunk);
        }

        /// <summary>
        /// Merge the two instance so that they have the same chunks and share the same ready counter.
        /// </summary>
        /// <param name="other">The other instance.</param>
        public void Merge(SynchronisedUpdate other)
        {
            // Update the references such that both instances have the same content
            this.content.AddOther(other.content);
            other.content = this.content;
        }

        /// <summary>
        /// The actual content of the class.
        /// </summary>
        private class Content
        {
            /// <summary>
            /// The count of chunks that are ready.
            /// </summary>
            private int readyCount;

            /// <summary>
            /// Initialises a new instance of the Content class.
            /// </summary>
            /// <param name="chunks">The chunks being synchronised.</param>
            public Content(Vector2I[] chunks)
            {
                this.Chunks = new Dictionary<Vector2I, bool>();
                foreach (Vector2I chunk in chunks)
                {
                    this.Chunks.Add(chunk, false);
                }
            }

            /// <summary>
            /// Gets the chunks being synchronised.
            /// </summary>
            public Dictionary<Vector2I, bool> Chunks { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the chunks are synchronised.
            /// </summary>
            public bool IsSynchronised
            {
                get { return this.readyCount == this.Chunks.Count; }
            }

            /// <summary>
            /// Indicates that a chunk is ready.
            /// </summary>
            /// <param name="chunk">The chunk.</param>
            public void SetReady(Vector2I chunk)
            {
                if (!this.Chunks[chunk])
                {
                    this.Chunks[chunk] = true;
                    this.readyCount++;
                }
            }

            /// <summary>
            /// Add the chunks from the other instance and update the ready count.
            /// </summary>
            /// <param name="other">The other instance.</param>
            public void AddOther(Content other)
            {
                foreach (KeyValuePair<Vector2I, bool> kvp in other.Chunks)
                {
                    if (!this.Chunks.ContainsKey(kvp.Key))
                    {
                        this.Chunks.Add(kvp.Key, kvp.Value);
                        if (kvp.Value)
                        {
                            this.readyCount++;
                        }
                    }
                }
            }
        }
    }
}