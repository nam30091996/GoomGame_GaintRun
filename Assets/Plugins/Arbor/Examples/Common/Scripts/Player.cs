//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Example
{
	/// <summary>
	/// Behavior of the player character. (General MonoBehaviour script)
	/// </summary>
	[AddComponentMenu("Arbor/Example/Player")]
	[RequireComponent(typeof(CharacterController))]
	public sealed class Player : MonoBehaviour
	{
		/// <summary>
		/// Move speed
		/// </summary>
		public float speed = 10.0f;

		/// <summary>
		/// Rotate speed
		/// </summary>
		public float rotateSpeed = 180.0f;

		/// <summary>
		/// Transform cache
		/// </summary>
		private Transform _Transform;

		/// <summary>
		/// CharacterController cache
		/// </summary>
		private CharacterController _Controller;

		/// <summary>
		/// Next rotation
		/// </summary>
		private Quaternion _NextRotation;

		// Use this for initialization
		void Start()
		{
			// Cache Transform and CharacterController
			_Transform = GetComponent<Transform>();
			_Controller = GetComponent<CharacterController>();

			_NextRotation = _Transform.rotation;
		}

		// Update is called once per frame
		void Update()
		{
			// Calculate moving direction.
			Vector3 moveDirection = Vector3.zero;
			if (_Controller.isGrounded)
			{
				Vector3 cameraForward = Camera.main.transform.forward;
				cameraForward.y = 0f;
				Vector3 cameraRight = Camera.main.transform.right;
				cameraRight.y = 0f;

				moveDirection = cameraRight * Input.GetAxis("Horizontal") + cameraForward * Input.GetAxis("Vertical");
				moveDirection.Normalize();

				if (!Mathf.Approximately(moveDirection.sqrMagnitude, 0))
				{
					_NextRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
				}
			}

			// Calculate velocity
			Vector3 velocity = moveDirection * speed;

			velocity += Physics.gravity * Time.deltaTime;

			// Move
			_Controller.Move(velocity * Time.deltaTime);

			// Rotate in moving direction.
			_Transform.rotation = Quaternion.RotateTowards(_Transform.rotation, _NextRotation, rotateSpeed * Time.deltaTime);
		}
	}
}