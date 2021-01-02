using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using TMPro;
using UnityEngine.LowLevel;
using UnityStandardAssets.Characters.ThirdPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq; 
/// <summary>
/// Creates and updates the gravies on platform  
/// </summary>
public class GravyManager : MonoBehaviourPunCallbacks, IPunObservable
{
    
    #region Gameplay Values 

    


    public float gravyPercent; 
    /// <summary>
    /// number of gravies in the game 
    /// </summary>
    [HideInInspector] public int gravyNum;

    [HideInInspector] public int platformNum;

    public GameObject platformParent;    
    public GameObject gravyPrefab;

    #endregion
    
    #region  Implementation Values

    /// <summary>
    /// number of gravy's left 
    /// </summary>
    [HideInInspector] public int currentGravyNum;

    private ThirdPersonCharacter playerTPC;
    
    /// <summary>
    /// index refers to children platforms. true if theres a gravy there
    /// DO NOT edit directly, use SetCustomValues["gravyArray"] instead
    /// </summary>
    private bool[] SYNC_gravyArray;
    #endregion

    #region Unity Callbacks 
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerUserInput.OnLocalPlayerSet.AddListener(AddPlayerListeners);

        //todo instead of checking if playercount is 1, check if its the start of a new round
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            platformNum = platformParent.transform.childCount;
            gravyNum = (int)(gravyPercent * (float)platformNum); 
            generateGravyArray(platformNum,gravyNum);
        }
    }
    
    #endregion
    
    #region PUN Callbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(SYNC_gravyArray);
        }

        if (stream.IsReading)
        {
            bool[] temp = (bool[])stream.ReceiveNext();
            SYNC_gravyArray = temp; 
            UpdateGravyObjects();
            if (!temp.SequenceEqual(SYNC_gravyArray))
            {
                return; 
            }
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
            SYNC_gravyArray[platNum] = false;
            UpdateGravyObjects(); //todo update later better place
        }
    }
    

    #endregion

    #region Private Functions

    /// <summary>
    /// to be called after the player is set
    /// </summary>
    void AddPlayerListeners(GameObject localPlayer)
    {
        playerTPC = localPlayer.GetComponent<ThirdPersonCharacter>();
        playerTPC.OnLand.AddListener(NotifyPlayerGetGravy);
    }
    /// <summary>
    /// spawns the available gravy objects based on gravyArray in custom properties
    /// updates gravyNum 
    /// </summary>
    void UpdateGravyObjects()
    {
        int gNum = 0; 
        for (int i = 0; i < SYNC_gravyArray.Length; i++)
        {
            GameObject platform = platformParent.transform.GetChild(i).gameObject;
            if (SYNC_gravyArray[i] && !HasGravyDisplay(i))
            {
                gNum++; 
                GameObject gravy = Instantiate(gravyPrefab, platform.transform);
                gravy.transform.localPosition = Vector3.zero + Vector3.up * 0.33f;
            }

            if (!SYNC_gravyArray[i] && HasGravyDisplay(i))
            {
                print(platform.name);
                print(platform.transform.GetChild(0).gameObject.name);
                Destroy(platform.transform.GetChild(0).gameObject); //todo get gravy with set or send signal 
            }
        }

        gravyNum = gNum; 
    }
    
    /// <summary>
    /// generates a gravy array, a bool table referring to platforms with gravies on them, and sets it to
    /// room custom properties.
    /// meant to be executed on the master client at round start 
    /// </summary>
    /// <param name="pNum"></param>
    /// <param name="gNum"></param>
    void generateGravyArray(int pNum, int gNum) 
    { 
        SYNC_gravyArray = Utility.GetRandomBoolArray(pNum, gNum);
        UpdateGravyObjects();
    }

    /// <summary>
    /// returns true if gravy is already displayed on that platform
    /// todo make it detect actual gravy objects insteasd of count childs
    /// </summary>
    /// <param name="childIndex"></param>
    /// <returns></returns>
    bool HasGravyDisplay(int childIndex)
    {
        return platformParent.transform.GetChild(childIndex).childCount > 0; 
    }

    void NotifyPlayerGetGravy()
    {
        if (SYNC_gravyArray == null) //todo this means you cannot get the gravy that u just spawned on 
            return; 
        
        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        int platNum = playerTPC.standPlatform.transform.GetSiblingIndex();
        bool touchedGravy = SYNC_gravyArray[platNum];

        if (touchedGravy)
        {
            photonView.RPC("RPC_ProcessGravyGet", RpcTarget.MasterClient, platNum);
        }
        
    }
    #endregion
}