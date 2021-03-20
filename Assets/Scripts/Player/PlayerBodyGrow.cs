using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyGrow : MonoBehaviour
{

    PlayerMovement playerMovement;
    PlayerIdentity playerIdentity; 
    //starting scale of the body
    public float startScale;

    /// <summary>
    /// max scale at max top speed
    /// </summary>
    public float maxScale;




    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerIdentity = GetComponentInParent<PlayerIdentity>(); 

        playerIdentity.OnGravyChange.AddListener(UpdateScale);
        UpdateScale(playerIdentity.Gravies); 
    }

    public void UpdateScale(int currentGravyNum)
    {
        transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * maxScale, currentGravyNum / playerMovement.graviesToMaxTopSpeed); 
    }
}
