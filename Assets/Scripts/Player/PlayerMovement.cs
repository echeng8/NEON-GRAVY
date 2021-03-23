
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

	/// <summary>
    /// The maximum top speed. Additional gravies do not increase the top speed beyond this point. 
    /// </summary>
	public float maxTopSpeed;

	/// <summary>
    /// The speed at zero streaks. 
    /// </summary>
	public float baseSpeed;

	/// <summary>
    /// How many gravies it takes to max out the top speed.
    /// </summary>
	public float graviesToMaxTopSpeed;

	/// <summary>
	/// How many streaks it takes to bounce at the current top speed. 
	/// </summary>
	public float streaksToTopSpeed;
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

	[SerializeField] float m_GroundCheckDistance = 0.1f;
	[SerializeField] private float groundCheckRaycastSpread;
	[SerializeField] private float groundCheckRaycastHeightOffset;

	Rigidbody m_Rigidbody;

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
			if (value != _platformBelow)
			{
				if (_platformBelow != null)
                {
					_platformBelow.GetComponent<PlatformAppearance>().OnPlatLeave.Invoke();
					InvokeOnLeavePlatformRPC(_platformBelow.transform.GetSiblingIndex());
				}


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
	public int streakForgive; //The amount of clicks before losing your streak
	private int streakmisses; //Current amount of misses on bounces
	
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
		pg = GetComponent<PlayerGravity>();

		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	private void Start()
	{
		//listen to events
		OnPlatformBelowChange.AddListener(InvokeOnTouchPlatformRPC); 
		GetComponent<PlayerGravity>().OnGravityChange.AddListener(respondToGravity);
		platformDetector.OnObjectChange.AddListener(SetPlatformBelow); 
		

		//init misc
		Streaks = 0;
		streakmisses = 0;
		streaksText = GameObject.Find("Streaks").GetComponent<TextMeshProUGUI>();
		PlatformBelow = platformDetector.ObjectDetected; 
		
	}
		
	public void ControlledUpdate()
	{
		ProcessBounce(); 
	}

	//universal callbacks 
	#endregion

	#region Custom Methods
	void ProcessBounce() //le big function
    {

		//get direction
		Vector3 pointToDash = new Vector3(0, 0, 0);
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
					//update streak and number of misses and calculate new velocity magnitude 
					Streaks++;
					streakmisses = 0;
					float velMagnitude = GetCurrentSpeed(Streaks);

					//apply velocity to new direction
					Vector3 dashDirection = (pointToDash - transform.position).normalized;
					transform.forward = (pointToDash - transform.position).normalized; //change facing direction
					Vector3 velocity = dashDirection * (velMagnitude);

					GetComponent<PlayerMoveSync>().UpdateMovementRPC(velocity, transform.position);

					//invoking bounce events
					OnBounce.Invoke();
					InvokeOnBouncePlatformRPC();
				}
				else
				{
					streakmisses++;
				}
				if (streakmisses >= streakForgive)
				{
					streakmisses = 0;
					Streaks = 0;
				}
			}
		}
		if (GetComponent<PlayerGravity>().GetGravity()) //todo change to be based on alive/dead
		{
			Streaks = 0;
		}
		UpdateStreakCounter();
	}

	float GetCurrentSpeed(int streaksNum)
    {
		float topSpeed = GetTopSpeed(GetComponent<PlayerIdentity>().Gravies); 
		return Mathf.Clamp(baseSpeed + streaksNum * (topSpeed / streaksToTopSpeed), 0, maxTopSpeed);
	}

	float GetTopSpeed(int graviesNum)
    {
		float maxAdditionalSpeed = maxTopSpeed - baseSpeed;
		return Mathf.Clamp(baseSpeed + graviesNum * (maxAdditionalSpeed / graviesToMaxTopSpeed),0, maxTopSpeed);
	}

	void SetPlatformBelow(GameObject platformBelow)
    {
		PlatformBelow = platformBelow; 
    }
		
	void UpdateStreakCounter()
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
	void InvokeOnLeavePlatformRPC(int platformSiblingIndex)
	{
		photonView.RPC("RPC_InvokeOnLeavePlatform", RpcTarget.All, platformSiblingIndex); 
	}
	#endregion
		
}
	

