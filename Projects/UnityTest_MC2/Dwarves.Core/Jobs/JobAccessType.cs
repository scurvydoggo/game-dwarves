// ----------------------------------------------------------------------------
// <copyright file="JobAccessType.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// Indicates the type of access required.
    /// </summary>
    public enum JobAccessType
    {
        /// <summary>
        /// The job will add/remove chunks or other high-level terrain components.
        /// </summary>
        ChunkLoading = 1,

        /// <summary>
        /// The job will access point data.
        /// </summary>
        Points = 2,

        /// <summary>
        /// The job will access mesh data.
        /// </summary>
        Mesh = 3
    }
}