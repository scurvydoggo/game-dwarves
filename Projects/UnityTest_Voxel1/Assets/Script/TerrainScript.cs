using UnityEngine;

/// <summary>
/// Script for terrain.
/// </summary>
public class TerrainScript : MonoBehaviour
{
    /// <summary>
    /// The terrain.
    /// </summary>
    private Terrain terrain;

    /// <summary>
    /// The block meshes.
    /// </summary>
    private BlockMeshCollection blockMeshes;

    /// <summary>
    /// The terrain mesh processor.
    /// </summary>
    private TerrainMeshProcessor terrainMeshProcessor;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.terrain = new Terrain();
        this.blockMeshes = new BlockMeshCollection();
        this.terrainMeshProcessor = new TerrainMeshProcessor(this.terrain, this.blockMeshes);
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}