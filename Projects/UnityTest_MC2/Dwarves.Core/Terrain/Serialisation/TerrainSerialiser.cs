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
        /// Serialise the point data of the chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk.</param>
        public void SerialisePointData(Vector2I chunkIndex, TerrainChunk chunk)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to deserialise the point data of the chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>True if the terrain chunk was deserialised.</returns>
        public bool TryDeserialisePoints(Vector2I chunkIndex, TerrainChunk chunk)
        {
            // TODO: This is not yet implemented
            return false;
        }
    }
}