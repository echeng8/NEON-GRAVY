using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// handles scoreboard: kills and gravy transfer 
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{

    #region Implementation Values

    public static GameManager instance;

    public TextMeshProUGUI killFeed;
    public TextMeshProUGUI leaderBoardDisplay;

    public List<Player> leaderBoard;
    private Player[] playerList;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetSpawn());
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

    #region Pun Callbacks

    private void OpRPC_ReportFall()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_ReportFall", RpcTarget.All,
                PlayerUserInput.localPlayerInstance.GetComponent<PlayerDeath>().lastAttacker);
        }
        else
        {
            SetKillFeed("I died");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //todo make checks 
        updateLeaderboard();
    }
    
    //room roster changes
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerList = PhotonNetwork.PlayerList; //refresh player name list
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerList = PhotonNetwork.PlayerList;
    }
    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
    #endregion

    /// <summary>
    /// only on host client 
    /// </summary>
    [PunRPC]
    private void RPC_ReportFall(int killerActorNumber, PhotonMessageInfo info)
    {
        Player deadPlayer = info.Sender;
        Player killer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);

        if (killerActorNumber == -1 || killer == null)
        {
            SetKillFeed($"{deadPlayer.NickName} has fallen.");
        }
        else
        {
            // process kill 

            int deadPlayerGravies = (int) deadPlayer.CustomProperties["gravies"];
            if (PhotonNetwork.IsMasterClient)
            {
                //add 1 to player kills
                int currentPlayerKills = (int) killer.CustomProperties["kills"];
                currentPlayerKills++;
                killer.SetCustomProperties(new Hashtable() {{"kills", currentPlayerKills}});


                //transfer gravy 
                int newKillerGravies = deadPlayerGravies +
                                   (int) killer.CustomProperties["gravies"];
                killer.SetCustomProperties(new Hashtable() {{"gravies", newKillerGravies}});
                deadPlayer.SetCustomProperties(new Hashtable() {{"gravies", 0}});
            }

            //todo make better 
            SetKillFeed($"{killer.NickName} killed {deadPlayer.NickName} for {deadPlayerGravies} gravies");
        }
    }


    //test for showing new gravies 



    void updateLeaderboard()
    {
        leaderBoard = playerList.ToList();
        leaderBoard.Sort(comparePlayerGravies);

        string lbString = "GRAVIEST KILLERS\n";
        foreach (Player p in leaderBoard)
        {
            lbString += $"{p.NickName} {p.CustomProperties["gravies"]}\n";
        }

        leaderBoardDisplay.text = lbString;
    }

    /// <summary>
    /// descending order 
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    int comparePlayerKills(Player p1, Player p2)
    {
        return (int) p2.CustomProperties["kills"] - (int) p1.CustomProperties["kills"];
    }

    int comparePlayerGravies(Player p1, Player p2)
    {
        return (int) p2.CustomProperties["gravies"] - (int) p1.CustomProperties["gravies"];
    }

    void SetKillFeed(string s)
    {
        killFeed.text = s;
    }

    IEnumerator SetSpawn()
    {
        if (PhotonNetwork.IsConnected)
        {
            while (PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"] == null) //ensures that gravyarray is loaded
            {
                yield return new WaitForSeconds(0.5f);
            }
            PhotonNetwork.Instantiate("Player", Vector3.up * 5f, Quaternion.identity); 
            
            //init local player properties 
            Hashtable playerProps = new Hashtable {{"kills", 0},{"gravies", 0}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps); 
        }

        PlayerUserInput.localPlayerInstance.GetComponent<PlayerDeath>().Spawn();
        PlayerUserInput.localPlayerInstance.GetComponent<PlayerDeath>().OnDeath.AddListener(OpRPC_ReportFall);

        playerList = PhotonNetwork.PlayerList; 
        

    }
}
