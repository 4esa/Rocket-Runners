﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RunFPS : MonoBehaviour
{

    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    public float updateInterval = 0.5f;

    public Text Text;

    private float accum = 0.0f; // FPS accumulated over the interval
    private float frames = 0.0f; // Frames drawn over the interval
    private float timeleft; // Left time for current interval


    void Start()
    {
        if (Text == null)
        {
            print ("FramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;  
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;
 
        // Interval ended - update GUI text and start new interval
        if( timeleft <= 0.0 )
        {
            // display two fractional digits (f2 format)
            Text.text = "FPS: " + (accum / frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}