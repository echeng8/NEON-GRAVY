using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
//using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Handles detecting player death, lasthit, and exposes events regarding and dying and respawning 
/// </summary>
public class PlayerDeath : MonoBehaviourPun
{
  
    #region Unity Events
    /// <summary>
    /// Triggers when the player dies.
    /// </summary>
    public UnityEvent OnDeath = new UnityEvent();

    /// <summary>
    /// networked event that triggers when players spawned
    /// </summary>
    public UnityEvent OnSpawn = new UnityEvent();

    #endregion
    
    #region Implementation Values
    
    [HideInInspector] public bool alive = false; 


    /// <summary>
    /// The lowest Y value that a player can have before they 'die'.
    /// </summary>
    [SerializeField] private float dieYValue;
    
    /// <summary>
    /// The maximum/minimum X value that a player can have before they 'die'.
    /// </summary>
    [SerializeField] private float dieXValue;
    
    /// <summary>
    /// The maximum/minimum Z value that a player can have before they 'die'.
    /// </summary>
    [SerializeField] private float dieZValue;

    /// <summary>
    /// the displacement above the platform that the player spawns from 
    /// </summary>
    public float spawnYOffset; 
    private Rigidbody rb;
    /// <summary>
    /// The last person to hit this player, by ActorNum.
    /// -1 when none. 
    /// </summary>
    public int lastAttacker = -1;
    
    #endregion 

    #region Unity Callbacks
    // Start is called before the first frame update
    void Awake()
    {
        alive = false;
        rb = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        OnSpawn.AddListener(ResetLastAttacker);
    }

    // Update is called once per frame
    void ControlledUpdate()
    {
        //Checking to see if below die point
        if (transform.position.y < dieYValue && alive) //death 
        {
            KillPlayer();
            transform.position = new Vector3(1000,transform.position.y,1000); //todo pan camera or something
        }
        if ((transform.position.x < GameObject.Find("MiniMapCamera").transform.position.x-dieXValue || transform.position.x > GameObject.Find("MiniMapCamera").transform.position.x+dieXValue) && alive) //death 
        {
            KillPlayer();
            transform.position = new Vector3(1000,transform.position.y,1000); //todo pan camera or something
        }
        
        if ((transform.position.z < GameObject.Find("MiniMapCamera").transform.position.z-dieZValue || transform.position.z > GameObject.Find("MiniMapCamera").transform.position.z+dieZValue) && alive) //death 
        {
            KillPlayer();
            transform.position = new Vector3(1000,transform.position.y,1000); //todo pan camera or something
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            KillPlayer(); 
        }

    }
    #endregion

    #region Spawn/Respawn Methods
    /// <summary>
    /// calls rpc_InvokeOnRevive via network or offline
    /// </summary>
    public void Spawn()
    {

        if (!alive)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_SpawnPlayer", RpcTarget.All, GetSpawnLocation());
            }
            else
            {
                RPC_SpawnPlayer(GetSpawnLocation());
            }
        }
    }
    
    /// <summary>
    /// returns vector3 of a gravyless platform
    /// </summary>
    private Vector3 GetSpawnLocation()
    {
        Vector3 spawnLoc = Vector3.zero;
        if (!PhotonNetwork.IsConnected) 
            spawnLoc = Vector3.zero;
        else
        {
            Transform platformParent = GameManager.instance.GetComponent<PlatformManager>().platformParent.transform;
            int platIndex = Random.Range(0, platformParent.childCount);
            spawnLoc = platformParent.GetChild(platIndex).position + Vector3.up * spawnYOffset;
        }

        return spawnLoc; 
    }
    
    /// <summary>
    /// sets alive, moves player to spawn, triggers onalive events 
    /// </summary>
    [PunRPC]
    void RPC_SpawnPlayer(Vector3 spawnLocation)
    {
        alive = true; //todo move custom properties? 
        OnSpawn.Invoke();

        if(photonView.IsMine)
        {
            GetComponent<PlayerMoveSync>().UpdateMovementRPC(Vector3.zero, spawnLocation);
        }
    }

    #endregion
    
    #region Death Methods
    /// <summary>
    /// Kills self. 
    /// Note: the optional argument is the lastattacker, not the player to be killed. 
    /// </summary>
    /// <param name="lastAttacker"></param>
    public void KillPlayer(int lastAttacker = -1)
    {
        if (alive)
        {
            if (lastAttacker != -1)
                UpdateLastAttacker(lastAttacker);

            
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_KillPlayer", RpcTarget.All);
            }
            else
            {
                RPC_KillPlayer();
            }
        }

    }
    
    
    /// <summary>
    /// sets alive and triggers ondeath events
    /// </summary>
    [PunRPC]
    void RPC_KillPlayer()
    {
        alive = false;
        OnDeath.Invoke();
    }


    #endregion

    #region Last Attacker Methods
    void UpdateLastAttacker(int attackerNum)
    {
        lastAttacker = attackerNum; 
    }
    
    private void ResetLastAttacker()
    {
        lastAttacker = -1; 
    }
    #endregion
    
}
