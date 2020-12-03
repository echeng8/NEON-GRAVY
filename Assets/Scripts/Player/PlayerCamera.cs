using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerUserInput.localPlayerInstance == null)
        {
            PlayerUserInput.OnLocalPlayerSet.AddListener(FollowPlayer);
        }
        else
        {
            FollowPlayer(PlayerUserInput.localPlayerInstance.gameObject);
        }
    }

    void FollowPlayer(GameObject player)
    {
        GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }
}
