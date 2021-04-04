using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations;
using UnityEngine.Events;

/// <summary>
/// Handles vestigal gravity toggling and Damage force
/// </summary>
public class PlayerGravity : MonoBehaviourPun
{
    #region Implementation Fields

    private bool gravity = false;
   
    //unity events
    public BoolEvent OnGravityChange = new BoolEvent();

    private Rigidbody rb;
    
    #endregion

    #region Unity Callbacks 

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //GetComponent<PlayerDeath>().OnDeath.AddListener(ResetOnDeath);
    }

    private void Start()
    {
        //todo make gravy start off by default then remove the whole friggin concept from the codebase
        photonView.RPC("RPC_SetGravity", RpcTarget.All, false); 
    }


    #endregion
    
    #region RPC and related Methods 


    /// <summary>
    /// sets gravity. if gravity is off, player is a physics object in space with previous velocity
    /// </summary>
    /// <param name="on"></param>
    [PunRPC]
    private void RPC_SetGravity(bool on)
    {
        gravity = on;
        OnGravityChange.Invoke(on);
    }
    
    #endregion

    #region Public Methods

    public bool GetGravity()
    {
        return gravity; 
    }
    

    #endregion

    #region Private Methods


    /// <summary>
    /// resets values on death, to be added to OnDeath event
    /// called for ALL clients by PlayerDeath through event triggering  
    /// </summary>
    private void ResetOnDeath()
    {
        RPC_SetGravity(false);
    }
    #endregion
    
}
