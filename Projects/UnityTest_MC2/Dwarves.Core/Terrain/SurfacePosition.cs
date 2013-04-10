// ----------------------------------------------------------------------------
// <copyright file="SurfacePosition.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain
{
    /// <summary>
    /// The position of the surface relative to the chunk.
    /// </summary>
    public enum SurfacePosition
    {
        /// <summary>
        /// The surface passes through the chunk.
        /// </summary>
        Inside,

        /// <summary>
        /// The surface lies above the chunk.
        /// </summary>
        Above,

        /// <summary>
        /// The surface lies below the chunk.
        /// </summary>
        Below
    }
}