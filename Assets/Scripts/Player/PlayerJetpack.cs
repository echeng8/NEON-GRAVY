using System;
using System.Collections;
using System.Collections.Generic;
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
    public float jetBoost;

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
        streaksText = GameObject.Find("Streaks").GetComponent<TextMeshProUGUI>();
    }

    public void ControlledUpdate()
    {
        if (!GetComponent<PlayerGravity>().GetGravity())
        {
            if (currentCharges > 0 && Input.GetButtonDown("Fire1"))
            {
                if (GetComponent<ThirdPersonCharacter>().PlatformBelow != null)
                {
                    Vector3 dashDirection = GetComponent<PlayerShoot>().GetComponent<PlayerShoot>().shootPointPivot.transform.forward;;
                    
                    GameObject platformBelow = GetComponent<ThirdPersonCharacter>().PlatformBelow;
                    currentCharges = currentCharges - 1;
                    float velMagnitude = Vector3.Magnitude(GetComponent<Rigidbody>().velocity);
                    GetComponent<Rigidbody>().velocity = velMagnitude * dashDirection; 
                    GetComponent<Rigidbody>().AddForce(dashDirection * jetpackRhythymForce * platformBelow.GetComponent<PlatformPower>().ChargePercentage * jetBoost, ForceMode.Impulse);
                    streak++;
                    platformBelow.GetComponent<PlatformPower>().RestartPower();
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
