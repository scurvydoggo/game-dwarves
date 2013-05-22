// ----------------------------------------------------------------------------
// <copyright file="JobId.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// Provides generation of Job IDs.
    /// </summary>
    public static class JobId
    {
        /// <summary>
        /// Locking object.
        /// </summary>
        private static readonly object IdLock = new object();

        /// <summary>
        /// The lowest unassigned ID.
        /// </summary>
        private static int lowestUnassignedId = 1;

        /// <summary>
        /// Generates a unique ID.
        /// </summary>
        /// <returns>The ID.</returns>
        public static int GenerateId()
        {
            lock (JobId.IdLock)
            {
                if (JobId.lowestUnassignedId == int.MaxValue)
                {
                    JobId.lowestUnassignedId = int.MinValue;
                }

                return JobId.lowestUnassignedId++;
            }
        }
    }
}