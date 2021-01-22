using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

/// <summary>
/// keeps a running list of players in the collider in PlayersInside
/// </summary>
public class PlayerDetector : MonoBehaviourPun
{

    public List<GameObject> PlayersInside = new List<GameObject>(); 

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return; 

        if(other.CompareTag("Player") && !PlayersInside.Contains(other.gameObject))
        {
            PlayersInside.Add(other.gameObject); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine)
            return;

        if (other.CompareTag("Player") && PlayersInside.Contains(other.gameObject))
        {
            PlayersInside.Remove(other.gameObject);
        }
    }
}
