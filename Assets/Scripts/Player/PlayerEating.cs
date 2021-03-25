using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun; 
/// <summary>
/// Agario-style functionality
/// When player touches another with less gravy then the one with bigger gravy kills the other. 
/// </summary>
public class PlayerEating : MonoBehaviourPun
{
    /// <summary>
    /// Dot product of forward vectors on collision must be greater than this amount to trigger a kill. 
    /// </summary>
    public float LethalDotProduct; 

    PlayerDeath playerDeath;
    PlayerIdentity playerIdentity;

    public Transform MyBody; 

    private void Awake()
    {
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

            //This player is eaten. 
            if(Utility.IsFacingSameDirection(MyBody.forward, other.transform.forward, LethalDotProduct) &&
               !Utility.IsBehind(MyBody.forward, MyBody.position, other.transform.position))
            {
                Camera.main.GetComponent<FreezeFrame>().FreezeCamera();
                playerDeath.KillPlayer(otherPV.OwnerActorNr); //kill self
                return;  
            }

            //We bounce off if they have more gravies
            if(other.GetComponentInParent<PlayerIdentity>().Gravies > playerIdentity.Gravies)
            {
                GetComponent<PlayerMoveSync>().UpdateMovementRPC(GetComponent<PlayerMovement>().Velocity.normalized * -GetComponent<PlayerMovement>().maxTopSpeed, transform.position); 
            }
        }
    }
    #endregion
}



