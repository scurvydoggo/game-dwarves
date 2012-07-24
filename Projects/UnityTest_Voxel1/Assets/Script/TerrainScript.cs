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
    private BlockMeshCloud meshCloud;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.terrain = new Terrain();
        this.meshCloud = new BlockMeshCloud();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}