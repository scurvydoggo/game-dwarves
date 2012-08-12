// ----------------------------------------------------------------------------
// <copyright file="CameraMovementComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Component for moving the camera component.
/// </summary>
public class CameraMovementComponent : MonoBehaviour
{
    /// <summary>
    /// The movement speed.
    /// </summary>
    public float Speed = 20;

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
        float moveDistance = this.Speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-moveDistance, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(moveDistance, 0, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.position += new Vector3(0, moveDistance, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.position += new Vector3(0, -moveDistance, 0);
        }
#endif
    }
}