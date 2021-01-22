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
/// also melee now 
/// </summary>
public class PlayerShoot : MonoBehaviourPun
{
    #region Gameplay Fields
    /// <summary>
    /// Seconds until attack is ready 
    /// </summary>
    public float attackCooldown;

    /// <summary>
    /// TODO 
    /// How many streaks the player must have before they unlock the projectile 
    /// </summary>
    public int streaksToUnlockProjectile; 


    #endregion

    #region Implementation References

    /// <summary>
    /// the forward distance from the spawning position that the projectile is spawned in 
    /// </summary>
    [SerializeField] private float forwardProjectileOffset;
    
    [SerializeField] private GameObject projectile;
    [SerializeField] public Transform shootPointPivot, shootingPosition;

    [SerializeField] public PlayerDetector meleeRangeColllider;


    [HideInInspector] public bool canAttack = false; 

    private Vector3 _lookAtPosition;
    
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

        //attacking Input Detection 
        if (Input.GetButtonDown("Fire2") && canAttack) //firing 
        {
            Fire();
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
    
    #endregion

}
