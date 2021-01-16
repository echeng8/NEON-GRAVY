using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPower : MonoBehaviour
{
    public float timeToCharge,timeStayCharged, timeStayDeactivated;

    public float ChargePercentage
    {
        get
        {
            if (timeCharged < timeStayDeactivated)
                return 0; 
            
            return Mathf.Clamp01((timeCharged - timeStayDeactivated) / timeToCharge); 

        }
    }

    /// <summary>
    /// forces the platform to go back to 0 charge 
    /// </summary>
    public void RestartPower()
    {
        timeCharged = 0; 
    }
    float timeCharged;

    // Start is called before the first frame update
    void Start()
    {
        timeCharged = 0; 
    }
    

    // Update is called once per frame
    void Update()
    {
        float colorIntensity = Mathf.Lerp(0, 255, ChargePercentage); 
        GetComponent<MeshRenderer>().materials[0].SetColor("Color_8A577280", new Color(255 - colorIntensity, colorIntensity, 0));
        timeCharged += Time.deltaTime;
        if (timeCharged > timeToCharge + timeStayCharged + timeStayDeactivated) 
        {
            timeCharged = 0; 
        }
    }
}
