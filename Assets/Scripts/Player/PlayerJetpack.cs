using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events; 

/// <summary>
/// Handles player bouncing from platforms 
/// </summary>
public class PlayerJetpack : MonoBehaviour
{
    
    #region Implementation Values
    /// <summary>
    /// Normal bounce force when above platforms
    /// </summary>
    public float bounceForce;
    
    /// <summary>
    /// Bonus force for special moves or moments 
    /// </summary>
    public float bonusForce;
    public int streak;

    public TextMeshProUGUI streaksText;

    public UnityEvent OnBounce = new UnityEvent();

    private Rigidbody rb;
    
    #endregion
    
    #region Unity Callbacks
    /// <summary>
    /// The stuff that is commented out is the stuff is just in case if we want to go back to holding down a button/wasd
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 

        streak = 0;
        streaksText = GameObject.Find("Streaks").GetComponent<TextMeshProUGUI>();
    }

    public void ControlledUpdate()
    {
        if (!GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GameObject platformBelow = GetComponent<ThirdPersonCharacter>().PlatformBelow;

                if (platformBelow != null)
                {
                    Vector3 dashDirection = GetComponent<PlayerShoot>().shootPointPivot.transform.forward;

                    float velMagnitude = Vector3.Magnitude(GetComponent<Rigidbody>().velocity);
                   
                    rb.velocity = velMagnitude * dashDirection; 
                    rb.AddForce(dashDirection * bounceForce, ForceMode.Impulse);
                    streak++;

                    //calls events 
                    platformBelow.GetComponent<PlatformAppearance>().OnBounce.Invoke(); 
                    OnBounce.Invoke(); 
                }
                else
                {
                    streak = 0;
                }
            }
        }
        if (GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
        {
            streak = 0;
        }
        StreakCounter();
    }
    #endregion
    
    #region Custom Methods
    void StreakCounter()
    {
        streaksText.text = $"Streaks x{streak}";
    }
    #endregion
    
}
