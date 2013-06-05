// ----------------------------------------------------------------------------
// <copyright file="MasterJobQueue.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    /// <summary>
    /// The master job queue. Master jobs require all chunk job queues to be pending (i.e. no chunk-specific jobs can
    /// execute concurrently).
    /// </summary>
    public class MasterJobQueue : JobQueue
    {
        /// <summary>
        /// Initialises a new instance of the MasterJobQueue class.
        /// </summary>
        public MasterJobQueue()
        {
        }
    }
}