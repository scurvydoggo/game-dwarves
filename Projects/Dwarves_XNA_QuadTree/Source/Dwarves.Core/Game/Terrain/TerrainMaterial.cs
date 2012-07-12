// ----------------------------------------------------------------------------
// <copyright file="TerrainMaterial.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    /// <summary>
    /// Defines the terrain materials that can exist.
    /// </summary>
    public enum TerrainMaterial : byte
    {
        /// <summary>
        /// Nothing (ie. air).
        /// </summary>
        None,

        /// <summary>
        /// Mud terrain.
        /// </summary>
        Mud
    }
}