// ----------------------------------------------------------------------------
// <copyright file="MeshGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.VoxelTerrain.Generation
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Generates the mesh for a chunk.
    /// </summary>
    public abstract class MeshGenerator
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
            ChunkNeighbours chunkNeighbours = this.GetNeighbourChunks(terrain, chunkIndex);

            // Update the meshes for each voxel in the chunk
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    // Get the voxel and its neighbours
                    Voxel voxel = chunk.GetVoxel(x, y);
                    VoxelNeighbours voxelNeighbours = this.GetNeighbourVoxels(x, y, chunk, chunkNeighbours);

                    // Update the voxel's mesh
                    this.UpdateVoxelMesh(chunk, voxel, voxelNeighbours);
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

            // Get the position in chunk coordinates
            Position chunkPos = Terrain.GetChunkCoordinates(position);

            // Get the neighbours of the chunk
            ChunkNeighbours chunkNeighbours = this.GetNeighbourChunks(terrain, chunkIndex);

            // Get the voxel and its neighbours
            Voxel voxel = chunk.GetVoxel(position.X, position.Y);
            VoxelNeighbours voxelNeighbours = this.GetNeighbourVoxels(chunkPos.X, chunkPos.Y, chunk, chunkNeighbours);

            // Update the voxel's mesh
            this.UpdateVoxelMesh(chunk, voxel, voxelNeighbours);

            if (updateNeighbours)
            {
                foreach (VoxelNeighbour neighbour in voxelNeighbours)
                {
                    // Get the neighbour's neighbors
                    VoxelNeighbours neighbourNeighbours =
                        this.GetNeighbourVoxels(chunkPos.X, chunkPos.Y, chunk, chunkNeighbours);

                    // Update the neighbour voxel's mesh
                    this.UpdateVoxelMesh(neighbour.Chunk, neighbour.Voxel, neighbourNeighbours);
                }
            }
        }

        /// <summary>
        /// Update the mesh for the given voxel.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="voxel">The voxel.</param>
        /// <param name="neighbours">The neighbouring voxels.</param>
        protected abstract void UpdateVoxelMesh(Chunk chunk, Voxel voxel, VoxelNeighbours neighbours);

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

            VoxelNeighbour voxelN, voxelNE, voxelE, voxelSE, voxelS, voxelSW, voxelW, voxelNW;

            if (!isBorderN && !isBorderE && !isBorderS && !isBorderW)
            {
                voxelN = new VoxelNeighbour(chunk.GetVoxel(chunkX, chunkY + 1), chunk);
                voxelNE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY + 1), chunk);
                voxelE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY), chunk);
                voxelSE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY - 1), chunk);
                voxelS = new VoxelNeighbour(chunk.GetVoxel(chunkX, chunkY - 1), chunk);
                voxelSW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY - 1), chunk);
                voxelW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY), chunk);
                voxelNW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY + 1), chunk);
            }
            else
            {
                // N
                if (isBorderN)
                {
                    if (chunkNeighbours.ChunkN != null)
                    {
                        voxelN = new VoxelNeighbour(
                            chunkNeighbours.ChunkN.GetVoxel(chunkX, 0),
                            chunkNeighbours.ChunkN);
                    }
                    else
                    {
                        voxelN = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelN = new VoxelNeighbour(chunk.GetVoxel(chunkX, chunkY + 1), chunk);
                }

                // NE
                if (isBorderN && isBorderE)
                {
                    if (chunkNeighbours.ChunkNE != null)
                    {
                        voxelNE = new VoxelNeighbour(
                            chunkNeighbours.ChunkNE.GetVoxel(0, 0),
                            chunkNeighbours.ChunkNE);
                    }
                    else
                    {
                        voxelNE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (isBorderN && !isBorderE)
                {
                    if (chunkNeighbours.ChunkN != null)
                    {
                        voxelNE = new VoxelNeighbour(
                            chunkNeighbours.ChunkN.GetVoxel(chunkX + 1, 0),
                            chunkNeighbours.ChunkN);
                    }
                    else
                    {
                        voxelNE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (!isBorderN && isBorderE)
                {
                    if (chunkNeighbours.ChunkE != null)
                    {
                        voxelNE = new VoxelNeighbour(
                            chunkNeighbours.ChunkE.GetVoxel(0, chunkY + 1),
                            chunkNeighbours.ChunkE);
                    }
                    else
                    {
                        voxelNE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelNE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY + 1), chunk);
                }

                // E
                if (isBorderE)
                {
                    if (chunkNeighbours.ChunkE != null)
                    {
                        voxelE = new VoxelNeighbour(
                            chunkNeighbours.ChunkE.GetVoxel(0, chunkY),
                            chunkNeighbours.ChunkE);
                    }
                    else
                    {
                        voxelE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY), chunk);
                }

                // SE
                if (isBorderS && isBorderE)
                {
                    if (chunkNeighbours.ChunkSE != null)
                    {
                        voxelSE = new VoxelNeighbour(
                            chunkNeighbours.ChunkSE.GetVoxel(0, Chunk.Height - 1),
                            chunkNeighbours.ChunkSE);
                    }
                    else
                    {
                        voxelSE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (isBorderS && !isBorderE)
                {
                    if (chunkNeighbours.ChunkS != null)
                    {
                        voxelSE = new VoxelNeighbour(
                            chunkNeighbours.ChunkS.GetVoxel(chunkX + 1, Chunk.Height - 1),
                            chunkNeighbours.ChunkS);
                    }
                    else
                    {
                        voxelSE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (!isBorderS && isBorderE)
                {
                    if (chunkNeighbours.ChunkE != null)
                    {
                        voxelSE = new VoxelNeighbour(
                            chunkNeighbours.ChunkE.GetVoxel(0, chunkY - 1),
                            chunkNeighbours.ChunkE);
                    }
                    else
                    {
                        voxelSE = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelSE = new VoxelNeighbour(chunk.GetVoxel(chunkX + 1, chunkY - 1), chunk);
                }

                // S
                if (isBorderS)
                {
                    if (chunkNeighbours.ChunkS != null)
                    {
                        voxelS = new VoxelNeighbour(
                            chunkNeighbours.ChunkS.GetVoxel(chunkX, Chunk.Height - 1),
                            chunkNeighbours.ChunkS);
                    }
                    else
                    {
                        voxelS = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelS = new VoxelNeighbour(chunk.GetVoxel(chunkX, chunkY - 1), chunk);
                }

                // SW
                if (isBorderS && isBorderW)
                {
                    if (chunkNeighbours.ChunkSW != null)
                    {
                        voxelSW = new VoxelNeighbour(
                            chunkNeighbours.ChunkSW.GetVoxel(Chunk.Width - 1, Chunk.Height - 1),
                            chunkNeighbours.ChunkSW);
                    }
                    else
                    {
                        voxelSW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (isBorderS && !isBorderW)
                {
                    if (chunkNeighbours.ChunkS != null)
                    {
                        voxelSW = new VoxelNeighbour(
                            chunkNeighbours.ChunkS.GetVoxel(chunkX - 1, Chunk.Height - 1),
                            chunkNeighbours.ChunkS);
                    }
                    else
                    {
                        voxelSW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (!isBorderS && isBorderW)
                {
                    if (chunkNeighbours.ChunkW != null)
                    {
                        voxelSW = new VoxelNeighbour(
                            chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY - 1),
                            chunkNeighbours.ChunkW);
                    }
                    else
                    {
                        voxelSW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelSW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY - 1), chunk);
                }

                // W
                if (isBorderW)
                {
                    if (chunkNeighbours.ChunkW != null)
                    {
                        voxelW = new VoxelNeighbour(
                            chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY),
                            chunkNeighbours.ChunkW);
                    }
                    else
                    {
                        voxelW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY), chunk);
                }

                // NW
                if (isBorderN && isBorderW)
                {
                    if (chunkNeighbours.ChunkNW != null)
                    {
                        voxelNW = new VoxelNeighbour(
                            chunkNeighbours.ChunkNW.GetVoxel(Chunk.Width - 1, 0),
                            chunkNeighbours.ChunkNW);
                    }
                    else
                    {
                        voxelNW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (isBorderN && !isBorderW)
                {
                    if (chunkNeighbours.ChunkN != null)
                    {
                        voxelNW = new VoxelNeighbour(
                            chunkNeighbours.ChunkN.GetVoxel(chunkX - 1, 0),
                            chunkNeighbours.ChunkN);
                    }
                    else
                    {
                        voxelNW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else if (!isBorderN && isBorderW)
                {
                    if (chunkNeighbours.ChunkW != null)
                    {
                        voxelNW = new VoxelNeighbour(
                            chunkNeighbours.ChunkW.GetVoxel(Chunk.Width - 1, chunkY - 1),
                            chunkNeighbours.ChunkW);
                    }
                    else
                    {
                        voxelNW = new VoxelNeighbour(Voxel.Air, null);
                    }
                }
                else
                {
                    voxelNW = new VoxelNeighbour(chunk.GetVoxel(chunkX - 1, chunkY + 1), chunk);
                }
            }

            return new VoxelNeighbours(voxelN, voxelNE, voxelE, voxelSE, voxelS, voxelSW, voxelW, voxelNW);
        }

        /// <summary>
        /// The neighbours of a chunk. If a neighbour is null, then the chunk is at the edge of the world.
        /// </summary>
        protected class ChunkNeighbours
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
        /// Information on a neighbouring voxel.
        /// </summary>
        protected class VoxelNeighbour
        {
            /// <summary>
            /// Initializes a new instance of the VoxelNeighbour class.
            /// </summary>
            /// <param name="voxel">The voxel.</param>
            /// <param name="chunk">The chunk that the neighbour belongs to.</param>
            public VoxelNeighbour(Voxel voxel, Chunk chunk)
            {
                this.Voxel = voxel;
                this.Chunk = chunk;
            }

            /// <summary>
            /// Gets the voxel.
            /// </summary>
            public Voxel Voxel { get; private set; }

            /// <summary>
            /// Gets the chunk that the neighbour belongs to.
            /// </summary>
            public Chunk Chunk { get; private set; }
        }

        /// <summary>
        /// The neighbours of a voxel.
        /// </summary>
        protected class VoxelNeighbours : IEnumerable<VoxelNeighbour>
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
                VoxelNeighbour voxelN,
                VoxelNeighbour voxelNE,
                VoxelNeighbour voxelE,
                VoxelNeighbour voxelSE,
                VoxelNeighbour voxelS,
                VoxelNeighbour voxelSW,
                VoxelNeighbour voxelW,
                VoxelNeighbour voxelNW)
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
            public VoxelNeighbour VoxelN { get; private set; }

            /// <summary>
            /// Gets the voxel to the top-right.
            /// </summary>
            public VoxelNeighbour VoxelNE { get; private set; }

            /// <summary>
            /// Gets the voxel to the right.
            /// </summary>
            public VoxelNeighbour VoxelE { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom-right.
            /// </summary>
            public VoxelNeighbour VoxelSE { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom.
            /// </summary>
            public VoxelNeighbour VoxelS { get; private set; }

            /// <summary>
            /// Gets the voxel to the bottom-left.
            /// </summary>
            public VoxelNeighbour VoxelSW { get; private set; }

            /// <summary>
            /// Gets the voxel to the left.
            /// </summary>
            public VoxelNeighbour VoxelW { get; private set; }

            /// <summary>
            /// Gets the voxel to the top-left.
            /// </summary>
            public VoxelNeighbour VoxelNW { get; private set; }

            /// <summary>
            /// Enumerate the neighbouring voxels.
            /// </summary>
            /// <returns>An enumerator for the neighbouring voxels.</returns>
            public IEnumerator<VoxelNeighbour> GetEnumerator()
            {
                yield return this.VoxelN;
                yield return this.VoxelNE;
                yield return this.VoxelE;
                yield return this.VoxelSE;
                yield return this.VoxelS;
                yield return this.VoxelSW;
                yield return this.VoxelW;
                yield return this.VoxelNW;
            }

            /// <summary>
            /// Enumerate the neighbouring voxels.
            /// </summary>
            /// <returns>An enumerator for the neighbouring voxels.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}