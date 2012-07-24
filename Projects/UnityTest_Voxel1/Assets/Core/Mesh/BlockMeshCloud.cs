using System.Collections.Generic;

/// <summary>
/// Manages the mesh data of terrain blocks.
/// </summary>
public class BlockMeshCloud
{
    /// <summary>
    /// Initializes a new instance of the BlockMeshCloud class.
    /// </summary>
    public BlockMeshCloud()
    {
        this.Meshes = new Dictionary<Vector2I, BlockMesh>();
    }

    /// <summary>
    /// Gets the mesh data per block.
    /// </summary>
    public Dictionary<Vector2I, BlockMesh> Meshes { get; private set; }

    /// <summary>
    /// Gets the block mesh at the given world position.
    /// </summary>
    /// <param name="worldX">The x position.</param>
    /// <param name="worldY">The y position.</param>
    /// <returns>The block mesh; Null if there is no mesh at the given position.</returns>
    public BlockMesh this[int worldX, int worldY]
    {
        get
        {
            BlockMesh mesh;
            if (this.Meshes.TryGetValue(new Vector2I(worldX, worldY), out mesh))
            {
                return mesh;
            }
            else
            {
                return null;
            }
        }
    }
}