// ----------------------------------------------------------------------------
// <copyright file="InputRegionComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using EntitySystem.Component;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The input region of the entity.
    /// </summary>
    public class InputRegionComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the InputRegionComponent class.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public InputRegionComponent(int x, int y, int width, int height)
        {
            this.Region = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public Rectangle Region { get; set; }
    }
}