using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using UnityEngine.Events; 

public class Launcher : MonoBehaviourPunCallbacks
{
    private static string gameVersion = "0";
    public UnityEvent OnConnectedSuccess = new UnityEvent();
    
    
    //test
    private double t; 
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    #region  Photon PUN Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        OnConnectedSuccess.Invoke();
        
        
        //test
        t = PhotonNetwork.Time; 
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        int roomID = (int)(Random.value * 10); 
        PhotonNetwork.CreateRoom($"TestRoom-{roomID}",roomOptions);
    }
    
    public override void OnJoinedRoom()
    {
        print($"My current room name is {PhotonNetwork.CurrentRoom.Name}");
    }


    public override void OnCreatedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }
    
    /// <summary>
    /// currently prints rooms
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("EXISTING ROOMS");
        foreach (RoomInfo roomInfo in roomList)
        {
            print(roomInfo.Name);
        }
        print("END ROOM LIST");
        
    }

    #endregion

    private void Update()
    {
        //test
        t += Time.deltaTime; 
        print(t);
    }

    #region Public Methods 


    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            
        }
    }

    public void SetLocalPlayerDisplayName(String name)
    {
        PhotonNetwork.NickName = name; 
        PlayerPrefs.SetString("Name", name);
    }
    #endregion
}
