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

    public void ControlledFixedUpdate()
    {
        if (!GetComponent<PlayerGravity>().GetGravity())
        {
            //if (CrossPlatformInputManager.GetAxis("Vertical") != 0f || CrossPlatformInputManager.GetAxis("Horizontal") != 0f)
            //{
                //timeHeld += Time.deltaTime;
                if (currentCharges > 0 && Input.GetKeyUp(KeyCode.Space))
                {
                    //timeHeld = 0;
                    currentCharges = currentCharges - 1;
                    GetComponent<Rigidbody>().AddForce(transform.forward*jetpackForce,ForceMode.Impulse);
                }
            //}
            //else
            //{
                //timeHeld = 0;
            //}
        }
        if (GetComponent<PlayerGravity>().GetGravity())
        {
            //timeHeld = 0;
            currentCharges = maxCharges;
        }
    }
}
