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
        /// The keyboard movement speed.
        /// </summary>
        public float KeySpeed = 20;

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
            this.planeZ = new Plane(Vector3.back, Vector3.zero);
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

            // Perform mouse-drag movement
            if (!this.PerformDragMovement())
            {
                // Perform keyboard movement
                if (this.PerformKeyboardMovement())
                {
                    this.underInertia = false;
                }
                else
                {
                    // Perform inertia-based movement
                    this.PerformInertiaMovement();
                }
            }
        }

        /// <summary>
        /// Perform mouse-drag movement.
        /// </summary>
        /// <returns>True if movement occurred.</returns>
        private bool PerformDragMovement()
        {
            if (Input.GetMouseButton(0))
            {
                this.dragVelocity = this.dragOrigin - this.GetWorldPoint(Input.mousePosition);
                this.transform.position += this.dragVelocity;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Perform keyboard-based movement.
        /// </summary>
        /// <returns>True if movement occurred.</returns>
        private bool PerformKeyboardMovement()
        {
            bool keyboardMoveOccurred = false;
            float moveDistance = this.KeySpeed * Time.smoothDeltaTime;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.position += new Vector3(-moveDistance, 0, 0);
                keyboardMoveOccurred = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.position += new Vector3(moveDistance, 0, 0);
                keyboardMoveOccurred = true;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.transform.position += new Vector3(0, moveDistance, 0);
                keyboardMoveOccurred = true;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.transform.position += new Vector3(0, -moveDistance, 0);
                keyboardMoveOccurred = true;
            }

            return keyboardMoveOccurred;
        }

        /// <summary>
        /// Perform inertia based movement.
        /// </summary>
        /// <returns>True if movement occurred.</returns>
        private bool PerformInertiaMovement()
        {
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
                    return true;
                }
                else
                {
                    this.underInertia = false;
                    return false;
                }
            }
            else
            {
                return false;
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