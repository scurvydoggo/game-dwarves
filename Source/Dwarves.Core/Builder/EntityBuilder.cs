// ----------------------------------------------------------------------------
// <copyright file="EntityBuilder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Builder
{
    using System;
    using EntitySystem;

    /// <summary>
    /// Abstract class for building entities.
    /// </summary>
    public abstract class EntityBuilder
    {
        /// <summary>
        /// Initializes a new instance of the EntityBuilder class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public EntityBuilder(WorldContext world)
        {
            this.World = world;
        }

        /// <summary>
        /// Gets or sets the entity being built.
        /// </summary>
        protected Entity CurrentEntity { get; set; }

        /// <summary>
        /// Gets the world context.
        /// </summary>
        protected WorldContext World { get; private set; }

        /// <summary>
        /// Begin building a new entity.
        /// </summary>
        public void Begin()
        {
            if (this.CurrentEntity != null)
            {
                throw new ApplicationException();
            }

            this.CurrentEntity = this.World.EntityManager.CreateEntity();
        }

        /// <summary>
        /// Stop building and return the entity.
        /// </summary>
        /// <returns>The entity that was built.</returns>
        public Entity End()
        {
            Entity entity = this.CurrentEntity;

            this.CurrentEntity = null;

            return entity;
        }

        /// <summary>
        /// Remove the entity that was being built from the system.
        /// </summary>
        public void Rollback()
        {
            if (this.CurrentEntity != null)
            {
                this.World.EntityManager.RemoveEntity(this.CurrentEntity);
            }
        }
    }
}