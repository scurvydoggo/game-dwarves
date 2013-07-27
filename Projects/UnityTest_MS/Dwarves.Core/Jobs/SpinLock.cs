// ----------------------------------------------------------------------------
// <copyright file="SpinLock.cs" company="Dematic">
//     Copyright © Dematic 2009-2013. All rights reserved
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System.Threading;

    /// <summary>
    /// A fast yet CPU-intensive locking mechanism.
    /// </summary>
    public class SpinLock
    {
        /// <summary>
        /// Indicates whether the lock is currently held.
        /// </summary>
        private int isLockHeld;

        /// <summary>
        /// The iterations to spin between each check on the lock status.
        /// </summary>
        private int spinIterations;

        /// <summary>
        /// Initialises a new instance of the SpinLock class.
        /// </summary>
        /// <param name="spinIterations">The iterations to spin between each check on the lock status.</param>
        public SpinLock(int spinIterations)
        {
            this.spinIterations = spinIterations;
        }

        /// <summary>
        /// Enter the lock.
        /// </summary>
        public void Enter()
        {
            int wait = this.spinIterations;
            while (Interlocked.CompareExchange(ref this.isLockHeld, 1, 0) != 0)
            {
                if (wait < 1000)
                {
                    Thread.SpinWait(wait);
                    wait *= 2;
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// Release the lock.
        /// </summary>
        public void Exit()
        {
            Interlocked.Exchange(ref this.isLockHeld, 0);
        }
    }
}