// ----------------------------------------------------------------------------
// <copyright file="JobReuse.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// Indicates how an attempt will be made to reuse an existing job.
    /// </summary>
    public enum JobReuse
    {
        /// <summary>
        /// Do not reuse any existing job.
        /// </summary>
        None,

        /// <summary>
        /// Reuse an equivalent job that is pending or executing.
        /// </summary>
        ReuseAny,

        /// <summary>
        /// Reuse an equivalent job that is pending.
        /// </summary>
        ReusePending,

        /// <summary>
        /// Merge the input parameters of this job into an equivalent job that is pending.
        /// </summary>
        MergePending
    }
}