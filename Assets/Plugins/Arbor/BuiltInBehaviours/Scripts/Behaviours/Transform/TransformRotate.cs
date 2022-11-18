//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor.StateMachine.StateBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Transformを回転する。
	/// </summary>
#else
	/// <summary>
	/// Rotate the transform.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Transform/TransformRotate")]
	[BuiltInBehaviour]
	public sealed class TransformRotate : TransformBehaviourBase
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 回転の座標空間
		/// </summary>
#else
		/// <summary>
		/// Coordinate space of rotation
		/// </summary>
#endif
		[SerializeField]
		private FlexibleSpace _Space = new FlexibleSpace(Space.Self);

#if ARBOR_DOC_JA
		/// <summary>
		/// 更新メソッドの種類
		/// </summary>
#else
		/// <summary>
		/// Type of update method
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(UpdateMethodType))]
		private FlexibleUpdateMethodType _UpdateMethodType = new FlexibleUpdateMethodType(UpdateMethodType.StateUpdate);

#if ARBOR_DOC_JA
		/// <summary>
		/// 回転速度
		/// </summary>
#else
		/// <summary>
		/// Rotational speed
		/// </summary>
#endif
		[SerializeField]
		private FlexibleVector3 _AngularVelocity = new FlexibleVector3();

		[SerializeField]
		[HideInInspector]
		private int _TransformRotate_SerializeVersion = 0;

		#region old

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_Space")]
		private Space _OldSpace = Space.Self;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_UpdateMethodType")]
		private UpdateMethodType _OldUpdateMethodType = UpdateMethodType.StateUpdate;

		#endregion // old

		#endregion // Serialize fields

		private const int kCurrentSerializeVersion = 1;

		void UpdateRotate()
		{
			Transform target = cachedTransform;
			if (target != null)
			{
				Vector3 eulerAngles = _AngularVelocity.value * Time.deltaTime;
				target.Rotate(eulerAngles, _Space.value);
			}
		}

		private void Update()
		{
			if (_UpdateMethodType.value != UpdateMethodType.Update)
			{
				return;
			}

			UpdateRotate();
		}

		// OnStateUpdate is called once per frame
		public override void OnStateUpdate()
		{
			if (_UpdateMethodType.value != UpdateMethodType.StateUpdate)
			{
				return;
			}

			UpdateRotate();
		}

		protected override void Reset()
		{
			base.Reset();

			_TransformRotate_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_Space = (FlexibleSpace)_OldSpace;
			_UpdateMethodType = (FlexibleUpdateMethodType)_OldUpdateMethodType;
		}

		void Serialize()
		{
			while (_TransformRotate_SerializeVersion != kCurrentSerializeVersion)
			{
				switch (_TransformRotate_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_TransformRotate_SerializeVersion++;
						break;
					default:
						_TransformRotate_SerializeVersion = kCurrentSerializeVersion;
						break;
				}
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			Serialize();
		}

		public override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();

			Serialize();
		}
	}
}