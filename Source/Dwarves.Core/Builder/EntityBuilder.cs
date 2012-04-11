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
        /// <param name="entityManager">The entity manager.</param>
        public EntityBuilder(EntityManager entityManager)
        {
            this.EntityManager = entityManager;
        }

        /// <summary>
        /// Gets or sets the entity being built.
        /// </summary>
        protected Entity Entity { get; set; }

        /// <summary>
        /// Gets the entity manager.
        /// </summary>
        protected EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Begin building a new entity.
        /// </summary>
        public void Begin()
        {
            if (this.Entity != null)
            {
                throw new ApplicationException();
            }

            this.Entity = this.EntityManager.CreateEntity();
        }

        /// <summary>
        /// Stop building and return the entity.
        /// </summary>
        /// <returns>The entity that was built.</returns>
        public Entity End()
        {
            Entity entity = this.Entity;

            this.Entity = null;

            return entity;
        }

        /// <summary>
        /// Remove the entity that was being built from the system.
        /// </summary>
        public void Rollback()
        {
            if (this.Entity != null)
            {
                this.EntityManager.RemoveEntity(this.Entity);
            }
        }
    }
}