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
        /// An air voxel that lies outside the terrain.
        /// </summary>
        public static readonly Voxel Air = new Voxel(TerrainMaterial.Air, TerrainConst.DensityMax);

        /// <summary>
        /// The byte containing the concatenation of the primary and secondary densities.
        /// </summary>
        private byte densityField;

        /// <summary>
        /// Initialises a new instance of the Voxel struct.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="density">The initial density.</param>
        public Voxel(TerrainMaterial material, byte density)
            : this()
        {
            this.Material = material;

            // Set both the primary and secondary densities
            this.densityField = (byte)((density << 4) | (density & 0x0F));
        }

        /// <summary>
        /// Gets the primary density.
        /// </summary>
        public byte Density
        {
            get
            {
                return (byte)(this.densityField & 0x0F);
            }
        }

        /// <summary>
        /// Gets the secondary density. This is the density which lies behind the digging area and should not change
        /// once set.
        /// </summary>
        public byte SecondaryDensity
        {
            get
            {
                return (byte)(this.densityField >> 4);
            }
        }

        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        public TerrainMaterial Material { get; set; }

        /// <summary>
        /// Set the primary density value.
        /// </summary>
        /// <param name="density">The density.</param>
        public void SetDensity(byte density)
        {
            this.densityField = (byte)((this.densityField & 0xF0) | (density & 0x0F));
        }
    }
}