// ----------------------------------------------------------------------------
// <copyright file="MeshGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    /// <summary>
    /// Generates the mesh for a chunk.
    /// </summary>
    public class MeshGenerator
    {
        /// <summary>
        /// Initializes a new instance of the MeshGenerator class.
        /// </summary>
        public MeshGenerator()
        {
        }

        /// <summary>
        /// Update the mesh for the given terrain chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public void UpdateChunk(Terrain terrain, Position chunkIndex)
        {
            Chunk chunk = terrain.GetChunk(chunkIndex);

            // Get the neighbours of the chunk
            ChunkNeighbours neighbours = this.GetNeighbourChunks(terrain, chunkIndex);

            // Update the meshes for each voxel in the chunk
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                }
            }
        }

        /// <summary>
        /// Update the voxel at the given world coordinates.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="position">The voxel position.</param>
        /// <param name="updateNeighbours">Indicates whether the neighbouring voxels should be updated.</param>
        public void UpdateVoxel(Terrain terrain, Position position, bool updateNeighbours)
        {
            Position chunkIndex = Terrain.GetChunkIndex(position.X, position.Y);
            Chunk chunk = terrain.GetChunk(chunkIndex);

            // Get the neighbours of the chunk
            ChunkNeighbours neighbours = this.GetNeighbourChunks(terrain, chunkIndex);

            // TODO
        }

        /// <summary>
        /// Get the neighbouring chunks.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        /// <returns>The chunk neighbours.</returns>
        private ChunkNeighbours GetNeighbourChunks(Terrain terrain, Position chunkIndex)
        {
            Chunk chunkN, chunkNE, chunkE, chunkSE, chunkS, chunkSW, chunkW, chunkNW;
            terrain.TryGetChunk(new Position(chunkIndex.X, chunkIndex.Y + 1), out chunkN);
            terrain.TryGetChunk(new Position(chunkIndex.X + 1, chunkIndex.Y + 1), out chunkNE);
            terrain.TryGetChunk(new Position(chunkIndex.X + 1, chunkIndex.Y), out chunkE);
            terrain.TryGetChunk(new Position(chunkIndex.X + 1, chunkIndex.Y - 1), out chunkSE);
            terrain.TryGetChunk(new Position(chunkIndex.X, chunkIndex.Y - 1), out chunkS);
            terrain.TryGetChunk(new Position(chunkIndex.X - 1, chunkIndex.Y - 1), out chunkSW);
            terrain.TryGetChunk(new Position(chunkIndex.X - 1, chunkIndex.Y), out chunkW);
            terrain.TryGetChunk(new Position(chunkIndex.X - 1, chunkIndex.Y + 1), out chunkNW);

            return new ChunkNeighbours(chunkN, chunkNE, chunkE, chunkSE, chunkS, chunkSW, chunkW, chunkNW);
        }

        /// <summary>
        /// Get the neighbouring voxels.
        /// </summary>
        /// <param name="chunkX">The x position.</param>
        /// <param name="chunkY">The y position.</param>
        /// <param name="chunk">The chunk.</param>
        /// <param name="chunkNeighbours">The neighbouring chunks.</param>
        /// <returns>The voxel neighbours.</returns>
        private VoxelNeighbours GetNeighbourVoxels(int chunkX, int chunkY, Chunk chunk, ChunkNeighbours chunkNeighbours)
        {
            // Check if the voxel position is on the border of the chunk
            bool isBorderW = chunkX == 0;
            bool isBorderE = chunkX == Chunk.Width - 1;
            bool isBorderN = chunkY == Chunk.Height - 1;
            bool isBorderS = chunkY == 0;

            Voxel voxelN, voxelNE, voxelE, voxelSE, voxelS, voxelSW, voxelW, voxelNW;

            if (!isBorderN && !isBorderE && !isBorderS && !isBorderW)
            {
                voxelN = chunk.GetVoxel(chunkX, chunkY + 1);
                voxelNE = chunk.GetVoxel(chunkX + 1, chunkY + 1);
                voxelE = chunk.GetVoxel(chunkX + 1, chunkY);
                voxelSE = chunk.GetVoxel(chunkX + 1, chunkY - 1);
                voxelS = chunk.GetVoxel(chunkX, chunkY - 1);
                voxelSW = chunk.GetVoxel(chunkX - 1, chunkY - 1);
                voxelW = chunk.GetVoxel(chunkX - 1, chunkY);
                voxelNW = chunk.GetVoxel(chunkX - 1, chunkY + 1);
            }
            else
            {
                // N
                if (isBorderN)
                {
                    voxelN = chunkNeighbours.ChunkN != null ?
                        chunkNeighbours.ChunkN.GetVoxel(chunkX, 0) : Voxel.Empty;
                }
                else
                {
                    voxelN = chunk.GetVoxel(chunkX, chunkY + 1);
                }

                // NE
                if (isBorderN && isBorderE)
                {
                    voxelNE = chunkNeighbours.ChunkNE != null ?
                        chunkNeighbours.ChunkNE.GetVoxel(0, 0) : Voxel.Empty;
                }
                else if (isBorderN && !isBorderE)
                {
                    voxelNE = chunkNeighbours.ChunkN != null ?
                        chunkNeighbours.ChunkN.GetVoxel(chunkX + 1, 0) : Voxel.Empty;
                }
                else if (!isBorderN && isBorderE)
                {
                    voxelNE = chunkNeighbours.ChunkE != null ?
                        chunkNeighbours.ChunkE.GetVoxel(0, chunkY + 1) : Voxel.Empty;
                }
                else
                {
                    voxelNE = chunk.GetVoxel(chunkX + 1, chunkY + 1);
                }

                // E
                if (isBorderE)
                {
                    voxelE = chunkNeighbours.ChunkE != null ?
                        chunkNeighbours.ChunkE.GetVoxel(0, chunkY) : Voxel.Empty;
                }
                else
                {
                    voxelE = chunk.GetVoxel(chunkX + 1, chunkY);
                }

                // SE
                if (isBorderS && isBorderE)
                {
                    voxelSE = chunkNeighbours.ChunkSE != null ?
                        chunkNeighbours.ChunkSE.GetVoxel(0, Chunk.Height - 1) : Voxel.Empty;
                }
                else if (isBorderS && !isBorderE)
                {
                    voxelSE = chunkNeighbours.ChunkS != null ?
                        chunkNeighbours.ChunkS.GetVoxel(chunkX + 1, Chunk.Height - 1) : Voxel.Empty;
                }
                else if (!isBorderS && isBorderE)
                {
                    voxelSE = chunkNeighbours.ChunkE != null ?
                        chunkNeighbours.ChunkE.GetVoxel(0, chunkY - 1) : Voxel.Empty;
                }
                else
                {
                    voxelSE = chunk.GetVoxel(chunkX + 1, chunkY - 1);
                }

                // S
                if (isBorderS)
                {
                    voxelS = chunkNeighbours.ChunkS != null ?
                        chunkNeighbours.ChunkS.GetVoxel(chunkX, Chunk.Height - 1) : Voxel.Empty;
                }
                else
                {
                    voxelS = chunk.GetVoxel(chunkX, chunkY - 1);
                }

                // SW
                if (isBorderS && isBorderW)
                {
                    voxelSW = chunkNeighbours.ChunkSW != null ?
                        chunkNeighbours.ChunkSW.GetVoxel(Chunk.Width - 1, Chunk.Height - 1) : Voxel.Empty;
                }
                else if (isBorderS && !isBorderW)
                {
                    voxelSW = chunkNeighbours.ChunkS != null ?
                        chunkNeighbours.ChunkS.GetVoxel(chunkX - 1, Chunk.Height - 1) : Voxel.Empty;
                }
                else if (!isBorderS && isBorderW)
                {
                    voxelSW = chunkNeighbours.ChunkW != null ?
                        chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY - 1) : Voxel.Empty;
                }
                else
                {
                    voxelSW = chunk.GetVoxel(chunkX - 1, chunkY - 1);
                }

                // W
                if (isBorderW)
                {
                    voxelW = chunkNeighbours.ChunkW != null ?
                        chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY) : Voxel.Empty;
                }
                else
                {
                    voxelW = chunk.GetVoxel(chunkX - 1, chunkY);
                }

                // NW
                if (isBorderN && isBorderW)
                {
                    voxelNW = chunkNeighbours.ChunkNW != null ?
                        chunkNeighbours.ChunkNW.GetVoxel(Chunk.Width - 1, 0) : Voxel.Empty;
                }
                else if (isBorderN && !isBorderW)
                {
                    voxelNW = chunkNeighbours.ChunkN != null ?
                        chunkNeighbours.ChunkN.GetVoxel(chunkX - 1, 0) : Voxel.Empty;
                }
                else if (!isBorderN && isBorderW)
                {
                    voxelNW = chunkNeighbours.ChunkW != null ?
                        chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY - 1) : Voxel.Empty;
                }
                else
                {
                    voxelNW = chunk.GetVoxel(chunkX - 1, chunkY + 1);
                }
            }

            return new VoxelNeighbours(voxelN, voxelNE, voxelE, voxelSE, voxelS, voxelSW, voxelW, voxelNW);
        }

        /// <summary>
        /// The neighbours of a chunk. If a neighbour is null, then the chunk is at the edge of the world.
        /// </summary>
        private class ChunkNeighbours
        {
            /// <summary>
            /// Initializes a new instance of the ChunkNeighbours class.
            /// </summary>
            /// <param name="chunkN">The chunk to the top.</param>
            /// <param name="chunkNE">The chunk to the top-right.</param>
            /// <param name="chunkE">The chunk to the right.</param>
            /// <param name="chunkSE">The chunk to the bottom-right.</param>
            /// <param name="chunkS">The chunk to the bottom.</param>
            /// <param name="chunkSW">The chunk to the bottom-left.</param>
            /// <param name="chunkW">The chunk to the left.</param>
            /// <param name="chunkNW">The chunk to the top-left.</param>
            public ChunkNeighbours(
                Chunk chunkN,
                Chunk chunkNE,
                Chunk chunkE,
                Chunk chunkSE,
                Chunk chunkS,
                Chunk chunkSW,
                Chunk chunkW,
                Chunk chunkNW)
            {
                this.ChunkN = chunkN;
                this.ChunkNE = chunkNE;
                this.ChunkE = chunkE;
                this.ChunkSE = chunkSE;
                this.ChunkS = chunkS;
                this.ChunkSW = chunkSW;
                this.ChunkW = chunkW;
                this.ChunkNW = chunkNW;
            }

            /// <summary>
            /// Gets the chunk to the top.
            /// </summary>
            public Chunk ChunkN { get; private set; }

            /// <summary>
            /// Gets the chunk to the top-right.
            /// </summary>
            public Chunk ChunkNE { get; private set; }

            /// <summary>
            /// Gets the chunk to the right.
            /// </summary>
            public Chunk ChunkE { get; private set; }

            /// <summary>
            /// Gets the chunk to the bottom-right.
            /// </summary>
            public Chunk ChunkSE { get; private set; }

            /// <summary>
            /// Gets the chunk to the bottom.
            /// </summary>
            public Chunk ChunkS { get; private set; }

            /// <summary>
            /// Gets the chunk to the bottom-left.
            /// </summary>
            public Chunk ChunkSW { get; private set; }

            /// <summary>
            /// Gets the chunk to the left.
            /// </summary>
            public Chunk ChunkW { get; private set; }

            /// <summary>
            /// Gets the chunk to the top-left.
            /// </summary>
            public Chunk ChunkNW { get; private set; }
        }

        /// <summary>
        /// The neighbours of a voxel.
        /// </summary>
        private class VoxelNeighbours
        {
            /// <summary>
            /// Initializes a new instance of the VoxelNeighbours class.
            /// </summary>
            /// <param name="voxelN">The voxel to the top.</param>
            /// <param name="voxelNE">The voxel to the top-right.</param>
            /// <param name="voxelE">The voxel to the right.</param>
            /// <param name="voxelSE">The voxel to the bottom-right.</param>
            /// <param name="voxelS">The voxel to the bottom.</param>
            /// <param name="voxelSW">The voxel to the bottom-left.</param>
            /// <param name="voxelW">The voxel to the left.</param>
            /// <param name="voxelNW">The voxel to the top-left.</param>
            public VoxelNeighbours(
                Voxel voxelN,
                Voxel voxelNE,
                Voxel voxelE,
                Voxel voxelSE,
                Voxel voxelS,
                Voxel voxelSW,
                Voxel voxelW,
                Voxel voxelNW)
            {
                this.VoxelN = voxelN;
                this.VoxelNE = voxelNE;
                this.VoxelE = voxelE;
                this.VoxelSE = voxelSE;
                this.VoxelS = voxelS;
                this.VoxelSW = voxelSW;
                this.VoxelW = voxelW;
                this.VoxelNW = voxelNW;
            }

            /// <summary>
            /// Gets the voxel to the top.
            /// </summary>
            public Voxel VoxelN { get; private set; }

            /// <summary>
            /// Gets the voxel to the top-right.
            /// </summary>
            public Voxel VoxelNE { get; private set; }

            /// <summary>
            /// Gets the voxel to the right.
            /// </summary>
            public Voxel VoxelE { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom-right.
            /// </summary>
            public Voxel VoxelSE { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom.
            /// </summary>
            public Voxel VoxelS { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom-left.
            /// </summary>
            public Voxel VoxelSW { get; private set; }

            /// <summary>
            /// Gets the voxel to the left.
            /// </summary>
            public Voxel VoxelW { get; private set; }

            /// <summary>
            /// Gets the voxel to the top-left.
            /// </summary>
            public Voxel VoxelNW { get; private set; }
        }
    }
}