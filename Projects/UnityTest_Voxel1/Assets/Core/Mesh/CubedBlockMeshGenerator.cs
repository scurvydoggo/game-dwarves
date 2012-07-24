
/// <summary>
/// Generates meshes for terrain blocks in which blocks are represented by simple cubes.
/// </summary>
public class CubedBlockMeshGenerator : BlockMeshGenerator
{
    /// <summary>
    /// Initializes a new instance of the CubedBlockMeshGenerator class.
    /// </summary>
    /// <param name="terrain">The terrain object from which the meshes are generated.</param>
    public CubedBlockMeshGenerator(Terrain terrain)
        : base(terrain)
    {
    }

    /// <summary>
    /// Populates the mesh cloud with data for the given chunk.
    /// </summary>
    /// <param name="meshCloud">The mesh cloud to be populated.</param>
    /// <param name="chunkIndex">The index of the chunk from which meshes will be generated.</param>
    public override void GenerateBlockMeshes(BlockMeshCloud meshCloud, Vector2I chunkIndex)
    {
        Chunk chunk = this.Terrain[chunkIndex];

        // Get the origin of the chunk in world coordinates
        Vector2I origin = new Vector2I(chunkIndex.X * Chunk.SizeX, chunkIndex.Y * Chunk.SizeY);

        // Get the neighbouring chunks so that boundary checks can be made. If a neighbour cannot be retrieved, then
        // we may be at the edge of the world, in which case that region shouldn't be accessible so all is ok
        Chunk chunkUp, chunkRight, chunkDown, chunkLeft;
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y + 1, chunkIndex.X), out chunkUp);
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X + 1), out chunkRight);
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y - 1, chunkIndex.X), out chunkDown);
        this.Terrain.TryGetChunk(new Vector2I(chunkIndex.Y, chunkIndex.X - 1), out chunkLeft);

        for (int index = Chunk.Navigation.Start; index < Chunk.Navigation.End; index += Chunk.Navigation.Next)
        {
            Block block = chunk[index];

            // Get the block above this one
            Block blockUp = Block.Unknown;
            if ((index & Chunk.MaskXNot) != 0)
            {
                blockUp = chunk[index + Chunk.Navigation.Up];
            }
            else if (chunkUp != null)
            {
                blockUp = chunkUp[index | Chunk.Navigation.LastRow];
            }

            // Get the block to the right of this one
            Block blockRight = Block.Unknown;
            if ((index & Chunk.MaskX) != Chunk.MaskX)
            {
                blockRight = chunk[index + Chunk.Navigation.Next];
            }
            else if (chunkRight != null)
            {
                blockRight = chunkRight[index & Chunk.MaskXNot];
            }

            // Get the block below this one
            Block blockBelow = Block.Unknown;
            if ((index & Chunk.Navigation.LastRow) != Chunk.Navigation.LastRow)
            {
                blockBelow = chunk[index + Chunk.Navigation.Down];
            }
            else if (chunkDown != null)
            {
                blockBelow = chunkDown[index & Chunk.MaskX];
            }

            // Get the block to the left of this one
            Block blockLeft = Block.Unknown;
            if ((index & Chunk.MaskX) != 0)
            {
                blockLeft = chunk[index + Chunk.Navigation.Left];
            }
            else if (chunkLeft != null)
            {
                blockLeft = chunkLeft[index | Chunk.SizeX];
            }
        }
    }
}