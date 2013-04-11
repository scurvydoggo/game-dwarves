// ----------------------------------------------------------------------------
// <copyright file="RGB.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Light
{
    /// <summary>
    /// Represents an RGB colour.
    /// </summary>
    public struct RGB
    {
        /// <summary>
        /// White color.
        /// </summary>
        public static readonly RGB White = new RGB(byte.MaxValue, byte.MaxValue, byte.MaxValue);

        /// <summary>
        /// Black color.
        /// </summary>
        public static readonly RGB Black = new RGB(byte.MinValue, byte.MinValue, byte.MinValue);

        /// <summary>
        /// Initialises a new instance of the RGB struct.
        /// </summary>
        /// <param name="r">The R value.</param>
        /// <param name="g">The G value.</param>
        /// <param name="b">The B value.</param>
        public RGB(byte r, byte g, byte b) : this()
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        /// <summary>
        /// Gets or sets the R value.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Gets or sets the G value.
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Gets or sets the B value.
        /// </summary>
        public byte B { get; set; }
    }
}
