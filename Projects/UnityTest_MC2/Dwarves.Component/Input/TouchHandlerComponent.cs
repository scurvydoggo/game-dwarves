// ----------------------------------------------------------------------------
// <copyright file="TouchHandlerComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using UnityEngine;

    /// <summary>
    /// Component capturing touch events and delegating behaviour to game objects.
    /// </summary>
    public class TouchHandlerComponent : MonoBehaviour
    {
        /// <summary>
        /// The plane at Z=0.
        /// </summary>
        private Plane planeZ;

        /// <summary>
        /// The Terrain's touchable component.
        /// </summary>
        private TouchableComponent terrainTouchable;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.planeZ = new Plane(Vector3.back, Vector3.zero);

            // Get a refernce to the terrain's touchable component
            GameObject terrain = GameObject.Find("Terrain");
            this.terrainTouchable = terrain.GetComponent<TouchableComponent>();
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Determine if the screen was touched/clicked
            Vector3 touchPosition;
            if (this.TryGetTouchPosition(out touchPosition))
            {
                TouchableComponent touched;
                Vector3 hitPoint;
                if (this.TryGetTouchedComponent(touchPosition, out touched, out hitPoint))
                {
                    touched.ProcessTouch(hitPoint);
                }
            }
        }

        /// <summary>
        /// Gets the current position of a touch or mouse click.
        /// </summary>
        /// <param name="touchPosition">The position.</param>
        /// <returns>True if a touch is currently being made.</returns>
        private bool TryGetTouchPosition(out Vector3 touchPosition)
        {
#if UNITY_IPHONE || UNITY_ANDRIOD
        if (Input.touchesCount == 1)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                touchPosition = touch.position;
                return true;
            }
        }
#else
            if (Input.GetMouseButton(0))
            {
                touchPosition = Input.mousePosition;
                return true;
            }
#endif

            // No valid touch is currently being made
            touchPosition = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Get the component that lies at the given screen position.
        /// </summary>
        /// <param name="touchPosition">The screen position of the touch.</param>
        /// <param name="touchable">The touchable component.</param>
        /// <param name="hitPoint">The point at which the component was touched in world coordinates.</param>
        /// <returns>The touchable component; Null if touchable object exists at the position.</returns>
        private bool TryGetTouchedComponent(Vector2 touchPosition, out TouchableComponent touchable, out Vector3 hitPoint)
        {
            // Cast a ray into the scene at the touched point
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);

            // See if an actor was hit
            // TODO

            // No actors were hit. See where the ray hits the terrain on the Z = 0 plane
            float distance;
            if (this.planeZ.Raycast(ray, out distance))
            {
                touchable = this.terrainTouchable;
                hitPoint = ray.GetPoint(distance);
                return true;
            }

            touchable = null;
            hitPoint = Vector3.zero;
            return false;
        }
    }
}