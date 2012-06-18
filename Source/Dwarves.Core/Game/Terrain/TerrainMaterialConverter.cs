// ----------------------------------------------------------------------------
// <copyright file="TerrainMaterialConverter.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Terrain
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Converts objects into their corresponding terrain material.
    /// </summary>
    public static class TerrainMaterialConverter
    {
        /// <summary>
        /// Mapping of colors to their terrain material representations
        /// </summary>
        private static readonly Dictionary<Color, TerrainMaterial> ColorMap = new Dictionary<Color, TerrainMaterial>
            {
                { Color.White, TerrainMaterial.None },
                { Color.Black, TerrainMaterial.Mud }
            };

        /// <summary>
        /// Gets the terrain material represented by the given color.
        /// </summary>
        /// <param name="color">The color representing a terrain material.</param>
        /// <returns>The terrain material for this color.</returns>
        public static TerrainMaterial GetValue(Color color)
        {
            if (ColorMap.ContainsKey(color))
            {
                return ColorMap[color];
            }
            else
            {
                throw new ArgumentException(string.Format("Color {0} is not mapped to a terrain material.", color));
            }
        }
    }
}