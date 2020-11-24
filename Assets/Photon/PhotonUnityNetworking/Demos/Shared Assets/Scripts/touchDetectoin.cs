using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchDetectoin : MonoBehaviour
{
    public bool touched = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        touched = true;
        print("hi");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
