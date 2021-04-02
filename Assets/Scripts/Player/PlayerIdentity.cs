using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

/// <summary>
/// The high level player script.
/// Handles enabling input signal transfer.
///  handles identifying and providing the PlayerGameObject 
///  provides access to photon custom properties: gravies and kills
///  identifies bot status 
/// </summary>

public class PlayerIdentity : MonoBehaviourPunCallbacks
{
    #region Photon Custom Properties
    public int Kills
    {
        get
        {
            if(isBot)
            {
                return 0; 
            } else
            {
                if (photonView.Owner.CustomProperties.ContainsKey("kills"))
                    return (int)photonView.Owner.CustomProperties["kills"];
                else
                    return 0; //edge case when properties have not been initialized 
            }
        }

        set
        {
            if(!isBot)
            {
                Hashtable h = new Hashtable { { "kills", value } };
                photonView.Owner.SetCustomProperties(h);
            }
        }
    }
    #endregion

    #region Implementation Values

    public bool isBot = false; 
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
            {
                isBot = true;
                gameObject.AddComponent<BotPlayer>(); 
            }
            else
            {
                photonView.Owner.TagObject = gameObject;
            }
        }
    }

    private void Start()
    {
        if(photonView.IsMine)
        {
            GetComponent<PlayerDeath>().OnDeath.AddListener(ClearKillsRPC); 
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

    /// <summary>
    /// clears the gravies and kills custom properites  
    /// </summary>
    void ClearKillsRPC()
    {
        photonView.Owner.SetCustomProperties(new Hashtable() {{ "kills", 0 } });
    }

    #endregion

}
