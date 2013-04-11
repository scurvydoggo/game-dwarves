// ----------------------------------------------------------------------------
// <copyright file="TerrainGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using Dwarves.Core.Lighting;
    using Dwarves.Core.Math;
    using Dwarves.Core.Math.Noise;
    using UnityEngine;

    /// <summary>
    /// Generates voxel terrain.
    /// </summary>
    public class TerrainGenerator
    {
        /// <summary>
        /// Initialises a new instance of the TerrainGenerator class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="noiseGenerator">The noise generator.</param>
        /// <param name="surfaceAmplitude">The distance from the mean surface height that the terrain oscillates.
        /// </param>
        public TerrainGenerator(DwarfTerrain terrain, INoiseGenerator noiseGenerator, int surfaceAmplitude)
        {
            this.Terrain = terrain;
            this.NoiseGenerator = noiseGenerator;
            this.SurfaceAmplitude = surfaceAmplitude;
        }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public DwarfTerrain Terrain { get; private set; }

        /// <summary>
        /// Gets or sets the noise generator.
        /// </summary>
        public INoiseGenerator NoiseGenerator { get; set; }

        /// <summary>
        /// Gets or sets the distance from the mean surface height that the terrain oscillates.
        /// </summary>
        public int SurfaceAmplitude { get; set; }

        /// <summary>
        /// Creates a new chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk.</returns>
        public TerrainChunk CreateChunk(Vector2I chunkIndex)
        {
            // Get/generate the surface heights for this chunk
            float[] surface;
            if (!this.Terrain.SurfaceHeights.TryGetValue(chunkIndex.X, out surface))
            {
                surface = this.GenerateSurfaceHeights(chunkIndex.X);
            }

            return this.CreateChunk(chunkIndex, surface);
        }

        /// <summary>
        /// Generate the heights for each x-coordinate for the given chunk.
        /// </summary>
        /// <param name="chunkIndexX">The chunk index's x component.</param>
        /// <returns>The surface heights.</returns>
        private float[] GenerateSurfaceHeights(int chunkIndexX)
        {
            var heights = new float[this.Terrain.ChunkWidth];

            int originX = chunkIndexX * this.Terrain.ChunkWidth;
            for (int x = 0; x < this.Terrain.ChunkWidth; x++)
            {
                // Generate the noise value at this x position
                float noise = this.NoiseGenerator.Generate(originX + x);

                // Obtain the height by scaling the noise with the surface amplitude
                heights[x] = noise * this.SurfaceAmplitude;
            }

            return heights;
        }

        /// <summary>
        /// Generate a new chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="surfaceHeights">The surface heights.</param>
        /// <returns>The chunk.</returns>
        private TerrainChunk CreateChunk(Vector2I chunkIndex, float[] surfaceHeights)
        {
            int originX = chunkIndex.X * this.Terrain.ChunkWidth;
            int originY = chunkIndex.Y * this.Terrain.ChunkHeight;

            // Fill the points
            TerrainPoint[,] points = new TerrainPoint[this.Terrain.ChunkWidth, this.Terrain.ChunkHeight];
            SurfacePosition? surfacePositionChunk = null;
            for (int x = 0; x < this.Terrain.ChunkWidth; x++)
            {
                // Determine where the surface lies for this x position
                float surface = surfaceHeights[x];
                int surfaceI = (int)System.Math.Floor(surface);
                float surfaceFractional = surface - surfaceI;

                // Create the points
                for (int y = 0; y < this.Terrain.ChunkHeight; y++)
                {
                    points[x, y] = this.CreatePoint(originX + x, originY + y, surfaceI, surfaceFractional);
                }

                // Determine whether the surface lies above/below/inside at this x position
                SurfacePosition surfacePositionX;
                if (surfaceI < originY)
                {
                    surfacePositionX = SurfacePosition.Below;
                }
                else
                {
                    if (surfaceI < originY + this.Terrain.ChunkHeight)
                    {
                        surfacePositionX = SurfacePosition.Inside;
                    }
                    else
                    {
                        surfacePositionX = SurfacePosition.Above;
                    }
                }

                // Check if the surface so far lies above/below/inside the chunk
                if (!surfacePositionChunk.HasValue)
                {
                    surfacePositionChunk = surfacePositionX;
                }
                else if (surfacePositionChunk.Value != surfacePositionX)
                {
                    surfacePositionChunk = SurfacePosition.Inside;
                }
            }

            return new TerrainChunk(points, surfacePositionChunk.Value);
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
            int val =  255 - (int)(System.Math.Abs((float)y) * 8);
            byte lightTest = val > 0 ? (byte)val : (byte)0;
            light = new Colour(lightTest, lightTest, lightTest);
            // TODO: Remove this

            // Create the voxel at each depth point
            TerrainVoxel[] voxels = new TerrainVoxel[this.Terrain.ChunkDepth];
            for (int z = 0; z < voxels.Length; z++)
            {
                voxels[z] = z > 0 && z < voxels.Length - 1 ?
                    new TerrainVoxel(material, density) : TerrainVoxel.CreateEmpty();
            }

            return new TerrainPoint(voxels, light);
        }
    }
}