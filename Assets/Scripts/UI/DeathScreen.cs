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
        if (PlayerIdentity.localPlayerInstance == null)
        {
            PlayerIdentity.OnLocalPlayerSet.AddListener(AddPlayerListeners);
        }
        else
        {
            AddPlayerListeners(PlayerIdentity.localPlayerInstance.gameObject);
        }
        
        gameObject.SetActive(false);
    }


    //tries to revive the player 
    public void TryRevive()
    {
        pd.Spawn();
    }
    
    
    void AddPlayerListeners(GameObject localPlayer)
    {
        if (pd == null)
        {
            pd = localPlayer.GetComponent<PlayerDeath>();
        }
        
        pd.OnDeath.AddListener(enableSelf);
        pd.OnSpawn.AddListener(disableSelf);

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
