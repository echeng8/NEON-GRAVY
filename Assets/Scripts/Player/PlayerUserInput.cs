using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// The high level player script.
/// Handles enabling input signal transfer and determining the local player 
/// </summary>
public class PlayerUserInput : MonoBehaviourPun
{    
    
    /// <summary>
    /// The number of hits a player can take until they are at axHitForce.
    /// The force for any given hit is calculated as hits / maxHits * maxHitForce
    /// The force will not exceed max hit force. 
    /// </summary>
    [SerializeField] public bool debugControlled;
    
    public static PlayerUserInput localPlayerInstance;
    public static GameObjectEvent OnLocalPlayerSet = new GameObjectEvent();
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!debugControlled || PhotonNetwork.IsConnected && !photonView.IsMine)
            return;
        
        gameObject.SendMessage("ControlledUpdate",null, SendMessageOptions.DontRequireReceiver);
        
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
}
