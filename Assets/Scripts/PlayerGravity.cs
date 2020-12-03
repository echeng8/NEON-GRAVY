using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

/// <summary>
/// Handles player gravity user toggling and gravity damage on attacked 
/// </summary>
public class PlayerGravity : MonoBehaviourPun
{
    
    /// <summary>
    /// force added on impulse when player is hit 
    /// </summary>
    [SerializeField] private float hitForce;
    
    [SerializeField]
    private bool gravity = true;
    public BoolEvent OnGravityChange = new BoolEvent();
    private int _timesHit = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// its update but it only calls when you're the local player OR set so in inspector
    /// playercontroller calls it 
    /// </summary>
    void ControlledUpdate()
    {
        //disables gravity 
        if (Input.GetButtonDown("Fire2"))
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_SetGravity",RpcTarget.All, !gravity);
            }
            else
            {
                RPC_SetGravity(!gravity);
                ApplyHitForce(transform.forward);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) //following code is only for the local client  
            return; 
        
        //handle damage
        if (other.CompareTag("Damage"))
        {
            if (!gravity)
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
        
        if(photonView.IsMine || !PhotonNetwork.IsConnected)
            GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
    }
}
