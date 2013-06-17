// ----------------------------------------------------------------------------
// <copyright file="CreateTerrainTest.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.TestRig
{
    using System.Collections.Generic;
    using Dwarves.Core;
    using Dwarves.Core.Math;

    /// <summary>
    /// A test for creating the terrain in the game.
    /// </summary>
    public class CreateTerrainTest : ITest
    {
        /// <summary>
        /// The X position of the camera.
        /// </summary>
        private int x;

        /// <summary>
        /// The Y position of the camera.
        /// </summary>
        private int y;

        /// <summary>
        /// Initialises a new instance of the CreateTerrainTest class.
        /// </summary>
        public CreateTerrainTest()
        {
            this.x = 0;
            this.y = 0;

            TerrainSystem.Initialise(
                4,
                4,
                5,
                3,
                10,
                1,
                10,
                10f,
                0.5f);

            TerrainSystem.Instance.Terrain.ChunkAdded += this.Terrain_ChunkAdded;
            TerrainSystem.Instance.Terrain.ChunkRemoved += this.Terrain_ChunkRemoved;
        }

        /// <summary>
        /// Update the component.
        /// </summary>
        public void Update()
        {
            int widthHalf = 4;
            int heightHalf = 2;
            int lookAhead = 1;

            var activeChunks = new HashSet<Vector2I>();
            for (int cX = this.x - widthHalf - lookAhead; cX < this.x + widthHalf + lookAhead; cX++)
            {
                for (int cY = this.y - heightHalf - lookAhead; cY < this.y + heightHalf + lookAhead; cY++)
                {
                    activeChunks.Add(new Vector2I(cX, cY));
                }
            }

            // Load and unload chunks
            TerrainSystem.Instance.Update(activeChunks);

            // Scroll the view
            this.x++;
            if (this.x % 3 == 0)
            {
                this.y++;
            }
        }

        /// <summary>
        /// Handle a chunk add event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkAdded(object sender, Vector2I chunkIndex)
        {
            // Create the chunk's game object (not done in this test)
        }

        /// <summary>
        /// Handle a chunk removal event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        private void Terrain_ChunkRemoved(object sender, Vector2I chunkIndex)
        {
            // Destroy the chunk's game object (not done in this test)
        }
    }
}