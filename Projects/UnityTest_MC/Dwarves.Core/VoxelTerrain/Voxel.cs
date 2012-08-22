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
        public static readonly Voxel Empty = new Voxel(TerrainMaterial.Air, 0, 0);

        /// <summary>
        /// Initializes a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The density.</param>
        /// <param name="color">The color.</param>
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
        /// Gets or sets the color. 15-bit RGB color with 5 bits per component. The last bit is unused.
        /// </summary>
        public short Color { get; set; }
    }
}