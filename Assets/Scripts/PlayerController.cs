using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Handles shooting: input, aiming, cooldown, instantiate projectile
/// Handles damage and getting hit. 
/// </summary>
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    /// Gameplay Values 
    [SerializeField] private float shootCoolDown;

    /// <summary>
    /// force added on impulse when player is hit 
    /// </summary>
    [SerializeField] private float maxHitForce;
    /// <summary>
    /// The number of hits a player can take until they are at axHitForce.
    /// The force for any given hit is calculated as hits / hitsToMaxHitForce * maxHitForce
    /// The force will not exceed max hit force. 
    /// </summary>
    [SerializeField] private int hitsToMaxHitForce; 
    
    [SerializeField] public bool debugControlled; 

    
    
    #region Implementation References
    public static PlayerController localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;
    
    private Vector3 _lookAtPosition; 
    private float _cdTimeLeft = 0;

    private int _timesHit = 0; 
    
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
        if (!debugControlled)
            return;

        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return; 
        
        
        //set look at position from mouse camera position 
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);

            _lookAtPosition = new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z);
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

    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) //following code is only for the local client  
            return; 
        
        if (other.CompareTag("Damage"))
        {
            photonView.RPC("RPC_BeHit",RpcTarget.All, other.transform.forward);
        }
    }
    #endregion
    
    
    #region  PUN Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        return;
        if (stream.IsWriting)
        {
            stream.SendNext(_lookAtPosition);
        }
        else if (stream.IsReading)
        {
            _lookAtPosition = (Vector3)stream.ReceiveNext();
        }
    }
    #endregion


    #region RPC

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
    /// Offline/Local Shoot. called by RPC
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    void Shoot(Vector3 pos, Vector3 dir)
    {
        Instantiate(projectile, pos, Quaternion.LookRotation(dir));
    }

    [PunRPC]
    void RPC_BeHit(Vector3 hitDirection)
    {
        DisablePlayerMesh(0.05f);
        _timesHit++; 
        //ApplyHitForce(hitDirection);
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// applies impulse force on the player towards hit direction 
    /// </summary>
    /// <param name="hitDirection"></param>
    private void ApplyHitForce(Vector3 hitDirection)
    {
        int hits = _timesHit <= hitsToMaxHitForce ? _timesHit : hitsToMaxHitForce;
        float hitForce = (float)hits / hitsToMaxHitForce;
        Vector3 hitDirSameY = new Vector3(hitDirection.x, 0, hitDirection.z);
        Vector3 forceDirection = (hitDirSameY).normalized * hitForce; 
        GetComponent<Rigidbody>().AddForce(hitDirection, ForceMode.Impulse);
    }
    
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
