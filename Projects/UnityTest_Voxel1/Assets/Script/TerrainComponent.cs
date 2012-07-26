using UnityEngine;

/// <summary>
/// Core component for terrain entity.
/// </summary>
public class TerrainComponent : MonoBehaviour
{
    /// <summary>
    /// Gets the terrain.
    /// </summary>
    public Terrain Terrain { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.Terrain = new Terrain();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}