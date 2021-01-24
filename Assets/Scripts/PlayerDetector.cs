using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

/// <summary>
/// keeps a running list of players in the collider in PlayersInside
/// </summary>
public class PlayerDetector : MonoBehaviourPun
{

    public List<PlayerCombat> PlayersInside;

    private void Start()
    {
        PlayersInside = new List<PlayerCombat>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return; 

        //TODO polish when the second hitbox is removed
        if(other.CompareTag("Player") && !PlayersInside.Contains(other.GetComponentInParent<PlayerCombat>()))
        {
            PlayersInside.Add(other.GetComponentInParent<PlayerCombat>()); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
            return;

        
        if (other.CompareTag("Player") && PlayersInside.Contains(other.GetComponentInParent<PlayerCombat>()))
        {

            PlayersInside.Remove(other.GetComponentInParent<PlayerCombat>());
        }
    }
}
