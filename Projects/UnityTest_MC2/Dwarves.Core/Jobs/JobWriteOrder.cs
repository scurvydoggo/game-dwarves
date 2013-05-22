// ----------------------------------------------------------------------------
// <copyright file="JobWriteOrder.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// Indicates the write order in relation to other jobs with the same write access.
    /// </summary>
    public enum JobWriteOrder
    {
        /// <summary>
        /// The write task has normal ordering.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The write task should occur before normal writes.
        /// </summary>
        Early = 1,

        /// <summary>
        /// The write task should occur after normal writes.
        /// </summary>
        Late = -1
    }
}