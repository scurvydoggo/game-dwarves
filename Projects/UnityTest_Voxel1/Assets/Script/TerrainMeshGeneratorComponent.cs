using UnityEngine;

/// <summary>
/// Component for generating terrain mesh data.
/// </summary>
[RequireComponent(typeof(TerrainComponent))]
public class TerrainMeshGeneratorComponent : MonoBehaviour
{
    /// <summary>
    /// Gets the mesh generator.
    /// </summary>
    public TerrainMeshGenerator TerrainMeshGenerator { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.TerrainMeshGenerator = new TerrainMeshGeneratorCubes();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}