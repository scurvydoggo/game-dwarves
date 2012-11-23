// ----------------------------------------------------------------------------
// <copyright file="TerrainGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using Dwarves.Core.Math.Noise;

    /// <summary>
    /// Generates voxel terrain.
    /// </summary>
    public class TerrainGenerator
    {
        /// <summary>
        /// The noise generator.
        /// </summary>
        private INoiseGenerator noiseGenerator;

        /// <summary>
        /// Initialises a new instance of the TerrainGenerator class.
        /// </summary>
        /// <param name="noiseGenerator">The noise generator.</param>
        public TerrainGenerator(INoiseGenerator noiseGenerator)
        {
            this.noiseGenerator = noiseGenerator;
        }
    }
}