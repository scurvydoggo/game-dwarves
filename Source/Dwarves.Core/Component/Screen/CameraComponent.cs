// ----------------------------------------------------------------------------
// <copyright file="CameraComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Screen
{
    using EntitySystem.Component;

    /// <summary>
    /// Component for game camera.
    /// </summary>
    public class CameraComponent : IComponent
    {
        /// <summary>
        /// The default width (in game world meters) of the camera projection.
        /// </summary>
        public const float DefaultProjectionWidth = 80.0f;

        /// <summary>
        /// The default height (in game world meters) of the camera projection.
        /// </summary>
        public const float DefaultProjectionHeight = 48.0f;

        /// <summary>
        /// The default size of a single zoom step.
        /// </summary>
        public const float DefaultZoomStepSize = 0.05f;

        /// <summary>
        /// Initializes a new instance of the CameraComponent class.
        /// </summary>
        public CameraComponent()
            : this(DefaultProjectionWidth, DefaultProjectionHeight, DefaultZoomStepSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CameraComponent class.
        /// </summary>
        /// <param name="projectionWidth">The width (in game world meters) of the camera projection.</param>
        /// <param name="projectionHeight">The height (in game world meters) of the camera projection.</param>
        /// <param name="zoomStepSize">The size of a single zoom step.</param>
        public CameraComponent(float projectionWidth, float projectionHeight, float zoomStepSize)
        {
            this.ProjectionWidth = projectionWidth;
            this.ProjectionHeight = projectionHeight;
            this.ZoomStepSize = zoomStepSize;
        }

        /// <summary>
        /// Gets or sets the width (in game world meters) of the camera projection.
        /// </summary>
        public float ProjectionWidth { get; set; }

        /// <summary>
        /// Gets or sets the height (in game world meters) of the camera projection.
        /// </summary>
        public float ProjectionHeight { get; set; }

        /// <summary>
        /// Gets or sets the size of a single zoom step.
        /// </summary>
        public float ZoomStepSize { get; set; }
    }
}