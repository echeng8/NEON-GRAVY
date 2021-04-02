using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles player appearance: color switching on gravity change, usernames 
/// </summary>
public class PlayerAppearance : MonoBehaviourPun
{
    public TextMeshPro nameTag;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && !GetComponent<PlayerIdentity>().isBot)
        {
            nameTag.GetComponent<TextMeshPro>().text = photonView.Owner.NickName;
        }

    }
}

