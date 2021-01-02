using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

public class Mirror : MonoBehaviour
{
    private Vector3 hitPos;
    private Vector3 normalVector;
    private Vector3 newestVector;
    public float power;
    public void OnTriggerEnter(Collider other)
    {
        hitPos = other.ClosestPointOnBounds(transform.position);
        normalVector = hitPos - transform.position;
        normalVector.y = 0f;
        Debug.DrawRay(hitPos,normalVector*10,Color.blue);
        print(other.transform.forward);
        newestVector = other.transform.forward - 2*(Vector3.Dot(other.transform.forward,normalVector))*normalVector/Mathf.Pow(Vector3.Magnitude(normalVector),2);
        other.transform.forward = newestVector;
        other.transform.GetComponent<Rigidbody>().AddForce(newestVector*power,ForceMode.Impulse);
    }
}