
using System;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable All

namespace UnityStandardAssets.Characters.ThirdPerson
{
	/// <summary>
	/// checks for platforms under player
	/// has extra methods for gravity change, (handle falling, disable istrigger, turns animator on)
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		
		#region Implementation Values
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		
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
				if (value != _platformBelow)
				{
					_platformBelow = value;
					OnPlatformBelowChange.Invoke(value);
					GetComponent<PlatformAppearance>().OnPlatLeave.Invoke();
				}
			}
		}

		public GameObject _platformBelow; 
		/// <summary>
		/// event that is called with the new platform that the player is now under. null if the new platform is no platform at all 
		/// </summary>
		public GameObjectEvent OnPlatformBelowChange = new GameObjectEvent();
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
			GetComponent<PlayerGravity>().OnGravityChange.AddListener(respondToGravity);
		}
		
		public void ControlledFixedUpdate()
		{
			CheckGroundStatus();
		}
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
		#endregion
		
	}
}
