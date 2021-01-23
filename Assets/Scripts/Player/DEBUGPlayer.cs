using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEBUG scripts for the player, to be removed in the final product
/// </summary>
public class DEBUGPlayer : MonoBehaviour
{

    private Rigidbody rb;

    private void Start()
    {
       rb = GetComponent<Rigidbody>(); 
    }

    void ControlledUpdate()
    {
        //Debug Stuff 
        if (Input.GetKeyDown(KeyCode.V))
        {
            print($"Velocity at Current Frame: {rb.velocity} magnitude {rb.velocity.magnitude}");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = GameManager.instance.GetComponent<GravyManager>().GetGraviedPlatformPosition() + Vector3.up * 5f; 
        }
    }
}