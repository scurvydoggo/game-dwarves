// ----------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    using System;

    /// <summary>
    /// A voxel, which represent the density of a point in a plane. Each 'cube' in the terrain is drawn in the space
    /// between voxel points.
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// The number of cubes drawn in the Z direction for voxels.
        /// </summary>
        public const int Depth = 4;

        /// <summary>
        /// An empty voxel.
        /// </summary>
        public static readonly Voxel Air = new Voxel(TerrainMaterial.Air, byte.MaxValue, 0);

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="densityAll">The density at all depths.</param>
        public Voxel(TerrainMaterial material, byte densityAll)
            : this(material, densityAll, short.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="densityAll">The density at all depths.</param>
        /// <param name="color">The color.</param>
        public Voxel(TerrainMaterial material, byte densityAll, short color)
            : this(material, densityAll, densityAll, densityAll, densityAll, color)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density0">The density at voxel depth 0.</param>
        /// <param name="density1">The density at voxel depth 1.</param>
        /// <param name="density2">The density at voxel depth 2.</param>
        /// <param name="density3">The density at voxel depth 3.</param>
        /// <param name="color">The color.</param>
        public Voxel(TerrainMaterial material, byte density0, byte density1, byte density2, byte density3, short color)
            : this()
        {
            this.Material = material;
            this.Density0 = density0;
            this.Density1 = density1;
            this.Density2 = density2;
            this.Density3 = density3;
            this.Color = color;
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public TerrainMaterial Material { get; set; }

        /// <summary>
        /// Gets or sets the density at voxel depth 0.
        /// </summary>
        public byte Density0 { get; set; }

        /// <summary>
        /// Gets or sets the density at voxel depth 1.
        /// </summary>
        public byte Density1 { get; set; }

        /// <summary>
        /// Gets or sets the density at voxel depth 2.
        /// </summary>
        public byte Density2 { get; set; }

        /// <summary>
        /// Gets or sets the density at voxel depth 3.
        /// </summary>
        public byte Density3 { get; set; }

        /// <summary>
        /// Gets or sets the color. 15-bit RGB color with 5 bits per component. The last bit is unused.
        /// </summary>
        public short Color { get; set; }

        /// <summary>
        /// Gets the voxel density at the given Z depth.
        /// </summary>
        /// <param name="z">The z depth, which must be between 0 and Voxel.Depth inclusively.</param>
        /// <returns>The density.</returns>
        public byte GetDensity(int z)
        {
            switch (z)
            {
                case 0:
                    return this.Density0;

                case 1:
                    return this.Density1;

                case 2:
                    return this.Density2;

                case 3:
                    return this.Density3;

                default:
                    throw new InvalidOperationException("Invalid z depth: " + z);
            }
        }
    }
}