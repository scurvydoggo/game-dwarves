// ----------------------------------------------------------------------------
// <copyright file="CameraMovementComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component for moving the camera component.
/// </summary>
public class CameraMovementComponent: MonoBehaviour
{
    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        // Check if the camera should pan
#if UNITY_IPHONE || UNITY_ANDRIOD
        // TODO        
#else
        if (Input.GetKey(KeyCode.LeftArrow))
        {

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {

        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {

        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {

        }
#endif
    }
}