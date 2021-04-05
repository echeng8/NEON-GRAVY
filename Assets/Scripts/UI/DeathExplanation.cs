using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System; 
public class DeathExplanation : MonoBehaviour
{
    public TextMeshProUGUI text;
    PlayerColorChange localPlayerCC;
    PlayerDeath localPlayerD;

    string originalText = "";

    private void Awake()
    {
        originalText = text.text; 
        PlayerIdentity.CallOnLocalPlayerSet(
            (GameObject playerGO) =>
            {
                localPlayerCC = playerGO.GetComponent<PlayerColorChange>();
                localPlayerD = playerGO.GetComponent<PlayerDeath>();
                localPlayerD.OnDeath.AddListener(ShowDeathText); 
            }); 
    }


    public void ShowDeathText()
    {
        if (localPlayerD.lastAttacker == -1)
        {
            text.enabled = false;
            return;
        }
        else
            text.enabled = true;

        //todo what if u fall off 

        PlatformState playerState = localPlayerCC.PlatState;
        PlatformState theirState = Utility.GetOpposingPlatState(playerState);
        print(playerState + "  " + theirState);
        text.text = originalText.Replace("OTHER", Utility.GetPlatString(theirState));
        text.text = text.text.Replace("YOURS", Utility.GetPlatString(playerState)); 
    }
}
