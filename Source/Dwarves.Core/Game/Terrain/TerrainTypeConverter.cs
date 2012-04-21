// ----------------------------------------------------------------------------
// <copyright file="TerrainTypeConverter.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Converts objects into their corresponding terrain type.
    /// </summary>
    public static class TerrainTypeConverter
    {
        /// <summary>
        /// Mapping of colors to their terrain type representations
        /// </summary>
        private static readonly Dictionary<Color, TerrainType> ColorMap = new Dictionary<Color, TerrainType>
            {
                { Color.White, TerrainType.None },
                { Color.Black, TerrainType.Mud }
            };

        /// <summary>
        /// Gets the terrain type value represented by the given color.
        /// </summary>
        /// <param name="color">The color representing a terrain type.</param>
        /// <returns>The terrain type for this color.</returns>
        public static TerrainType GetValue(Color color)
        {
            if (ColorMap.ContainsKey(color))
            {
                return ColorMap[color];
            }
            else
            {
                throw new ArgumentException(string.Format("Color {0} is not mapped to a terrain type.", color));
            }
        }
    }
}