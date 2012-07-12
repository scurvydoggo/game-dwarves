// ----------------------------------------------------------------------------
// <copyright file="InputFocusComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using EntitySystem.Component;

    /// <summary>
    /// Indicates whether the entity has input focus.
    /// </summary>
    public class InputFocusComponent : IComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity has input focus.
        /// </summary>
        public bool HasFocus { get; set; }
    }
}