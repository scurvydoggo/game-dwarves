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

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        // Get a reference to the related terrain components
        this.cTerrain = this.GetComponent<TerrainComponent>();
    }
	
    /// <summary>
    /// Remove the block at the given x/y position.
    /// </summary>
    /// <param name="worldX">The x position.</param>
    /// <param name="worldY">The y position.</param>
	public void RemoveBlock(int worldX, int worldY)
	{
		// TODO
	}
}