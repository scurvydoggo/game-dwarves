// ----------------------------------------------------------------------------
// <copyright file="ChunkVoxelGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core.Noise;
    using UnityEngine;

    /// <summary>
    /// Generates the voxels for a chunk.
    /// </summary>
    public class ChunkVoxelGenerator
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public const int DefaultSurfaceOrigin = 0;

        /// <summary>
        /// Default value.
        /// </summary>
        public const int DefaultSurfaceAmplitude = 50;

        /// <summary>
        /// Initialises a new instance of the ChunkVoxelGenerator class.
        /// </summary>
        /// <param name="noiseGenerator">The noise generator.</param>
        public ChunkVoxelGenerator(NoiseGenerator noiseGenerator)
        {
            this.NoiseGenerator = noiseGenerator;
            this.SurfaceOrigin = DefaultSurfaceOrigin;
            this.SurfaceAmplitude = DefaultSurfaceAmplitude;
        }

        /// <summary>
        /// Gets the noise generator.
        /// </summary>
        public NoiseGenerator NoiseGenerator { get; private set; }

        /// <summary>
        /// Gets or sets the Y position around which the generated surface oscillates
        /// </summary>
        public int SurfaceOrigin { get; set; }

        /// <summary>
        /// Gets or sets the maximum Y distance that the surface can fluctuate from the origin (above or below).
        /// </summary>
        public int SurfaceAmplitude { get; set; }

        /// <summary>
        /// Generate the heights for each x-coordinate for the chunk at the given x index.
        /// </summary>
        /// <param name="chunkIndexX">The chunk index x component.</param>
        /// <returns>The surface heights.</returns>
        public float[] GenerateSurfaceHeights(int chunkIndexX)
        {
            // Generate the array of surface heights
            var heights = new float[Chunk.Width];

            int originX = chunkIndexX * Chunk.Width;
            for (int x = 0; x < Chunk.Width; x++)
            {
                // Generate the noise value at this x position
                float noise = this.NoiseGenerator.Generate(originX + x);

                // Obtain the height by scaling the noise with the surface amplitude
                heights[x] = this.SurfaceOrigin + (noise * this.SurfaceAmplitude);
            }

            return heights;
        }

        /// <summary>
        /// Generate the voxels for the given terrain chunk.
        /// </summary>
        /// <param name="voxels">The voxels to populate.</param>
        /// <param name="surfaceHeights">The height of the surface at each x position.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void Generate(ChunkVoxels voxels, float[] surfaceHeights, Position chunkIndex)
        {
            int originY = chunkIndex.Y * Chunk.Height;

            for (int x = 0; x < Chunk.Width; x++)
            {
                float surfaceHeightF = surfaceHeights[x];
                int surfaceHeightI = (int)Math.Floor(surfaceHeightF);
                float deltaHeight = surfaceHeightF - surfaceHeightI;

                for (int y = 0; y < Chunk.Height; y++)
                {
                    int height = originY + y;
                    
                    // Create the voxel at this point
                    Voxel voxel;
                    if (height > surfaceHeightI)
                    {
                        // This voxel lies above the surface
                        voxel = Voxel.Air;
                    }
                    else
                    {
                        // Determine the material
                        var material = TerrainMaterial.Dirt;

                        if (height == surfaceHeightI)
                        {
                            // This voxel lies on the surface, so scale the density by the noise value
                            byte density = (byte)(Voxel.DensityMax - (Voxel.DensityMax * deltaHeight));

                            // The density property stores 2 densities. The 'foreground' density which is that which
                            // can be dug, and the 'background' density which represents the original density
                            // Set the foreground and background densities both
                            density = (byte)((density << 4) | density);

                            voxel = new Voxel(material, density);
                        }
                        else
                        {
                            // The voxel lies under the surface
                            voxel = new Voxel(material, Voxel.DensityMin);
                        }
                    }

                    // Set the voxel
                    voxels[x, y] = voxel;
                }
            }
        }

        /// <summary>
        /// Fill the terrain with the given material below the surface and air above.
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="surfaceHeights">The y value of each surface point.</param>
        /// <param name="material">The material to fill below surface.</param>
        private void FillAroundSurface(
            ChunkVoxels chunk,
            Position chunkIndex,
            int[] surfaceHeights,
            TerrainMaterial material)
        {
            for (int chunkX = 0; chunkX < Chunk.Width; chunkX++)
            {
                int surfaceHeight = surfaceHeights[chunkX];

                for (int chunkY = 0; chunkY < Chunk.Height; chunkY++)
                {
                    if (chunkY < surfaceHeight)
                    {
                        chunk[chunkX, chunkY] = new Voxel(material, Voxel.DensityMin);
                    }
                    else if (chunkY > surfaceHeight)
                    {
                        chunk[chunkX, chunkY] = new Voxel(TerrainMaterial.Air, Voxel.DensityMax);
                    }
                }
            }
        }
    }
}