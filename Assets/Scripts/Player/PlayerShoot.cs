using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events; 
using UnityEngine.Animations;

/// <summary>
/// Handles player shooting: input, aiming, cooldown, instantiate projectile
/// </summary>
public class PlayerShoot : MonoBehaviourPun, IPunObservable
{
    /// Gameplay Values 
    [SerializeField] private float shootCoolDown; //todo move value to projectile later 

    /// <summary>
    /// the forward distance from the spawning position that the projectile is spawned in 
    /// </summary>
    [SerializeField] private float forwardProjectileOffset; 
    #region Implementation References
    
    
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;

    private Vector3 _lookAtPosition; 
    private float _cdTimeLeft = 0;
    

    #endregion

    private float timeToCharge;
    
    
    #region Implementation Values

    private float SYNC_timeHeld; 
    
    #endregion
    
    #region Unity Callbacks



    private void ControlledUpdate()
    {
        //set look at position from mouse camera position 
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, shootPointPivot.position); 
        float rayLength;
        
        Debug.DrawRay(shootingPosition.position, Vector3.up * 3, Color.yellow);
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            
            Debug.DrawRay(pointTolook, (pointTolook - shootPointPivot.position).normalized * 2f);
            shootPointPivot.LookAt(pointTolook);
        }

        //Shooting Input Detection 
        if (Input.GetButton("Fire1")) //charging
        {
            SYNC_timeHeld += Time.deltaTime; 

        }

        if (Input.GetButtonUp("Fire1")) //firing 
        {
            SYNC_timeHeld = 0; 
            
            //set spawn positin to projSpawn 
            Vector3 projSpawn = shootingPosition.position;
            projSpawn += shootingPosition.forward * forwardProjectileOffset; 
                        
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer, projSpawn,   shootPointPivot.forward);
            }
            else
            {
                Shoot(projSpawn, shootPointPivot.forward);
            }

        }
    }
    #endregion
    
    
    #region  PUN Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SYNC_timeHeld);
        }

        if (stream.IsReading)
        {
            SYNC_timeHeld = (float)stream.ReceiveNext();
            print(SYNC_timeHeld); 
        }
    }

    #endregion


    #region RPC and Associated Private Methods

    /// <summary>
    /// calls Shoot
    /// todo optimize by using a single byte for radians rotation on y axis? 
    /// </summary>
    [PunRPC]
    void RPC_Shoot(Vector3 position, Vector3 direction, PhotonMessageInfo info)
    {
        Shoot(position, direction, info.Sender.ActorNumber);
    }

    /// <summary>
    /// Shoots projectile by instantiation.
    /// set actorNum on projectile 
    /// Offline/Local Shoot. called by RPC
    /// </summary>
    /// <param name="pos">the position of the bullet when it spawns</param>
    /// <param name="dir"></param>
    /// <param name="senderActorNum"></param>
    void Shoot(Vector3 pos, Vector3 dir, int senderActorNum = -1)
    {
        GameObject p = Instantiate(projectile, pos, Quaternion.LookRotation(dir));

        if (PhotonNetwork.IsConnected)
        {
            p.GetComponent<Projectile>().shooterActorNum = senderActorNum;
        }
    }

    #endregion

    #region Private Methods


    
    private void DisablePlayerMesh(float duration)
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        this.Invoke(() => GetComponentInChildren<SkinnedMeshRenderer>().enabled = true, duration);
    }
    
    
    #endregion

}
