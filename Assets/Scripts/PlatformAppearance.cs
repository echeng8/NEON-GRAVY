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
    
    /// <summary>
    /// Event invoked when players leave platform
    /// </summary>
    public UnityEvent OnPlatLeave = new UnityEvent();
    
    private Animator platAnimator;
    private ParticleSystem platParticles;

    private void Start()
    {
        platAnimator = GetComponent<Animator>();
        platParticles = GetComponentInChildren<ParticleSystem>();
        OnBounce.AddListener(BounceOnPlat);
        OnTouch.AddListener(TouchOnPlat);
    }
    
    /// <summary>
    /// make the platform specified by platNum brighter or something 
    /// </summary>
    /// <param name="platNum"></param>
    public void BounceOnPlat()
    {
        platAnimator.Play("PlatformBounce");
        platParticles.Play();
    }
    public void TouchOnPlat()
    {
        platAnimator.Play("PlatformIsTouched");
    }
}