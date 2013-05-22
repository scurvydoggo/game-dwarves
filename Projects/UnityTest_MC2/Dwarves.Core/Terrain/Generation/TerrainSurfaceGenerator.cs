// ----------------------------------------------------------------------------
// <copyright file="TerrainSurfaceGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using Dwarves.Core.Math.Noise;

    /// <summary>
    /// Generates the terrain surface.
    /// </summary>
    public class TerrainSurfaceGenerator
    {
        /// <summary>
        /// The noise generator.
        /// </summary>
        private INoiseGenerator noiseGenerator;

        /// <summary>
        /// Initialises a new instance of the TerrainSurfaceGenerator class.
        /// </summary>
        /// <param name="noiseGenerator">The noise generator.</param>
        public TerrainSurfaceGenerator(INoiseGenerator noiseGenerator)
        {
            this.noiseGenerator = noiseGenerator;
        }

        /// <summary>
        /// Generate the heights for each x-coordinate for the given chunk.
        /// </summary>
        /// <param name="chunkIndexX">The chunk index's x component.</param>
        /// <returns>The surface heights.</returns>
        public float[] GenerateSurfaceHeights(int chunkIndexX)
        {
            var heights = new float[Metrics.ChunkWidth];

            int originX = chunkIndexX * Metrics.ChunkWidth;
            for (int x = 0; x < Metrics.ChunkWidth; x++)
            {
                // Generate the noise value at this x position
                float noise = this.noiseGenerator.Generate(originX + x);

                // Obtain the height by scaling the noise with the surface amplitude
                heights[x] = noise * Metrics.SurfaceAmplitude;
            }

            return heights;
        }
    }
}