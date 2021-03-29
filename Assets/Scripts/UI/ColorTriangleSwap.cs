using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/// <summary>
/// swaps the triangle sprite when the player changes colors
/// </summary>
[RequireComponent(typeof(Image))]
public class ColorTriangleSwap : MonoBehaviour
{

    /// <summary>
    /// Follow RGB order. 
    /// </summary>
    public Sprite[] triangleVersions = new Image[3]; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void RespondToPlayerColorChange(PlatformState state)
    {
        switch (state)
        {
            case PlatformState.FIRE:
                GetComponent<Image>().sprite = triangleVersions[0];
                break;
            case PlatformState.GRASS:
                GetComponent<Image>().sprite = triangleVersions[1];
                break; 
            case PlatformState.WATER:
                GetComponent<Image>().sprite = triangleVersions[2];
                break;
        }
    } 
}
