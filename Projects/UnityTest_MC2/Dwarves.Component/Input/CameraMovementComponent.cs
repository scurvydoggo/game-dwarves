// ----------------------------------------------------------------------------
// <copyright file="CameraMovementComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using UnityEngine;

    /// <summary>
    /// Component for moving the camera component.
    /// </summary>
    public class CameraMovementComponent : MonoBehaviour
    {
        /// <summary>
        /// The duration the camera will continue panning under inertia after a drag.
        /// </summary>
        public float Deceleration = -1;

        /// <summary>
        /// The plane at Z=0.
        /// </summary>
        private Plane planeZ;

        /// <summary>
        /// The origin of the current drag.
        /// </summary>
        private Vector3 dragOrigin;

        /// <summary>
        /// The velocity of the drag.
        /// </summary>
        private Vector3 dragVelocity;

        /// <summary>
        /// The direction of the inertia drag.
        /// </summary>
        private Vector3 inertiaDirection;

        /// <summary>
        /// Indicates whether the camera is moving under inertia from a mouse drag.
        /// </summary>
        private bool underInertia;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.planeZ = new Plane(Vector3.back, 1);
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Check if we're at a mouse-drag endpoint
            if (Input.GetMouseButtonDown(0))
            {
                this.dragOrigin = this.GetWorldPoint(Input.mousePosition);
                this.underInertia = false;
                return;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                this.underInertia = true;
                this.inertiaDirection = Vector3.Normalize(this.dragVelocity);
            }

            if (Input.GetMouseButton(0))
            {
                // Perform mouse-drag movement
                this.dragVelocity = this.dragOrigin - this.GetWorldPoint(Input.mousePosition);
                this.transform.position += this.dragVelocity;
            }
            else
            {
                // Perform inertia-based movement
                if (this.underInertia)
                {
                    float distance = this.Deceleration * Time.smoothDeltaTime;

                    this.dragVelocity.x += Vector3.Dot(this.inertiaDirection, Vector3.right) * distance;
                    if (this.dragVelocity.x * this.inertiaDirection.x < 0)
                    {
                        this.dragVelocity.x = 0;
                    }

                    this.dragVelocity.y += Vector3.Dot(this.inertiaDirection, Vector3.up) * distance;
                    if (this.dragVelocity.y * this.inertiaDirection.y < 0)
                    {
                        this.dragVelocity.y = 0;
                    }

                    if (this.dragVelocity.x != 0 && this.dragVelocity.y != 0)
                    {
                        this.transform.position += this.dragVelocity;
                    }
                    else
                    {
                        this.underInertia = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the world point at the given screen point.
        /// </summary>
        /// <param name="screenPoint">The screen point.</param>
        /// <returns>The world point.</returns>
        private Vector3 GetWorldPoint(Vector3 screenPoint)
        {
            // Cast a ray into the scene at the screen point
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            float distance;
            if (this.planeZ.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}