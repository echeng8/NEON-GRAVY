using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using TMPro; 

/// <summary>
/// Handles player appearance: color switching on gravity change
/// </summary>
public class PlayerAppearance : MonoBehaviour
{
    public Renderer head;
    public Renderer cube;

    public TextMeshPro nameTag; 

    private void Start()
    {
        GetComponent<PlayerGravity>().OnGravityChange.AddListener(ChangeColor);
        if(PhotonNetwork.IsConnected)
            nameTag.GetComponent<TextMeshPro>().text = PhotonNetwork.NickName; 
    }

    //todo draft code
    public void ChangeColor(bool gravityOn)
    {
        if (gravityOn)
        {
            head.materials[0].SetColor("_Color",Color.white);
            cube.materials[0].SetColor("_Color", Color.white);
        }
        else
        {
            head.materials[0].SetColor("_Color",Color.red);
            cube.materials[0].SetColor("_Color",Color.red);
        }
    }



}
