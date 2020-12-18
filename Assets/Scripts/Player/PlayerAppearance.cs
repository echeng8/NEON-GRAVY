using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using TMPro; 

/// <summary>
/// Handles player appearance: color switching on gravity change, usernames 
/// </summary>
public class PlayerAppearance : MonoBehaviourPun
{
    public Renderer head;
    public Renderer cube;

    public TextMeshPro nameTag; 

    private void Start()
    {
        GetComponent<PlayerGravity>().OnGravityChange.AddListener(ChangeColor);
        if(PhotonNetwork.IsConnected)
            nameTag.GetComponent<TextMeshPro>().text = photonView.Owner.NickName; 
    }

    //todo draft code
    public void ChangeColor(bool gravityOn)
    {
        if (gravityOn)
        {
            head.materials[0].SetColor("_BaseColor",Color.white);
            cube.materials[0].SetColor("_BaseColor", Color.white);
        }
        else
        {
            head.materials[0].SetColor("_BaseColor",Color.red);
            cube.materials[0].SetColor("_BaseColor",Color.red);
        }
    }



}
