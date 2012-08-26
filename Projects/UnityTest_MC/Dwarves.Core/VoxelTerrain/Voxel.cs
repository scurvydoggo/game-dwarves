// ----------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain
{
    /// <summary>
    /// A voxel.
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// An empty voxel.
        /// </summary>
        public static readonly Voxel Empty = new Voxel(TerrainMaterial.Air, byte.MaxValue, 0);

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density0">The density at voxel depth 0.</param>
        public Voxel(TerrainMaterial material, byte density0)
            : this(material, density0, short.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density0">The density at voxel depth 0.</param>
        /// <param name="color">The color.</param>
        public Voxel(TerrainMaterial material, byte density0, short color)
            : this(material, density0, 0, 0, 0, color)
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
    }
}