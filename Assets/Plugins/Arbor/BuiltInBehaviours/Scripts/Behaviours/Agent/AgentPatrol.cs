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
	/// 指定位置の周辺を巡回させる。
	/// </summary>
#else
	/// <summary>
	/// To patrol the vicinity of the specified location.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Agent/AgentPatrol")]
	[BuiltInBehaviour]
	public sealed class AgentPatrol : AgentMoveBase
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 移動半径
		/// </summary>
#else
		/// <summary>
		/// Moving radius
		/// </summary>
#endif
		[SerializeField]
		private FlexibleFloat _Radius = new FlexibleFloat(0f);

#if ARBOR_DOC_JA
		/// <summary>
		/// パトロールする中心タイプ
		/// </summary>
#else
		/// <summary>
		/// Center type to patrol
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(PatrolCenterType))]
		private FlexiblePatrolCenterType _CenterType = new FlexiblePatrolCenterType(PatrolCenterType.InitialPlacementPosition);

#if ARBOR_DOC_JA
		/// <summary>
		/// 中心Transformの指定(CenterTypeがTransformのみ)
		/// </summary>
#else
		/// <summary>
		/// Specifying the center transform (CenterType is Transform only)
		/// </summary>
#endif
		[SerializeField]
		private FlexibleTransform _CenterTransform = new FlexibleTransform();

#if ARBOR_DOC_JA
		/// <summary>
		/// 中心の指定(CenterTypeがCustomのみ)
		/// </summary>
#else
		/// <summary>
		/// Specify the center (CenterType is Custom only)
		/// </summary>
#endif
		[SerializeField]
		private FlexibleVector3 _CenterPosition = new FlexibleVector3();

		[SerializeField]
		[HideInInspector]
		private int _AgentPatrol_SerializeVersion = 0;

		#region old

		[SerializeField]
		[FormerlySerializedAs("_Radius")]
		[HideInInspector]
		private float _OldRadius = 0f;

		[SerializeField]
		[FormerlySerializedAs("_CenterType")]
		[HideInInspector]
		private PatrolCenterType _OldCenterType = PatrolCenterType.InitialPlacementPosition;

		#endregion // old

		#endregion // Serialize fields

		private const int kCurrentSerializeVersion = 1;

		Vector3 _StateStartPosition;

		public override void OnStateBegin()
		{
			base.OnStateBegin();

			AgentController agentController = cachedAgentController;
			if (agentController != null)
			{
				_StateStartPosition = agentController.agentTransform.position;
			}
		}

		protected override void OnUpdateAgent()
		{
			AgentController agentController = cachedAgentController;
			if (agentController != null)
			{
				float speed = _Speed.value;
				float radius = _Radius.value;

				switch (_CenterType.value)
				{
					case PatrolCenterType.InitialPlacementPosition:
						agentController.Patrol(speed, radius);
						break;
					case PatrolCenterType.StateStartPosition:
						agentController.Patrol(_StateStartPosition, speed, radius);
						break;
					case PatrolCenterType.Transform:
						Transform centerTransform = _CenterTransform.value;
						if (centerTransform != null)
						{
							agentController.Patrol(centerTransform.position, speed, radius);
						}
						break;
					case PatrolCenterType.Custom:
						agentController.Patrol(_CenterPosition.value, speed, radius);
						break;
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();

			_AgentPatrol_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_Radius = (FlexibleFloat)_OldRadius;
			_CenterType = (FlexiblePatrolCenterType)_OldCenterType;
		}

		void Serialize()
		{
			while (_AgentPatrol_SerializeVersion != kCurrentSerializeVersion)
			{
				switch (_AgentPatrol_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_AgentPatrol_SerializeVersion++;
						break;
					default:
						_AgentPatrol_SerializeVersion = kCurrentSerializeVersion;
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
