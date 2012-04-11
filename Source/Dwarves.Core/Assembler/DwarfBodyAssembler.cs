// ----------------------------------------------------------------------------
// <copyright file="DwarfBodyAssembler.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler
{
    /// <summary>
    /// Assembles dwarf body components.
    /// </summary>
    public class DwarfBodyAssembler : BodyAssembler
    {
        /// <summary>
        /// The family for dwarf sprites.
        /// </summary>
        private const string Family = "dwarf";

        /// <summary>
        /// Initializes a new instance of the DwarfBodyAssembler class.
        /// </summary>
        /// <param name="world">The world context.</param>
        public DwarfBodyAssembler(WorldContext world)
            : base(world)
        {
        }

        /// <summary>
        /// Gets the sprite family for this body.
        /// </summary>
        protected override string SpriteFamily
        {
            get
            {
                return Family;
            }
        }
    }
}
