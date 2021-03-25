using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public enum PlatformState {FIRE, WATER, GRASS}; 

public class PlatformAppearance : MonoBehaviour
{

    public PlatformState CurrentState {
        set
        {
            SetColorToReflectState(value); 
            _currentState = value; 
        }
        get
        {
            return _currentState; 
        }
    }

    [SerializeField]
    PlatformState _currentState; 

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

    public void InitalizeState()
    {
        CurrentState = (PlatformState)(transform.GetSiblingIndex() % 3); 
    }

    void SetColorToReflectState(PlatformState state)
    {
        Material[] mats = new Material[2]; 
        mats[1] = Resources.Load("PlatformColors/Platform", typeof(Material)) as Material;
        switch (state)
        {
            case PlatformState.FIRE:
                mats[0] = Resources.Load("PlatformColors/PE_Red", typeof(Material)) as Material;
                break;
            case PlatformState.WATER:
                mats[0] = Resources.Load("PlatformColors/PE_Blue", typeof(Material)) as Material;
                break;
            case PlatformState.GRASS:
                mats[0] = Resources.Load("PlatformColors/PE_Green", typeof(Material)) as Material;
                break;
        }
        GetComponent<MeshRenderer>().sharedMaterials = mats; 
    }

    //private void Start()
    //{
    //    OnBounce.AddListener(bounceOnPlatEvents);
    //    camAnimator = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Animator>();
    //}

    ///// <summary>
    ///// make the platform specified by platNum brighter or something 
    ///// </summary>
    ///// <param name="platNum"></param>
    //public void bounceOnPlatEvents()
    //{
    //    camAnimator.Play("Camera OnBounce");
    //}
}