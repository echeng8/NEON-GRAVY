using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MeshFlash : MonoBehaviour
{
    Renderer meshFilter; 

    private void Start()
    {
        meshFilter = GetComponent<Renderer>();
        meshFilter.enabled = false; 
    }

    /// <summary>
    /// turns on the meshfilter and turns it back off in the given seconds
    /// </summary>
    /// <param name="seconds"></param>$
    public void Flash(float seconds)
    {
        meshFilter.enabled = true;
        this.Invoke(() => meshFilter.enabled = false, seconds); 
    }
}
