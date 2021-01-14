using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerJetpack : MonoBehaviour
{
    public float jetpackForce;
    public int maxCharges;
    //private float timeHeld;
    private int currentCharges;
    //public float chargeTime;
/// <summary>
/// The stuff that is commented out is the stuff is just in case if we want to go back to holding down a button/wasd
/// </summary>
    private void Start()
    {
        currentCharges = maxCharges;
    }

    public void ControlledUpdate()
    {
        if (!GetComponent<PlayerGravity>().GetGravity())
        {
            if (currentCharges > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                currentCharges = currentCharges - 1;
                GetComponent<Rigidbody>().AddForce(transform.forward*jetpackForce,ForceMode.Impulse);
            }
        }
        if (GetComponent<PlayerGravity>().GetGravity())
        {
            currentCharges = maxCharges;
        }
    }
}
