// ----------------------------------------------------------------------------
// <copyright file="ChunkUsage.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    using System;

    /// <summary>
    /// Indicates how a terrain chunk is to be used.
    /// </summary>
    [Flags]
    public enum ChunkUsage : byte
    {
        /// <summary>
        /// Only the voxel data is required. This is the default.
        /// </summary>
        Data = 0,

        /// <summary>
        /// The chunk needs to be processed for rendering.
        /// </summary>
        Rendering = 1,

        /// <summary>
        /// The chunk needs to be processed for physics.
        /// </summary>
        Physics = 2,

        /// <summary>
        /// The chunk needs to be processed for rednering and physics.
        /// </summary>
        RenderingAndPhysics = 3
    }
}