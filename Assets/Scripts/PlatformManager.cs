using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime; 
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityStandardAssets.Characters.ThirdPerson;
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


    [HideInInspector] public int platformNum;
    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerUserInput.localPlayerInstance == null)
        {
            PlayerUserInput.OnLocalPlayerSet.AddListener(AddPlayerListeners);
        }
        else
        {
            AddPlayerListeners(PlayerUserInput.localPlayerInstance.gameObject);
        }

        
        platformNum = platformParent.transform.childCount;
        GetComponent<GravyManager>().LoadGravies();
    }


    

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

        //todo see if there is an error for players that are already on a platfrom when the local player joins so this does not turn on the platform that they're standing on 
        //turn on platforms when players are on them
        foreach (int pActorNums in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            
            string playerPlatHash = $"{pActorNums}plat"; 
            if (propertiesThatChanged.ContainsKey(playerPlatHash))
            {
                byte platNum = (byte)propertiesThatChanged[playerPlatHash];
                if (platNum != 255)
                {
                    TurnOnPlat(platNum); 
                }
            }
        }
        
    }

    
    /// <summary>
    /// make the platform specified by platNum brighter or something 
    /// </summary>
    /// <param name="platNum"></param>
    public void TurnOnPlat(int platNum)
    {
        platformParent.transform.GetChild(platNum).GetComponent<MeshRenderer>().materials[0].SetColor("Color_8A577280", Color.yellow);
    }

    void AddPlayerListeners(GameObject player)
    {
        player.GetComponent<ThirdPersonCharacter>().OnPlatformBelowChange.AddListener(UpdatePlayerPlatformStatus);
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
