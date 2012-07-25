using UnityEngine;

/// <summary>
/// Script for terrain.
/// </summary>
public class TerrainScript : MonoBehaviour
{
    /// <summary>
    /// The terrain.
    /// </summary>
    private TerrainBlocks terrain;

    /// <summary>
    /// The block meshes.
    /// </summary>
    private TerrainMesh meshCloud;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.terrain = new TerrainBlocks();
        this.meshCloud = new TerrainMesh();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}