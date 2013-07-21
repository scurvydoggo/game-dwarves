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
                    int worldX = originX + x;
                    int worldY = originY + y;

                    // Calculate the background and foreground densities
                    byte background = this.GetBackgroundDensity(worldX, worldY, surface);

                    // Get the cave densities for this point
                    PointDensities densities = this.GetCaveDensitiesForPoint(worldX, worldY);
                    if (densities.Origin < background)
                    {
                        // The density cannot be less than the background (ie. if the background is dug out, the
                        // foreground cannot be filled in)
                        densities.Origin = background;
                    }

                    // Update the densities
                    byte foreground = densities.Origin > background ? densities.Origin : background;
                    this.UpdateForegroundIfGreaterDensity(chunk, x, y, foreground);
                    if (x < Metrics.ChunkWidth - 1)
                    {
                        this.UpdateForegroundIfGreaterDensity(chunk, x + 1, y, densities.Right);
                    }

                    if (y < Metrics.ChunkHeight - 1)
                    {
                        this.UpdateForegroundIfGreaterDensity(chunk, x, y + 1, densities.Up);
                    }

                    // Determine the material
                    TerrainMaterial material = this.GetMaterial(worldX, worldY, surface);

                    // TODO: Remove this
                    int val = 255 - (int)(System.Math.Abs((float)worldY) * 8);
                    byte lightTest = val > 0 ? (byte)val : (byte)0;
                    var light = new Colour(lightTest, lightTest, lightTest);
                    // TODO: Remove this

                    // Update the point
                    TerrainPoint point = chunk.Points[x, y];
                    point.Background = background;
                    point.Material = material;
                    point.Light = light;
                }
            }

            chunk.SurfacePosition = surfacePosition.Value;
        }

        /// <summary>
        /// Updates the foreground density if the given density is larger than the point's.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="density">The foreground density.</param>
        private void UpdateForegroundIfGreaterDensity(TerrainChunk chunk, int x, int y, byte density)
        {
            TerrainPoint point = chunk.Points[x, y];
            if (point.Foreground < density)
            {
                point.Foreground = density;
            }
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
            caves.Add(new CaveAttributes(0.4f, random.Next(), 0.025f));

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
        /// Gets the densities for the given point's up/right directional segments. These densities are produced by the
        /// cave digging algorithm.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The densities.</returns>
        private PointDensities GetCaveDensitiesForPoint(int x, int y)
        {
            var densities = new PointDensities();

            foreach (CaveAttributes cave in this.caves)
            {
                float xF = x * cave.Frequency;
                float yF = y * cave.Frequency;

                float origin = this.baseGenerator.Generate(xF, yF);
                float up = this.baseGenerator.Generate(xF, yF + cave.Frequency);
                float right = this.baseGenerator.Generate(xF + cave.Frequency, yF);

                // Calculate the up segment densities
                SegmentDensities segmentUp = this.CalculateDensities(origin, up, cave.BoundaryValue);
                SegmentDensities segmentRight = this.CalculateDensities(origin, right, cave.BoundaryValue);

                byte dOrigin = segmentUp.Start > segmentRight.Start ? segmentUp.Start : segmentRight.Start;
                if (densities.Origin < origin)
                {
                    densities.Origin = dOrigin;
                }

                if (densities.Up < segmentUp.End)
                {
                    densities.Up = segmentUp.End;
                }

                if (densities.Right < segmentRight.End)
                {
                    densities.Right = segmentRight.End;
                }
            }

            return densities;
        }

        /// <summary>
        /// Calculate the densities of the segment for the given noise values.
        /// </summary>
        /// <param name="origin">The noise value at the origin.</param>
        /// <param name="other">The noise value at the other end of the segment.</param>
        /// <param name="boundary">The noise value at which the terrain boundary lies.</param>
        /// <returns>The densities.</returns>
        private SegmentDensities CalculateDensities(float origin, float other, float boundary)
        {
            if (origin < boundary)
            {
                if (other < boundary)
                {
                    // The segment is fully within the terrain
                    return new SegmentDensities(TerrainPoint.DensityMin, TerrainPoint.DensityMin);
                }
                else
                {
                    // The terrain boundary cuts through this surface with the 'open air' away from the origin
                    float intersection = (boundary - origin) / (other - origin);
                    if (intersection > 0.5f)
                    {
                        return new SegmentDensities(
                            TerrainPoint.DensityMin,
                            (byte)(TerrainPoint.DensityMax * (1.5f - intersection)));
                    }
                    else
                    {
                        return new SegmentDensities(
                            (byte)(TerrainPoint.DensityMax * (0.5f - intersection)),
                            TerrainPoint.DensityMax);
                    }
                }
            }
            else
            {
                if (other < boundary)
                {
                    // The terrain boundary cuts through this surface with the 'open air' at the origin side
                    float boundaryN = other - origin;
                    float intersection = boundaryN != 0 ? (boundary - origin) / boundaryN : 0;
                    if (intersection > 0.5f)
                    {
                        return new SegmentDensities(
                            TerrainPoint.DensityMax,
                            (byte)(TerrainPoint.DensityMax * (intersection - 0.5f)));
                    }
                    else
                    {
                        return new SegmentDensities(
                            (byte)(TerrainPoint.DensityMax * (intersection + 0.5f)),
                            TerrainPoint.DensityMin);
                    }
                }
                else
                {
                    // The segment is fully outside the terrain
                    return new SegmentDensities(TerrainPoint.DensitySurface + 1, TerrainPoint.DensitySurface + 1);
                }
            }
        }

        /// <summary>
        /// The densities of two segments that share the same bottom/left point. A segment is the space between two
        /// adjacent terrain points which has significance a terrain edge cuts through it.
        /// </summary>
        private struct SegmentDensities
        {
            /// <summary>
            /// Initialises a new instance of the SegmentDensities structure.
            /// </summary>
            /// <param name="start">The density at the start of the segment.</param>
            /// <param name="end">The density at the end of the segment.</param>
            public SegmentDensities(byte start, byte end)
                : this()
            {
                this.Start = start;
                this.End = end;
            }

            /// <summary>
            /// Gets the density at the start of the segment.
            /// </summary>
            public byte Start { get; private set; }

            /// <summary>
            /// Gets the density at the end of the segment.
            /// </summary>
            public byte End { get; private set; }
        }

        /// <summary>
        /// The densities for a point its segments in the up and right directions.
        /// </summary>
        private struct PointDensities
        {
            /// <summary>
            /// Gets or sets the density at the origin.
            /// </summary>
            public byte Origin { get; set; }

            /// <summary>
            /// Gets or sets the density at the upward segment's endpoint.
            /// </summary>
            public byte Up { get; set; }

            /// <summary>
            /// Gets or sets the density at the right-ward segment's endpoint.
            /// </summary>
            public byte Right { get; set; }
        }
    }
}