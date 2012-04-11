// ----------------------------------------------------------------------------
// <copyright file="BodyAssembler.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler
{
    using System;
    using EntitySystem;

    /// <summary>
    /// Assembles body components for entities.
    /// </summary>
    public class BodyAssembler
    {
        /// <summary>
        /// Assemble the body for the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="args">The assembler args.</param>
        public void AssembleBody(Entity entity, Args args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Body assembler args.
        /// </summary>
        public class Args
        {
            /// <summary>
            /// Gets or sets the sprite family.
            /// </summary>
            public string SpriteFamily { get; set; }
        }
    }
}