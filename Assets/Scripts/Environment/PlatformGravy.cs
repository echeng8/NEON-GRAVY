using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives players who land on the platform gravy 
/// </summary>
public class PlatformGravy : MonoBehaviour, IPlatformPlayerCallbacks
{

    public bool hasGravy = false;
    /// <summary>
    /// sendmessage from ThirdPersohCharacter
    /// </summary>
    public void OnLocalPlayerLand()
    {
        print(name);
    }

    public void OnLocalPlayerLeave()
    {
        print(name + name);
    }
}
