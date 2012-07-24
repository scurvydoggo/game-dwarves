using System.Collections.Generic;

/// <summary>
/// Processes the terrain to generate the mesh data for each block.
/// </summary>
public abstract class TerrainMeshWorker
{
    /// <summary>
    /// The worker responsible for processing the meshes.
    /// </summary>
    private ParallelWorker<Vector2I> worker;

    /// <summary>
    /// Initializes a new instance of the TerrainMeshProcessor class.
    /// </summary>
    /// <param name="terrain">The terrain which is processed.</param>
    /// <param name="blockMeshes">The collection of block meshes.</param>
    public TerrainMeshWorker(Terrain terrain, BlockMeshCollection blockMeshes)
    {
        this.Terrain = terrain;
        this.BlockMeshes = blockMeshes;
        this.worker = new ParallelWorker<Vector2I>();
    }

    /// <summary>
    /// Gets the terrain which is processed.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Gets the collection of block meshes.
    /// </summary>
    public BlockMeshCollection BlockMeshes { get; private set; }

    /// <summary>
    /// Enqueues a task to process the chunk with the given index.
    /// </summary>
    /// <param name="chunkIndex">The chunk index.</param>
    public void EnqueueForMeshGeneration(Vector2I chunkIndex)
    {
        this.worker.Enqueue(this.ProcessChunk, chunkIndex);
    }
}