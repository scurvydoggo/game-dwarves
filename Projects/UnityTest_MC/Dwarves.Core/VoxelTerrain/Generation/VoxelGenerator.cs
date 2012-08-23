// ----------------------------------------------------------------------------
// <copyright file="VoxelGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    using Dwarves.Core.Noise;

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

        /// <summary>
        /// Gets or sets the Y position around which the generated surface oscillates
        /// </summary>
        public int SurfaceOrigin { get; set; }

        /// <summary>
        /// Gets or sets the maximum Y distance that the surface can fluctuate from the origin (above or below).
        /// </summary>
        public int SurfaceMaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the period of oscillation for the generated surface.
        /// </summary>
        public int SurfacePeriod { get; set; }

        /// <summary>
        /// Generate the voxels for the chunk.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void Generate(ChunkVoxels chunk, Position chunkIndex)
        {
            int[] surfaceBoundary = this.GenerateSurface(chunk, chunkIndex);
        }

        /// <summary>
        /// Generate the surface voxels and return an array indicating the Y value of each surface point.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>Array indicating the Y value of each surface point.</returns>
        private int[] GenerateSurface(ChunkVoxels chunk, Position chunkIndex)
        {
            int[] surfaceBoundary = new int[Chunk.Width];

            if (chunkIndex.Y >= this.SurfaceOrigin - this.SurfaceMaxHeight)
            {
                if (chunkIndex.Y <= this.SurfaceOrigin + this.SurfaceMaxHeight)
                {
                    // The surface may cut through this chunk
                    for (int x = 0; x < Chunk.Width; x++)
                    {
                        // Calculate the point of the position to input into the noise function.
                        float surfaceX = (chunkIndex.X + x) / this.SurfacePeriod;

                        // Get the noise for this point
                        float noise = SimplexNoise.Generate(surfaceX, this.Seed);

                        // Calculate the height of the surface relative to (0,0) in chunk coordinates
                        int surfaceDistance = (int)((noise * this.SurfaceMaxHeight) + (noise > 0 ? 0.5f : -0.5f));
                        int surfaceHeight = this.SurfaceOrigin + surfaceDistance - chunkIndex.Y;

                        // Check the limits
                        if (surfaceHeight < 0)
                        {
                            surfaceHeight = 0;
                        }
                        else if (surfaceHeight > Chunk.Height)
                        {
                            surfaceHeight = Chunk.Height;
                        }

                        surfaceBoundary[x] = surfaceHeight;
                    }
                }
                else
                {
                    // This chunk is above the heighest possible surface point, so keep all heights to 0 (ie. do nothing)
                }
            }
            else
            {
                // This chunk is below the lowest possible surface point, so set all heights to max
                for (int x = 0; x < Chunk.Height; x++)
                {
                    surfaceBoundary[x] = Chunk.Height;
                }
            }

            return surfaceBoundary;
        }
    }
}