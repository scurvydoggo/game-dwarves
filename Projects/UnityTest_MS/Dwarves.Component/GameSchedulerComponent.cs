// ----------------------------------------------------------------------------
// <copyright file="GameSchedulerComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component
{
    using System;
    using Dwarves.Core;
    using UnityEngine;

    /// <summary>
    /// GameScheduler component.
    /// </summary>
    public class GameSchedulerComponent : MonoBehaviour
    {
        /// <summary>
        /// The last thrown exception.
        /// </summary>
        private Exception lastThrown;

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            // Invoke each action that was queued for invocation
            foreach (Action action in GameScheduler.Instance.TakeInvokeList())
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Displays an error dialog if an unhandled exception has occurred.
        /// </summary>
        public void OnGUI()
        {
            if (GameScheduler.Instance.UnhandledException != null)
            {
                if (this.lastThrown != GameScheduler.Instance.UnhandledException)
                {
                    this.lastThrown = GameScheduler.Instance.UnhandledException;
                    throw this.lastThrown;
                }

                string error = GameScheduler.Instance.UnhandledException.Message;
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty);
                GUI.Box(new Rect(0, (Screen.height / 2) - 30, Screen.width, 60), error);
                if (GUI.Button(new Rect((Screen.width / 2) + 100, Screen.height / 2, 100, 20), "OK"))
                {
                    Application.Quit();
                }
            }
        }
    }
}