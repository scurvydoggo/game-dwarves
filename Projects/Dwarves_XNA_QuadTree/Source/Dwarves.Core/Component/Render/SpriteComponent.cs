// ----------------------------------------------------------------------------
// <copyright file="SpriteComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Render
{
    using EntitySystem.Component;

    /// <summary>
    /// Component for a sprite entity.
    /// </summary>
    public class SpriteComponent : IComponent
    {
        /// <summary>
        /// Initializes a new instance of the SpriteComponent class.
        /// </summary>
        /// <param name="spriteName">The sprite name.</param>
        public SpriteComponent(string spriteName)
        {
            this.SpriteName = spriteName;
        }

        /// <summary>
        /// Gets or sets the sprite name.
        /// </summary>
        public string SpriteName { get; set; }
    }
}