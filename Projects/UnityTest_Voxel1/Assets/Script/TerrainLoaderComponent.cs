using UnityEngine;

/// <summary>
/// Component for loading terrain chunks.
/// </summary>
[RequireComponent(typeof(TerrainComponent))]
[RequireComponent(typeof(TerrainMeshGeneratorComponent))]
public class TerrainLoaderComponent : MonoBehaviour
{
    /// <summary>
    /// Gets the terrain loader.
    /// </summary>
    public TerrainLoader TerrainLoader { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.TerrainLoader = new TerrainLoader();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}