// ----------------------------------------------------------------------------
// <copyright file="INoiseGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math.Noise
{
    /// <summary>
    /// Generates noise for points along a plane.
    /// </summary>
    public interface INoiseGenerator
    {
        /// <summary>
        /// Generate 1D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <returns>The noise value.</returns>
        float Generate(float x);

        /// <summary>
        /// Generate 2D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <returns>The noise value.</returns>
        float Generate(float x, float y);

        /// <summary>
        /// Generate 3D noise.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The noise value.</returns>
        float Generate(float x, float y, float z);
    }
}