using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGravIcon : MonoBehaviour
{
    Vector3 rotationEuler;
    public void hudGravSwitch(bool gravOn)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.Rotate(new Vector3(0, 0, 180));
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
