// ----------------------------------------------------------------------------
// <copyright file="TerrainState.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    /// <summary>
    /// Defines the states that terrain can be in.
    /// </summary>
    public enum TerrainState
    {
        /// <summary>
        /// Indicates that this is not terrain.
        /// </summary>
        Empty,

        /// <summary>
        /// Indicates that this is terrain.
        /// </summary>
        Terrain,

        /// <summary>
        /// Indicates that this was terrain but it has been dug out.
        /// </summary>
        Dug
    }
}