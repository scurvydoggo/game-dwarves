// ----------------------------------------------------------------------------
// <copyright file="MutationArgs.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Mutation
{
    using System;

    /// <summary>
    /// Argument for terrain mutation events.
    /// </summary>
    public class MutationArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the MutationArgs class.
        /// </summary>
        /// <param name="mutations">The world positions that mutated.</param>
        public MutationArgs(params Position[] mutations)
        {
            this.Mutations = mutations;
        }

        /// <summary>
        /// Gets the world positions that mutated.
        /// </summary>
        public Position[] Mutations { get; private set; }
    }
}