// ----------------------------------------------------------------------------
// <copyright file="TerrainPointGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Lighting;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;

    /// <summary>
    /// Generates terrain voxels.
    /// </summary>
    public class TerrainPointGenerator
    {
        /// <summary>
        /// The base noise generator.
        /// </summary>
        private INoiseGenerator baseGenerator;

        /// <summary>
        /// The surface noise generator.
        /// </summary>
        private INoiseGenerator surfaceGenerator;

        /// <summary>
        /// The cave attributes.
        /// </summary>
        private CaveAttributes[] caves;

        /// <summary>
        /// Initialises a new instance of the TerrainPointGenerator class.
        /// </summary>
        /// <param name="baseGenerator">The base noise generator.</param>
        /// <param name="seed">The seed value for the terrain.</param>
        /// <param name="octaves">The number of octaves of noise used by the terrain generator.</param>
        /// <param name="baseFrequency">The base frequency which is the frequency of the lowest octave used by the
        /// terrain generator.</param>
        /// <param name="persistence">The persistence value, which determines the amplitude for each octave used by the
        /// terrain generator.</param>
        public TerrainPointGenerator(
            INoiseGenerator baseGenerator,
            int seed,
            int octaves,
            float baseFrequency,
            float persistence)
        {
            this.baseGenerator = baseGenerator;
            this.surfaceGenerator =
                new CompoundNoiseGenerator(this.baseGenerator, seed, (byte)octaves, baseFrequency, persistence);

            // Set the cave attributes
            this.caves = this.CreateCaveAttributes(seed);
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
                float noise = this.surfaceGenerator.Generate(originX + x);

                // Obtain the height by scaling the noise with the surface amplitude
                heights[x] = noise * Metrics.SurfaceAmplitude;
            }

            return heights;
        }

        /// <summary>
        /// Creates a new chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="surfaceHeights">The surface heights.</param>
        public void GeneratePoints(TerrainChunk chunk, float[] surfaceHeights)
        {
            int originX = chunk.Index.X * Metrics.ChunkWidth;
            int originY = chunk.Index.Y * Metrics.ChunkHeight;

            SurfacePosition? surfacePosition = null;
            for (int x = 0; x < Metrics.ChunkWidth; x++)
            {
                // Determine where the surface lies for this x position
                float surface = surfaceHeights[x];

                // Update the on-going check for the surface position
                surfacePosition = this.UpdateSurfacePosition(surfacePosition, surface, originY);

                for (int y = 0; y < Metrics.ChunkHeight; y++)
                {
                    chunk.Points[x, y] = this.CreatePoint(originX + x, originY + y, surface);
                }
            }

            chunk.SurfacePosition = surfacePosition.Value;
        }

        /// <summary>
        /// Creates the cave attributes.
        /// </summary>
        /// <param name="seed">The seed value for the terrain.</param>
        /// <returns>The cave attributes.</returns>
        private CaveAttributes[] CreateCaveAttributes(int seed)
        {
            var caves = new List<CaveAttributes>();
            var random = new Random(seed);

            // Test cave
            caves.Add(new CaveAttributes(random.Next(), 0.5f));

            return caves.ToArray();
        }

        /// <summary>
        /// Determines the surface position relative to an entire chunk during point generation.
        /// </summary>
        /// <param name="current">The previously identified surface position.</param>
        /// <param name="surface">The surface position.</param>
        /// <param name="chunkOriginY">The Y origin of the chunk.</param>
        /// <returns>THe updated surface position.</returns>
        private SurfacePosition UpdateSurfacePosition(SurfacePosition? current, float surface, int chunkOriginY)
        {
            if (current.HasValue && current.Value == SurfacePosition.Inside)
            {
                return current.Value;
            }
            else
            {
                int surfaceI = (int)System.Math.Floor(surface);
                SurfacePosition surfacePositionX;
                if (surfaceI < chunkOriginY)
                {
                    surfacePositionX = SurfacePosition.Below;
                }
                else
                {
                    if (surfaceI < chunkOriginY + Metrics.ChunkHeight)
                    {
                        surfacePositionX = SurfacePosition.Inside;
                    }
                    else
                    {
                        surfacePositionX = SurfacePosition.Above;
                    }
                }

                if (current.HasValue && current.Value != surfacePositionX)
                {
                    return SurfacePosition.Inside;
                }
                else
                {
                    return surfacePositionX;
                }
            }
        }

        /// <summary>
        /// Creates the point at the given world position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="surface">The surface height at this x position.</param>
        /// <returns>The point.</returns>
        private TerrainPoint CreatePoint(int x, int y, float surface)
        {
            int surfaceI = (int)System.Math.Floor(surface);
            float surfaceFractional = surface - surfaceI;

            byte foreground;
            byte background;
            TerrainMaterial material;
            Colour? light;

            // Determine the density and material at this point
            if (y > surfaceI)
            {
                // The voxel lies above the surface
                foreground = TerrainPoint.DensityMax;
                background = TerrainPoint.DensityMax;
                material = TerrainMaterial.Undefined;
                light = Colour.White;
            }
            else
            {
                material = TerrainMaterial.Dirt;
                light = Colour.White;

                if (y < surfaceI)
                {
                    // The voxel lies under the surface
                    foreground = TerrainPoint.DensityMin;
                    background = TerrainPoint.DensityMin;
                }
                else
                {
                    // This voxel lies on the surface, so scale the density by the noise value
                    foreground = (byte)(TerrainPoint.DensityMax - (TerrainPoint.DensityMax * surface));
                    background = (byte)(TerrainPoint.DensityMax - (TerrainPoint.DensityMax * surface));
                }

                // Dig out underground features in the foreground
                // TODO
                // foreDensity = ???
            }

            // TODO: Remove this
            int val = 255 - (int)(System.Math.Abs((float)y) * 8);
            byte lightTest = val > 0 ? (byte)val : (byte)0;
            light = new Colour(lightTest, lightTest, lightTest);
            // TODO: Remove this

            return new TerrainPoint(foreground, background, material, light);
        }
    }
}