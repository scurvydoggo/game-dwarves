// ----------------------------------------------------------------------------
// <copyright file="IMethodGetBounds.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Bounds
{
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// Implements a method of obtaining an entity's bounds.
    /// </summary>
    public interface IMethodGetBounds
    {
        /// <summary>
        /// Get the entity's bounds in chunk-coordinates.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The bounds.</returns>
        RectangleI GetChunkBounds(GameObject entity);
    }
}