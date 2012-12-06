// ----------------------------------------------------------------------------
// <copyright file="TerrainSerialiser.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Serialisation
{
    using System;
    using Dwarves.Core.Math;

    /// <summary>
    /// Serialises and deserialises terrain.
    /// </summary>
    public class TerrainSerialiser
    {
        /// <summary>
        /// Serialise the given terrain chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunk">The chunk index.</param>
        public void Serialise(Terrain terrain, Vector2I chunk)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to deserialise the given terrain chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunk">The chunk index.</param>
        /// <returns>True if the terrain chunk was deserialised.</returns>
        public bool TryDeserialise(Terrain terrain, Vector2I chunk)
        {
            // TODO: This is not yet implemented
            return false;
        }
    }
}