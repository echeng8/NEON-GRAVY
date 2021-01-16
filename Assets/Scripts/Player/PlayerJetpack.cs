using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerJetpack : MonoBehaviour
{
    public float jetpackForce;
    public float jetpackRhythymForce;
    public int maxCharges;
    //private float timeHeld;
    private int currentCharges;

    public int streak;

    public TextMeshProUGUI streaksText;
    //public float chargeTime;
/// <summary>
/// The stuff that is commented out is the stuff is just in case if we want to go back to holding down a button/wasd
/// </summary>
    private void Start()
    {
        currentCharges = maxCharges;
        streak = 0;
    }

    public void ControlledUpdate()
    {
        if (!GetComponent<PlayerGravity>().GetGravity())
        {
            if (currentCharges > 0 && Input.GetKeyDown(KeyCode.Space))
            {
                if (GetComponent<ThirdPersonCharacter>().PlatformBelow != null && GetComponent<ThirdPersonCharacter>().PlatformBelow.GetComponent<PlatformPower>().ChargePercentage >= 1)
                {
                    currentCharges = currentCharges - 1;
                    float velMagnitude = Vector3.Magnitude(GetComponent<Rigidbody>().velocity);
                    GetComponent<Rigidbody>().velocity = velMagnitude * transform.forward;
                    GetComponent<Rigidbody>().AddForce(transform.forward * jetpackRhythymForce, ForceMode.Impulse);
                    streak++;
                }
                else
                {
                    currentCharges = currentCharges - 1;
                    GetComponent<Rigidbody>().AddForce(transform.forward * jetpackForce, ForceMode.Impulse);
                    streak = 0;
                }
            }
        }
        if (GetComponent<PlayerGravity>().GetGravity())
        {
            currentCharges = maxCharges;
            streak = 0;
        }

        StreakCounter();

    }
    void StreakCounter()
    {
        streaksText.text = $"Streaks x{streak}";
    }

}
