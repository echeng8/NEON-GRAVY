using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player appearance: color switching on gravity change
/// </summary>
public class PlayerAppearance : MonoBehaviour
{
    public Renderer Head;

    private void Start()
    {
        GetComponent<PlayerGravity>().OnGravityChange.AddListener(ChangeColor);   
    }

    //todo draft code
    public void ChangeColor(bool gravityOn)
    {
        if (gravityOn)
        {
            Head.materials[0].SetColor("_Color",Color.white);
        }
        else
        {
            Head.materials[0].SetColor("_Color",Color.red);
        }
        
    }
}
