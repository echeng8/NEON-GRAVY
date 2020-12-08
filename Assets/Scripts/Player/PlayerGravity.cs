using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations;

/// <summary>
/// Handles player gravity user toggling and gravity damage on attacked
/// </summary>
public class PlayerGravity : MonoBehaviourPun
{
    
    /// <summary>
    /// force added on impulse when player is hit 
    /// </summary>
    [SerializeField] private float hitForce;
/// <summary>
/// Seconds the player is invulnerable after being hit. 
/// </summary>
    [SerializeField] private float hitInvulSec; 
    [SerializeField]
    
    private bool gravity = true;
    
    /// <summary>
    /// cannot be hit, even if grav off
    /// </summary>
    private bool hitInvulnerable = false; 
    public BoolEvent OnGravityChange = new BoolEvent();
    private int _timesHit = 0;
    
    //implementation
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// its update but it only calls when you're the local player OR set so in inspector
    /// playercontroller calls it 
    /// </summary>
    void ControlledUpdate()
    {
        //disables gravity on input 
        if (Input.GetButtonDown("Fire2"))
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_SetGravity",RpcTarget.All, !gravity);
            }
            else
            {
                RPC_SetGravity(!gravity);
            }
        }

        //clamp velocity.y to negative or 0 
        if(!gravity)
            rb.velocity = new Vector3(rb.velocity.x,Mathf.Clamp(rb.velocity.y, float.MinValue, 0f), rb.velocity.z);


        
        //Debug Stuff 
        if (Input.GetKeyDown(KeyCode.V))
        {
            print($"Velocity at Current Frame: {rb.velocity} magnitude {rb.velocity.magnitude}");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            OpRPC_BeHit(transform.forward);
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
            if (!gravity && !hitInvulnerable && !isMyBullet)
            {
                OpRPC_BeHit(other.transform.forward);
            }
            else
            {
                return; //todo gravity device takes damage 
            }
            
        }
    }

    private void OnValidate()
    {
        //!!!this will call gravity change twice if something else was edited
        OnGravityChange.Invoke(gravity);
    }

    /// <summary>
    /// Op for 'optional' if online
    /// Calls RPC_BeHit with photonview if online, directly if otherwise 
    /// </summary>
    /// <param name="hitDirection"></param>
    void OpRPC_BeHit(Vector3 hitDirection)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_BeHit",RpcTarget.All, hitDirection);
        }
        else
        {
            RPC_BeHit(hitDirection);
        }
    }
    
    [PunRPC]
    void RPC_BeHit(Vector3 hitDirection)
    {
        //todo invoke event to broadcast attacker and hit status to some central server 
        _timesHit++;
        
        //process hit invulnerability 
        hitInvulnerable = true;
        this.Invoke(() => hitInvulnerable = false, hitInvulSec);
        
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            ApplyHitForce(hitDirection);
        }
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
    
    /// <summary>
    /// applies impulse force on the player towards hit direction
    /// based on hits
    /// todo make hitforce dependent on gravity setting 
    /// </summary>
    /// <param name="hitDirection"></param>
    private void ApplyHitForce(Vector3 hitDirection)
    {
        float hitForce = this.hitForce;
        
        Vector3 hitDirSameY = new Vector3(hitDirection.x, 0, hitDirection.z);
        Vector3 forceDirection = (hitDirSameY).normalized * hitForce;

        forceDirection += GetComponent<Rigidbody>().velocity;

        print($"Applied {forceDirection} force on player.");
        GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
    }
}
