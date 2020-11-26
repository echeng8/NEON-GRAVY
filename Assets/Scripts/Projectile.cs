using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviourPun
{
    [SerializeField] float speed;
    [SerializeField] private float duration; 
    
    private Rigidbody _rigidbody;

    // Update is called once per frame
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.Invoke(() => Destroy(gameObject), duration);
    }
    
    private void FixedUpdate()
    {
        var transform1 = transform;
        _rigidbody.MovePosition(transform1.position + transform1.forward * (speed * Time.deltaTime));
        //print(speed + "   " + transform.position + "    "+ transform1.forward * (speed * Time.deltaTime) );
    }
}