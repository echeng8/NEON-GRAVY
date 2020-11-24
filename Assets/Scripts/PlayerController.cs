using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Handles shooting: input, aiming, cooldown, instantiate projectile
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform shootPointPivot, shootingPosition;
    [SerializeField] private float coolDown;


    private void Update()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            
            shootPointPivot.LookAt(new Vector3(pointTolook.x, shootingPosition.position.y, pointTolook.z));
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(projectile, shootingPosition.position,   Quaternion.LookRotation(shootPointPivot.forward));
        }
    }


    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere();
    }
}
