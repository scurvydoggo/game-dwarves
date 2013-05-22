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
        /// Called once per frame after other Update methods.
        /// </summary>
        public void LateUpdate()
        {
            // Wait for any jobs that are to be completed this frame
            JobSystem.Instance.WaitForFrameJobs();

            // Invoke each action that was queued for invocation
            foreach (Action action in GameScheduler.Instance.TakeInvokeList())
            {
                action.Invoke();
            }
        }
    }
}