using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlatformAttach : MonoBehaviour
{
    //Start is called before the first frame update

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Platform" && gameObject.GetComponent<PlayerGravity>().GetGravity())
        {
            transform.parent = other.transform;
            print("I is on platform");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == transform.parent)
        {
            transform.parent = null;
            print("I is off");
        }
    }
}
