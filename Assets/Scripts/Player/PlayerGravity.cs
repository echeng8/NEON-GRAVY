using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityStandardAssets.Characters.ThirdPerson;

/// <summary>
/// Handles player gravity user toggling AND damage handling
/// todo separate damage recording and death functionality into other script 
/// </summary>
public class PlayerGravity : MonoBehaviourPun
{

    #region Gameplay Fields

    /// <summary>
    /// force added on impulse when player is hit 
    /// </summary>
    [SerializeField] private float hitForce;

    /// <summary>
    /// hits do not exceed this velocity regardless of force
    /// </summary>
    [SerializeField] private float maxVelocity; 

    /// <summary>
    /// Seconds the player is invulnerable after being hit. 
    /// </summary>
    [SerializeField] private float hitInvulSec;

    /// <summary>
    /// The lowest Y value that a player can have before they 'die'.
    /// </summary>
    [SerializeField] private float dieYValue;

    /// <summary>
    /// The times you must be hit before you go Grav Off by force. 
    /// </summary>
    [SerializeField] private int gravDurability;

    /// <summary>
    /// The time in seconds when your gravity is off after being hit gravDurabilty times. 
    /// </summary>
    [SerializeField] private float gravBrokenTime;
    
    
    #endregion

    #region Implementation Fields

    private bool gravity = true;
    
    /// <summary>
    /// cannot be hit, even if grav off
    /// </summary>
    private bool hitInvulnerable = false;

    //unity events
    public BoolEvent OnGravityChange = new BoolEvent();

    /// <summary>
    /// Triggers when the player recalls.
    /// Passes the player's actor number and the actor number of their last attacker if available, otherwise -1
    /// </summary>
    public IntEvent OnFall = new IntEvent();
    
    /// <summary>
    /// The last person to hit this player, by ActorNum.
    /// -1 when none. 
    /// </summary>
    private int lastAttacker = -1;

    /// <summary>
    /// underlying value of CurrentDurabillity property 
    /// </summary>
    private int _currentDurability = 0;

    /// <summary>
    /// set: limits to zero or greater, calls RPC with each set 
    /// 
    /// </summary>
    public int CurrentDurability
    {
        get => _currentDurability;
        set
        {
            _currentDurability = Mathf.Clamp(value, 0, int.MaxValue);
            durabilityDisplay.text = _currentDurability.ToString(); 
        }
    }


    public TextMeshPro durabilityDisplay; 
    private Rigidbody rb;
    
    #endregion

    #region Unity Callbacks 

    
private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        CurrentDurability = gravDurability; 
        
        //placeholder, durabiilty text not final todo refactor 
        durabilityDisplay.text = gravDurability.ToString(); 
    }

    private void Start()
    {
        GetComponent<ThirdPersonCharacter>().OnLand.AddListener(resetLastAttacker);
    }

    /// <summary>
    /// its update but it only calls when you're the local player OR set so in inspector
    /// playercontroller calls it 
    /// </summary>
    void ControlledUpdate()
    {
        //disables gravity on input 
        if (Input.GetButtonDown("Fire2") && CurrentDurability > 0)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_SetGravity", RpcTarget.All, !gravity);
            }
            else
            {
                RPC_SetGravity(!gravity);
            }
        }

        //clamp velocity.y to negative or 0 
        if (!gravity)
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, float.MinValue, 0f), rb.velocity.z);

        
        //Checking to see if below die point
        if (transform.position.y < dieYValue) //death 
        {
            OnFall.Invoke(lastAttacker);
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_SetDurability", RpcTarget.All, gravDurability);
            }
            Recall();
        }


        //Debug Stuff 
        if (Input.GetKeyDown(KeyCode.V))
        {
            print($"Velocity at Current Frame: {rb.velocity} magnitude {rb.velocity.magnitude}");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Recall(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) //following code is only for the local client  
            return;

        //handle damage
        if (other.CompareTag("Damage"))
        {
            //todo polish
            bool isMyBullet = PhotonNetwork.LocalPlayer.ActorNumber == other.GetComponent<Projectile>().shooterActorNum;
            
            if (!hitInvulnerable && !isMyBullet)
            {
                
                //todo move to projectileProfiles format 
                int damageEndured = other.GetComponent<Projectile>().getDamage(); 
                photonView.RPC("RPC_ProcessHit", RpcTarget.All, damageEndured, other.GetComponent<Projectile>().shooterActorNum);
                
                if (!gravity)
                {
                    Vector3 hitDirection = other.transform.forward; //todo get force from projectile 
                    ApplyHitForce(hitDirection);
                }
            }
        }
    }

    //to trigger gravity changes in debug 
    private void OnValidate()
    {
        //!!!this will call gravity change twice if something else was edited
        OnGravityChange.Invoke(gravity);
    }
    #endregion
    
    #region RPC and related Methods 

    /// <summary>
    /// RPC_RecordHit does three thing 
    /// 1) tells other clients who hit them
    /// 2) handles durability damage locally and updates peers 
    /// 3) processes hit invulernability
    /// </summary>
    /// <param name="attackerNum"></param>b
    [PunRPC]
    void RPC_ProcessHit(int damageEndured, int attackerNum = -1)
    {
        if (attackerNum != -1)
        {
            lastAttacker = attackerNum;
            
        }

        if (gravity)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_SetDurability", RpcTarget.All, CurrentDurability - damageEndured);
                ProcessDurabilityDamage();
            }
        }
        else
        {
            //process hit invulnerability 
            hitInvulnerable = true;
            this.Invoke(() => hitInvulnerable = false, hitInvulSec);
        }
    }

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

    [PunRPC]
    private void RPC_SetDurability(int d)
    {
        CurrentDurability = d; 
    }
    
    /// <summary>
    /// Handles recovery after the player has had their gravity broken. 
    /// </summary>
    [PunRPC]
    private void RPC_RecoverGravity()
    {
        //todo onrecover 
        CurrentDurability = gravDurability;
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
    /// applies impulse force on the player towards hit direction
    /// based on hits
    /// </summary>
    /// <param name="hitDirection"></param>
    private void ApplyHitForce(Vector3 hitDirection)
    {
        float hitForce = this.hitForce;

        Vector3 hitDirSameY = new Vector3(hitDirection.x, 0, hitDirection.z);
        Vector3 forceDirection = (hitDirSameY).normalized * hitForce;

        forceDirection += GetComponent<Rigidbody>().velocity;
        
        rb.AddForce(forceDirection, ForceMode.Impulse);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    private void resetLastAttacker()
    {
        lastAttacker = -1; 
    }

    /// <summary>
    /// dying and debug teleport to back
    /// clears velocity 
    /// </summary>
    private void Recall()
    {
        transform.position = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    /// <summary>
    /// Handles the durability break. Disables gravity and re-enables it when ready.
    /// Calls RPC_SetGravity and RPC_RecoverGravity
    /// </summary>
    private void ProcessDurabilityDamage()
    {
        if (_currentDurability <= 0)
        {
            photonView.RPC("RPC_SetGravity", RpcTarget.All, false);
            this.Invoke(() => photonView.RPC("RPC_RecoverGravity", RpcTarget.All), gravBrokenTime);
        }
    }
    
    #endregion


}
