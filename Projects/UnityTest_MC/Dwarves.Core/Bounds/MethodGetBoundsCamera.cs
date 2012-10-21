// ----------------------------------------------------------------------------
// <copyright file="MethodGetBoundsCamera.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Bounds
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Implements a method of obtaining a camera's bounds.
    /// </summary>
    public class MethodGetBoundsCamera : IMethodGetBounds
    {
        /// <summary>
        /// The plane at Z=0.
        /// </summary>
        private Plane planeZ;

        /// <summary>
        /// Initialises a new instance of the MethodGetBoundsCamera class.
        /// </summary>
        public MethodGetBoundsCamera()
        {
            this.planeZ = new Plane(Vector3.back, Vector3.zero);
        }

        /// <summary>
        /// Get the entity's bounds in chunk-coordinates.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The bounds.</returns>
        public Rectangle GetChunkBounds(GameObject entity)
        {
            // Cast a ray at the bottom left and top right corners of the viewport
            Ray bottomRay = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
            Ray topRay = Camera.main.ViewportPointToRay(new Vector3(1, 1, 0));

            // Determine the distance to Z=0 along the ray vector (since the ray will be at an angle)
            float bottomDistance;
            float topDistance;
            if (this.planeZ.Raycast(bottomRay, out bottomDistance) && this.planeZ.Raycast(topRay, out topDistance))
            {
                // Get the position where the ray intersects the Z=0 plane
                Vector3 bottom = bottomRay.GetPoint(Math.Abs(bottomDistance));
                Vector3 top = topRay.GetPoint(Math.Abs(topDistance));

                return new Rectangle((int)bottom.x - 2, (int)top.y + 1, (int)(top.x - bottom.x) + 4, (int)(top.y - bottom.y) + 2);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
    }
}