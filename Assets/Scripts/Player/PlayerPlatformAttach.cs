using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Events;

/// <summary>
/// This attaches the player to moving platforms
/// </summary>
public class PlayerPlatformAttach : MonoBehaviour
{
    //Start is called before the first frame update
    private ThirdPersonCharacter tpc;
    private Vector3 previousPosition;
    private PlayerGravity pg;
    private void Awake()
    {
        tpc = GetComponent<ThirdPersonCharacter>();
        pg = gameObject.GetComponent<PlayerGravity>();
    }

    private void ControlledLateUpdate()
    {
        //checks current platform its if moving, will change position based on platform movement
        if (tpc.PlatformBelow != null && tpc.PlatformBelow.GetComponent<Animator>() != null && pg.GetGravity())
        {
            if (previousPosition == Vector3.zero)
            {
                return;
            }
            
            transform.position += tpc.PlatformBelow.transform.position - previousPosition;
        }
    }

    private void ControlledUpdate()
    {
        //checks current platform if its moving, bases position on platform if true bases position on (0,0,0) if false, does not change player position
        if(tpc.PlatformBelow != null && tpc.PlatformBelow.GetComponent<Animator>() != null)
        {
            previousPosition = tpc.PlatformBelow.transform.position;
        }
        else
        {
            previousPosition = Vector3.zero;
        }
    }
}