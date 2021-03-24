using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Adjusts body color by platform change 
/// </summary>
public class BodyColor : MonoBehaviour
{

    private void Start()
    {
        SetColor(PlatformState.GRASS); 
    }
    public Material[] ColorsRGB; 

    public void SetColor(PlatformState state)
    {
        switch (state)
        {
            case PlatformState.FIRE:
                GetComponent<MeshRenderer>().material = ColorsRGB[0]; 
                break;
            case PlatformState.WATER:
                GetComponent<MeshRenderer>().material = ColorsRGB[2]; 
                break;
            case PlatformState.GRASS:
                GetComponent<MeshRenderer>().material = ColorsRGB[1]; 
                break;
        }
    }
}
