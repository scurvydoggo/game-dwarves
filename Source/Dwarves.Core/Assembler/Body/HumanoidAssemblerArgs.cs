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
        /// <param name="family">The sprite family.</param>
        public HumanoidAssemblerArgs(string family)
        {
            this.Family = family;
        }

        /// <summary>
        /// Gets or sets the sprites family that this body belongs to.
        /// </summary>
        public string Family { get; set; }
    }
}