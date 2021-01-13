using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEBUG scripts for the player, to be removed in the final product
/// </summary>
public class DEBUGPlayer : MonoBehaviour
{
    void ControlledUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = GameManager.instance.GetComponent<GravyManager>().GetGraviedPlatformPosition(); 
        }
    }
}
