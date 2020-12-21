using System;
using System.Collections;
using System.Collections.Generic;
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

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Hashtable playerProps = new Hashtable {{"kills", 0}};
        newPlayer.SetCustomProperties(playerProps); 
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
            SetKillFeed($"{deadPlayer.NickName} was killed by {killer.NickName}");
            
        }
    }

    void SetKillFeed(string s)
    {
        killFeed.text = s; 
    }
    
}
