// ----------------------------------------------------------------------------
// <copyright file="Vector2I.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math
{
    /// <summary>
    /// An 2D integer vector.
    /// </summary>
    public struct Vector2I
    {
        /// <summary>
        /// A vector at with zero values.
        /// </summary>
        public static readonly Vector2I Zero = new Vector2I(0, 0);

        /// <summary>
        /// Initialises a new instance of the Vector2I struct.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public Vector2I(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        public int Y { get; set; }
    }
}