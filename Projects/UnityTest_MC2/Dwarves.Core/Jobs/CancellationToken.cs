// ----------------------------------------------------------------------------
// <copyright file="CancellationToken.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core.Jobs
{
    using System;

    /// <summary>
    /// Propagates notification that operations should be cancelled.
    /// </summary>
    public class CancellationToken
    {
        /// <summary>
        /// Gets a value indicating whether cancellation has been requested.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Cancel the operation.
        /// </summary>
        public void Cancel()
        {
            this.IsCancelled = true;
        }

        /// <summary>
        /// Throws a CancelledException if cancellation has been requested.
        /// </summary>
        public void ThrowIfCancelled()
        {
            if (this.IsCancelled)
            {
                throw new CancelledException(this);
            }
        }

        /// <summary>
        /// The exception that is thrown upon cancellation.
        /// </summary>
        public class CancelledException : Exception
        {
            /// <summary>
            /// Initialises a new instance of the CancelledException class.
            /// </summary>
            /// <param name="token">The cancellation token that triggered the cancellation.</param>
            public CancelledException(CancellationToken token)
            {
                this.Token = token;
            }

            /// <summary>
            /// Gets the cancellation token that triggered the cancellation.
            /// </summary>
            public CancellationToken Token { get; private set; }
        }
    }
}