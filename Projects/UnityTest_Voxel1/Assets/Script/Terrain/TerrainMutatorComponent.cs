// ----------------------------------------------------------------------------
// <copyright file="TerrainMutatorComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component for mutating the terrain.
/// </summary>
[RequireComponent(typeof(TerrainComponent))]
public class TerrainMutatorComponent : MonoBehaviour
{
    /// <summary>
    /// The core terrain component.
    /// </summary>
    private TerrainComponent cTerrain;

    private TerrainRenderComponent cTerrainRender;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        // Get a reference to the related terrain components
        this.cTerrain = this.GetComponent<TerrainComponent>();
        this.cTerrainRender = this.GetComponent<TerrainRenderComponent>();
    }

    /// <summary>
    /// Remove the block at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    public void RemoveBlock(Vector2I position)
    {
        // Remove the block data
        this.cTerrain.Terrain.Blocks[position.X, position.Y] = Block.None;

        // Remove the block's mesh
        this.cTerrain.Terrain.Mesh.RemoveMesh(position);
    }
}