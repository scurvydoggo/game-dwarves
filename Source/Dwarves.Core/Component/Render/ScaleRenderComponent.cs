// ----------------------------------------------------------------------------
// <copyright file="ScaleRenderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Render
{
    using EntitySystem.Component;

    /// <summary>
    /// The scaling ratio for entity rendering.
    /// </summary>
    public class ScaleRenderComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the ScaleRenderComponent class.
        /// </summary>
        /// <param name="scale">The scaling ratio.</param>
        public ScaleRenderComponent(float scale)
        {
            this.Scale = scale;
        }

        /// <summary>
        /// Gets or sets the scaling ratio.
        /// </summary>
        public float Scale { get; set; }
    }
}