using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NameInput : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("Name");
    }
}
