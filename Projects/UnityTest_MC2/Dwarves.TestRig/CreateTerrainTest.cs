// ----------------------------------------------------------------------------
// <copyright file="CreateTerrainTest.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using Dwarves.Core.Geometry;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using Dwarves.Core.Terrain.Engine;

    /// <summary>
    /// A test for creating the terrain in the game.
    /// </summary>
    public class CreateTerrainTest : ITest
    {
        /// <summary>
        /// Initialises a new instance of the CreateTerrainTest class.
        /// </summary>
        public CreateTerrainTest()
        {
            TerrainManager.Initialise(
                TerrainEngineType.Standard,
                4,
                4,
                5,
                3,
                1,
                10,
                1,
                10,
                10f,
                0.5f);

            TerrainManager.Instance.Terrain.ChunkAdded += this.Terrain_ChunkAdded;
            TerrainManager.Instance.Terrain.ChunkRemoved += this.Terrain_ChunkRemoved;
        }

        /// <summary>
        /// Update the component.
        /// </summary>
        public void Update()
        {
            var activeChunks = new Vector2I[]
                {
                    new Vector2I(0, 0)
                };

            // Load and unload chunks
            TerrainManager.Instance.LoadUnloadChunks(activeChunks);

            // Build the mesh for this chunk
            foreach (Vector2I chunk in TerrainManager.Instance.Terrain.Chunks)
            {
                if (TerrainManager.Instance.Terrain.RebuildRequired(chunk))
                {
                    MeshData meshData = TerrainManager.Instance.TerrainMeshBuilder.CreateMesh(chunk);

                    // Pretend that the mesh data was applied to a chunk game object
                    TerrainManager.Instance.Terrain.GetChunk(chunk).RebuildRequired = false;
                }
            }
        }

        /// <summary>
        /// Handle a chunk add event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunk">The chunk index.</param>
        private void Terrain_ChunkAdded(object sender, Vector2I chunk)
        {
            // Create the chunk's game object (not done in this test)
        }

        /// <summary>
        /// Handle a chunk removal event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunk">The chunk index.</param>
        private void Terrain_ChunkRemoved(object sender, Vector2I chunk)
        {
            // Destroy the chunk's game object (not done in this test)
        }
    }
}