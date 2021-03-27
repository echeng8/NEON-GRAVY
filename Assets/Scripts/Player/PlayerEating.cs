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
            PhotonView otherPV = other.GetComponentInParent<PhotonView>(); 

            if (otherPV.IsMine || !other.GetComponentInParent<PlayerDeath>().alive) //quit if colliding with self or a dead person 
            {
                return;
            }

            PlatformState myState = playerColorChange.PlatState;
            PlatformState theirState = other.GetComponentInParent<PlayerColorChange>().PlatState; 

            if(myState == theirState) //bounce off
            {
                GetComponent<PlayerMoveSync>().UpdateMovementRPC(GetComponent<PlayerMovement>().Velocity.normalized * -GetComponent<PlayerMovement>().topSpeed, transform.position);
            } else if (IsEatenBy(myState, theirState)) {
                //This player is eaten. 
                Camera.main.GetComponent<FreezeFrame>().FreezeCamera();
                playerDeath.KillPlayer(otherPV.OwnerActorNr); //kill self
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



