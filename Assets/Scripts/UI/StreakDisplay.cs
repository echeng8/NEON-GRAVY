using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StreakDisplay : MonoBehaviour
{
    
    TextMeshProUGUI streakText;

    //player ref
    PlayerMovement pm; 
    // Start is called before the first frame update
    void Start()
    {
        streakText = GetComponent<TextMeshProUGUI>();
        PlayerIdentity.CallOnLocalPlayerSet(initPlayer); 
    }


    void initPlayer(GameObject p)
    {
        pm = p.GetComponent<PlayerMovement>();
        p.GetComponent<PlayerColorChange>().OnColorStreakChange.AddListener(UpdateStreakCounter);
    }


    void UpdateStreakCounter(int streakNum)
    {

        streakText.text = $"{streakNum}/3";
        streakText.color = GetStreakColor(pm.GetComponent<PlayerColorChange>().GetPlatformBelowState());

    }

    Color GetStreakColor(PlatformState state)
    {
        switch (state)
        {
            case PlatformState.FIRE:
                return Color.red;
            case PlatformState.GRASS:
                return Color.green;
            case PlatformState.WATER:
                return Color.blue;
        }
        return Color.white;
    }
}
