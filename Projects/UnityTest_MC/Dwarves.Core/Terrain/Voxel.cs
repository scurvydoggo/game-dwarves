// ----------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// A voxel, which represent the density of a point in a plane. Each 'cube' in the terrain is drawn in the space
    /// between voxel points.
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// The number of cubes dug in the Z direction for voxels.
        /// </summary>
        public const byte DigDepth = 3;

        /// <summary>
        /// The number of cubes drawn in the Z direction for voxels.
        /// </summary>
        public const byte DrawDepth = 6;

        /// <summary>
        /// The minimum density value, indicating fully inside the surface.
        /// </summary>
        public const byte DensityMin = 0x00;

        /// <summary>
        /// The maximum density value, indicating fully outside the surface.
        /// </summary>
        public const byte DensityMax = 0x0F;

        /// <summary>
        /// The mask for the secondary density value which is used for terrain behind the dug area (i.e. the original
        /// density prior to the digging).
        /// </summary>
        public const byte DensityMaskSecondary = 0xF0;

        /// <summary>
        /// An empty voxel.
        /// </summary>
        public static readonly Voxel Air =
            new Voxel(TerrainMaterial.Air, Voxel.DensityMax | Voxel.DensityMaskSecondary, 0);

        /// <summary>
        /// Initialises a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The density.</param>
        public Voxel(TerrainMaterial material, byte density)
            : this(material, density, short.MaxValue)
        {
        }

        /// <summary>
        /// Initialises a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The density.</param>
        /// <param name="color">The colour.</param>
        public Voxel(TerrainMaterial material, byte density, short color)
            : this()
        {
            this.Material = material;
            this.Density = density;
            this.Color = color;
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
        /// Gets or sets the colour. 15-bit RGB colour with 5 bits per component. The last bit is unused.
        /// </summary>
        public short Color { get; set; }
    }
}