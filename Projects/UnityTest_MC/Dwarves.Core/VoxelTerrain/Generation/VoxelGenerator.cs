// ----------------------------------------------------------------------------
// <copyright file="VoxelGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    using Dwarves.Core.Noise;

    /// <summary>
    /// Generates the voxels for a chunk.
    /// </summary>
    public class VoxelGenerator
    {
        /// <summary>
        /// Default value.
        /// </summary>
        public const int DefaultSurfaceOrigin = 0;

        /// <summary>
        /// Default value.
        /// </summary>
        public const int DefaultSurfaceMaxHeight = 50;

        /// <summary>
        /// Default value.
        /// </summary>
        public const int DefaultSurfacePeriod = 100;

        /// <summary>
        /// Initializes a new instance of the VoxelGenerator class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        public VoxelGenerator(float seed)
        {
            this.Seed = seed;
            this.SurfaceOrigin = DefaultSurfaceOrigin;
            this.SurfaceMaxHeight = DefaultSurfaceMaxHeight;
            this.SurfacePeriod = DefaultSurfacePeriod;
        }

        /// <summary>
        /// Gets the seed value.
        /// </summary>
        public float Seed { get; private set; }

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
        /// Generate the voxels for the given terrain chunk.
        /// </summary>
        /// <param name="voxels">The voxels to populate.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void Generate(ChunkVoxels voxels, Position chunkIndex)
        {
            // TEST: Just fill all with dirt
            for (int i = 0; i < Chunk.Width * Chunk.Height; i++)
            {
                voxels[i] = new Voxel(TerrainMaterial.Dirt, byte.MinValue);
            }

            // Create the surface voxels
            //int[] surfaceHeights = this.GenerateSurface(voxels, chunkIndex, TerrainMaterial.Dirt);

            // Now fill the rest of the terrain
            //this.FillAroundSurface(voxels, chunkIndex, surfaceHeights, TerrainMaterial.Dirt);
        }

        /// <summary>
        /// Generate the surface voxels and return an array indicating the y value of each surface point.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="surfaceMaterial">The surface material.</param>
        /// <returns>Array indicating the y value of each surface point.</returns>
        private int[] GenerateSurface(ChunkVoxels chunk, Position chunkIndex, TerrainMaterial surfaceMaterial)
        {
            int[] surfaceBoundary = new int[Chunk.Width];

            if (chunkIndex.Y >= this.SurfaceOrigin - this.SurfaceMaxHeight)
            {
                if (chunkIndex.Y <= this.SurfaceOrigin + this.SurfaceMaxHeight)
                {
                    // The surface may cut through this chunk
                    for (int chunkX = 0; chunkX < Chunk.Width; chunkX++)
                    {
                        // Calculate the point of the position to input into the noise function.
                        float surfaceX = (chunkIndex.X + chunkX) / this.SurfacePeriod;

                        // Get the noise for this point
                        float noise = SimplexNoise.Generate(surfaceX, this.Seed);

                        // Calculate the height from the surface origin
                        float offsetYFloat = noise * this.SurfaceMaxHeight;
                        int offsetY = (int)offsetYFloat;

                        // Determine the y coordinate of the surface in chunk coordinates
                        int chunkY = this.SurfaceOrigin + offsetY - chunkIndex.Y;

                        // Check the limits
                        if (chunkY < 0)
                        {
                            surfaceBoundary[chunkX] = 0;
                        }
                        else if (chunkY > Chunk.Height)
                        {
                            surfaceBoundary[chunkX] = Chunk.Height;
                        }
                        else
                        {
                            // The surface cuts inside this chunk
                            surfaceBoundary[chunkX] = chunkY;

                            // Now calculate the density of the voxel at this height
                            byte density;
                            float densityFloat = (offsetYFloat - offsetY) * byte.MaxValue;
                            if (densityFloat >= byte.MaxValue)
                            {
                                density = byte.MaxValue;
                            }
                            else
                            {
                                density = (byte)(densityFloat + 0.5f);
                            }

                            // Update the surface voxel
                            chunk[chunkX, chunkY] = new Voxel(surfaceMaterial, density);
                        }
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
                        chunk[chunkX, chunkY] = new Voxel(material, byte.MinValue);
                    }
                    else if (chunkY > surfaceHeight)
                    {
                        chunk[chunkX, chunkY] = new Voxel(TerrainMaterial.Air, byte.MaxValue);
                    }
                }
            }
        }
    }
}