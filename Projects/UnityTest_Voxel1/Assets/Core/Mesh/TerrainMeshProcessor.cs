using System.Collections.Generic;

/// <summary>
/// Processes the terrain to generate the mesh data for each block.
/// </summary>
public class TerrainMeshProcessor
{
    /// <summary>
    /// The queue of chunks that need to be processed.
    /// </summary>
    private Queue<Chunk> queue;

    /// <summary>
    /// Initializes a new instance of the TerrainMeshProcessor class.
    /// </summary>
    /// <param name="terrain">The terrain which is processed.</param>
    /// <param name="blockMeshes">The collection of block meshes.</param>
    public TerrainMeshProcessor(Terrain terrain, BlockMeshCollection blockMeshes)
    {
        this.Terrain = terrain;
        this.BlockMeshes = blockMeshes;
        this.queue = new Queue<Chunk>();
    }

    /// <summary>
    /// Gets the terrain which is processed.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Gets the collection of block meshes.
    /// </summary>
    public BlockMeshCollection BlockMeshes { get; private set; }
}
