// ----------------------------------------------------------------------------
// <copyright file="TerrainRenderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component for rendering the terrain.
/// </summary>
[RequireComponent(typeof(TerrainComponent))]
public class TerrainRenderComponent : MonoBehaviour
{
    /// <summary>
    /// The core terrain component.
    /// </summary>
	private TerrainComponent cTerrain;
	
    /// <summary>
    /// Gets the mesh generator.
    /// </summary>
    public TerrainMeshGenerator MeshGenerator { get; private set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.MeshGenerator = new TerrainMeshGeneratorCubes();
		
        // Get a reference to the related terrain components
        this.cTerrain = this.GetComponent<TerrainComponent>();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
    }
}