// ----------------------------------------------------------------------------
// <copyright file="MathExtension.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math
{
    /// <summary>
    /// A set of math functions used in this project.
    /// </summary>
    public static class MathExtension
    {
        /// <summary>
        /// Determines if the given number is a power of two.
        /// </summary>
        /// <param name="value">The number.</param>
        /// <returns>True if the number is a power of two.</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value != 0) && ((value & (value - 1)) == 0);
        }
    }
}