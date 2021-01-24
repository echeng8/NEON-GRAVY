using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

[RequireComponent(typeof(TextMeshProUGUI))]
public class UpgradeAlerts : MonoBehaviour
{
    TextMeshProUGUI textMesh; 
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        PlayerIdentity.CallOnLocalPlayerSet(AddPlayerListeners); 
    }

    void AddPlayerListeners(GameObject player)
    {
        PlayerCombat pc = player.GetComponent<PlayerCombat>();

        pc.OnStrikeUnlocked.AddListener(AlertMeleeUnlock);
        pc.OnShootUnlocked.AddListener(AlertShootUnlock);
        player.GetComponent<PlayerDeath>().OnDeath.AddListener(ClearAlerts); 
    }

    void AlertMeleeUnlock()
    {
        textMesh.text = "hit"; 
    }

    void AlertShootUnlock()
    {
        textMesh.text = "hit + shoot"; 
    }

    void ClearAlerts()
    {
        textMesh.text = ""; 
    }
}
