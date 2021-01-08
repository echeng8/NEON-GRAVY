using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

/// <summary>
/// tracks who is on what platform with byte array where each element is an actor number
/// notes: byte at 255 means no actor present
/// </summary>
public class PlatformManager : MonoBehaviour
{
    
    /// <summary>
    /// The parent of all platforms in the game, used for managing gravies. 
    /// </summary>
    public GameObject platformParent;
    
    
    [HideInInspector] public int platformNum;
    // Start is called before the first frame update
    void Awake()
    {
        platformNum = platformParent.transform.childCount;
        GetComponent<GravyManager>().LoadGravies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
