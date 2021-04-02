﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
/// <summary>
/// main bot component
/// controls player behavior to act as a bot 
/// </summary>
public class BotPlayer : MonoBehaviour
{

    PlayerMovement pMovement;

    private void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pMovement.OnPlatformBelowChange.AddListener(BotBounce); 
    }

    void BotBounce(GameObject platformBelow)
    {
        if(platformBelow != null)
        {
            pMovement.Bounce(GetRandomDirection()); 
        }
    } 
    void InitBotCustomProperties()
    {
        Hashtable h = new Hashtable { { "plat_state", Convert.ToByte(0) }, { "kills", 0 } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
    }
    Vector3 GetRandomDirection()
    {
        Vector2 randomVec = UnityEngine.Random.insideUnitCircle;
        return new Vector3(randomVec.x, transform.position.y, randomVec.y); 
    }
}
