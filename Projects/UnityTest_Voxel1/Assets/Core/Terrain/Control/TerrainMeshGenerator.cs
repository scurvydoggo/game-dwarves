// ----------------------------------------------------------------------------
// <copyright file="TerrainMeshGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Generates meshes for terrain blocks.
/// </summary>
public abstract class TerrainMeshGenerator
{
    /// <summary>
    /// Indicates how many blocks should be rendered depth-wise for terrain walls.
    /// </summary>
    public const int BlockDepth = 3;

    /// <summary>
    /// Initializes a new instance of the TerrainMeshGenerator class.
    /// </summary>
    public TerrainMeshGenerator()
    {
        this.MaterialLookup = new MaterialLookup();
    }

    /// <summary>
    /// Gets or sets the material lookup instance.
    /// </summary>
    protected MaterialLookup MaterialLookup { get; set; }

    /// <summary>
    /// Update the terrain's mesh for the given chunk.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="chunkIndex">The index of the chunk to update the mesh for.</param>
    public virtual void UpdateChunkMesh(Terrain terrain, Vector2I chunkIndex)
    {
        Chunk chunk = terrain.Blocks[chunkIndex];
		
		// Ensure that the chunk is of the render type
		if (chunk.Usage & ChunkUsage.Rendering == 0)
		{
			throw new ApplicationException(string.Format("The chunk {0} does not support rendering.", chunkIndex));
		}
		
        // Get the origin of the chunk in world coordinates
        Vector2I chunkOrigin = new Vector2I(chunkIndex.X * Chunk.SizeX, chunkIndex.Y * Chunk.SizeY);

        // Get the neighbouring chunks so that boundary checks can be made. If a neighbour cannot be retrieved, then
        // we may be at the edge of the world, in which case that region shouldn't be accessible so all is ok
        Chunk chunkUp, chunkRight, chunkDown, chunkLeft;
        terrain.Blocks.TryGetChunk(new Vector2I(chunkIndex.Y + 1, chunkIndex.X), out chunkUp);
        terrain.Blocks.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X + 1), out chunkRight);
        terrain.Blocks.TryGetChunk(new Vector2I(chunkIndex.Y - 1, chunkIndex.X), out chunkDown);
        terrain.Blocks.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X - 1), out chunkLeft);

        for (int x = 0; x < Chunk.SizeX; x++)
        {
            for (int y = 0; y < Chunk.SizeY; y++)
            {
                // Get the block index
                int index = Chunk.GetBlockIndex(x, y);

                // Get the block
                Block block = chunk[index];

                // Calculate the block position in world coordinates
                var blockPos = new Vector2I(chunkOrigin.X + x, chunkOrigin.Y + y);

                // If there is no block here, remove any mesh that may exist at this position and continue
                if (block.BlockType == BlockType.None)
                {
                    terrain.Mesh.RemoveMesh(blockPos);
                    continue;
                }

                // Get the block above this one
                Block blockUp;
                if ((index & Chunk.MaskXNot) != 0)
                {
                    blockUp = chunk[index + Chunk.Navigation.Up];
                }
                else if (chunkUp != null)
                {
                    blockUp = chunkUp[index | Chunk.Navigation.LastRow];
                }
                else
                {
                    blockUp = Block.None;
                }

                // Get the block to the right of this one
                Block blockRight;
                if ((index & Chunk.MaskX) != Chunk.MaskX)
                {
                    blockRight = chunk[index + Chunk.Navigation.Next];
                }
                else if (chunkRight != null)
                {
                    blockRight = chunkRight[index & Chunk.MaskXNot];
                }
                else
                {
                    blockRight = Block.None;
                }

                // Get the block below this one
                Block blockDown;
                if ((index & Chunk.Navigation.LastRow) != Chunk.Navigation.LastRow)
                {
                    blockDown = chunk[index + Chunk.Navigation.Down];
                }
                else if (chunkDown != null)
                {
                    blockDown = chunkDown[index & Chunk.MaskX];
                }
                else
                {
                    blockDown = Block.None;
                }

                // Get the block to the left of this one
                Block blockLeft;
                if ((index & Chunk.MaskX) != 0)
                {
                    blockLeft = chunk[index + Chunk.Navigation.Prev];
                }
                else if (chunkLeft != null)
                {
                    blockLeft = chunkLeft[index | Chunk.MaskX];
                }
                else
                {
                    blockLeft = Block.None;
                }

                // Create the mesh for this block
                BlockMesh mesh = this.CreateBlockMesh(block, blockPos, blockUp, blockRight, blockDown, blockLeft);

                // Add the mesh to the cloud
                terrain.Mesh.SetMesh(blockPos, mesh);
            }
        }
    }
	
    /// <summary>
    /// Remove the terrain's mesh for the given chunk.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="chunkIndex">The index of the chunk to remove the mesh for.</param>
    public virtual void RemoveChunkMesh(Terrain terrain, Vector2I chunkIndex)
    {
        Chunk chunk = terrain.Blocks[chunkIndex];
		
        // Get the origin of the chunk in world coordinates
        Vector2I chunkOrigin = new Vector2I(chunkIndex.X * Chunk.SizeX, chunkIndex.Y * Chunk.SizeY);
		
		// Remove all blocks for the chunk from the mesh
        for (int x = chunkOrigin.X; x < chunkOrigin.X + Chunk.SizeX; x++)
        {
            for (int y = chunkOrigin.Y; y < chunkOrigin.Y + Chunk.SizeY; y++)
            {
                terrain.Mesh.RemoveMesh(new Vector2I(x, y));
			}
		}
	}

    /// <summary>
    /// Create a mesh for the given block.
    /// </summary>
    /// <param name="block">The block to create the mesh for.</param>
    /// <param name="position">The position of the block.</param>
    /// <param name="blockUp">The block above.</param>
    /// <param name="blockRight">The block to the right.</param>
    /// <param name="blockDown">The block below.</param>
    /// <param name="blockLeft">The block to the left.</param>
    /// <returns>The mesh data for the block.</returns>
    protected abstract BlockMesh CreateBlockMesh(
        Block block,
        Vector2I position,
        Block blockUp,
        Block blockRight,
        Block blockDown,
        Block blockLeft);
}