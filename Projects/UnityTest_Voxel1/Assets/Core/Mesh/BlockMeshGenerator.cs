/// <summary>
/// Generates meshes for terrain blocks.
/// </summary>
public abstract class BlockMeshGenerator
{
    /// <summary>
    /// Initializes a new instance of the BlockMeshGenerator class.
    /// </summary>
    /// <param name="terrain">The terrain object from which the meshes are generated.</param>
    public BlockMeshGenerator(Terrain terrain)
    {
        this.Terrain = terrain;
    }

    /// <summary>
    /// Gets the terrain object from which the meshes are generated.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Populates the mesh cloud with data for the given chunk.
    /// </summary>
    /// <param name="meshCloud">The mesh cloud to be populated.</param>
    /// <param name="chunkIndex">The index of the chunk from which meshes will be generated.</param>
    public virtual void GenerateBlockMeshes(BlockMeshCloud meshCloud, Vector2I chunkIndex)
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
                blockUp = Block.Unknown;
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
                blockRight = Block.Unknown;
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
                blockDown = Block.Unknown;
            }

            // Get the block to the left of this one
            Block blockLeft;
            if ((index & Chunk.MaskX) != 0)
            {
                blockLeft = chunk[index + Chunk.Navigation.Prev];
            }
            else if (chunkLeft != null)
            {
                blockLeft = chunkLeft[index | Chunk.SizeX];
            }
            else
            {
                blockLeft = Block.Unknown;
            }

            // Create the mesh for this block
            BlockMesh mesh = this.CreateBlockMesh(block, blockUp, blockRight, blockDown, blockLeft);
        }
    }

    /// <summary>
    /// Create a mesh for the given block.
    /// </summary>
    /// <param name="block">The block to create the mesh for.</param>
    /// <param name="blockUp">The block above.</param>
    /// <param name="blockRight">The block to the right.</param>
    /// <param name="blockDown">The block below.</param>
    /// <param name="blockLeft">The block to the left.</param>
    /// <returns></returns>
    protected abstract BlockMesh CreateBlockMesh(
        Block block,
        Block blockUp,
        Block blockRight,
        Block blockDown,
        Block blockLeft);
}