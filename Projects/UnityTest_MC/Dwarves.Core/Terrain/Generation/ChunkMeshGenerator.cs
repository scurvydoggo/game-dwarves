// ----------------------------------------------------------------------------
// <copyright file="ChunkMeshGenerator.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Terrain.Generation
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Generates the mesh for a chunk.
    /// </summary>
    public abstract class ChunkMeshGenerator
    {
        #region Public Methods

        /// <summary>
        /// Update the mesh for the given terrain chunk.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="chunkIndex">The chunk index.</param>
        public virtual void UpdateChunk(VoxelTerrain terrain, Position chunkIndex)
        {
            Chunk chunk = terrain.Chunks[chunkIndex];

            // Get the neighbours of the chunk
            Chunk chunkN, chunkNE, chunkE;
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X, chunkIndex.Y + 1), out chunkN);
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X + 1, chunkIndex.Y + 1), out chunkNE);
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X + 1, chunkIndex.Y), out chunkE);

            // Get the origin point of the chunk
            var chunkOrigin = new Position(chunkIndex.X * Chunk.Width, chunkIndex.Y * Chunk.Height);
            
            // Update the meshes for each voxel in the chunk
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    var chunkPos = new Position(x, y);
                    var worldPos = new Position(chunkOrigin.X + x, chunkOrigin.Y + y);

                    // Get the voxel 2x2 voxel square with this position in the lower-left corner
                    VoxelInfo voxel = new VoxelInfo(chunk.GetVoxel(chunkPos), chunk, chunkPos);
                    VoxelSquare voxelSquare = this.GetVoxelSquare(voxel, worldPos, chunkN, chunkNE, chunkE);

                    // Update the voxel's mesh
                    this.UpdateVoxelMesh(voxelSquare);
                }
            }
        }

        /// <summary>
        /// Update the voxel at the given world coordinates.
        /// </summary>
        /// <param name="terrain">The terrain.</param>
        /// <param name="position">The voxel position.</param>
        /// <param name="updateNeighbours">Indicates whether the neighbouring voxels should be updated.</param>
        public virtual void UpdateVoxel(VoxelTerrain terrain, Position position, bool updateNeighbours)
        {
            // Get the chunk
            Position chunkIndex = VoxelTerrain.GetChunkIndex(position.X, position.Y);
            Chunk chunk;
            if (!terrain.Chunks.TryGetValue(chunkIndex, out chunk))
            {
                // The given position is outside the known world, so do nothing
                return;
            }

            // Get the position in chunk coordinates
            Position chunkPos = VoxelTerrain.GetChunkCoordinates(position);

            // Get the neighbours of the chunk
            Chunk chunkN, chunkNE, chunkE;
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X, chunkIndex.Y + 1), out chunkN);
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X + 1, chunkIndex.Y + 1), out chunkNE);
            terrain.Chunks.TryGetValue(new Position(chunkIndex.X + 1, chunkIndex.Y), out chunkE);

            // Get the voxel 2x2 voxel square with this position in the lower-left corner
            VoxelInfo voxel = new VoxelInfo(chunk.GetVoxel(chunkPos), chunk, chunkPos);
            VoxelSquare voxelSquare = this.GetVoxelSquare(voxel, position, chunkN, chunkNE, chunkE);

            // Update the voxel's mesh
            this.UpdateVoxelMesh(voxelSquare);

            // Update the neighbouring voxels that use this voxel as a corner point
            if (updateNeighbours)
            {
                this.UpdateVoxel(terrain, new Position(position.X - 1, position.Y), false);
                this.UpdateVoxel(terrain, new Position(position.X - 1, position.Y - 1), false);
                this.UpdateVoxel(terrain, new Position(position.X, position.Y - 1), false);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Update the mesh for the given 2x2 square of voxels.
        /// </summary>
        /// <param name="voxelSquare">The 2x2 square of voxels surrounding the mesh to update.</param>
        protected abstract void UpdateVoxelMesh(VoxelSquare voxelSquare);

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the 2x2 square of adjacent voxels with the given voxel in the lower-left corner.
        /// </summary>
        /// <param name="voxel">The voxel in the square's lower-left corner.</param>
        /// <param name="worldOrigin">The world position of the lower-left corner.</param>
        /// <param name="chunkN">The chunk above the voxel's chunk.</param>
        /// <param name="chunkNE">The chunk to the upper-right of the voxel's chunk.</param>
        /// <param name="chunkE">The chunk to the left of the voxel's chunk.</param>
        /// <returns>The voxel square.</returns>
        private VoxelSquare GetVoxelSquare(
            VoxelInfo voxel,
            Position worldOrigin,
            Chunk chunkN,
            Chunk chunkNE,
            Chunk chunkE)
        {
            VoxelInfo voxelN, voxelNE, voxelE;

            // Check if the voxel position is on the border of the chunk
            bool isBorderN = voxel.Position.Y == Chunk.Height - 1;
            bool isBorderE = voxel.Position.X == Chunk.Width - 1;
            
            // Get the positions of the neighbouring voxels
            var positionN = new Position(voxel.Position.X, voxel.Position.Y + 1);
            var positionNE = new Position(voxel.Position.X + 1, voxel.Position.Y + 1);
            var positionE = new Position(voxel.Position.X + 1, voxel.Position.Y);
            
            // Get the neighbouring voxels
            if (!isBorderN && !isBorderE)
            {
                voxelN = new VoxelInfo(voxel.Chunk.GetVoxel(positionN), voxel.Chunk, positionN);
                voxelNE = new VoxelInfo(voxel.Chunk.GetVoxel(positionNE), voxel.Chunk, positionNE);
                voxelE = new VoxelInfo(voxel.Chunk.GetVoxel(positionE), voxel.Chunk, positionE);
            }
            else
            {
                // North
                if (isBorderN)
                {
                    if (chunkN != null)
                    {
                        var pos = new Position(voxel.Position.X, 0);
                        voxelN = new VoxelInfo(chunkN.GetVoxel(pos), chunkN, pos);
                    }
                    else
                    {
                        voxelN = new VoxelInfo(Voxel.Air);
                    }
                }
                else
                {
                    voxelN = new VoxelInfo(voxel.Chunk.GetVoxel(positionN), voxel.Chunk, positionN);
                }

                // North-East
                if (isBorderN && isBorderE)
                {
                    if (chunkNE != null)
                    {
                        var pos = new Position(0, 0);
                        voxelNE = new VoxelInfo(chunkNE.GetVoxel(pos), chunkNE, pos);
                    }
                    else
                    {
                        voxelNE = new VoxelInfo(Voxel.Air);
                    }
                }
                else if (isBorderN && !isBorderE)
                {
                    if (chunkN != null)
                    {
                        var pos = new Position(voxel.Position.X + 1, 0);
                        voxelNE = new VoxelInfo(chunkN.GetVoxel(pos), chunkN, pos);
                    }
                    else
                    {
                        voxelNE = new VoxelInfo(Voxel.Air);
                    }
                }
                else if (!isBorderN && isBorderE)
                {
                    if (chunkE != null)
                    {
                        var pos = new Position(0, voxel.Position.Y + 1);
                        voxelNE = new VoxelInfo(chunkE.GetVoxel(pos), chunkE, pos);
                    }
                    else
                    {
                        voxelNE = new VoxelInfo(Voxel.Air);
                    }
                }
                else
                {
                    voxelNE = new VoxelInfo(voxel.Chunk.GetVoxel(positionNE), voxel.Chunk, positionNE);
                }

                // East
                if (isBorderE)
                {
                    if (chunkE != null)
                    {
                        var pos = new Position(0, voxel.Position.Y);
                        voxelE = new VoxelInfo(chunkE.GetVoxel(pos), chunkE, pos);
                    }
                    else
                    {
                        voxelE = new VoxelInfo(Voxel.Air);
                    }
                }
                else
                {
                    voxelE = new VoxelInfo(voxel.Chunk.GetVoxel(positionE), voxel.Chunk, positionE);
                }
            }

            return new VoxelSquare(worldOrigin, voxel, voxelN, voxelNE, voxelE);
        }

        #endregion

        #region Inner Classes

        /// <summary>
        /// Information on a voxel.
        /// </summary>
        protected class VoxelInfo
        {
            /// <summary>
            /// Initializes a new instance of the VoxelInfo class. This is for a voxel that lies outside the world.
            /// </summary>
            /// <param name="voxel">The voxel.</param>
            public VoxelInfo(Voxel voxel) : this(voxel, null, Position.Zero)
            {
            }

            /// <summary>
            /// Initializes a new instance of the VoxelInfo class.
            /// </summary>
            /// <param name="voxel">The voxel.</param>
            /// <param name="chunk">The chunk that the voxel belongs to.</param>
            /// <param name="position">The voxel's position in the chunk.</param>
            public VoxelInfo(Voxel voxel, Chunk chunk, Position position)
            {
                this.Voxel = voxel;
                this.Chunk = chunk;
                this.Position = position;
            }

            /// <summary>
            /// Gets the voxel.
            /// </summary>
            public Voxel Voxel { get; private set; }

            /// <summary>
            /// Gets the chunk that the voxel belongs to. If this is null then the voxel lies outside the world.
            /// </summary>
            public Chunk Chunk { get; private set; }

            /// <summary>
            /// Gets the voxel's position in the chunk.
            /// </summary>
            public Position Position { get; private set; }
        }

        /// <summary>
        /// A 2x2 square of voxels.
        /// </summary>
        protected class VoxelSquare : IEnumerable<VoxelInfo>
        {
            /// <summary>
            /// Initializes a new instance of the VoxelSquare class.
            /// </summary>
            /// <param name="worldOrigin">The world position of the lower-left corner.</param>
            /// <param name="lowerLeft">The voxel at the lower-left corner.</param>
            /// <param name="lowerRight">The voxel at the lower-right corner.</param>
            /// <param name="upperLeft">The voxel at the upper-left corner.</param>
            /// <param name="upperRight">The voxel at the upper-right corner.</param>
            public VoxelSquare(
                Position worldOrigin,
                VoxelInfo lowerLeft,
                VoxelInfo lowerRight,
                VoxelInfo upperLeft,
                VoxelInfo upperRight)
            {
                this.WorldOrigin = worldOrigin;
                this.LowerLeft = lowerLeft;
                this.LowerRight = upperRight;
                this.UpperLeft = lowerRight;
                this.UpperRight = upperLeft;
            }

            /// <summary>
            /// Gets the world position of the lower-left corner.
            /// </summary>
            public Position WorldOrigin { get; private set; }

            /// <summary>
            /// Gets the voxel at the lower-left corner.
            /// </summary>
            public VoxelInfo LowerLeft { get; private set; }

            /// <summary>
            /// Gets the voxel at the lower-right corner.
            /// </summary>
            public VoxelInfo LowerRight { get; private set; }

            /// <summary>
            /// Gets the voxel at the upper-left corner.
            /// </summary>
            public VoxelInfo UpperLeft { get; private set; }

            /// <summary>
            /// Gets the voxel at the upper-right corner.
            /// </summary>
            public VoxelInfo UpperRight { get; private set; }

            /// <summary>
            /// Enumerate the voxels.
            /// </summary>
            /// <returns>An enumerator for the voxels.</returns>
            public IEnumerator<VoxelInfo> GetEnumerator()
            {
                yield return this.LowerLeft;
                yield return this.LowerRight;
                yield return this.UpperLeft;
                yield return this.UpperRight;
            }

            /// <summary>
            /// Enumerate the voxels.
            /// </summary>
            /// <returns>An enumerator for the voxels.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        #endregion
    }
}