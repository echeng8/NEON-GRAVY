using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change the color of the player when the last 3 platforms are the same state.
/// </summary>
public class PlayerColorChange : MonoBehaviour
{
    public BodyColor bodyColor; 
    
    public int platStreak;
    /// <summary>
    /// The platfrom state of the last bounce
    /// </summary>
    PlatformState lastPlatState = PlatformState.FIRE;

    private void Start()
    {
        GetComponent<PlayerMovement>().OnBounce.AddListener(RespondToBounce);

        //init 
        platStreak = 0; 
    }

    void RespondToBounce()
    {
        PlatformState state = GetPlatformBelowState();
        ProcessNewBounce(state); 
    }

    PlatformState GetPlatformBelowState()
    {
        return GetComponent<PlayerMovement>().PlatformBelow.GetComponent<PlatformAppearance>().CurrentState;
    } 

    /// <summary>
    /// Updates platStreak to reflect new bounce state
    /// </summary>
    /// <param name="state"></param>
    void ProcessNewBounce(PlatformState state)
    {
        
        if (state == lastPlatState)
            platStreak++;
        else
            platStreak = 1; 

        if(platStreak == 3)
        {
            bodyColor.SetColor(state); 
        }

        lastPlatState = state;
    }
}
