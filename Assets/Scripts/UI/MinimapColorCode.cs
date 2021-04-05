using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MinimapColorCode : MonoBehaviour
{
    private Renderer ren;

    void Start()
    {
        ren = GetComponent<Renderer>();
        if (GetComponentInParent<PlayerIdentity>().IsMyPlayer())
        {
            ren.material.SetColor("_BaseColor", Color.yellow);
        }
    }
}
