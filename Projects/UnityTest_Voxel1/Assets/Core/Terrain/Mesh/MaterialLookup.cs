// ----------------------------------------------------------------------------
// <copyright file="MaterialLookup.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

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
            { BlockType.Mud, MaterialType.Mud }
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