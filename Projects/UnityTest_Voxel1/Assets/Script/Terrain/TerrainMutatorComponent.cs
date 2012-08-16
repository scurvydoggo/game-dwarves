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
    /// The terrain render component.
    /// </summary>
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
    /// Dig the block at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    public void DigBlock(Vector2I position)
    {
        Block block = this.cTerrain.Terrain.Blocks[position.X, position.Y];

        // Do nothing if this block cannot be dug
        if (((byte)block.BlockType & Block.MaskDiggable) == 0)
        {
            return;
        }

        // Set the block as dug
        this.cTerrain.Terrain.Blocks[position.X, position.Y] =
            new Block((BlockType)((byte)block.BlockType & Block.MaskDig));

        // Update the block and neighbours
        this.cTerrainRender.MeshGenerator.UpdateBlock(this.cTerrain.Terrain, position);
        this.cTerrainRender.MeshGenerator.UpdateBlockNeighbours(this.cTerrain.Terrain, position);
    }

    /// <summary>
    /// Remove the block at the given world position.
    /// </summary>
    /// <param name="position">The position.</param>
    public void RemoveBlock(Vector2I position)
    {
        // Do nothing if this block is already removed
        if (this.cTerrain.Terrain.Blocks[position.X, position.Y].BlockType == BlockType.None)
        {
            return;
        }

        // Remove the block data
        this.cTerrain.Terrain.Blocks[position.X, position.Y] = Block.None;

        // Remove the block's mesh
        this.cTerrain.Terrain.Mesh.RemoveMesh(position);

        // Recreate the neighbouring meshes
        this.cTerrainRender.MeshGenerator.UpdateBlockNeighbours(this.cTerrain.Terrain, position);
    }
}