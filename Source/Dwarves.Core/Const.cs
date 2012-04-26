// ----------------------------------------------------------------------------
// <copyright file="Const.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves
{
    /// <summary>
    /// The game constants.
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// The pixel size of tile sprites.
        /// </summary>
        public const int TileSize = 16;

        /// <summary>
        /// The scaling factor for converting pixels to meters in the physics world.
        /// </summary>
        public const float PixelsToMeters = 0.2f;

        /// <summary>
        /// Collision group.
        /// </summary>
        public const short CollisionGroupDwarf = -2;
    }
}