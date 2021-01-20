using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
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
    /// Camera used for slight jolt when players bounce off platform.
    /// </summary>
    private Animator camAnimator;
    

    
    /// <summary>
    /// Event invoked when players leave platform
    /// </summary>
    public UnityEvent OnPlatLeave = new UnityEvent();
    
    private Animator platAnimator;
    private ParticleSystem platParticles;


    private void Start()
    {
        OnBounce.AddListener(bounceOnPlatEvents);
        camAnimator = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Animator>();
    }
    
    /// <summary>
    /// make the platform specified by platNum brighter or something 
    /// </summary>
    /// <param name="platNum"></param>
    public void bounceOnPlatEvents()
    {
        camAnimator.Play("Camera OnBounce");
    }
}