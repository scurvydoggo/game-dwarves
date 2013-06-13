// ----------------------------------------------------------------------------
// <copyright file="MasterJobQueueState.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Dwarves.Core.Math;

    /// <summary>
    /// The state of the master job queue.
    /// </summary>List
    public class MasterJobQueueState
    {
        /// <summary>
        /// The dictionary tracking the last queued job to add/remove a chunk.
        /// </summary>
        private Dictionary<Vector2I, AddRemoveContext> addRemoveChunks;

        /// <summary>
        /// Initialises a new instance of the MasterJobQueueState class.
        /// </summary>
        public MasterJobQueueState()
        {
            this.addRemoveChunks = new Dictionary<Vector2I, AddRemoveContext>();
        }

        /// <summary>
        /// Check whether a AddChunks job can execute.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="id">The identifier for the job.</param>
        /// <returns>True if the job can be executed.</returns>
        public bool CanAddChunks(List<Vector2I> chunks, Guid id)
        {
            return this.CanAddRemoveChunks(chunks, id, true);
        }

        /// <summary>
        /// Check whether a RemoveChunks job can execute.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="id">The identifier for the job.</param>
        /// <returns>True if the job can be executed.</returns>
        public bool CanRemoveChunks(List<Vector2I> chunks, Guid id)
        {
            return this.CanAddRemoveChunks(chunks, id, false);
        }

        /// <summary>
        /// Completes a AddChunks job.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="id">The identifier for the job.</param>
        public void CompleteAddRemoveChunks(List<Vector2I> chunks, Guid id)
        {
            lock ((this.addRemoveChunks as ICollection).SyncRoot)
            {
                foreach (Vector2I chunk in chunks)
                {
                    if (this.addRemoveChunks[chunk].Id.Equals(id))
                    {
                        this.addRemoveChunks.Remove(chunk);
                    }
                }
            }
        }

        /// <summary>
        /// Check whether a RemoveChunks job can execute.
        /// </summary>
        /// <param name="chunks">The chunks.</param>
        /// <param name="id">The identifier for the job.</param>
        /// <param name="isAdd">Indicates whether this is an Add job; False indicates a Remove job.</param>
        /// <returns>True if the job can be executed.</returns>
        private bool CanAddRemoveChunks(List<Vector2I> chunks, Guid id, bool isAdd)
        {
            List<Vector2I> toRemove = null;
            lock ((this.addRemoveChunks as ICollection).SyncRoot)
            {
                foreach (Vector2I chunk in chunks)
                {
                    AddRemoveContext context;
                    if (this.addRemoveChunks.TryGetValue(chunk, out context))
                    {
                        // If the last queued job is an add and the new job is a removal (and vice versa), allow it
                        if (context.IsAdd != isAdd)
                        {
                            this.addRemoveChunks[chunk] = new AddRemoveContext(id, isAdd);
                        }
                        else
                        {
                            if (toRemove == null)
                            {
                                toRemove = new List<Vector2I>();
                            }

                            toRemove.Add(chunk);
                        }
                    }
                    else
                    {
                        this.addRemoveChunks.Add(chunk, new AddRemoveContext(id, isAdd));
                    }
                }
            }

            if (toRemove != null)
            {
                foreach (Vector2I chunk in toRemove)
                {
                    chunks.Remove(chunk);
                }
            }

            return chunks.Count > 0;
        }

        /// <summary>
        /// The context for the latest add/remove chunk job.
        /// </summary>
        private struct AddRemoveContext
        {
            /// <summary>
            /// Initialises a new instance of the AddRemoveContext struct.
            /// </summary>
            /// <param name="id">The ID.</param>
            /// <param name="isAdd">Indicates whether this is an Add job; False indicates a Remove job.</param>
            public AddRemoveContext(Guid id, bool isAdd)
                : this()
            {
                this.Id = id;
                this.IsAdd = isAdd;
            }

            /// <summary>
            /// Gets the ID.
            /// </summary>
            public Guid Id { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is an Add job; False indicates a Remove job.
            /// </summary>
            public bool IsAdd { get; private set; }
        }
    }
}