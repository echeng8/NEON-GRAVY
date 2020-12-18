using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Player;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == Player && other.gameObject.GetComponent<PlayerGravity>().GetGravity())
        {
            Player.transform.parent = transform;
            print("I is on platform");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;
            print("I is off");
        }
    }
}
