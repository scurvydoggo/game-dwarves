// ----------------------------------------------------------------------------
// <copyright file="TerrainTouchComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Dwarves.Component.Terrain
{
    using System;
    using Dwarves.Component.Input;
    using Dwarves.Core;
    using Dwarves.Core.Math;
    using Dwarves.Core.Terrain;
    using UnityEngine;

    /// <summary>
    /// A component that responds to being touched.
    /// </summary>
    [RequireComponent(typeof(TerrainComponent))]
    public class TerrainTouchComponent : TouchableComponent
    {
        /// <summary>
        /// The terrain manager.
        /// </summary>
        private TerrainManager terrainManager;

        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Start()
        {
            this.terrainManager = this.GetComponent<TerrainComponent>().TerrainManager;
        }

        /// <summary>
        /// Handles the on-touch behaviour for the component.
        /// </summary>
        /// <param name="hitPoint">The point at which the component was touched in world coordinates.</param>
        public override void ProcessTouch(Vector3 hitPoint)
        {
            var position = new Vector2I((int)Math.Floor(hitPoint.x), (int)Math.Floor(hitPoint.y));
            var offset = new Vector2(Math.Abs(hitPoint.x - position.X), Math.Abs(hitPoint.y - position.Y));

            // Dig at the touched point
            this.terrainManager.TerrainMutator.Dig(position, offset);
        }
    }
}