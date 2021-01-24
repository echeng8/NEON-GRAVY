using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAppearance : MonoBehaviour
{
    /// <summary>
    /// If true, the charge percentage would be bound to the player game object that is in the parent. This is to allowed networked charge appearances (e.g. laser).
    /// If this is false, then this binds to the local player (e.g. UI elements).
    /// </summary>
    //[SerializeField] private bool bindToParentPlayer; 
    private PlayerCombat playerCombat; 
    private Animator an; 
    // Start is called before the first frame update
    void Start()
    {
        if (transform.root.CompareTag("Player")) // binds to parent player 
        {
            playerCombat = GetComponentInParent<PlayerCombat>();
        }
        else //binds to local player
        {
            PlayerIdentity.CallOnLocalPlayerSet(InitializePlayerReference);
        }
   
        an = GetComponent<Animator>();
    }

    //todo change the appearance based on the Streakss or somethin 
    // Update is called once per frame
    // void Update()
    // {
    //     if (playerCombat == null || playerCombat._timeToCharge == 0)
    //         return; 
    //     float chargePercent = Mathf.Clamp01(playerCombat.SYNC_timeHeld / playerCombat._timeToCharge);  
    //     an.SetFloat("ChargePercentage", chargePercent );
    // }
    void InitializePlayerReference(GameObject plyr)
    {
        playerCombat = plyr.GetComponent<PlayerCombat>();
    }
}
