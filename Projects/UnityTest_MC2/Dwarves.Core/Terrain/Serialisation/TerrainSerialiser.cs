// ----------------------------------------------------------------------------
// <copyright file="TerrainSerialiser.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Serialisation
{
    using System;
    using Dwarves.Core.Math;

    /// <summary>
    /// Serialises and deserialises terrain.
    /// </summary>
    public class TerrainSerialiser
    {
        /// <summary>
        /// Initialises a new instance of the TerrainSerialiser class.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        public TerrainSerialiser(VoxelTerrain terrain)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets the terrain.
        /// </summary>
        public VoxelTerrain Terrain { get; private set; }

        /// <summary>
        /// Serialise the given terrain chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        public void Serialise(Vector2I chunk)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to deserialise the given terrain chunk.
        /// </summary>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>True if the terrain chunk was deserialised.</returns>
        public bool TryDeserialise(Vector2I chunk)
        {
            // TODO: This is not yet implemented
            return false;
        }
    }
}