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

            if(!otherPV.IsMine)
            {
                int myGravies = playerIdentity.Gravies;

                if (Utility.IsFacingSameDirection(MyBody.forward, other.transform.forward, LethalDotProduct) &&
                    !Utility.IsBehind(MyBody.forward, MyBody.position, other.transform.position))
                {
                    playerDeath.KillPlayer(otherPV.OwnerActorNr); //kill self 
                }
            }

        }
    }
    #endregion
}



