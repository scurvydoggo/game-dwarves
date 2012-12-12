// ----------------------------------------------------------------------------
// <copyright file="MarchingCubes.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Geometry
{
    using Dwarves.Core.Math;

    /// <summary>
    /// The tables used by the marching cubes algorithm.
    /// </summary>
    public static class MarchingCubes
    {
        /// <summary>
        /// The relative vector for each corner.
        /// </summary>
        public static readonly Vector3I[] CornerVector =
            {
                new Vector3I(0, 0, 0),
                new Vector3I(1, 0, 0),
                new Vector3I(0, 0, 1),
                new Vector3I(1, 0, 1),
                new Vector3I(0, 1, 0),
                new Vector3I(1, 1, 0),
                new Vector3I(0, 1, 1),
                new Vector3I(1, 1, 1)
            };
    }
}