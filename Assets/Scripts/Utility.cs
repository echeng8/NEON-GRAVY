using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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
    /// Returns the universal time in seconds
    /// </summary>
    /// <param name="???"></param>
    /// <param name="???"></param>
    /// <returns></returns>
    public static double UniversalTimeS()
    {
        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.Time;
        }
        else
        {
            return Time.time; 
        }
    }
}

[System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> {}
[System.Serializable] public class BoolEvent : UnityEvent<bool> {}
[System.Serializable] public class ActorsEvent : UnityEvent<int,int> {}
[System.Serializable] public class IntEvent : UnityEvent<int> {}