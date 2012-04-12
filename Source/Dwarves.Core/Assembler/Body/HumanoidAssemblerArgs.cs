// ----------------------------------------------------------------------------
// <copyright file="HumanoidAssemblerArgs.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Assembler.Body
{
    /// <summary>
    /// Arguments for the humanoid assembler.
    /// </summary>
    public class HumanoidAssemblerArgs
    {
        /// <summary>
        /// Initializes a new instance of the HumanoidAssemblerArgs class.
        /// </summary>
        /// <param name="spriteFamily">The sprite family.</param>
        /// <param name="collisionGroup">The collision group for the body.</param>
        public HumanoidAssemblerArgs(string spriteFamily, short collisionGroup)
        {
            this.SpriteFamily = spriteFamily;
            this.CollisionGroup = collisionGroup;
        }

        /// <summary>
        /// Gets or sets the sprites family that this body belongs to.
        /// </summary>
        public string SpriteFamily { get; set; }

        /// <summary>
        /// Gets or sets the collision group for the body.
        /// </summary>
        public short CollisionGroup { get; set; }
    }
}