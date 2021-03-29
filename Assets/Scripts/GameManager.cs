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
//using UnityStandardAssets.Characters.ThirdPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// handles scoreboard to display and increments kills on killers 
/// Hides room when it is half capacity so that only code joiners can enter
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    #region Gameplay Values
    #endregion

    #region Implementation Values

    //Component References
    public static GameManager instance;
   
    public PlatformManager platformManager; //quick ref 

    //UI (todo move out?) 
    public TextMeshProUGUI killFeed;
    public TextMeshProUGUI leaderBoardDisplay;


    public List<Player> leaderBoard;
    public Player[] playerList;

    /// <summary>
    /// The person with ALL the gravies. 
    /// </summary>
    private Player _gravyKing;
    
    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        //init
        instance = this;
        platformManager = GetComponent<PlatformManager>(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        SetSpawn(); 
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

    #region RPCs
    
        /// <summary>
    /// Optional RPC report fall. Reports the fall through RPC if online, otherwise, simply set KillFeed to "i died". 
    /// </summary>
    private void ReportFallRPC()
    {
        photonView.RPC("RPC_ReportFall", RpcTarget.All,
            PlayerIdentity.localPlayerInstance.GetComponent<PlayerDeath>().lastAttacker);
    }
        
    
    /// <summary>
    /// Tell people who died.
    /// Transfer Gravy from death to killer.
    /// Reset the game when the gravy king is killed. 
    /// </summary>
    /// <param name="killerActorNumber"></param>
    /// <param name="info"></param>
    [PunRPC]
    private void RPC_ReportFall(int killerActorNumber, PhotonMessageInfo info)
    {
        Player deadPlayer = info.Sender;
        Player killer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);

        if (killerActorNumber == -1 || killer == null) // if they fell without being attacked
        {
            SetKillFeed($"{deadPlayer.NickName} is gone.");
        }
        else
        {
            //increment kill on killer 
            if (PhotonNetwork.LocalPlayer.ActorNumber == killerActorNumber)
            {
                PlayerIdentity.localPlayerInstance.Kills++; 
            }

            //tell people who died via killfeed
            SetKillFeed($"{killer.NickName} ended {deadPlayer.NickName}.");
        }  
    }


    #endregion
    
    #region Pun Callbacks

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(changedProps.ContainsKey("kills"))
            updateLeaderboard();
    }

    //room roster changes
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerList = PhotonNetwork.PlayerList; //refresh player name list

        if (PhotonNetwork.IsMasterClient)
            HideRoomIfHalfFull(); 

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerList = PhotonNetwork.PlayerList;

        if (PhotonNetwork.IsMasterClient)
            HideRoomIfHalfFull();

    }



    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
    #endregion

    #region  Private Methods

    /// <summary>
    /// Sorts players by kills and outputs the text to the leaderBoardDisplay.
    /// </summary>
    void updateLeaderboard()
    {
        leaderBoard = playerList.ToList();
        leaderBoard.Sort(ComparePlayerKills);

        string lbString = "";
        foreach (Player p in leaderBoard)
        {
            lbString += $"{p.NickName} {p.CustomProperties["kills"]}\n";
        }

        leaderBoardDisplay.text = lbString;
    }

    int ComparePlayerKills(Player p1, Player p2)
    {
        ///tood null reference here sometimes 
        if (!p1.CustomProperties.ContainsKey("kills") || !p2.CustomProperties.ContainsKey("kills"))
        {
            print("ERROR: kills not intialized");
            return 0; 
        }

        return (int) p2.CustomProperties["kills"] - (int) p1.CustomProperties["kills"];
    }

    void SetKillFeed(string s)
    {
        killFeed.text = s;
    }

    
    void SetSpawn()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
          
        //init local player properties 
        Hashtable playerProps = new Hashtable {{ "plat_state", 0 }, { "kills", 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);


        PlayerIdentity.localPlayerInstance.GetComponent<PlayerDeath>().Spawn();
        PlayerIdentity.localPlayerInstance.GetComponent<PlayerDeath>().OnDeath.AddListener(ReportFallRPC);

        playerList = PhotonNetwork.PlayerList;
    }

    void HideRoomIfHalfFull()
    {
        if (playerList.Length > PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }
    #endregion
   
}
