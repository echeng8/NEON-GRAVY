using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerDeath pd; 
    
    void Start()
    {
        if (PlayerUserInput.localPlayerInstance == null)
        {
            PlayerUserInput.OnLocalPlayerSet.AddListener(AddPlayerListeners);
        }
        else
        {
            AddPlayerListeners(PlayerUserInput.localPlayerInstance.gameObject);
        }
        
        gameObject.SetActive(false);
    }


    //tries to revive the player 
    public void TryRevive()
    {
        pd.Revive();
    }
    
    
    void AddPlayerListeners(GameObject localPlayer)
    {
        if (pd == null)
        {
            pd = localPlayer.GetComponent<PlayerDeath>();
        }
        
        pd.OnDeath.AddListener(enableSelf);
        pd.OnRevive.AddListener(disableSelf);

}
    
    

    void disableSelf()
    {
        
        gameObject.SetActive(false);
    }

    void enableSelf()
    {
        gameObject.SetActive(true);
    }
    
}
