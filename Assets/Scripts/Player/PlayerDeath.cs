using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Handles detecting player death, lasthit, and exposes events regarding and dying and respawning 
/// </summary>
public class PlayerDeath : MonoBehaviourPun
{
    
    /// <summary>
    /// Triggers when the player dies.
    /// </summary>
    public UnityEvent OnDeath = new UnityEvent();

    public UnityEvent OnRevive = new UnityEvent();

    public bool alive = true; 
    /// <summary>
    /// The lowest Y value that a player can have before they 'die'.
    /// </summary>
    [SerializeField] private float dieYValue;

    private Rigidbody rb;
    public int lastAttacker = -1; 

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        alive = true; 
    }
    
    private void Start()
    {
        GetComponent<ThirdPersonCharacter>().OnLand.AddListener(ResetLastAttacker);
        GetComponent<PlayerGravity>().OnHit.AddListener(UpdateLastAttacker); 
    }

    // Update is called once per frame
    void ControlledUpdate()
    {
        //Checking to see if below die point
        if (transform.position.y < dieYValue && alive) //death 
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_InvokeOnDeath", RpcTarget.All);
            }
            else
            {
                RPC_InvokeOnDeath();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Recall(); 
        }

    }

    /// <summary>
    /// calls rpc_InvokeOnRevive via network or offline
    /// </summary>
    public void Revive()
    {
        if (!alive)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_InvokeOnAlive", RpcTarget.All);
            }
            else
            {
                RPC_InvokeOnAlive();
            }

        }
    }
    
    /// <summary>
    /// sets alive and triggers onalive events 
    /// </summary>
    [PunRPC]
    void RPC_InvokeOnAlive()
    {
        alive = true; 
        Recall();
        OnRevive.Invoke();

    }
    
    
    /// <summary>
    /// sets aliv eand triggers ondeath events
    /// </summary>
    [PunRPC]
    void RPC_InvokeOnDeath()
    {
        alive = false;
        OnDeath.Invoke();
    }

    /// <summary>
    /// dying and debug teleport to back
    /// clears velocity 
    /// </summary>
    private void Recall()
    {
        transform.position = Vector3.zero;
        rb.velocity = Vector3.zero;
    }

    void UpdateLastAttacker(int attackerNum)
    {
        lastAttacker = attackerNum; 
    }
    
    private void ResetLastAttacker()
    {
        lastAttacker = -1; 
    }
    
    
}
