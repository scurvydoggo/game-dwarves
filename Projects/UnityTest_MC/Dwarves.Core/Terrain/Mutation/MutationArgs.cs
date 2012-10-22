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
        /// <param name="changedPositions">The world positions that changed.</param>
        public MutationArgs(params Position[] changedPositions)
        {
            this.ChangedPositions = changedPositions;
        }

        /// <summary>
        /// Gets the world positions that changed.
        /// </summary>
        public Position[] ChangedPositions { get; private set; }
    }
}