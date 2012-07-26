using System;
using System.Collections.Generic;

/// <summary>
/// Provides a mapping from block to material.
/// <para />
/// NOTE: This is a first-pass implementation as it provides only a single material per block. Eventually texture
/// layering will need to be implemented to provide natural-looking transitions from one block type to another (likely
/// implemented via a custom shader).
/// </summary>
public class MaterialLookup
{
    /// <summary>
    /// Maps block types to their base material.
    /// </summary>
    private static readonly Dictionary<BlockType, MaterialType> BaseMaterials = new Dictionary<BlockType, MaterialType>
        {
            { BlockType.Dirt, MaterialType.Dirt }
        };

    /// <summary>
    /// Gets the material type for the given block.
    /// </summary>
    /// <param name="blockType">The block type.</param>
    /// <returns>The material type.</returns>
    public MaterialType GetMaterial(BlockType blockType)
    {
        MaterialType material;

        if (!BaseMaterials.TryGetValue(blockType, out material))
        {
            throw new ApplicationException("Material not defined for block type: " + blockType.ToString());
        }

        return material;
    }
}