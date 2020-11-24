using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Handles shooting: input, aiming, cooldown, instantiate projectile
/// Handles damage and getting hit. 
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Implementation References
    /// </summary>
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;

    /// <summary>
    /// Gameplay Values 
    /// </summary>
    [SerializeField] private float coolDown;
    
    [SerializeField] public bool debugControlled; 
    
    private float cdTimeLeft = 0;

    private void Update()
    {
        if (!debugControlled)
            return; 
        
        //Look at Camera and shooting
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);

            shootPointPivot.LookAt(new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z));
        }

        //Shooting
        cdTimeLeft -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && cdTimeLeft <= 0)
        {
            Shoot();
            cdTimeLeft = coolDown;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage"))
        {
            //placeholder code for indicating damage
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            this.Invoke(() => GetComponentInChildren<SkinnedMeshRenderer>().enabled = true, 1f); 
        }
    }

    /// <summary>
/// this also spends one bullet
/// </summary>
    void Shoot()
    {
        Instantiate(projectile, shootingPosition.position,   Quaternion.LookRotation(shootPointPivot.forward));
    }

}
