// ----------------------------------------------------------------------------
// <copyright file="TerrainConst.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// Terrain related constants.
    /// </summary>
    public static class TerrainConst
    {
        /// <summary>
        /// The maximum density value. This lies outside the surface.
        /// </summary>
        public const byte DensityMax = 0x0E;

        /// <summary>
        /// The minimum density value. This lies inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The density at which the surface lies (aka the isolevel).
        /// </summary>
        public const byte DensitySurface = 0x07;

        /// <summary>
        /// The number of cubes dug in the Z direction for voxels.
        /// </summary>
        public const byte DigDepth = 4;

        /// <summary>
        /// The number of cubes drawn in the Z direction for voxels.
        /// </summary>
        public const byte DrawDepth = 8;
    }
}