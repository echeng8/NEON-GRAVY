using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FreezeFrame : MonoBehaviour
{
    public RenderTexture freezeFrameTexture;
    public IEnumerator Freezing (float seconds)
    {
        Camera.main.targetTexture = freezeFrameTexture;

        yield return new WaitForSeconds(seconds);

        Camera.main.targetTexture = null;
    }

    public void FreezeCamera()
    {
        StartCoroutine(Freezing(3));
    }
}
