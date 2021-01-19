using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

/// <summary>
/// The high level player script.
/// Handles enabling input signal transfer.
/// Also handles identifying and providing the PlayerGameObject 
/// </summary>
public class PlayerIdentity : MonoBehaviourPun
{    
    
    #region Implementation Values
    /// <summary>
    /// The number of hits a player can take until they are at axHitForce.
    /// The force for any given hit is calculated as hits / maxHits * maxHitForce
    /// The force will not exceed max hit force. 
    /// </summary>
    [SerializeField] public bool debugControlled;
    
    public static PlayerIdentity localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();

    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (localPlayerInstance == null)
        {
            if (!PhotonNetwork.IsConnected || photonView.AmOwner)
            {
                SetLocalPlayer();
            }
        }

        if (PhotonNetwork.IsConnected)
        {
            if (photonView.Owner == null)
                Destroy(gameObject); // this is the offline character for offline testing. 
            else 
                photonView.Owner.TagObject = gameObject; 
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!debugControlled || PhotonNetwork.IsConnected && !photonView.IsMine)
            return;

        gameObject.SendMessage("ControlledUpdate", null, SendMessageOptions.DontRequireReceiver); 
    }
    
    void FixedUpdate()
    {
        if (!debugControlled || PhotonNetwork.IsConnected && !photonView.IsMine)
            return;
        
        gameObject.SendMessage("ControlledFixedUpdate", null, SendMessageOptions.DontRequireReceiver);
    }

    void LateUpdate()
    {
        if (!debugControlled || PhotonNetwork.IsConnected && !photonView.IsMine)
            return;
        
        gameObject.SendMessage("ControlledLateUpdate", null, SendMessageOptions.DontRequireReceiver);
    }
    
    #endregion

    #region Unity Event Methods
    /// <summary>
    /// Executes the given function when the player loads, with the player's gameobject as a parameter. 
    /// </summary>
    /// <param name="func"></param>
    public static void CallOnLocalPlayerSet(UnityAction<GameObject> func)
    {
        if (localPlayerInstance == null)
        {
            OnLocalPlayerSet.AddListener(func);
        }
        else
        {
            func(localPlayerInstance.gameObject);
        }
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// set itself as LocalPlayerInstance
    /// invokes OnLocalPlayerSet event
    /// </summary>
    void SetLocalPlayer()
    {
        localPlayerInstance = this;
        OnLocalPlayerSet.Invoke(gameObject);
        gameObject.name = "Local Player"; 
    }
    #endregion
    
}
