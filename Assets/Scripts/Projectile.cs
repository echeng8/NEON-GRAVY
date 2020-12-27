using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Gameplay Values
    [SerializeField] float speed;
    [SerializeField] private float duration;

    public int gravOffDamage, gravOnDamage; // durability damage when the bullet is shot grav on or grav off 
    
    #endregion

    #region  Implementation Values

    public int shooterActorNum = -1; 
    
    private Rigidbody _rigidbody;
    private bool shotWithGravOff;
    public bool ShotWithGravOff => shotWithGravOff;

    #endregion

    #region  Unity Callbacks

    
    // Update is called once per frame
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.Invoke(() => Destroy(gameObject), duration);
        shotWithGravOff = isShooterGravOff();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerUserInput.localPlayerInstance.gameObject != other.gameObject)
        {
            print("i kille dmyself");
            Destroy(gameObject);
        }
    }
 
    private void FixedUpdate()
    {
        var transform1 = transform;
        _rigidbody.MovePosition(transform1.position + transform1.forward * (speed * Time.deltaTime));
    }

    #endregion

    #region  Public Functions

    public int getDamage()
    {
        return shotWithGravOff ? gravOffDamage : gravOnDamage; 
        
    }


    #endregion

    #region  Private Functions

    /// <summary>
    /// returns true if the shooter of the projectile is grav off, false otherwise 
    /// </summary>
    /// <returns></returns>
    private bool isShooterGravOff()
    {
        if (shooterActorNum == -1)
            return false; //inapplicable
        
        GameObject shooterGameObject = (GameObject)PhotonNetwork.CurrentRoom.GetPlayer(shooterActorNum).TagObject;
        return !shooterGameObject.GetComponent<PlayerGravity>().GetGravity();
    } 

    #endregion
    

}