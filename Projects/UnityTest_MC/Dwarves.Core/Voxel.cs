// ----------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    /// <summary>
    /// A voxel.
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        public byte Density { get; set; }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// Gets or sets the color. 15-bit RGB color with 5 bits per component. The last bit is unused.
        /// </summary>
        public short Color { get; set; }
    }
}