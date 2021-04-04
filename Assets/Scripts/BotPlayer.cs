using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// main bot component
/// controls player behavior to act as a bot 
/// </summary>
public class BotPlayer : MonoBehaviour
{
    PlayerMovement pMovement;
    float respawnDelay = 5f;


    private void Awake()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)  
            InitBotCustomProperties(); 
    }

    private void Start()
    {
        pMovement = GetComponent<PlayerMovement>();
        pMovement.OnPlatformBelowChange.AddListener(BotBounce);
        PlayerDeath pDeath = GetComponent<PlayerDeath>();
        pDeath.OnDeath.AddListener(
            () => this.Invoke(pDeath.Spawn, respawnDelay)
            );
        
    }
    

    void BotBounce(GameObject platformBelow)
    {
        if(platformBelow != null)
        {
            pMovement.Bounce(GetTopPlayerDirection()); 
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
        return new Vector3(randomVec.x, 0, randomVec.y); 
    }

    Vector3 GetTopPlayerDirection()
    {
        List<Player> players = GameManager.instance.leaderBoard;
        if (players == null)
        {
            return GetRandomDirection();
        }
        Vector3 topPlayerPosition = (players[0].TagObject as GameObject).transform.position;
        Vector2 topPlayerDirection = new Vector2(topPlayerPosition.x - transform.position.x,topPlayerPosition.z - transform.position.z).normalized;
        return new Vector3(topPlayerDirection.x,0,topPlayerDirection.y);
    }
}

