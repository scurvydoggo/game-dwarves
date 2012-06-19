// ----------------------------------------------------------------------------
// <copyright file="LightFront.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Game.Light
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This represents a linear segment of light that fades from 100% to 0% luminosity. The length of the light itself
    /// (ie. the length from 100% to 0% luminosity) is not specified in this class; that value is world-defined (the
    /// reason for this is because light can span multiple squares in a quad-tree, which is difficult to update if the 
    /// light length changes; instead, light length will be a variable in the scope of the entire quad-tree).
    /// </summary>
    public class LightFront
    {
        /// <summary>
        /// Initializes a new instance of the LightFront class.
        /// </summary>
        /// <param name="x">The x coordinate of the point from which light emants.</param>
        /// <param name="y">The y coordinate of the point from which light emants.</param>
        /// <param name="direction">The direction into which the light fades.</param>
        public LightFront(int x, int y, LightDirection direction)
            : this(x, y, direction, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LightFront class.
        /// </summary>
        /// <param name="x">The x coordinate of the point from which light emants.</param>
        /// <param name="y">The y coordinate of the point from which light emants.</param>
        /// <param name="direction">The direction into which the light fades.</param>
        /// <param name="baseLength">The length of the base for non-diagonally directed light.</param>
        public LightFront(int x, int y, LightDirection direction, int baseLength)
        {
            this.Point = new Point(x, y);
            this.Direction = direction;
            this.BaseLength = baseLength;
        }

        /// <summary>
        /// Gets or sets the point from which the light emants. If the light is in an up/down/left/right direction, this
        /// point is the top/left point of the 'base' from which light emants.
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Gets or sets the direction into which the light fades.
        /// </summary>
        public LightDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the length of the base. For diagonal direction light this is unused.
        /// </summary>
        public int BaseLength { get; set; }
    }

    /// <summary>
    /// Indicates the direction that light travels.
    /// </summary>
    public enum LightDirection
    {
        /// <summary>
        /// The light fades upwards.
        /// </summary>
        Up,

        /// <summary>
        /// The light fades diagonally to the upper right.
        /// </summary>
        UpRight,

        /// <summary>
        /// The light fades to the right.
        /// </summary>
        Right,

        /// <summary>
        /// The light fades diagonally to the bottom right.
        /// </summary>
        DownRight,

        /// <summary>
        /// The light fades downwards.
        /// </summary>
        Down,

        /// <summary>
        /// The light fades diagonally to the bottom left.
        /// </summary>
        DownLeft,

        /// <summary>
        /// The light fades to the left.
        /// </summary>
        Left,

        /// <summary>
        /// The light fades diagonally to the upper left.
        /// </summary>
        UpLeft
    }
}