// ----------------------------------------------------------------------------
// <copyright file="ScaleSpatialComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Spatial
{
    using EntitySystem.Component;

    /// <summary>
    /// The scaling ratio for entity spatial attributes.
    /// </summary>
    public class ScaleSpatialComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the ScaleSpatialComponent class.
        /// </summary>
        /// <param name="scale">The scaling ratio.</param>
        public ScaleSpatialComponent(float scale)
        {
            this.Scale = scale;
        }

        /// <summary>
        /// Gets or sets the scaling ratio.
        /// </summary>
        public float Scale { get; set; }
    }
}