using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(PhotonNetwork.SendRate);
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Instantiate("Player", Vector3.zero + Vector3.up * 2, Quaternion.identity);
    }
    
}
