// ----------------------------------------------------------------------------
// <copyright file="Vector3I.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Math
{
    /// <summary>
    /// An 3D integer vector.
    /// </summary>
    public struct Vector3I
    {
        /// <summary>
        /// A vector with zero values.
        /// </summary>
        public static readonly Vector3I Zero = new Vector3I(0, 0, 0);

        /// <summary>
        /// Initialises a new instance of the Vector3I struct.
        /// </summary>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        public Vector3I(int x, int y, int z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the x position.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the y position.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the z position.
        /// </summary>
        public int Z { get; set; }
    }
}