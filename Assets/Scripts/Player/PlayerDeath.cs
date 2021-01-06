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
    private GameObject platforms;
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
    /// <summary>
    /// The last person to hit this player, by ActorNum.
    /// -1 when none. 
    /// </summary>
    private int lastAttacker = -1;
 

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        platforms = GameObject.Find("Platforms");
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
        bool[] respawnPlatforms = (bool[]) PhotonNetwork.CurrentRoom.CustomProperties["gravyArray"];
        int j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        while (respawnPlatforms[j] == true)
        {
            j = UnityEngine.Random.Range(0, respawnPlatforms.Length);
        }
        transform.position = platforms.gameObject.transform.GetChild(j).position + Vector3.up * 2;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
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
