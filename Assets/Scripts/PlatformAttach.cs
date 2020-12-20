using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject gameObject;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameObject.Find("Local Player") && other.gameObject.GetComponent<PlayerGravity>().GetGravity())
        {
            GameObject.Find("Local Player").transform.parent = transform;
            print("I is on platform");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameObject.Find("Local Player"))
        {
            GameObject.Find("Local Player").transform.parent = null;
            print("I is off");
        }
    }
}
