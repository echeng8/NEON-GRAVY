
using System;
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
	public float maxVelocity; 

    #endregion
    
    #region Implementation Values
    [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;

	[SerializeField] float m_GroundCheckDistance = 0.1f;
	[SerializeField] private float groundCheckRaycastSpread;
	[SerializeField] private float groundCheckRaycastHeightOffset;

	Rigidbody m_Rigidbody;
	private BoxCollider m_BoxCollider;

	/// <summary>
	/// the collider that is only on when the grav is off for hitting into walls 
	/// </summary>
	private PlayerGravity pg;
	Animator m_Animator;

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
			if(value == null && _platformBelow != null)
			{
				_platformBelow.GetComponent<PlatformAppearance>().OnPlatLeave.Invoke();
				InvokeOnLeavePlatformRPC();
			}

			if (value != _platformBelow)
			{
				_platformBelow = value;
				OnPlatformBelowChange.Invoke(value);
			}
		}
	}
	private GameObject _platformBelow;

	/// <summary>
	/// event that is called with the new platform that the player is now under. null if the new platform is no platform at all 
	/// </summary>
	public GameObjectEvent OnPlatformBelowChange = new GameObjectEvent();
		
	/// <summary>
	/// the velocity that is added with each bounce
	/// *note a player's velocity caps out as determined by PlayerGravity
	/// </summary>
	public float addedBounceVelocity = 3; 

	public int Streaks
	{
		get
		{
			return _streaks;
		}
		set
        {
			_streaks = value;
			OnStreakChange.Invoke(value); 	
        }
	}
	private int _streaks; 

	public TextMeshProUGUI streaksText;

	public IntEvent OnStreakChange = new IntEvent(); 

	/// <summary>
	/// To be invoked when the player bounces, generally for local player effects.
	/// Not to be confused with PlatformanAppearance.OnBounce, which is generally for networked univversal effects. 
	/// </summary>
	public UnityEvent OnBounce = new UnityEvent();
	public UnityEvent OnLeave = new UnityEvent();
	
	#endregion

	#region Unity Callbacks
	void Awake()
	{
		m_Animator = GetComponent<Animator>();
		m_Rigidbody = GetComponent<Rigidbody>();
		m_BoxCollider = GetComponent<BoxCollider>();
		pg = GetComponent<PlayerGravity>();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	private void Start()
	{
		//listen to events
		OnPlatformBelowChange.AddListener(InvokeOnTouchPlatformRPC); 
		GetComponent<PlayerGravity>().OnGravityChange.AddListener(respondToGravity);

		//init misc
		Streaks = 0;
		streaksText = GameObject.Find("Streaks").GetComponent<TextMeshProUGUI>();
	}
		
	public void ControlledUpdate()
	{
		Vector3 pointToDash = new Vector3(0,0,0);
		Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, transform.position); 
		float rayLength;
        
		Debug.DrawRay(transform.position, Vector3.up * 3, Color.cyan);
		if (groundPlane.Raycast(cameraRay, out rayLength))
		{
			pointToDash = cameraRay.GetPoint(rayLength);
			Debug.DrawRay(pointToDash, (pointToDash - transform.position).normalized * 2f);
		}
			
		//Player Bounce

		if (!GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
		{
			if (Input.GetButtonDown("Fire1"))
			{

				if (PlatformBelow != null) //THE BOUNCE
				{
					Vector3 dashDirection = (pointToDash - transform.position).normalized;
					transform.forward = (pointToDash - transform.position).normalized;

					float velMagnitude = Vector3.Magnitude(GetComponent<Rigidbody>().velocity);
                   
					//TODO move to PlayerMovement (cant PlayerJetpack just get renamed to PlayerMovement?)
					Vector3 velocity = dashDirection * (velMagnitude + addedBounceVelocity); 

					GetComponent<PlayerMoveSync>().UpdateMovementRPC(velocity,transform.position);
                  
					//invoking bounce events
					OnBounce.Invoke(); 
					InvokeOnBouncePlatformRPC(); 

					Streaks++;

				}
				else
				{
					Streaks = 0;
				}
			}
		}
		if (GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
		{
			Streaks = 0;
		}
		StreakCounter();
	}

	public void ControlledFixedUpdate()
    {
		CheckGroundStatus();
	}

	//universal callbacks 
	
	private void Update()
	{
		m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, maxVelocity);
	}

	//public void FixedUpdate()
	//{
	//	//CheckGroundStatus();
	//}
	#endregion

	#region Custom Methods

	void CheckGroundStatus()
	{

		RaycastHit hitInfo = new RaycastHit(); //maybe optimization here? 

		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character

		Vector3 extents = m_BoxCollider.bounds.extents;
		Vector3 originPoint = transform.position + (Vector3.up * groundCheckRaycastHeightOffset);
		Vector3[] rayCastPoints = new Vector3[4];


		//todo decouple this from the boxcollider during run time (calculate and store these points on startup) 
		rayCastPoints[0] = originPoint + new Vector3(extents.x, 0, extents.z) * (1 + groundCheckRaycastSpread);
		rayCastPoints[1] = originPoint + new Vector3(-extents.x, 0, -extents.z) * (1 + groundCheckRaycastSpread);
		rayCastPoints[2] = originPoint + new Vector3(extents.x, 0, -extents.z) * (1 + groundCheckRaycastSpread);
		rayCastPoints[3] = originPoint + new Vector3(-extents.x, 0, extents.z) * (1 + groundCheckRaycastSpread);

		foreach (Vector3 raycastPoint in rayCastPoints)
		{

			if (Physics.Raycast(raycastPoint, -transform.up, out hitInfo, m_GroundCheckDistance))
			{
				break;
			}
		}

#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene viewp
		foreach (Vector3 raycastPoint in rayCastPoints)
		{
			Debug.DrawLine(raycastPoint, raycastPoint + (Vector3.down * m_GroundCheckDistance));
		}
		Debug.DrawLine(hitInfo.point, hitInfo.point + (Vector3.down * 50), Color.blue);

#endif

		if (hitInfo.collider != null) // theres something under you 
		{
			PlatformBelow = hitInfo.collider.gameObject;
		}
		else // if theres nothing under you 
		{
			PlatformBelow = null;
		}
	}
		
	void StreakCounter()
	{
		if(streaksText != null)
			streaksText.text = $"Streaks x{Streaks}";
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

	#region Gravity Methods

	/// <summary>
	/// For gravity change
	/// </summary>
	void respondToGravity(bool gravityOn)
	{
		m_Animator.enabled = gravityOn;
		m_BoxCollider.isTrigger = !gravityOn;
	}

	/// <summary>
	/// For player falling
	/// </summary>
	void HandleAirborneMovement()
	{
		if (!m_Animator.enabled) // don't apply gravity if animator is down 
			return;

		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);
	}


	//platform event invoking 
	[PunRPC]
	void RPC_InvokeOnTouchPlatform()
    {
		if(PlatformBelow != null)
			PlatformBelow.GetComponent<PlatformAppearance>().OnTouch.Invoke(); 
    }

	void InvokeOnTouchPlatformRPC(GameObject _obj)
    {
		photonView.RPC("RPC_InvokeOnTouchPlatform", RpcTarget.All); 
    }
	
	[PunRPC]
	void RPC_InvokeOnLeavePlatform(int platformNum)
	{ 
		GameObject otherPlayerPlatformBelow = GameManager.instance.platformManager.GetPlatform(platformNum); 
		otherPlayerPlatformBelow.GetComponent<PlatformAppearance>().OnPlatLeave.Invoke();
	}
    
	/// <summary>
	/// invokes the platfrombelow's OnLeave event across all networks
	/// </summary>
	void InvokeOnLeavePlatformRPC()
	{
		int platformNum = _platformBelow.transform.GetSiblingIndex();
		photonView.RPC("RPC_InvokeOnLeavePlatform", RpcTarget.All, platformNum); 
	}
	#endregion
		
}
	

