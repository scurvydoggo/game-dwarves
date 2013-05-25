// ----------------------------------------------------------------------------
// <copyright file="JobFactory.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;

    /// <summary>
    /// Provides factory methods for use in Job creation.
    /// </summary>
    public static class JobFactory
    {
        /// <summary>
        /// Creates a JobInfo instance with AddSurfaceHeights behaviour.
        /// </summary>
        /// <returns>The job info.</returns>
        public static JobInfo AddSurfaceHeights()
        {
            return new JobInfo(
                JobBehaviour.AddSurfaceHeights,
                new JobAccess(JobAccessType.ChunkLoading),
                JobWriteOrder.Normal);
        }

        /// <summary>
        /// Creates a JobInfo instance with AddChunk behaviour.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job info.</returns>
        public static JobInfo AddChunk(Vector2I chunk)
        {
            return new JobInfo(
                JobBehaviour.AddChunk,
                new JobAccess(JobAccessType.ChunkLoading, chunk),
                JobWriteOrder.Normal);
        }

        /// <summary>
        /// Creates a JobInfo instance with RemoveChunk behaviour.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job info.</returns>
        public static JobInfo RemoveChunk(Vector2I chunk)
        {
            return new JobInfo(
                JobBehaviour.RemoveChunk,
                new JobAccess(JobAccessType.ChunkLoading, chunk),
                JobWriteOrder.Early);
        }

        /// <summary>
        /// Creates a JobInfo instance with LoadPoints behaviour.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job info.</returns>
        public static JobInfo LoadPoints(Vector2I chunk)
        {
            return new JobInfo(
                JobBehaviour.LoadPoints,
                new JobAccess(JobAccessType.Points, chunk),
                JobWriteOrder.Early);
        }

        /// <summary>
        /// Creates a JobInfo instance with RebuildMesh behaviour.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job info.</returns>
        public static JobInfo RebuildMesh(Vector2I chunk)
        {
            return new JobInfo(
                JobBehaviour.RebuildMesh,
                new JobAccess(JobAccessType.Mesh, chunk),
                JobWriteOrder.Early,
                new JobAccess(JobAccessType.Points, TerrainChunk.GetNeighbours(chunk)));
        }

        /// <summary>
        /// Creates a JobInfo instance with UpdateMeshFilter behaviour.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <returns>The job info.</returns>
        public static JobInfo UpdateMeshFilter(Vector2I chunk)
        {
            return new JobInfo(
                JobBehaviour.UpdateMeshFilter,
                null,
                JobWriteOrder.Normal,
                new JobAccess(JobAccessType.Mesh, chunk));
        }
    }
}