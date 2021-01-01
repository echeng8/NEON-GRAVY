using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Creates and updates the gravies on platform  
/// </summary>
public class GravyManager : MonoBehaviourPunCallbacks
{

    public float gravyPercent; 
    /// <summary>
    /// number of gravies in the game 
    /// </summary>
    [HideInInspector] public int gravyNum;

    [HideInInspector] public int platformNum; 

    
    public GameObject gravyPrefab;

    #region  Implementation Values

    /// <summary>
    /// number of gravy's left 
    /// </summary>
    [HideInInspector] public int currentGravyNum; 

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //todo instead of checking if playercount is 1, check if its the start of a new round
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            platformNum = transform.childCount;
            gravyNum = (int)(gravyPercent * (float)platformNum); 
            generateGravyArray(platformNum,gravyNum);
        }
        else // if you just joined 
        {
            loadGravyObjects();
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
//        loadGravyObjects();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey("gravyArray"))
            loadGravyObjects();
    }

    /// <summary>
    /// spawns the available gravy objects based on gravyArray in custom properties
    /// updates gravyNum 
    /// </summary>
    void loadGravyObjects()
    {
        bool[] gravyArray = (bool[])PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"];

        int gNum = 0; 
        for (int i = 0; i < gravyArray.Length; i++)
        {
            GameObject platform = transform.GetChild(i).gameObject;
            if (gravyArray[i] && !hasGravyDisplay(i))
            {
                gNum++; 
                GameObject gravy = Instantiate(gravyPrefab, platform.transform);
                gravy.transform.localPosition = Vector3.zero + Vector3.up * 0.33f;
            }

            if (!gravyArray[i] && hasGravyDisplay(i))
            {
                Destroy(platform.transform.GetChild(0).gameObject); //todo get gravy with set or send signal 
            }
        }

        gravyNum = gNum; 
    }

    /// <summary>
    /// returns true if gravy is already displayed on that platform
    /// todo make it detect actual gravy objects insteasd of count childs
    /// </summary>
    /// <param name="childIndex"></param>
    /// <returns></returns>
    bool hasGravyDisplay(int childIndex)
    {
        return transform.GetChild(childIndex).childCount > 0; 
    }
    
    
    
}