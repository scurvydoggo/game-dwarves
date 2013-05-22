// ----------------------------------------------------------------------------
// <copyright file="CameraBoundsComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Bounds
{
    using System;
    using Dwarves.Core.Math;
    using UnityEngine;

    /// <summary>
    /// Provides the 2D bounds of a camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraBoundsComponent : MonoBehaviour
    {
        /// <summary>
        /// The plane at Z=0.
        /// </summary>
        private static readonly Plane PlaneZ = new Plane(Vector3.back, Vector3.zero);

        /// <summary>
        /// The camera component.
        /// </summary>
        private Camera cCamera;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.cCamera = this.GetComponent<Camera>();
        }

        /// <summary>
        /// Get the bounds in world coordinates.
        /// </summary>
        /// <returns>The bounds.</returns>
        public RectangleI GetBounds()
        {
            // Cast a ray at the bottom left and top right corners of the viewport
            Ray bottomRay = this.camera.ViewportPointToRay(new Vector3(0, 0, 0));
            Ray topRay = this.camera.ViewportPointToRay(new Vector3(1, 1, 0));

            // Determine the distance to Z=0 along the ray vector (since the ray will be at an angle)
            float bottomDistance;
            float topDistance;
            if (CameraBoundsComponent.PlaneZ.Raycast(bottomRay, out bottomDistance) &&
                CameraBoundsComponent.PlaneZ.Raycast(topRay, out topDistance))
            {
                // Get the position where the ray intersects the Z=0 plane
                Vector3 bottom = bottomRay.GetPoint(Math.Abs(bottomDistance));
                Vector3 top = topRay.GetPoint(Math.Abs(topDistance));

                return new RectangleI(
                    (int)bottom.x - 2, (int)top.y + 1, (int)(top.x - bottom.x) + 4, (int)(top.y - bottom.y) + 2);
            }
            else
            {
                return RectangleI.Empty;
            }
        }
    }
}