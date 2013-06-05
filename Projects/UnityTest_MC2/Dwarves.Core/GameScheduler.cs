// ----------------------------------------------------------------------------
// <copyright file="GameScheduler.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Allows for queuing actions to be executed on the main thread.
    /// </summary>
    public class GameScheduler
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static GameScheduler instance = new GameScheduler();

        /// <summary>
        /// The locking object for the invocation list.
        /// </summary>
        private readonly object invokeListLock = new object();

        /// <summary>
        /// The invocation list.
        /// </summary>
        private List<Action> invokeList;

        /// <summary>
        /// Prevents a default instance of the GameScheduler class from being created.
        /// </summary>
        private GameScheduler()
        {
            this.invokeList = new List<Action>();
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static GameScheduler Instance
        {
            get { return GameScheduler.instance; }
        }

        /// <summary>
        /// Gets the unhandled exception to be processed on the main thread, causing the game to exit.
        /// </summary>
        public Exception UnhandledException { get; private set; }

        /// <summary>
        /// Enqueues an action to be invoked on the main thread.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        public void Invoke(Action action)
        {
            lock (this.invokeListLock)
            {
                this.invokeList.Add(action);
            }
        }

        /// <summary>
        /// Take the actions that have been queued for invocation on the main thread and then clear the list.
        /// </summary>
        /// <returns>The invocation list.</returns>
        public Action[] TakeInvokeList()
        {
            lock (this.invokeListLock)
            {
                var actions = this.invokeList.ToArray();
                this.invokeList.Clear();
                return actions;
            }
        }

        /// <summary>
        /// Sets an unhandled exception which causes the game to halt.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public void SetUnhandledException(Exception ex)
        {
            if (this.UnhandledException == null)
            {
                this.UnhandledException = ex;
            }
        }
    }
}