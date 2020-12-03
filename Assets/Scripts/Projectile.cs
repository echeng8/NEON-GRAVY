using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] private float duration;

    public int shooterActorNum = -1; 
    
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        var transform1 = transform;
        _rigidbody.MovePosition(transform1.position + transform1.forward * (speed * Time.deltaTime));
    }
}