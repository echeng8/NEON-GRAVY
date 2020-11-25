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
public class PlayerController : MonoBehaviourPun
{
    /// <summary>
    /// Implementation References
    /// </summary>
    public static PlayerController localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;

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
            bool isRoomObject = photonView.Owner != null &&
                                photonView.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber;

            if (!PhotonNetwork.IsConnected || !isRoomObject)
            {
                SetLocalPlayer();
            }
            else
                Destroy(gameObject); // this is the offline character for offline testing. 
        }
    }

    private void Update()
    {
        if (!debugControlled)
            return; 
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return; 
        
        
        //Look at Camera and shooting
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);

            shootPointPivot.LookAt(new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z));
        }

        //Shooting
        _cdTimeLeft -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && _cdTimeLeft <= 0)
        {
            Shoot();
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

    /// <summary>
    /// 
    /// </summary>
    void Shoot()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(projectile.name,shootingPosition.position,Quaternion.LookRotation(shootPointPivot.forward));
        }
        else
        {
            Instantiate(projectile, shootingPosition.position,   Quaternion.LookRotation(shootPointPivot.forward));
        }
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
