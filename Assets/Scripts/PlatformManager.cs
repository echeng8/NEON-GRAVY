using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime; 
using UnityEngine;
using UnityEngine.PlayerLoop;
//using UnityStandardAssets.Characters.ThirdPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// tracks who is on what platform with byte array where each element is an actor number
/// notes: byte at 255 means no actor present
/// </summary>
public class PlatformManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The parent of all platforms in the game, used for managing gravies. 
    /// </summary>
    public GameObject platformParent;
    
    /// <summary>
    /// number of children in the platform parent (ie the platforms) 
    /// </summary>
    [HideInInspector]
    public int PlatformNum => platformParent.transform.childCount;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            gameObject.SetActive(false);
            return; 
        }
        
        PlayerIdentity.CallOnLocalPlayerSet(AddPlayerListeners);
        
        GetComponent<GravyManager>().LoadGravies();
    }


    

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

        //todo see if there is an error for players that are already on a platfrom when the local player joins so this does not turn on the platform that they're standing on 
        //turn on platforms when players are on them
        foreach (int pActorNums in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            //todo maybe kill this stupid fucking thing 
            string playerPlatHash = $"{pActorNums}plat"; 
            if (propertiesThatChanged.ContainsKey(playerPlatHash))
            {
                byte platNum = (byte)propertiesThatChanged[playerPlatHash];
            }
        }
        
    }

    public GameObject GetPlatform(int index)
    {
        return platformParent.transform.GetChild(index).gameObject; 
    }

    void AddPlayerListeners(GameObject player)
    {
        player.GetComponent<PlayerMovement>().OnPlatformBelowChange.AddListener(UpdatePlayerPlatformStatus);
    }

    /// <summary>
    /// Updates the playerPlatHash property, where the key is [actorNumber]plat and the value is a byte of the platform index
    /// that the player is currently touching 
    /// </summary>
    /// <param name="newPlatform"></param>
    void UpdatePlayerPlatformStatus(GameObject newPlatform)
    {
        string playerPlatHash = $"{PhotonNetwork.LocalPlayer.ActorNumber}plat";
        byte platNum = 255; 
        if (newPlatform != null)
        {
            platNum = (byte) newPlatform.transform.GetSiblingIndex(); 
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {{playerPlatHash, platNum}}); 
        
        //let everyone know that playActorNum touched platNum
    }


}
