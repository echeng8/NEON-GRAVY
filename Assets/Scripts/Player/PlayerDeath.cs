using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;
/// <summary>
/// Handles detecting player death
/// </summary>
public class PlayerDeath : MonoBehaviourPun
{
    
    /// <summary>
    /// Triggers when the player dies.
    /// </summary>
    public UnityEvent OnDeath = new UnityEvent();

    public UnityEvent OnRevive = new UnityEvent(); 

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
    }
    
    private void Start()
    {
        GetComponent<ThirdPersonCharacter>().OnLand.AddListener(resetLastAttacker);
        GetComponent<PlayerGravity>().OnHit.AddListener(updateLastAttacker); 
    }

    // Update is called once per frame
    void ControlledUpdate()
    {
        //Checking to see if below die point
        if (transform.position.y < dieYValue) //death 
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

    [PunRPC]
    void RPC_InvokeOnDeath()
    {
        OnDeath.Invoke();
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
            Recall(); 
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

    void updateLastAttacker(int attackerNum)
    {
        lastAttacker = attackerNum; 
    }
    private void resetLastAttacker()
    {
        lastAttacker = -1; 
    }
    
    
}
