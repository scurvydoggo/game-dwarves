using System.Collections.Generic;

/// <summary>
/// Manages the mesh data of terrain blocks.
/// </summary>
public class BlockMeshCollection
{
    /// <summary>
    /// Initializes a new instance of the BlockMeshCollection class.
    /// </summary>
    public BlockMeshCollection()
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