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
    /// <summary>
    /// Implementation References
    /// </summary>
    public static PlayerController localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;

    private Vector3 _lookAtPosition; 
    
    /// <summary>
    /// Gameplay Values 
    /// </summary>
    [SerializeField] private float coolDown;
    [SerializeField] public bool debugControlled; 
    
    private float _cdTimeLeft = 0;

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
        
        //shootPointPivot.LookAt(_lookAtPosition);
        
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return; 
        
        
        //set look at position from camera pos
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);

            _lookAtPosition = new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z);
            shootPointPivot.LookAt(_lookAtPosition);
        }

        //Shooting
        _cdTimeLeft -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && _cdTimeLeft <= 0)
        {
            photonView.RPC("RPC_Shoot", RpcTarget.AllViaServer);
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

    /// <summary>
    /// shoots the bullet 
    /// </summary>
    [PunRPC]
    void RPC_Shoot()
    {
        Instantiate(projectile, shootingPosition.position,   Quaternion.LookRotation(shootPointPivot.forward));
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

}
