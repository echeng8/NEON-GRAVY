using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

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
    /// <param name="force"></param>
    /// <param name="velocity"></param>
    /// <param name="position"></param>
    public void UpdateMovementRPC(Vector3 force, Vector3 velocity, Vector3 position)
    {
        photonView.RPC("RPC_UpdateMovementRPC", RpcTarget.AllViaServer, force, velocity, position); 
    }
    /// <summary>
    /// Adds the force and position to the player to all clients via RPC.
    /// </summary>
    /// <param name="force"></param>
    /// <param name="pos"></param>
    public void AddForceRPC(Vector3 force, Vector3 pos) {
        photonView.RPC("RPC_AddForce", RpcTarget.AllViaServer, force, pos); 
    }

    /// <summary>
    /// Sets the velocity to the player in all clients via RPC.
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="pos"></param>
    public void SetVelocityRPC(Vector3 velocity, Vector3 pos)
    {
        photonView.RPC("RPC_SetVelocity", RpcTarget.AllViaServer, velocity, pos);
    }

    /// <summary>
    /// Sets the position of the player in all clients via RPC. 
    /// </summary>
    /// <param name="position"></param>
    public void SetPositionRPC(Vector3 position)
    {
        photonView.RPC("RPC_SetPositionRPC", RpcTarget.AllViaServer, position);
    }

    [PunRPC]
    private void RPC_UpdateMovementRPC(Vector3 force, Vector3 velocity, Vector3 pos)
    {
        transform.position = pos;
        rb.velocity = velocity; 
        rb.AddForce(force, ForceMode.Impulse);
    }

    [PunRPC]
    private void RPC_AddForce(Vector3 force, Vector3 pos)
    {
        transform.position = pos;
        rb.AddForce(force, ForceMode.Impulse);
    }


    [PunRPC]
    private void RPC_SetVelocity(Vector3 velocity, Vector3 pos)
    {
        transform.position = pos;
        rb.velocity = velocity; 
    }

    [PunRPC] 
    private void RPC_SetPosition(Vector3 position)
    {
        transform.position = position; 
    }
    
    
}
