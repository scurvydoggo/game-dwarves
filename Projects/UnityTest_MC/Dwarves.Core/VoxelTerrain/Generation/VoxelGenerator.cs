// ----------------------------------------------------------------------------
// <copyright file="VoxelGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    /// <summary>
    /// Generates the voxels.
    /// </summary>
    public class VoxelGenerator
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGenerator class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public VoxelGenerator(float seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// Gets or sets the seed value.
        /// </summary>
        public float Seed { get; set; }
    }
}