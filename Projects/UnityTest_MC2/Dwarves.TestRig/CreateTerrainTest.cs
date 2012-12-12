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
        /// The terrain.
        /// </summary>
        private TerrainManager terrainManager;

        /// <summary>
        /// Initialises a new instance of the CreateTerrainTest class.
        /// </summary>
        public CreateTerrainTest()
        {
            this.terrainManager = new TerrainManager(
                TerrainEngineType.Standard,
                4,
                4,
                5,
                1,
                3,
                1,
                10,
                1,
                10,
                10f,
                0.5f);

            this.terrainManager.Terrain.ChunkAdded += this.Terrain_ChunkAdded;
            this.terrainManager.Terrain.ChunkRemoved += this.Terrain_ChunkRemoved;
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

            this.terrainManager.LoadUnloadChunks(activeChunks);
        }

        /// <summary>
        /// Handle a chunk add event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunk">The chunk index.</param>
        private void Terrain_ChunkAdded(object sender, Vector2I chunk)
        {
            // Remove any existing mesh data for this chunk
            this.terrainManager.Terrain.Meshes.Remove(chunk);

            // Build the mesh for this chunk
            MeshData meshData = this.terrainManager.TerrainMeshBuilder.CreateMesh(chunk);
        }

        /// <summary>
        /// Handle a chunk removal event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunk">The chunk index.</param>
        private void Terrain_ChunkRemoved(object sender, Vector2I chunk)
        {
            // Clear the mesh
            this.terrainManager.Terrain.Meshes.Remove(chunk);
        }
    }
}