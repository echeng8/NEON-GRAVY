using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro; 
public class Launcher : MonoBehaviourPunCallbacks
{
    public UnityEvent OnConnectedSuccess = new UnityEvent();
    public TMP_InputField roomCodeInput;

    const String randomCharacterString = "123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #region  Photon PUN Callbacks

    public override void OnConnectedToMaster()
    {
        print("Current region is: " + PhotonNetwork.CloudRegion);  
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        OnConnectedSuccess.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 16; 

        int roomID = (int)(Random.value * 10); 
        PhotonNetwork.CreateRoom(GenerateRandomRoomCode(),roomOptions);
    }
    
    public override void OnJoinedRoom()
    {
        print($"My current room name is {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("OnJoinRoomFailed"); 
        roomCodeInput.text = "";  
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

    #region Public Methods 

    /// <summary>
    /// todo support multiple regions
    /// </summary>
    public void ConnectToRegion()
    {
        PhotonNetwork.ConnectToRegion("usw"); 
    }
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnectedAndReady)
        {
            print(roomCodeInput.text);
            print(roomCodeInput.text.Length);
            if (roomCodeInput.text != "") 
            {
                PhotonNetwork.JoinRoom(roomCodeInput.text.ToUpper()); 
            } else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
    }

    public void SetLocalPlayerDisplayName(String name)
    {
        PhotonNetwork.NickName = name; 
        PlayerPrefs.SetString("Name", name);
    }


    #endregion
    #region Private Methods

    /// <summary>
    /// returns a random string of 4 numbers
    /// </summary>
    /// <returns></returns>
    private string GenerateRandomRoomCode(int length = 4)
    {
        string code = "";
        for(int i = 0; i < length; i++)
        {
            code += randomCharacterString[Random.Range(0, randomCharacterString.Length)]; 
        }
        return code; 
        
    }
    #endregion
}
