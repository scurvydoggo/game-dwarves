using System.Collections.Generic;

/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class CubedBlockMeshGenerator : BlockMeshGenerator
{
    /// <summary>
    /// Populates the mesh cloud with data for the given chunk.
    /// </summary>
    /// <param name="meshCloud">The mesh cloud to be populated.</param>
    /// <param name="chunkIndex">The index of the chunk from which meshes will be generated.</param>
    public override void GenerateBlockMeshes(BlockMeshCloud meshCloud, Vector2I chunkIndex)
    {
        Chunk chunk = this.Terrain[chunkIndex];

        // Get the origin of the chunk in world coordinates
        Vector2I origin = new Vector2I(chunkIndex.X * ChunkSizeX, chunkIndex.Y * ChunkSizeY);

        // Get the neighbouring chunks so that boundary checks can be made. If a neighbour cannot be retrieved, then
        // we may be at the edge of the world, in which case that region shouldn't be accessible so all is ok
        Chunk chunkUp, chunkRight, chunkDown, chunkLeft;
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y + 1, chunkIndex.X), out chunkUp); 
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X + 1), out chunkRight);
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y - 1, chunkIndex.X), out chunkDown);
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X - 1), out chunkLeft);

        for (int blockIndex = Chunk.Navigation.Start; chunk < Chunk.Navigation.End; chunk += Chunk.Navigation.Next)
        {
            Block block = chunk[blockIndex];

            // Get the block above this one
            Block blockUp = null;
            if (blockIndex >= Chunk.SizeX)
            {
                blockUp = chunk[blockIndex + Chunk.Navigation.Up];
            }
            else if (chunkUp != null)
            {
                blockUp = chunkUp[blockIndex + Chunk.Navigation.LastRowStart];
            }

            // Get the block to the right of this one
            Block blockRight = null;
            if (blockIndex & Chunk.MaskX < Chunk.MaskX)
            {
                blockRight = chunk[blockIndex + Chunk.Navigation.Right];
            }
            else if (chunkRight != null)
            {
                blockRight = chunkRight[blockIndex & Chunk.MaskXNot];
            }

            // Get the block below this one
            Block blockBelow = null;
            if (blockIndex < Chunk.Navigation.LastRowStart)
            {
                blockBelow = chunk[blockIndex + Chunk.Navigation.Down];
            }
            else if (chunkBelow != null)
            {
                blockBelow = chunkBelow[blockIndex & Chunk.MaskX];
            }

            // Get the block to the left of this one
            Block blockLeft = null;
            if (blockIndex & Chunk.MaskX > 0)
            {
                blockLeft = chunk[blockIndex + Chunk.Navigation.Left];
            }
            else if (chunkLeft != null)
            {
                blockLeft = chunkLeft[blockIndex + Chunk.SizeX - 1];
            }
        }
    }
}