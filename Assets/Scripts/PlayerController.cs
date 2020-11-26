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
    #region Implementation References
    public static PlayerController localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;
    
    private Vector3 _lookAtPosition; 
    private float _cdTimeLeft = 0;
    #endregion
    
    /// <summary>
    /// Gameplay Values 
    /// </summary>
    [SerializeField] private float coolDown;
    [SerializeField] public bool debugControlled; 
    

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
            
            _cdTimeLeft = coolDown;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Damage"))
        {
            //placeholder code for indicating damage
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            this.Invoke(() => GetComponentInChildren<SkinnedMeshRenderer>().enabled = true, 1f); 
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
    
    #endregion

    #region Private Methods
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
