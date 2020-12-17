using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
    
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Returns the time in milliseconds since the start of the hour in UTC timezone.
    /// todo optimize, might be laggy if called every frame 
    /// </summary>
    /// <param name="???"></param>
    /// <param name="???"></param>
    /// <returns></returns>
    public static int universalTimeMS() 
    {
        var uTime = System.DateTime.UtcNow;
        return uTime.Second * 1000 + uTime.Millisecond; 
    }

    /// <summary>
    /// Returns the lerp t value based on universalTimeMS with a given intervalTime. 
    /// </summary>
    /// <param name="intervalTime"> in seconds, only 3 decimal places/param>
    /// <returns></returns>
    // public static float getSinLerpT(float intervalTimeSeconds)
    // {
    //     int intervalTimeMS = (int)(intervalTimeSeconds * 1000); 
    //     
    //     float x = (2 * Mathf.PI / intervalTimeMS) % intervalTimeSeconds - (Mathf.PI / 2) 
    // }
}

[System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> {}
[System.Serializable] public class BoolEvent : UnityEvent<bool> {}
[System.Serializable] public class ActorsEvent : UnityEvent<int,int> {}
[System.Serializable] public class IntEvent : UnityEvent<int> {}