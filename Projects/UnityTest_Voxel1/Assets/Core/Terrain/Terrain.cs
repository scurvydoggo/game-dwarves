// ----------------------------------------------------------------------------
// <copyright file="Terrain.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Represents the terrain.
/// </summary>
public class Terrain
{
    /// <summary>
    /// Initializes a new instance of the Terrain class.
    /// </summary>
    public Terrain()
    {
        this.Blocks = new TerrainBlocks();
        this.Mesh = new TerrainMesh();
    }

    /// <summary>
    /// Gets the terrain block data.
    /// </summary>
    public TerrainBlocks Blocks { get; private set; }

    /// <summary>
    /// Gets the terrain mesh data.
    /// </summary>
    public TerrainMesh Mesh { get; private set; }
}