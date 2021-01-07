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
    private GameObject platforms;
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
    /// <summary>
    /// The last person to hit this player, by ActorNum.
    /// -1 when none. 
    /// </summary>
    public int lastAttacker = -1;
 

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
<<<<<<< HEAD
        alive = true; 
=======
        platforms = GameObject.Find("Platforms");
>>>>>>> 144db0991a44ad6852ada7da34f894885a367bc9
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
    
    private GameObject platforms;
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
        bool[] respawnPlatforms = (bool[]) PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"];
        int j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        while (respawnPlatforms[j] == true)
        {
            j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        }
        transform.position = platforms.gameObject.transform.GetChild(j).position + Vector3.up * 2;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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
