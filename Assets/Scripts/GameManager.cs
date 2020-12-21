using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
/// <summary>
/// handles deaths and scoreboard and kills tracking  
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{

    public TextMeshProUGUI killFeed;


    public List<Player> leaderBoard; 
    private Player[] playerList;

    #region Unity Callbacks 

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero + Vector3.up * 2, Quaternion.identity);
        }
        
        PlayerUserInput.localPlayerInstance.GetComponent<PlayerGravity>().OnFall.AddListener(OpRPC_ReportFall);
    }

    private void Update()
    {
        //application stuff
        if (Input.GetButtonDown("Cancel"))
        {
            PhotonNetwork.LeaveRoom();
        }
    }


    #endregion

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerList = PhotonNetwork.PlayerList; 
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerList = PhotonNetwork.PlayerList; 
        
        //init player properties 
        Hashtable playerProps = new Hashtable {{"kills", 0}};
        newPlayer.SetCustomProperties(playerProps); 
    }

    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    private void OpRPC_ReportFall(int lastAttacker)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_ReportFall", RpcTarget.All, PlayerUserInput.localPlayerInstance.photonView.Owner.ActorNumber, lastAttacker);
        }
        else
        {
            SetKillFeed("I died");
        }
    }

    
    /// <summary>
    /// only on host client 
    /// </summary>
    /// <param name="deadActorNumber"></param>
    /// <param name="killerActorNumber"></param>
    [PunRPC]
    private void RPC_ReportFall(int deadActorNumber, int killerActorNumber)
    {
        Player deadPlayer = PhotonNetwork.CurrentRoom.GetPlayer(deadActorNumber);
        Player killer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        
        if (killerActorNumber == -1 || killer == null)
        {
            SetKillFeed($"{deadPlayer.NickName} has fallen.");
        }
        else
        { // process kill 

            if (PhotonNetwork.IsMasterClient)
            {
                //add 1 to player kills
                int currentPlayerKills = (int)killer.CustomProperties["kills"];
                currentPlayerKills++; 
                killer.SetCustomProperties(new Hashtable() {{"kills", currentPlayerKills}}); 
            }

            
            SetKillFeed($"{deadPlayer.NickName} was killed by {killer.NickName}");
            updateLeaderboard();
        }
    }

    void updateLeaderboard()
    {
        leaderBoard = playerList.ToList(); 
        leaderBoard.Sort(comparePlayerKills);
        
        
        foreach (Player p in leaderBoard)
        {
            print(p.NickName + p.CustomProperties["kills"]);
        }
    }

    int comparePlayerKills(Player p1, Player p2)
    {
        return (int)p1.CustomProperties["kills"] - (int)p2.CustomProperties["kills"]; 
    }

    void SetKillFeed(string s)
    {
        killFeed.text = s; 
    }
    
}
