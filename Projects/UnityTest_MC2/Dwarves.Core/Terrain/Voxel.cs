// ----------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// A voxel which represent the density of a point in a plane.
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// The minimum density value. This lies inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The maximum density value. This lies outside the surface.
        /// </summary>
        public const byte DensityMax = 0x0E;

        /// <summary>
        /// The density at which the surface lies (aka the isolevel).
        /// </summary>
        public const byte DensitySurface = 0x07;

        /// <summary>
        /// An air voxel that lies outside the terrain.
        /// </summary>
        public static readonly Voxel Air = new Voxel(TerrainMaterial.Air, Voxel.DensityMax);

        /// <summary>
        /// Initialises a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The initial density.</param>
        public Voxel(TerrainMaterial material, byte density)
            : this()
        {
            this.Material = material;
            this.Density = density;
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public TerrainMaterial Material { get; set; }

        /// <summary>
        /// Gets or sets the density.
        /// </summary>
        public byte Density { get; set; }
    }
}