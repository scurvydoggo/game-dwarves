// ----------------------------------------------------------------------------
// <copyright file="Position.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    /// <summary>
    /// An integer-based 2D vector.
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// A position at 0,0.
        /// </summary>
        public static readonly Position Zero = new Position(0, 0);

        /// <summary>
        /// Initializes a new instance of the Position struct.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        public Position(int x, int y)
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