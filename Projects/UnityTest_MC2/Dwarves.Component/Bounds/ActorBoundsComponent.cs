// ----------------------------------------------------------------------------
// <copyright file="ActorBoundsComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Bounds
{
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// Component for the bounds of an actor. An actor is a game object that requires the chunk within which it lies to
    /// be loaded.
    /// </summary>
    public class ActorBoundsComponent : MonoBehaviour
    {
        /// <summary>
        /// Get the bounds in world coordinates.
        /// </summary>
        /// <returns>The bounds.</returns>
        public RectangleI GetBounds()
        {
            // TODO
            return RectangleI.Empty;
        }
    }
}