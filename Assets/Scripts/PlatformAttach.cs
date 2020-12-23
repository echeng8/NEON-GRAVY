using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlatformAttach : MonoBehaviour
{
    //Start is called before the first frame update
    private ThirdPersonCharacter tpc;
    private Vector3 previousPosition;
    private Rigidbody rb;
    private PlayerGravity pg;
    private void Awake()
    {
        tpc = GetComponent<ThirdPersonCharacter>();
        rb = GetComponent<Rigidbody>();
        pg = gameObject.GetComponent<PlayerGravity>();
    }

    private void ControlledFixedUpdate(){
        if (tpc.standPlatform != null && tpc.standPlatform.CompareTag("Platform") && pg.GetGravity())
        {
            if (previousPosition == Vector3.zero)
            {
                 return;
            } 

            rb.MovePosition(rb.position + tpc.standPlatform.transform.position - previousPosition);
        }
        if (tpc.standPlatform != null && tpc.standPlatform.CompareTag("Platform"))
        {
            previousPosition = tpc.standPlatform.transform.position;
        }
        else
        {
            previousPosition = Vector3.zero;
        }
    }
}
