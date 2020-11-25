using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerController.localPlayerInstance == null)
        {
            PlayerController.OnLocalPlayerSet.AddListener(FollowPlayer);
        }
        else
        {
            FollowPlayer(PlayerController.localPlayerInstance.gameObject);
        }
    }

    void FollowPlayer(GameObject player)
    {
        GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }
}
