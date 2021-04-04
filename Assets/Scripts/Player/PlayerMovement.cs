
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun; 
using TMPro;
// ReSharper disable All

/// <summary>
/// checks for platforms under player
/// has extra methods for gravity change, (handle falling, disable istrigger, turns animator on)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviourPun
{
	#region Gameplay Values

	/// <summary>
    /// The top speed. 
    /// </summary>
	public float topSpeed;

	/// <summary>
    /// The speed at zero streaks. 
    /// </summary>
	public float baseSpeed;

	/// <summary>
	/// How many streaks it takes to bounce at the current top speed. 
	/// </summary>
	public float streaksToTopSpeed;

	/// <summary>
	/// How much does speed reduce when changing directions
	/// </summary>
	public float directionDrag;

	/// <summary>
	/// time until platfromBelow is set to null after leaving a platform
	/// (NOTE: error prone if too long, use with caution
	/// </summary>
	public float coyoteTime; 
		
	#endregion

	#region Implementation Values

	public Vector3 Velocity
    {
		get
        {
			return GetComponent<Rigidbody>().velocity; 
        }
    }
	
	public ObjectDetector platformDetector; 

	[Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;

	Rigidbody m_Rigidbody;

	/// <summary>
	/// the collider that is only on when the grav is off for hitting into walls 
	/// </summary>
	private PlayerGravity pg;
	Animator m_Animator;


	public int streakForgive; //The amount of clicks before losing your streak
	private int streakMisses; //Current amount of misses on bounces

	public TextMeshProUGUI streaksText;


	/// <summary>
	/// To be invoked when the player bounces, generally for local player effects.
	/// Not to be confused with PlatformanAppearance.OnBounce, which is generally for networked univversal effects. 
	/// </summary>
	public UnityEvent OnBounce = new UnityEvent();
	public UnityEvent OnLeave = new UnityEvent();

	public GameObject Body;


	/// <summary>
	/// the platform the player is standing on
	/// </summary>
	public GameObject PlatformBelow
	{
		get
		{
			return _platformBelow; 
		}
		set
		{
			if (value != _platformBelow)
			{
				if(value == null)
                {
					StartCoroutine(ProcessCoyoteTime(_platformBelow)); 
                } else
                {
					//print("I entered " + value.gameObject.name + "  " + value.transform.GetSiblingIndex());
					_platformBelow = value;
					OnPlatformBelowChange.Invoke(value);
				}
			}
		}
	}
	private GameObject _platformBelow;


	private GameObject lastPlatformBounce; 
	/// <summary>
	/// event that is called with the new platform that the player is now under. null if the new platform is no platform at all 
	/// </summary>
	public GameObjectEvent OnPlatformBelowChange = new GameObjectEvent();
	
	#endregion

	#region Unity Callbacks
	void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		pg = GetComponent<PlayerGravity>();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	private void Start()
	{
		//listen to events
		OnPlatformBelowChange.AddListener(InvokeOnTouchPlatformRPC); 
		platformDetector.OnObjectChange.AddListener(SetPlatformBelow);
		platformDetector.OnObjectLeave.AddListener(ProcessOnLeavePlatform); 

		//init misc
		streakMisses = 0;
		PlatformBelow = platformDetector.ObjectDetected; 
	}
	void SetPlatformBelow(GameObject platformBelow)
	{
		PlatformBelow = platformBelow;
	}


	public void Update()
	{
		if(photonView.Owner == PhotonNetwork.LocalPlayer)
        {
			if (Input.GetButton("Fire1"))
			{
				Bounce(GetPointerPosInWorld()-transform.position);
			}
		}
	}
	#endregion

	#region Custom Methods
	/// <summary>
	/// bounce in the given direction if there is a valid PlatformBelow
	/// </summary>
	/// <param name="direction"></param>
	public void Bounce(Vector3 dashDirection)
    {
		if (PlatformBelow != null && PlatformBelow != lastPlatformBounce) //THE BOUNCE
		{
			lastPlatformBounce = PlatformBelow;

			//invoking bounce events
			OnBounce.Invoke();
			InvokeOnBouncePlatformRPC();

			//update streak and number of misses and calculate new velocity magnitude 
			float velMagnitude = GetCurrentSpeed(GetComponent<PlayerColorChange>().colorStreak);

			//apply velocity to new direction
			//Vector3 dashDirection = (direction - transform.position).normalized;
			float dashAngle = Math.Abs(Vector3.SignedAngle(transform.forward, dashDirection.normalized, transform.up));
			// transform.forward = (pointToDash - transform.position).normalized; //change facing direction
			Vector3 velocity = dashDirection.normalized * velMagnitude * (1 - (dashAngle * Body.transform.localScale.magnitude * directionDrag / 1080));

			GetComponent<PlayerMoveSync>().UpdateMovementRPC(velocity, transform.position);
		}
	} 

	Vector3 GetPointerPosInWorld()
    {
		Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, transform.position);
		float rayLength;


		if (groundPlane.Raycast(cameraRay, out rayLength))
		{
			return cameraRay.GetPoint(rayLength);
		}
		return Vector3.zero; 
	}
	IEnumerator ProcessCoyoteTime(GameObject platformReference)
    {
		yield return new WaitForSeconds(coyoteTime); 

		if (PlatformBelow == platformReference)
        {
		    _platformBelow = null;
		} 

    }

	float GetCurrentSpeed(int streaksNum)
    {
		return Mathf.Clamp(baseSpeed + streaksNum * (topSpeed / streaksToTopSpeed), 0, topSpeed);
	}

	void ProcessOnLeavePlatform(GameObject plat)
    {
		if (photonView.IsMine)
		{
//			print("I exited " + plat.gameObject.name + "  " + plat.transform.GetSiblingIndex());
			InvokeOnLeavePlatformRPC(plat.transform.GetSiblingIndex());
		}
	}

	//platform event rpcs
	[PunRPC]
	void RPC_InvokeOnBouncePlatform(byte platformNum)
	{
		//we derive this from network to avoid standplatform desync issues
		GameObject otherPlayerPlatformBelow = GameManager.instance.platformManager.GetPlatform(platformNum); 
		otherPlayerPlatformBelow.GetComponent<PlatformAppearance>().OnBounce.Invoke();
        
	}
    
	/// <summary>
	/// invokes the platfrombelow's OnBounce event across all networks
	/// </summary>
	void InvokeOnBouncePlatformRPC()
	{
		byte platNum = (byte)PlatformBelow.transform.GetSiblingIndex(); 
		photonView.RPC("RPC_InvokeOnBouncePlatform", RpcTarget.All, platNum); 
	}

	#endregion
	void InvokeOnTouchPlatformRPC(GameObject _obj)
    {
		photonView.RPC("RPC_InvokeOnTouchPlatform", RpcTarget.All); 
    }

	//platform event invoking 
	[PunRPC]
	void RPC_InvokeOnTouchPlatform()
	{
		if (PlatformBelow != null)
			PlatformBelow.GetComponent<PlatformAppearance>().OnTouch.Invoke();
	}


	/// <summary>
	/// invokes the platfrombelow's OnLeave event across all networks
	/// </summary>
	void InvokeOnLeavePlatformRPC(int platformSiblingIndex)
	{
		photonView.RPC("RPC_InvokeOnLeavePlatform", RpcTarget.All, platformSiblingIndex);
	}

	[PunRPC]
	void RPC_InvokeOnLeavePlatform(int platformNum)
	{ 
		GameObject otherPlayerPlatformBelow = GameManager.instance.platformManager.GetPlatform(platformNum); 
		otherPlayerPlatformBelow.GetComponent<PlatformAppearance>().OnPlatLeave.Invoke();
	}

}
	

