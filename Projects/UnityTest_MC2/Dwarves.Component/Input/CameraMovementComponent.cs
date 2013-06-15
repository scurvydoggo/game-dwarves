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
        /// The plane at Z=0.
        /// </summary>
        private Plane planeZ;

        /// <summary>
        /// The origin of the current drag.
        /// </summary>
        private Vector3 dragOrigin;

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
            if (Input.GetMouseButtonDown(0))
            {
                this.dragOrigin = this.GetWorldPoint(Input.mousePosition);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                var currentPos = this.GetWorldPoint(Input.mousePosition);
                Vector3 movePos = this.dragOrigin - currentPos;
                this.transform.position += movePos;
            }
            else
            {
                // Check if the camera should pan
                float moveDistance = this.KeySpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    this.transform.position += new Vector3(-moveDistance, 0, 0);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    this.transform.position += new Vector3(moveDistance, 0, 0);
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    this.transform.position += new Vector3(0, moveDistance, 0);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    this.transform.position += new Vector3(0, -moveDistance, 0);
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