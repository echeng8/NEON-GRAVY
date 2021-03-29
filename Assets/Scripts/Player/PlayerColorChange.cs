using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
/// <summary>
/// Change the color of the player when the last 3 platforms are the same state. 
/// </summary>
public class PlayerColorChange : MonoBehaviourPunCallbacks
{
    //Photon Custom Properties 
    public PlatformState PlatState
    {
        get
        {
            return (PlatformState) Convert.ToInt32(photonView.Owner.CustomProperties["plat_state"] ); 
        } 
        set
        {
            Hashtable h = new Hashtable { { "plat_state", Convert.ToByte((int)value) } };
            photonView.Owner.SetCustomProperties(h);
        }
    }

    public PlatformStateEvent OnPlatStateChange = new PlatformStateEvent();

    /// <summary>
    /// The number of times the player has bounced on the same platform color (lastPlatState) in a row. 
    /// </summary>
    public int colorStreak;
    /// <summary>
    /// The platfrom state of the last bounce
    /// </summary>
    PlatformState lastPlatState = PlatformState.FIRE;

    #region Unity Callbacks 
    private void Start()
    {
        //initalize color of other players that have loaded
        if (!photonView.IsMine)
        {
            OnPlatStateChange.Invoke((PlatformState)((int)PlatState));
        } else //init yourself 
        {
            GetComponent<PlayerMovement>().OnBounce.AddListener(RespondToBounce);

            //init vars
            colorStreak = 0;

            GetComponent<PlayerDeath>().OnDeath.AddListener(ClearStreaks); 
        }
    }

    #endregion

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("plat_state"))
            OnPlatStateChange.Invoke(PlatState); 
    }


    #region Custom Methods 

    /// <summary>
    /// processes bouncing for color change LOCALLY 
    /// </summary>
    void RespondToBounce()
    {
        PlatformState state = GetPlatformBelowState();
        ProcessNewBounce(state); 
    }

    public PlatformState GetPlatformBelowState()
    {
        return GetComponent<PlayerMovement>().PlatformBelow.GetComponent<PlatformAppearance>().CurrentState;
    } 

    /// <summary>
    /// Updates colorStreak to reflect new bounce state.
    /// LOCAL 
    /// </summary>
    /// <param name="state"></param>
    void ProcessNewBounce(PlatformState state)
    {
        if (state == lastPlatState)
            colorStreak++;
        else
            colorStreak = 1; 

        if(colorStreak == 3)
        {
            PlatState = state; //photon custom property 
        }

        lastPlatState = state;
    }

    void ClearStreaks()
    {
        colorStreak = 0; 
    } 

    #endregion
}
