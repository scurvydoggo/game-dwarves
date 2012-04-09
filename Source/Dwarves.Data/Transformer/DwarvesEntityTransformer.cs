// ----------------------------------------------------------------------------
// <copyright file="DwarvesEntityTransformer.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Data.Transformer
{
    using System.Collections.Generic;
    using EntitySystem;
    using EntitySystem.Data;

    /// <summary>
    /// Responsible for transforming an EntityManager into a serializable or deserializable state.
    /// </summary>
    public class DwarvesEntityTransformer : IEntityTransformer
    {
        /// <summary>
        /// The list of entity transformers.
        /// </summary>
        private List<IEntityTransformer> transformerList;

        /// <summary>
        /// Initializes a new instance of the DwarvesEntityTransformer class.
        /// </summary>
        public DwarvesEntityTransformer()
        {
            this.transformerList = new List<IEntityTransformer>();
        }

        /// <summary>
        /// Transform the given EntityManager into a state that is ready for deserialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        public void TransformPostLoad(EntityManager entityManager)
        {
            // Perform all transformations
            foreach (IEntityTransformer transformer in this.transformerList)
            {
                transformer.TransformPostLoad(entityManager);
            }
        }

        /// <summary>
        /// Transform the given EntityManager into a state that is ready for serialization.
        /// </summary>
        /// <param name="entityManager">The EntityManager to transform.</param>
        public void TransformPreSave(EntityManager entityManager)
        {
            // Perform all transformations
            foreach (IEntityTransformer transformer in this.transformerList)
            {
                transformer.TransformPreSave(entityManager);
            }
        }
    }
}