using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAppearance : MonoBehaviour
{
    private PlayerShoot ps; 
    private Animator an; 
    // Start is called before the first frame update
    void Start()
    {
        PlayerUserInput.CallOnLocalPlayerSet(InitializePlayerReference);
        an = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps == null || ps._timeToCharge == 0)
            return; 
        float chargePercent = Mathf.Clamp01(ps.SYNC_timeHeld / ps._timeToCharge);  
        an.SetFloat("ChargePercentage", chargePercent );
    }
    void InitializePlayerReference(GameObject plyr)
    {
        ps = plyr.GetComponent<PlayerShoot>();
    }
}
