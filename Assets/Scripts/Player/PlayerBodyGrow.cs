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

    }
}
