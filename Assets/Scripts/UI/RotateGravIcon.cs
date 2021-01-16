using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGravIcon : MonoBehaviour
{
    Vector3 rotationEuler;
    public void hudGravSwitch(bool gravOn)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (gravOn == true)
        {
            rectTransform.rotation = Quaternion.Euler(new Vector3(rectTransform.rotation.eulerAngles.x, rectTransform.rotation.eulerAngles.y, 180));
        }
        else
        {
            rectTransform.rotation = Quaternion.Euler(new Vector3(rectTransform.rotation.eulerAngles.x, rectTransform.rotation.eulerAngles.y, 0));
        }
    }
    void AddPlayerListeners(GameObject p)
    {
        p.GetComponent<PlayerGravity>().OnGravityChange.AddListener(hudGravSwitch);
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerIdentity.CallOnLocalPlayerSet(AddPlayerListeners);
    }
}
