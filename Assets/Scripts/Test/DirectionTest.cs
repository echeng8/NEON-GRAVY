using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTest : MonoBehaviour
{

    public Transform obj1, obj2; 


    // Update is called once per frame
    void Update()
    {
        bool sameDir = Utility.IsFacingSameDirection(obj1.forward, obj2.forward, 0.75f);
        bool behind = Utility.IsBehind(obj1.forward, obj1.position, obj2.position); 
        print($"Same Direction: {sameDir} Behind: {behind}" ); 
    }
}
