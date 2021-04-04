using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun; 

/// <summary>
/// Kills other players when they are the color you beat. 
/// </summary>
public class PlayerEating : MonoBehaviourPun
{

    PlayerColorChange playerColorChange; 
    PlayerDeath playerDeath;
    PlayerIdentity playerIdentity;

    public Transform MyBody; 

    private void Awake()
    {

        playerColorChange = GetComponent<PlayerColorChange>(); 
        playerDeath = GetComponentInParent<PlayerDeath>();
        playerIdentity = GetComponentInParent<PlayerIdentity>(); 
    }

    #region Unity Callbacks
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return;


        if(other.CompareTag("Body"))
        {
            if (!other.GetComponentInParent<PlayerDeath>().alive) //quit if colliding with a dead person 
            {
                return;
            }

            PlatformState myState = playerColorChange.PlatState;
            PlatformState theirState = other.GetComponentInParent<PlayerColorChange>().PlatState; 

            if(myState == theirState) //bounce off
            {
                GetComponent<PlayerMoveSync>().UpdateMovementRPC(GetComponent<PlayerMovement>().Velocity.normalized * -GetComponent<PlayerMovement>().topSpeed, transform.position);
            } else if (IsEatenBy(myState, theirState)) {

                int otherIndex = other.GetComponentInParent<PlayerIdentity>().GetID(); 

                //This player is eaten.
                playerDeath.KillPlayerRPC(otherIndex); //kill self

                if(photonView.Owner == PhotonNetwork.LocalPlayer)
                {
                    Camera.main.GetComponent<FreezeFrame>().FreezeCamera(); 
                }
            }
        }
    }


    /// <summary>
    /// returns true if selfstate is eaten by theirstate
    /// </summary>
    /// <param name="selfState"></param>
    /// <param name="theirState"></param>
    /// <returns></returns>
    public bool IsEatenBy(PlatformState selfState, PlatformState theirState)
    {
        if (selfState == PlatformState.FIRE && theirState == PlatformState.WATER ||
            selfState == PlatformState.GRASS && theirState == PlatformState.FIRE || 
            selfState == PlatformState.WATER && theirState == PlatformState.GRASS)
            return true;

        return false; 
    }

    #endregion
}



