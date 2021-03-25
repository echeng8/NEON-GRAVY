using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Adjusts body color by plat state change on PlayColorChange
/// </summary>
public class BodyColor : MonoBehaviour
{
    public Material[] ColorsRGB;

    private void Start()
    {
        SetColor(PlatformState.WATER);
        GetComponentInParent<PlayerColorChange>().OnPlatStateChange.AddListener(SetColor); 
    }

    public void SetColor(PlatformState state)
    {
        print("trying to set color to: " + state); 
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
