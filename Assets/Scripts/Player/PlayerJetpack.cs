using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events;
using Photon.Pun; 

/// <summary>
/// Handles player bouncing from platforms 
/// </summary>
public class PlayerJetpack : MonoBehaviourPun
{

    #region Implementation Values

    /// <summary>
    /// the velocity that is added with each bounce
    /// *note a player's velocity caps out as determined by PlayerGravity
    /// </summary>
    public float addedBounceVelocity = 3; 

    public int streak;

    public TextMeshProUGUI streaksText;


    public UnityEvent OnBounce = new UnityEvent();

    private Rigidbody rb;
    
    #endregion
    
    #region Unity Callbacks
    /// <summary>
    /// The stuff that is commented out is the stuff is just in case if we want to go back to holding down a button/wasd
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 

        streak = 0;
        streaksText = GameObject.Find("Streaks").GetComponent<TextMeshProUGUI>();

        
    }

    public void ControlledUpdate()
    {
        Vector3 pointToDash = new Vector3(0,0,0);
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position); 
        float rayLength;
        
        Debug.DrawRay(transform.position, Vector3.up * 3, Color.cyan);
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            pointToDash = cameraRay.GetPoint(rayLength);
            Debug.DrawRay(pointToDash, (pointToDash - transform.position).normalized * 2f);
        }
        
        if (!GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GameObject platformBelow = GetComponent<ThirdPersonCharacter>().PlatformBelow;

                if (platformBelow != null) //THE BOUNCE
                {
                    Vector3 dashDirection = (pointToDash - transform.position).normalized;
                    transform.forward = (pointToDash - transform.position).normalized;

                    float velMagnitude = Vector3.Magnitude(GetComponent<Rigidbody>().velocity);
                   
                    //TODO move to PlayerMovement (cant PlayerJetpack just get renamed to PlayerMovement?)
                    Vector3 velocity = dashDirection * (velMagnitude + addedBounceVelocity); 

                    GetComponent<PlayerMoveSync>().UpdateMovementRPC(velocity,transform.position);
                    
                    OnBounce.Invoke(); 
                    InvokeOnBouncePlatformRPC(); 

                    streak++;

                }
                else
                {
                    streak = 0;
                }
            }
        }
        if (GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
        {
            streak = 0;
        }
        StreakCounter();
    }
    #endregion
    
    #region Custom Methods
    void StreakCounter()
    {
        streaksText.text = $"Streaks x{streak}";
    }

    //platform event rpcs
    [PunRPC]
    void RPC_InvokeOnBouncePlatform()
    {
        GetComponent<ThirdPersonCharacter>().PlatformBelow.GetComponent<PlatformAppearance>().OnBounce.Invoke();
        
    }
    
    void InvokeOnBouncePlatformRPC()
    {
        photonView.RPC("RPC_InvokeOnBouncePlatform", RpcTarget.All); 
    }


    #endregion

}
