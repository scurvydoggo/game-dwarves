// ----------------------------------------------------------------------------
// <copyright file="TerrainVoxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// A voxel which represent the density of a point in a plane.
    /// </summary>
    public class TerrainVoxel
    {
        /// <summary>
        /// The minimum density value. This lies inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The maximum density value. This lies outside the surface.
        /// </summary>
        public const byte DensityMax = 0xFF;

        /// <summary>
        /// The density at which the surface lies (aka the isolevel).
        /// </summary>
        public const byte DensitySurface = 0x7F;
        
        /// <summary>
        /// Initialises a new instance of the TerrainVoxel class.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The initial density.</param>
        public TerrainVoxel(TerrainMaterial material, byte density)
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

        /// <summary>
        /// Creates an empty voxel with an undefined material and a maximum density (i.e. an 'air' voxel).
        /// </summary>
        /// <returns>The voxel.</returns>
        public static TerrainVoxel CreateEmpty()
        {
            return new TerrainVoxel(TerrainMaterial.Undefined, TerrainVoxel.DensityMax);
        }
    }
}