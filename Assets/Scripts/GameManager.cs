using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate("Player", Vector3.zero + Vector3.up * 2, Quaternion.identity);
        }
        
        PlayerUserInput.localPlayerInstance.GetComponent<PlayerGravity>().OnFall.AddListener(OpRPC_ReportFall);
    }

    private void OpRPC_ReportFall(int lastAttacker)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_ReportFall", RpcTarget.All, PlayerUserInput.localPlayerInstance.photonView.Owner.ActorNumber, lastAttacker);
        }
        else
        {
            print("I died");
        }
    }

    private void RPC_ReportFall(int deadActorNumber, int killerActorNumber)
    {
        Player deadPlayer = PhotonNetwork.CurrentRoom.GetPlayer(deadActorNumber);
        if (killerActorNumber == -1)
        {
            print($"{deadPlayer.NickName} has fallen.");
        }
        else
        {
            Player killer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
            print($"{deadPlayer.NickName} was killed by {killer.NickName}");
        }
    }
    
}
