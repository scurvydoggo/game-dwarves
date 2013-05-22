// ----------------------------------------------------------------------------
// <copyright file="JobBehaviour.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// Indicates the job behaviour.
    /// </summary>
    public enum JobBehaviour
    {
        /// <summary>
        /// The job has no significant behaviour. General jobs are not applicable for reuse.
        /// </summary>
        General,

        /// <summary>
        /// Add surface heights.
        /// </summary>
        AddSurfaceHeights,

        /// <summary>
        /// Add a chunk.
        /// </summary>
        AddChunk,

        /// <summary>
        /// Remove a chunk.
        /// </summary>
        RemoveChunk,

        /// <summary>
        /// Loads the point data through generation or deserialisation.
        /// </summary>
        LoadPoints,

        /// <summary>
        /// Rebuild the mesh data.
        /// </summary>
        RebuildMesh,

        /// <summary>
        /// Update the MeshFilter component.
        /// </summary>
        UpdateMeshFilter
    }
}