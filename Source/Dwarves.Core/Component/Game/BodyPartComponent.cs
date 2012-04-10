// ----------------------------------------------------------------------------
// <copyright file="BodyPartComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Game
{
    using EntitySystem;

    /// <summary>
    /// Represents a body part belonging to a parent entity.
    /// </summary>
    public class BodyPartComponent
    {
        /// <summary>
        /// Initializes a new instance of the BodyPartComponent class.
        /// </summary>
        /// <param name="parentEntity">The parent entity.</param>
        public BodyPartComponent(Entity parentEntity)
        {
            this.ParentEntity = parentEntity;
        }

        /// <summary>
        /// Gets the parent entity.
        /// </summary>
        public Entity ParentEntity { get; private set; }
    }
}