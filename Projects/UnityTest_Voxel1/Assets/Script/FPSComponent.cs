// ----------------------------------------------------------------------------
// <copyright file="FPSComponent.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Attach this to a GUIText to make a frames/second indicator.
/// <para />
/// It calculates frames/second over each updateInterval, so the display does not keep changing wildly.
/// <para />
/// It is also fairly accurate at very low FPS counts (under 10). We do this not by simply counting frames per interval,
/// but by accumulating FPS for each frame. This way we end up with correct overall FPS even if the interval renders
/// something like 5.5 frames.
/// </summary>
public class FPSComponent : MonoBehaviour
{
    /// <summary>
    /// The frequency of updates.
    /// </summary>
    public float UpdateInterval = 0.5f;

    /// <summary>
    /// FPS accumulated over the interval.
    /// </summary>
    private float accum = 0;

    /// <summary>
    /// Frames drawn over the interval.
    /// </summary>
    private int frames = 0;

    /// <summary>
    /// Time left for the current interval.
    /// </summary>
    private float timeleft;

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        if (!this.guiText)
        {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            this.enabled = false;
            return;
        }

        this.timeleft = this.UpdateInterval;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        this.frames++;

        // Interval ended - update GUI text and start new interval
        if (this.timeleft <= 0)
        {
            // display two fractional digits (f2 format)
            float fps = this.accum / this.frames;
            string format = string.Format("{0:F2} FPS", fps);
            this.guiText.text = format;

            if (fps < 30)
            {
                this.guiText.material.color = Color.yellow;
            }
            else
            {
                if (fps < 10)
                {
                    this.guiText.material.color = Color.red;
                }
                else
                {
                    this.guiText.material.color = Color.green;
                }
            }

            this.timeleft = this.UpdateInterval;
            this.accum = 0.0F;
            this.frames = 0;
        }
    }
}