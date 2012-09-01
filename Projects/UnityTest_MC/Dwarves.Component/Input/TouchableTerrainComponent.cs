// ----------------------------------------------------------------------------
// <copyright file="TouchableTerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Input
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A component that responds to being touched.
    /// </summary>
    public class TouchableTerrainComponent : TouchableComponent
    {
        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Handles the on-touch behaviour for the component.
        /// </summary>
        /// <param name="hitPoint">The point at which the component was touched in world coordinates.</param>
        public override void OnTouch(Vector3 hitPoint)
        {
            // Remove the block at the touched point
            // TODO
        }
    }
}