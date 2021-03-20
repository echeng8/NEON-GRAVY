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
    PlayerDeath playerDeath;
    PlayerIdentity playerIdentity; 
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

                if (other.GetComponentInParent<PlayerIdentity>().Gravies > myGravies) //they eat us 
                {
                    playerDeath.KillPlayer(otherPV.OwnerActorNr); 
                }
            }

        }
    }


    #endregion
}
