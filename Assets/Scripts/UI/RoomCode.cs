using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro; 

[RequireComponent(typeof(TextMeshProUGUI))]
public class RoomCode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = GetComponent<TextMeshProUGUI>().text.Replace("%code%", PhotonNetwork.CurrentRoom.Name); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
