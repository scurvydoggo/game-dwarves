// ----------------------------------------------------------------------------
// <copyright file="Light.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Lighting
{
    /// <summary>
    /// Represents a lighting value.
    /// </summary>
    public struct Light
    {
        /// <summary>
        /// Blinding light.
        /// </summary>
        public static readonly Light White = new Light(byte.MaxValue, byte.MaxValue, byte.MaxValue);

        /// <summary>
        /// Pure darkness.
        /// </summary>
        public static readonly Light Black = new Light(byte.MinValue, byte.MinValue, byte.MinValue);

        /// <summary>
        /// Initialises a new instance of the Light struct.
        /// </summary>
        /// <param name="r">The R value.</param>
        /// <param name="g">The G value.</param>
        /// <param name="b">The B value.</param>
        public Light(byte r, byte g, byte b) : this()
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