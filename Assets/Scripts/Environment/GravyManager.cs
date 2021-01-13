using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using TMPro;
using UnityEngine.LowLevel;
using UnityStandardAssets.Characters.ThirdPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Creates and updates the gravies on platform
/// handles player gravy detection and awarding 
/// </summary>
public class GravyManager : MonoBehaviourPunCallbacks
{
    
    #region Gameplay Values
    
    public float gravyPercent; 
    
    #endregion
    
    #region  Implementation Values
    
    public GameObject gravyPrefab;

    /// <summary>
    /// starting number of gravies in the game 
    /// </summary>
    [HideInInspector] public int startingGravyNum;

    
    /// <summary>
    /// number of gravy's left in the game. synced across all clients
    /// </summary>
    [HideInInspector]
    public int CurrentGravyNum
    {
        get => currentGravyNum;
        set
        {
            currentGravyNum = value;
            OnGravyNumChanged.Invoke(value);
        }
    }
    private int currentGravyNum;
    public IntEvent OnGravyNumChanged = new IntEvent(); 

    
    private PlatformManager platformManager;
    private ThirdPersonCharacter playerTPC;
    
    /// <summary>
    /// index refers to children platforms. true if theres a gravy there
    /// DO NOT edit directly, use SetCustomValues["gravyArray"] instead
    /// </summary>
    private bool[] SYNC_gravyArray;
    
    
    #endregion

    #region Unity Callbacks 
    
    
    
    /// <summary>
    /// Spawns the gravies on the platforms by loading the GravyArray from PhotonCustomProperties or generating them if the client is the first master.
    /// </summary>
    public void LoadGravies()
    {        
        
        if (!PhotonNetwork.IsConnected)
            return; 
        
        platformManager = GetComponent<PlatformManager>();

        //set up listeners for landing gravy detection 
        PlayerIdentity.CallOnLocalPlayerSet(AddPlayerListeners);
        
        //generate or load gravies 
        //todo instead of checking if playercount is 1, check if its the start of a new round
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            startingGravyNum = (int)(gravyPercent * platformManager.platformNum); 
            generateGravyArray(platformManager.platformNum,startingGravyNum);
        }
        else
        {
            UpdateGravyObjects();
        }
    }

    public static int GetGravylessPlatform()
    {
        bool[] respawnPlatforms = (bool[]) PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"];
        int j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        while (respawnPlatforms[j] == true)
        {
            j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        }

        return j; 
    }
    
    #endregion
    
    #region PUN Callbacks 
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //todo optimize maybe 
        if (propertiesThatChanged.ContainsKey("gravyArray"))
        {
            UpdateGravyObjects(); 
        }
    }
    #endregion

    #region  RPC

    /// <summary>
    /// Master Client awards gravy to players after checking to see if gravy is there
    /// </summary>
    /// <param name="platNum"></param>
    /// <param name="info"></param>
    [PunRPC]
    void RPC_ProcessGravyGet(int platNum, PhotonMessageInfo info)
    {
        if (SYNC_gravyArray[platNum]) // gravy is there
        {
            //award the player
            int newGravies = (int)info.Sender.CustomProperties["gravies"] + 1;
            Hashtable h  = new Hashtable{{"gravies", newGravies}};
            info.Sender.SetCustomProperties(h); 
            
            //delete the gravy 
            removeGravy(platNum);
        }
    }
    

    #endregion

    #region Public Functions

    public Vector3 GetGraviedPlatformPosition()
    {
        for (int i = 0; i < SYNC_gravyArray.Length; i++)
        {
            if (SYNC_gravyArray[i])
            {
                return platformManager.platformParent.transform.GetChild(i).transform.position; 
            }
        }
        return Vector3.zero; 
    }
    #endregion
    #region Private Functions

    /// <summary>
    /// to be called after the player is set
    /// </summary>
    void AddPlayerListeners(GameObject localPlayer)
    {
        playerTPC = localPlayer.GetComponent<ThirdPersonCharacter>();
        playerTPC.OnLand.AddListener(CheckPlayerGetGravy);
    }
    /// <summary>
    /// updates gravy variables AND spawns or deletes grav display based on Gravy Array 
    /// updates gravyNum 
    /// </summary>
    void UpdateGravyObjects()
    {
        SYNC_gravyArray = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"]; 
        CurrentGravyNum = SYNC_gravyArray.Count(s => s == true);

        //todo remove reduncies where this is run multiple times needlessly
        for (int i = 0; i < SYNC_gravyArray.Length; i++)
        {
            GameObject platform = platformManager.platformParent.transform.GetChild(i).gameObject;
            if (SYNC_gravyArray[i] && !HasGravyDisplay(i))
            {
                GameObject gravy = Instantiate(gravyPrefab, platform.transform);
                gravy.transform.localPosition = Vector3.zero + Vector3.up * 0.33f;
            }

            if (!SYNC_gravyArray[i] && HasGravyDisplay(i))
            {
                Destroy(platform.transform.GetChild(0).gameObject); //todo get gravy with set or send signal 
            }
        }
    }
    
    /// <summary>
    /// generates a gravy array, a bool table referring to platforms with gravies on them, and sets it to
    /// room custom properties
    /// </summary>
    /// <param name="pNum"></param>
    /// <param name="gNum"></param>
    void generateGravyArray(int pNum, int gNum) 
    { 
        bool[] gravyArray = Utility.GetRandomBoolArray(pNum, gNum);
        
        Hashtable h = new Hashtable();
        h.Add("gravyArray", gravyArray);

        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
    }

    /// <summary>
    /// returns true if gravy is already displayed on that platform
    /// todo make it detect actual gravy objects insteasd of count childs
    /// </summary>
    /// <param name="childIndex"></param>
    /// <returns></returns>
    bool HasGravyDisplay(int childIndex)
    {
        return platformManager.platformParent.transform.GetChild(childIndex).childCount > 0; 
    }

    
    
    /// <summary>
    /// checks if player is getting a platform by LANDING on it, if yes send it to master client for processsing
    /// </summary>
    void CheckPlayerGetGravy()
    {
        if (SYNC_gravyArray == null || SYNC_gravyArray.Length == 0)
            return; 
        
        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        int platNum = playerTPC.PlatformBelow.transform.GetSiblingIndex();
        bool touchedGravy = SYNC_gravyArray[platNum];

        if (touchedGravy)
        {
            photonView.RPC("RPC_ProcessGravyGet", RpcTarget.MasterClient, platNum);
        }
    }
    /// <summary>
    /// removes the gravy by updating the GravyArray in setcustomproperties 
    /// </summary>
    /// <param name="platIndex"></param>
    void removeGravy(int platIndex)
    {
        SYNC_gravyArray[platIndex] = false; 
        Hashtable h = new Hashtable {{"gravyArray", SYNC_gravyArray}};
        PhotonNetwork.CurrentRoom.SetCustomProperties(h); 
    }
    #endregion
}