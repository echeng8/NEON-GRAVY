using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; 

/// <summary>
/// Updates network peers on Rigidbody Addforce, Velocity, and transform.position changes
/// Such changes on other scripts MUST use this component in order to be networked. 
/// </summary>
public class PlayerMoveSync : MonoBehaviourPun
{
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    /// <summary>
    /// Adds force, sets veloctiy from the starting position on all clients via RPC. 
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="position"></param>
    public void UpdateMovementRPC(Vector3 velocity, Vector3 position)
    {
        photonView.RPC("RPC_UpdateMovementRPC", RpcTarget.All, velocity, position); 
    }

    /// <summary>
    /// Sets the position of the player in all clients via RPC. 
    /// </summary>
    /// <param name="position"></param>
    public void SetPositionRPC(Vector3 position)
    {
        photonView.RPC("RPC_SetPositionRPC", RpcTarget.All, position);
    }

    
    [PunRPC]
    private void RPC_UpdateMovementRPC(Vector3 velocity, Vector3 pos, PhotonMessageInfo info)
    {
        transform.position = pos;
        rb.velocity = velocity;


        //lag compensation
        if (velocity != Vector3.zero)
        {
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            transform.position += velocity * lag;
            //todo lerp to this position maybe?  
        }
    }

    [PunRPC] 
    private void RPC_SetPosition(Vector3 position)
    {

        transform.position = position; 
    }
    
    
}
