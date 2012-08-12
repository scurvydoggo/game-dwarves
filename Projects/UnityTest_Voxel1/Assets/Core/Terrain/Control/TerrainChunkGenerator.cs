// ----------------------------------------------------------------------------
// <copyright file="TerrainChunkGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Responsible for dynamically generating terrain chunks.
/// </summary>
public class TerrainChunkGenerator
{
    /// <summary>
    /// Gets the Y coordinate at which the surface is offset from.
    /// </summary>
    public const int SurfaceOriginY = 0;

    /// <summary>
    /// Gets the maxiumum Y distance that the surface can fluctuate from the surface origin (above or below).
    /// </summary>
    public const int SurfaceMaxDistanceY = 100;

    /// <summary>
    /// Generate a chunk and return it. The terrain object remains unmodified.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="chunkIndex">The chunk index.</param>
    /// <returns>The chunk.</returns>
    public Chunk GenerateChunk(Terrain terrain, Vector2I chunkIndex)
    {
        Chunk chunk = new Chunk();

        // Calculate the position of the chunk in world coordinates
        var chunkPos = new Vector2I(chunkIndex.X * Chunk.SizeX, chunkIndex.Y * Chunk.SizeY);

        // Get the surface heights for this chunk
        int[] surfaceHeights = this.GenerateSurfaceHeights(chunkPos);

        // For now, fill the terrain with mud under the surface
        for (int x = 0; x < Chunk.SizeX; x++)
        {
            int surfaceHeight = surfaceHeights[x];
            if (surfaceHeight > 0)
            {
                for (int y = 0; y < surfaceHeight; y++)
                {
                    chunk[x, y] = new Block(BlockType.Mud);
                }
            }
        }

        return chunk;
    }

    /// <summary>
    /// Generate the array of heights for the surface in this chunk. No ground may appear above the surface points.
    /// </summary>
    /// <param name="chunkPos">The origin position of the chunk.</param>
    /// <returns>An array of surface heights in chunk coordinates for each x position in the chunk.</returns>
    private int[] GenerateSurfaceHeights(Vector2I chunkPos)
    {
        int[] heights = new int[Chunk.SizeY];

        if (chunkPos.Y >= SurfaceOriginY - SurfaceMaxDistanceY)
        {
            // This chunk is in the region of the surface
            if (chunkPos.Y <= SurfaceOriginY + SurfaceMaxDistanceY)
            {
                for (int x = 0; x < Chunk.SizeX; x++)
                {
                    // Get the noise for this point
                    float noise = SimplexNoise.Generate(x);

                    // Calculate the height of the surface relative to (0,0) in chunk coordinates
                    int surfaceDistance = (int)((noise * SurfaceMaxDistanceY) + (noise > 0 ? 0.5f : -0.5f));
                    int surfaceHeight = SurfaceOriginY + surfaceDistance - chunkPos.Y;
                    
					// Check the limits
					if (surfaceHeight < 0)
                    {
                        surfaceHeight = 0;
                    }
					else if (surfaceHeight > Chunk.SizeY)
					{
						surfaceHeight = Chunk.SizeY;
					}

                    heights[x] = surfaceHeight;
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
            for (int x = 0; x < Chunk.SizeX; x++)
            {
                heights[x] = Chunk.SizeY;
            }
        }

        return heights;
    }
}