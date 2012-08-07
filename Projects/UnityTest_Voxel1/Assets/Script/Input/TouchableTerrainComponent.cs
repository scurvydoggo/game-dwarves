// ----------------------------------------------------------------------------
// <copyright file="TouchableTerrainComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// A component that responds to being touched.
/// </summary>
[RequireComponent(typeof(TerrainMutatorComponent))]
public class TouchableTerrainComponent : TouchableComponent
{
    /// <summary>
    /// The terrain mutator component.
    /// </summary>
    private TerrainMutatorComponent cTerrainMutator;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        // Get a reference to the related terrain components
        this.cTerrainMutator = this.GetComponent<TerrainMutatorComponent>();
    }

    /// <summary>
    /// Handles the on-touch behaviour for the component.
    /// </summary>
    /// <param name="hitPoint">The point at which the component was touched in world coordinates.</param>
    public override void OnTouch(Vector3 hitPoint)
    {
        // Remove the block at the touched point
        this.cTerrainMutator.RemoveBlock(new Vector2I((int)Math.Floor(hitPoint.x), (int)Math.Ceiling(hitPoint.y)));
    }
}