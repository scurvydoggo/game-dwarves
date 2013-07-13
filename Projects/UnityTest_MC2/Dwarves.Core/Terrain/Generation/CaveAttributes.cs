// ----------------------------------------------------------------------------
// <copyright file="CaveAttributes.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    /// <summary>
    /// The attributes for a generated cave.
    /// </summary>
    public class CaveAttributes
    {
        /// <summary>
        /// Initialises a new instance of the CaveAttributes class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <param name="boundaryValue">The value at which the cave boundary lies.</param>
        public CaveAttributes(int seed, float boundaryValue)
        {
            this.Seed = seed;
            this.BoundaryValue = boundaryValue;
        }

        /// <summary>
        /// Gets the seed for this cave.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Gets the value at which the cave boundary lies.
        /// </summary>
        public float BoundaryValue { get; private set; }
    }
}