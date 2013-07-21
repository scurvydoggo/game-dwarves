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
                surfacePosition = this.CheckSurfacePosition(surfacePosition, surface, originY);

                for (int y = 0; y < Metrics.ChunkHeight; y++)
                {
                    // Calculate the background and foreground densities
                    byte background = this.GetBackgroundDensity(originX + x, originY + y, surface);
                    byte foreground = background;

                    var test = this.GetMaxSegmentDensities(originX + x, originY + y);

                    // Determine the material
                    TerrainMaterial material = this.GetMaterial(originX + x, originY + y, surface);

                    // TODO: Remove this
                    int val = 255 - (int)(System.Math.Abs((float)(originY + y)) * 8);
                    byte lightTest = val > 0 ? (byte)val : (byte)0;
                    var light = new Colour(lightTest, lightTest, lightTest);
                    // TODO: Remove this

                    // Update the point
                    TerrainPoint point = chunk.Points[x, y];
                    point.Background = background;
                    point.Material = material;
                    point.Light = light;
                    if (foreground > point.Foreground)
                    {
                        point.Foreground = foreground;
                    }
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
            caves.Add(new CaveAttributes(0.5f, random.Next(), 0.05f));

            return caves.ToArray();
        }

        /// <summary>
        /// Determines the surface position relative to an entire chunk during point generation.
        /// </summary>
        /// <param name="current">The previously identified surface position.</param>
        /// <param name="surface">The surface position.</param>
        /// <param name="chunkOriginY">The Y origin of the chunk.</param>
        /// <returns>THe updated surface position.</returns>
        private SurfacePosition CheckSurfacePosition(SurfacePosition? current, float surface, int chunkOriginY)
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
        /// Determines the background density at the given world position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="surface">The surface height at this x position.</param>
        /// <returns>The background density.</returns>
        private byte GetBackgroundDensity(int x, int y, float surface)
        {
            int surfaceI = (int)System.Math.Floor(surface);
            if (y > surfaceI)
            {
                // The point lies above the surface
                return TerrainPoint.DensityMax;
            }
            else
            {
                if (y < surfaceI)
                {
                    // The point lies fully below the surface
                    return TerrainPoint.DensityMin;
                }
                else
                {
                    // The surface lies between this point and the point above it, so interpolate the density
                    return (byte)(TerrainPoint.DensityMax - (TerrainPoint.DensityMax * (surface - surfaceI)));
                }
            }
        }

        /// <summary>
        /// Determines the material at the given world position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="surface">The surface height at this x position.</param>
        /// <returns>The material.</returns>
        private TerrainMaterial GetMaterial(int x, int y, float surface)
        {
            int surfaceI = (int)System.Math.Floor(surface);
            if (y > surfaceI)
            {
                return TerrainMaterial.Undefined;
            }
            else
            {
                return TerrainMaterial.Dirt;
            }
        }

        /// <summary>
        /// Gets the maximum density at the end points of upper and right segments from the given point. The maximum
        /// densities are selected by generating each gave at these points and taking the largest 'dug out' values.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The maximum densities at the three points of the upper and right segments.</returns>
        private SegmentDensities GetMaxSegmentDensities(int x, int y)
        {
            var densities = new SegmentDensities();

            foreach (CaveAttributes cave in this.caves)
            {
                float xF = x * cave.Frequency;
                float yF = y * cave.Frequency;

                float origin = this.baseGenerator.Generate(xF, yF);
                float up = this.baseGenerator.Generate(xF, yF + cave.Frequency);
                float right = this.baseGenerator.Generate(xF + cave.Frequency, yF);
            }

            return densities;
        }

        /// <summary>
        /// The densities of two segments that share the same bottom/left point. A segment is the space between two
        /// adjacent terrain points which has significance a terrain edge cuts through it.
        /// </summary>
        private struct SegmentDensities
        {
            /// <summary>
            /// Gets or sets the density at the origin point of the two segments.
            /// </summary>
            public byte DensityOrigin { get; set; }

            /// <summary>
            /// Gets or sets the density at the top point of the vertical segment.
            /// </summary>
            public byte DensityUp { get; set; }

            /// <summary>
            /// Gets or sets the density at the right point of the horizontal segment.
            /// </summary>
            public byte DensityRight { get; set; }
        }
    }
}