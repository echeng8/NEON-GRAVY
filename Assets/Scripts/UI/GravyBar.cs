using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravyBar : MonoBehaviour
{
    private GravyManager gm;
    private Slider slider; 
    private void Start()
    {
        gm = GameManager.instance.GetComponent<GravyManager>();
        slider = GetComponent<Slider>(); 
        gm.OnGravyNumChanged.AddListener(UpdateGravyBar);
    }

    void UpdateGravyBar(int currentGravyNum)
    {
        print(currentGravyNum + "  " + gm.startingGravyNum);
        slider.value = ((float)currentGravyNum / gm.startingGravyNum);
    }
}
