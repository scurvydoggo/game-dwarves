// ----------------------------------------------------------------------------
// <copyright file="TerrainPointGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
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

            // Create the terrain feature generators
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
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="surfaceHeights">The surface heights.</param>
        public void GeneratePoints(Vector2I chunkIndex, TerrainChunk chunk, float[] surfaceHeights)
        {
            int originX = chunkIndex.X * Metrics.ChunkWidth;
            int originY = chunkIndex.Y * Metrics.ChunkHeight;

            // Fill the points
            SurfacePosition? surfacePosition = null;
            for (int x = 0; x < Metrics.ChunkWidth; x++)
            {
                // Determine where the surface lies for this x position
                float surface = surfaceHeights[x];
                int surfaceI = (int)System.Math.Floor(surface);
                float surfaceFractional = surface - surfaceI;

                // Create the points
                for (int y = 0; y < Metrics.ChunkHeight; y++)
                {
                    chunk.Points[x, y] = this.CreatePoint(originX + x, originY + y, surfaceI, surfaceFractional);
                }

                // Determine whether the surface lies above/below/inside at this x position
                SurfacePosition surfacePositionX;
                if (surfaceI < originY)
                {
                    surfacePositionX = SurfacePosition.Below;
                }
                else
                {
                    if (surfaceI < originY + Metrics.ChunkHeight)
                    {
                        surfacePositionX = SurfacePosition.Inside;
                    }
                    else
                    {
                        surfacePositionX = SurfacePosition.Above;
                    }
                }

                // Check if the surface so far lies above/below/inside the chunk
                if (!surfacePosition.HasValue)
                {
                    surfacePosition = surfacePositionX;
                }
                else if (surfacePosition.Value != surfacePositionX)
                {
                    surfacePosition = SurfacePosition.Inside;
                }
            }

            chunk.SurfacePosition = surfacePosition.Value;
        }

        /// <summary>
        /// Creates the point at the given world position.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="surfaceI">The integral portion of the surface height at this x position.</param>
        /// <param name="surfaceFractional">The fractional portion of the surface height at this x position.</param>
        /// <returns>The point. Null value represents an 'air' point.</returns>
        private TerrainPoint CreatePoint(int x, int y, int surfaceI, float surfaceFractional)
        {
            byte density;
            TerrainMaterial material;
            Colour? light;

            // Determine the density and material at this point
            if (y == surfaceI)
            {
                // This voxel lies on the surface, so scale the density by the noise value
                density = (byte)(TerrainVoxel.DensityMax - (TerrainVoxel.DensityMax * surfaceFractional));
                material = TerrainMaterial.Dirt;
                light = Colour.White;
            }
            else if (y <= surfaceI)
            {
                // The voxel lies under the surface
                density = TerrainVoxel.DensityMin;
                material = TerrainMaterial.Dirt;
                light = Colour.White;
            }
            else
            {
                // The voxel lies above the surface
                density = TerrainVoxel.DensityMax;
                material = TerrainMaterial.Undefined;
                light = Colour.White;
            }

            // TODO: Remove this
            int val = 255 - (int)(System.Math.Abs((float)y) * 8);
            byte lightTest = val > 0 ? (byte)val : (byte)0;
            light = new Colour(lightTest, lightTest, lightTest);
            // TODO: Remove this

            // Create the voxel at each depth point
            TerrainVoxel[] voxels = new TerrainVoxel[Metrics.ChunkDepth];
            for (int z = 0; z < voxels.Length; z++)
            {
                voxels[z] = z > 0 && z < voxels.Length - 1 ?
                    new TerrainVoxel(material, density) : TerrainVoxel.CreateEmpty();
            }

            return new TerrainPoint(voxels, light);
        }
    }
}