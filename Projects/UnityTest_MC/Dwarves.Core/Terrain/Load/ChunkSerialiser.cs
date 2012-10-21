// ----------------------------------------------------------------------------
// <copyright file="ChunkSerialiser.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Load
{
    using System;
    using System.Collections.Generic;
    using Dwarves.Core;
    using Dwarves.Core.Terrain;

    /// <summary>
    /// Provides serialization and deserialization of terrain chunks.
    /// </summary>
    public class ChunkSerialiser
    {
        /// <summary>
        /// The path to each chunk file, keyed by chunk index.
        /// </summary>
        private Dictionary<Position, string> chunkFiles;

        /// <summary>
        /// Initialises a new instance of the ChunkSerialiser class.
        /// </summary>
        public ChunkSerialiser()
        {
            this.chunkFiles = new Dictionary<Position, string>();

            // Populate the list of chunk files
            // TODO
        }

        /// <summary>
        /// Serialize a chunk.
        /// </summary>
        /// <param name="chunk">The chunk to serialize.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void SerialiseChunk(Chunk chunk, Position chunkIndex)
        {
            // NOTE: This should occur in a seperate thread, since terrain serialization isn't high priority.
            // However, care should be taken to consider the situation when a terrain is being serialized in another thread
            // and then a new deserialization request comes in for that chunk. In this case the deserialization thread
            // should block
        }

        /// <summary>
        /// Attempt to deserialise a chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <param name="chunk">The deserialised chunk.</param>
        /// <returns>True if chunk was deserialised; False if a serialization file does not exist for chunk.</returns>
        public bool TryDeserialiseChunk(Position chunkIndex, out Chunk chunk)
        {
            string chunkFile;
            if (!this.chunkFiles.TryGetValue(chunkIndex, out chunkFile))
            {
                // A chunk file doesn't exist for the given chunk
                chunk = null;
                return false;
            }

            // TODO: Deserialize the file
            throw new NotImplementedException("Chunk deserialization is not yet implemented.");
        }
    }
}