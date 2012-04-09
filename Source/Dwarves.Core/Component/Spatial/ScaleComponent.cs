// ----------------------------------------------------------------------------
// <copyright file="ScaleComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Spatial
{
    using EntitySystem.Component;

    /// <summary>
    /// The entity's scaling ratio.
    /// </summary>
    public class ScaleComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the ScaleComponent class.
        /// </summary>
        /// <param name="scale">The scaling ratio.</param>
        public ScaleComponent(float scale)
        {
            this.Scale = scale;
        }

        /// <summary>
        /// Gets or sets the scaling ratio.
        /// </summary>
        public float Scale { get; set; }
    }
}