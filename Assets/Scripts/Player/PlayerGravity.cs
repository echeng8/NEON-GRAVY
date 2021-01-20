using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Handles vestigal gravity toggling and Damage force
/// </summary>
public class PlayerGravity : MonoBehaviourPun
{

    #region Gameplay Fields

    /// <summary>
    /// force added on impulse when player is hit 
    /// </summary>
    [SerializeField] private float hitForce;

    
    /// <summary>
    /// Set the maximum speed in the air
    /// </summary>
    [SerializeField] private float maxSpeed;
    
    
    #endregion

    #region Implementation Fields

    private bool gravity = false;
    


    //unity events
    public BoolEvent OnGravityChange = new BoolEvent();

    /// <summary>
    /// passes the actor number of the attacker 
    /// Triggers when the player recalls.
    /// </summary>
    public IntEvent OnHit = new IntEvent(); 
    
    private Rigidbody rb;
    
    #endregion

    #region Unity Callbacks 

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        
        GetComponent<PlayerDeath>().OnDeath.AddListener(ResetOnDeath);
    }

    private void Start()
    {
        //todo make gravy start off by default then remove the whole friggin concept from the codebase
        photonView.RPC("RPC_SetGravity", RpcTarget.All, false); 
    }


    /// <summary>
    /// its update but it only calls when you're the local player OR set so in inspector
    /// playercontroller calls it 
    /// </summary>
    void ControlledUpdate()
    {
        //clamp velocity.y to negative or 0 
        if (!gravity)
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, float.MinValue, 0f), rb.velocity.z);

        //clamp velocity matgnitude
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        //Debug Stuff 
        if (Input.GetKeyDown(KeyCode.V))
        {
            print($"Velocity at Current Frame: {rb.velocity} magnitude {rb.velocity.magnitude}");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) //following code is only for the local client  
            return;

        //handle damage
        if (other.CompareTag("Damage"))
        {
            //todo polish
            bool isMyBullet = PhotonNetwork.LocalPlayer.ActorNumber == other.GetComponent<Projectile>().shooterActorNum;
            
            if (!isMyBullet)
            {
                //todo move to projectileProfiles format 
                photonView.RPC("RPC_ProcessHit", RpcTarget.All, other.GetComponent<Projectile>().shooterActorNum);

                //todo move fortces to PlayerMovement
                Vector3 hitDirection = other.transform.forward; //todo get force from projectile 
                ApplyHitForce(hitDirection);
                
            }
        }
    }


    #endregion
    
    #region RPC and related Methods 

    /// <summary>
    /// RPC_RecordHit tells other clients who hit them
    /// </summary>
    /// <param name="attackerNum"></param>b
    [PunRPC]
    void RPC_ProcessHit(int attackerNum = -1)
    {
        OnHit.Invoke(attackerNum);
    }

    /// <summary>
    /// sets gravity. if gravity is off, player is a physics object in space with previous velocity
    /// </summary>
    /// <param name="on"></param>
    [PunRPC]
    private void RPC_SetGravity(bool on)
    {
        gravity = on;
        OnGravityChange.Invoke(on);
    }
    
    #endregion

    #region Public Methods

    public bool GetGravity()
    {
        return gravity; 
    }
    

    #endregion

    #region Private Methods
    
    /// <summary>
    /// applies impulse force on the player towards hit direction
    /// based on hits
    /// </summary>
    /// <param name="hitDirection"></param>
    private void ApplyHitForce(Vector3 hitDirection)
    {
        float hitForce = this.hitForce;

        Vector3 hitDirSameY = new Vector3(hitDirection.x, 0, hitDirection.z);
        Vector3 forceDirection = (hitDirSameY).normalized * hitForce;

        forceDirection += GetComponent<Rigidbody>().velocity;
        
        //TODO network velocity
        rb.AddForce(forceDirection, ForceMode.Impulse);
    }

    /// <summary>
    /// resets values on death, to be added to OnDeath event
    /// called for ALL clients by PlayerDeath through event triggering  
    /// </summary>
    private void ResetOnDeath()
    {
        RPC_SetGravity(false);
    }
    #endregion


}
