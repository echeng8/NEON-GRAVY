using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Cinemachine.Utility;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events; 
using UnityEngine.Animations;

/// <summary>
/// Handles player identification AND shooting: input, aiming, cooldown, instantiate projectile
/// </summary>
public class PlayerController : MonoBehaviourPun
{
    /// Gameplay Values 
    [SerializeField] private float shootCoolDown;
    
    /// <summary>
    /// The number of hits a player can take until they are at axHitForce.
    /// The force for any given hit is calculated as hits / maxHits * maxHitForce
    /// The force will not exceed max hit force. 
    /// </summary>
    [SerializeField] public bool debugControlled;
    
    #region Implementation References
    
    public static PlayerController localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;

    private Vector3 _lookAtPosition; 
    private float _cdTimeLeft = 0;
    

    #endregion
    
    #region Unity Callbacks

    private void Awake()
    {
        if (localPlayerInstance == null)
        {
            if (!PhotonNetwork.IsConnected || photonView.AmOwner)
            {
                SetLocalPlayer();
            }
        }

        if (PhotonNetwork.IsConnected && photonView.Owner == null)
        {
            Destroy(gameObject); // this is the offline character for offline testing. 
        }
    }

    private void Update()
    {
        if (!debugControlled || PhotonNetwork.IsConnected && !photonView.IsMine)
            return;
        
        gameObject.SendMessage("ControlledUpdate");
        gameObject.SendMessage("ControlledFixedUpdate");
        
        //set look at position from mouse camera position 
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, shootingPosition.position); 
        float rayLength;
        
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);

            _lookAtPosition = new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z);
            Debug.DrawRay(_lookAtPosition, (_lookAtPosition - transform.position).normalized * 2f);
            shootPointPivot.LookAt(_lookAtPosition);
        }

        //Shooting Input Detection 
        _cdTimeLeft -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && _cdTimeLeft <= 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer,shootingPosition.position,   shootPointPivot.forward);
            }
            else
            {
                Shoot(shootingPosition.position, shootPointPivot.forward);
            }
            
            _cdTimeLeft = shootCoolDown;
        }
        
        //DEBUG controls
        if (Input.GetButtonDown("Cancel"))
        {
            transform.position = Vector3.zero;
        }

    }
    
    #endregion
    
    
    #region  PUN Callbacks


    #endregion


    #region RPC and Associated Private Methods

    /// <summary>
    /// calls Shoot
    /// todo optimize by using a single byte for radians rotation on y axis? 
    /// </summary>
    [PunRPC]
    void RPC_Shoot(Vector3 position, Vector3 direction)
    {
        Shoot(position, direction);
    }
    
    /// <summary>
    /// Shoots projectile by instantiation.
    /// set actorNum on projectile 
    /// Offline/Local Shoot. called by RPC
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    void Shoot(Vector3 pos, Vector3 dir)
    {
        GameObject p = Instantiate(projectile, pos, Quaternion.LookRotation(dir));
        
        if(PhotonNetwork.IsConnected)
            p.GetComponent<Projectile>().shooterActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    #endregion

    #region Private Methods


    
    private void DisablePlayerMesh(float duration)
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        this.Invoke(() => GetComponentInChildren<SkinnedMeshRenderer>().enabled = true, duration);
    }
    
    
    /// <summary>
    /// set itself as LocalPlayerInstance
    /// invokes OnLocalPlayerSet event
    /// </summary>
    void SetLocalPlayer()
    {
        localPlayerInstance = this;
        OnLocalPlayerSet.Invoke(gameObject);
    }
    
    #endregion

}
