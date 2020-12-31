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
    //private float _cdTimeLeft = 0;
    
    [SerializeField] private float timeToCharge;
    #endregion


    
    
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
            if (SYNC_timeHeld >= timeToCharge)
            {
                Fire();
            }
            SYNC_timeHeld = 0;
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
    /// shoots
    /// todo optimize by using a single byte for radians rotation on y axis? 
    /// </summary>
    [PunRPC]
    void RPC_SpawnProj(Vector3 position, Vector3 direction, PhotonMessageInfo info = new PhotonMessageInfo())
    {
        GameObject p = Instantiate(projectile, position, Quaternion.LookRotation(direction));
        p.GetComponent<Projectile>().shooterActorNum = PhotonNetwork.IsConnected ? info.Sender.ActorNumber : -1;
    }
    

    #endregion

    #region Private Methods


    /// <summary>
    /// determines shoot position and shoots via network or offline 
    /// </summary>
    void Fire()
    {
        //set spawn position to projSpawn 
        Vector3 projSpawn = shootingPosition.position;
        projSpawn += shootingPosition.forward * forwardProjectileOffset; 
                        
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_SpawnProj", RpcTarget.AllViaServer, projSpawn,   shootPointPivot.forward);
        }
        else
        {
            RPC_SpawnProj(projSpawn, shootPointPivot.forward);
        }
    }
    
    private void DisablePlayerMesh(float duration)
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        this.Invoke(() => GetComponentInChildren<SkinnedMeshRenderer>().enabled = true, duration);
    }
    
    
    #endregion

}
