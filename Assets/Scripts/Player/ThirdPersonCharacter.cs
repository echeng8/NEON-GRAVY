
using System;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable All

namespace UnityStandardAssets.Characters.ThirdPerson
{
	
	/// <summary>
	/// Handles backend movement, colliders, and animators
	/// responds to gravity change
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		

		//[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others

		
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;
		[SerializeField] private float groundCheckRaycastSpread;
		[SerializeField] private float groundCheckRaycastHeightOffset;
		
		/// <summary>
		/// Invoked whenever the player touches the ground with Grav-On 
		/// </summary>
		public UnityEvent OnLand = new UnityEvent();

		Rigidbody m_Rigidbody;
		private BoxCollider m_BoxCollider;

		/// <summary>
		/// the collider that is only on when the grav is off for hitting into walls 
		/// </summary>
		private PlayerGravity pg; 
		[SerializeField] private Collider m_gravOffCollider; 
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		

		bool m_Crouching;
		
		/// <summary>
		/// coyote time in seconds
		/// </summary>
		public float coyoteTime;

		/// <summary>
		/// time player has been off ground since last time 
		/// </summary>
		private float timeOffGround;
		
		/// <summary>
		/// the platform the player is standing on
		/// </summary>
		public GameObject standPlatform;
		
		
		void Awake()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_BoxCollider = GetComponent<BoxCollider>();
			pg = GetComponent<PlayerGravity>(); 

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}

		private void Start()
		{
			
			//listen to events
			GetComponent<PlayerGravity>().OnGravityChange.AddListener(respondToGravity);
		}


		public void Move(Vector3 move)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation();

			// coyote time 
			if (!m_IsGrounded)
			{
				timeOffGround += Time.deltaTime; 
			}
			else
			{
				timeOffGround = 0; 
			}

			if (timeOffGround > coyoteTime)
			{
				HandleAirborneMovement(); // start falling 
			}
				

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}
		

		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("OnGround", m_IsGrounded);
			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}
			
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			if (!m_Animator.enabled) // don't apply gravity if animator is down 
				return; 
			
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);
			
		}

		

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}


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

			if (hitInfo.collider != null)
			{
				standPlatform = hitInfo.collider.gameObject; //todo remove redunancy
				
				if (!pg.GetGravity())
					return;

				//if this is the frame that you just landed in 
				//delete velocity if you were falling 
				if (!m_IsGrounded)
				{
					m_Rigidbody.velocity = Vector3.zero;
					OnLand.Invoke();
				}
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				if (standPlatform != null)
				{
					standPlatform = null;
				}
				if (!pg.GetGravity())
					return;

				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
		
		#region Custom Scripts

		void respondToGravity(bool gravityOn)
		{
			m_Animator.enabled = gravityOn;
			m_BoxCollider.isTrigger = !gravityOn;
			m_gravOffCollider.enabled = !gravityOn;
		}
		#endregion
		
	}
}
