using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlatformAppearance : MonoBehaviour
{
    /// <summary>
    /// Event invoked when players hover over the platform and are ready to bounce off of it. 
    /// </summary>
    public UnityEvent OnTouch = new UnityEvent();

    /// <summary>
    /// Event invoked when players bounce off the platform. 
    /// </summary>
    public UnityEvent OnBounce = new UnityEvent(); 
}
