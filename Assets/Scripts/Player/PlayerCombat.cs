using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events; 
using UnityEngine.Animations;

/// <summary>
/// Handles player shooting: input, aiming, cooldown, instantiate projectile
/// also melee now 
/// </summary>
public class PlayerCombat : MonoBehaviourPunCallbacks
{
    #region Gameplay Fields
    /// <summary>
    /// Seconds until attack is ready 
    /// </summary>
    public float attackCooldown;

    /// <summary>
    /// TODO 
    /// How many gravies the player must have before they unlock the melee or ranged 
    /// </summary>
    public int graviesMeleeUnlock, graviesRangedUnlock; 

    /// <summary>
    /// velocity added when player is hit 
    /// the player's direction is reset to the attack's direction 
    /// </summary>
    [SerializeField] private float hitVelocity;

    #endregion

    #region Implementation References

    /// <summary>
    /// the forward distance from the spawning position that the projectile is spawned in 
    /// </summary>


    [SerializeField] private float forwardProjectileOffset;

    //references
    [SerializeField] private GameObject projectile;
    [SerializeField] public Transform shootPointPivot, shootingPosition;
    [SerializeField] public PlayerDetector meleeRangeCollider;
    


    private Rigidbody rb;  

    private Vector3 _lookAtPosition;

   
    /// <summary>
    /// If attack is not on cooldown. 
    /// </summary>
    public bool CanAttack 
    {
        get => currentAttackCooldown <= 0; 
    }
    [HideInInspector] public float currentAttackCooldown = 0;

    //whether the player has unlocked shooting or striking 

    #region Unlocking Strike and Shoot and its Events 
    public bool CanStrike
    {
        get => _canStrike;
        set
        {
            if (value == true && _canStrike == false)
            {
                OnStrikeUnlocked.Invoke();
            }
            _canStrike = value;
        }
    }

    public bool CanShoot
    {
        get => _canShoot;
        set
        {
            if (value == true && _canShoot == false)
            {
                OnShootUnlocked.Invoke();
            }
            _canShoot = value;
        }
    }
    bool _canStrike = false;
    bool _canShoot = false; 
    public UnityEvent OnStrikeUnlocked = new UnityEvent();
    public UnityEvent OnShootUnlocked = new UnityEvent();

    #endregion

    //events

    /// <summary>
    /// Invoked whenever a player attacks. networked
    /// </summary>
    public UnityEvent OnAttack = new UnityEvent();

    /// <summary>
    /// Invoked whenever a player attacks with the shoot buff active. networked
    /// </summary>
    public UnityEvent OnShoot = new UnityEvent();

    /// <summary>
    /// passes the actor number of the attacker 
    /// Triggers when the player recalls.
    /// </summary>
    public IntEvent OnAttacked = new IntEvent();

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        CanShoot = false;
        CanStrike = false; 
    }

    private void ControlledUpdate()
    {
        //AIMING 
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);     //set look at position from mouse camera position 
        Plane groundPlane = new Plane(Vector3.up, shootPointPivot.position); 
        float rayLength;
        
        Debug.DrawRay(shootingPosition.position, Vector3.up * 3, Color.yellow);
        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            
            Debug.DrawRay(pointTolook, (pointTolook - shootPointPivot.position).normalized * 2f);
            shootPointPivot.LookAt(pointTolook);
        }


        //ATTACK INPUT DETECTION
        if (Input.GetButtonDown("Fire2") && currentAttackCooldown  <= 0) //firing 
        {
            AttackRPC();
            currentAttackCooldown = attackCooldown; 
        }

        if(currentAttackCooldown > 0)
           currentAttackCooldown -= Time.deltaTime; 
        
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.IsMine)
            return; 

        //detecting bullet hits
        if (other.CompareTag("Damage"))
        {
            //todo polish
            bool isMyBullet = PhotonNetwork.LocalPlayer.ActorNumber == other.GetComponent<Projectile>().shooterActorNum ||
                                other.GetComponent<Projectile>().shooterActorNum == -1;

            if (!isMyBullet)
            {
                print("I" + PhotonNetwork.LocalPlayer.ActorNumber + " was hit by bullet from " + other.GetComponent<Projectile>().shooterActorNum); 
                BeHitRPC(other.GetComponent<Projectile>().shooterActorNum, other.transform.forward);
            }
        }
    }
    #endregion

    #region RPC and Associated Private Methods

    /// <summary>
    /// Attacks with melee by applying hit force against everything in attackCollider
    /// If active, also shoots projectile
    /// </summary>
    void AttackRPC()
    {
        //waste, 3 rpc's in one function, combine into 1 ? 

        if (!CanAttack)
            return; 

        //melee
        if (CanStrike)
        {
            InvokeOnAttackRPC();
            
            foreach (PlayerCombat playerC in meleeRangeCollider.PlayersInside)
            {

                playerC.BeHitRPC(PhotonNetwork.LocalPlayer.ActorNumber, shootPointPivot.transform.forward);
            }
        }



        if (CanShoot)
            ShootRPC();
    }

    /// <summary>
    /// Pushes the player back and tracks the lastAttacker. calls RPC's
    /// for melee
    /// </summary>
    public void BeHitRPC(int attackerActorNum, Vector3 hitDirection)
    {
        //event trigger
        photonView.RPC("RPC_InvokeOnAttacked", RpcTarget.All, attackerActorNum); 

        //apply hit velocity
        Vector3 hitDirSameY = new Vector3(hitDirection.x, 0, hitDirection.z);
        Vector3 newVelocity = (hitDirSameY).normalized * (rb.velocity.magnitude + hitVelocity);

        GetComponent<PlayerMoveSync>().UpdateMovementRPC(newVelocity, transform.position);
    }

    /// <summary>
    /// shoots
    /// todo optimize by using a single byte for radians rotation on y axis? 
    /// </summary>
    [PunRPC]
    void RPC_SpawnProj(Vector3 position, Vector3 direction, PhotonMessageInfo info = new PhotonMessageInfo())
    {
        //print("Spawning projectile at " + position + info.ToString()); 
        GameObject p = Instantiate(projectile, position, Quaternion.LookRotation(direction));
        p.GetComponent<Projectile>().shooterActorNum = PhotonNetwork.IsConnected ? info.Sender.ActorNumber : -1;

        OnShoot.Invoke(); 
    }

    /// <summary>
    /// Shoots projectile from offset   shootingPosition, calls rpc 
    /// </summary>
    void ShootRPC()
    {
        //set spawn position to projSpawn
        Vector3 projSpawn = shootingPosition.position;
        projSpawn += shootingPosition.forward * forwardProjectileOffset; //adds offset

        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_SpawnProj", RpcTarget.AllViaServer, projSpawn, shootPointPivot.forward);
        }
        else
        {
            RPC_SpawnProj(projSpawn, shootPointPivot.forward);
        }
    }

    void InvokeOnAttackRPC()
    {
        photonView.RPC("RPC_InvokeOnAttack", RpcTarget.All); 
    }

    [PunRPC]
    void RPC_InvokeOnAttack()
    {
        OnAttack.Invoke(); 
    }

    [PunRPC]
    void RPC_InvokeOnAttacked(int attackerNum)
    {
        OnAttacked.Invoke(attackerNum); 
    }
    #endregion

    #region Pun Callbacks

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(changedProps.ContainsKey("gravies"))
        {
          
            CanShoot = (int)changedProps["gravies"] >= graviesRangedUnlock;
        
            CanStrike = (int)changedProps["gravies"] >= graviesMeleeUnlock;
        }
    }

    #endregion

}
