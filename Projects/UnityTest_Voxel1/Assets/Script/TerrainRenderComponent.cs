// ----------------------------------------------------------------------------
// <copyright file="TerrainRenderComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component for rendering the terrain.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(TerrainComponent))]
public class TerrainRenderComponent : MonoBehaviour
{
    /// <summary>
    /// The core terrain component.
    /// </summary>
	private TerrainComponent cTerrain;
	
    /// <summary>
    /// The mesh filter component.
    /// </summary>
	private MeshFilter cMeshFilter;
	
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
        this.cMeshFilter = this.GetComponent<MeshFilter>();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
		// Check if the terrain mesh needs to be rebuilt
		if (this.cTerrain.Terrain.Mesh.MeshChanged)
		{
			this.RebuildMesh();
		}
    }
	
    /// <summary>
    /// Rebuild the geometry on the mesh filter.
    /// </summary>
    private void RebuildMesh()
    {
		// TODO
		
		// Reset the mesh changed flag
		this.cTerrain.Terrain.Mesh.ResetMeshChanged();
	}
}