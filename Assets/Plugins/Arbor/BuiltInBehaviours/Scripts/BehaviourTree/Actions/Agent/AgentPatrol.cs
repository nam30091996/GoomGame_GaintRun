//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor.BehaviourTree.Actions
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
		/// Agentの更新タイプ。
		/// </summary>
#else
		/// <summary>
		/// Agent update type.
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(AgentUpdateType))]
		private FlexibleAgentUpdateType _UpdateType = new FlexibleAgentUpdateType(AgentUpdateType.Time);

#if ARBOR_DOC_JA
		/// <summary>
		/// Intervalの時間タイプ。
		/// </summary>
#else
		/// <summary>
		/// Interval time type.
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(TimeType))]
		private FlexibleTimeType _TimeType = new FlexibleTimeType(TimeType.Normal);

#if ARBOR_DOC_JA
		/// <summary>
		/// 移動先を変更するまでのインターバル(秒)。
		/// </summary>
#else
		/// <summary>
		/// Interval (seconds) before moving destination is changed.
		/// </summary>
#endif
		[SerializeField]
		private FlexibleFloat _Interval = new FlexibleFloat();

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
		private FlexibleFloat _Radius = new FlexibleFloat();

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
		[HideInInspector]
		[FormerlySerializedAs("_UpdateType")]
		private AgentUpdateType _OldUpdateType = AgentUpdateType.Time;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_TimeType")]
		private TimeType _OldTimeType = TimeType.Normal;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_CenterType")]
		private PatrolCenterType _OldCenterType = PatrolCenterType.InitialPlacementPosition;

		#endregion // old

		#endregion // Serialize fields

		private const int kCurrentSerializeVersion = 1;

		Vector3 _ActionStartPosition;

		private bool _IsStartExecuted = false;
		private float _BeginTime = 0f;
		private float _NextInterval = 0f;
		private bool _IsDone = false;

		private AgentUpdateType _CacheUpdateType;
		private TimeType _CacheTimeType;

		protected override void OnStart()
		{
			base.OnStart();

			AgentController agentController = cachedAgentController;
			if (agentController != null)
			{
				_ActionStartPosition = agentController.agentTransform.position;
			}

			_CacheUpdateType = _UpdateType.value;
			_CacheTimeType = _TimeType.value;

			_IsStartExecuted = false;
			_IsDone = false;
			_BeginTime = TimeUtility.CurrentTime(_CacheTimeType);
			_NextInterval = 0f;
		}

		void OnUpdateAgent()
		{
			AgentController agentController = cachedAgentController;
			if (agentController != null)
			{
				switch (_CenterType.value)
				{
					case PatrolCenterType.InitialPlacementPosition:
						agentController.Patrol(_Speed.value, _Radius.value);
						break;
					case PatrolCenterType.StateStartPosition:
						agentController.Patrol(_ActionStartPosition, _Speed.value, _Radius.value);
						break;
					case PatrolCenterType.Transform:
						Transform centerTransform = _CenterTransform.value;
						if (centerTransform != null)
						{
							agentController.Patrol(centerTransform.position, _Speed.value, _Radius.value);
						}
						break;
					case PatrolCenterType.Custom:
						agentController.Patrol(_CenterPosition.value, _Speed.value, _Radius.value);
						break;
				}
			}
		}

		protected override void OnExecute()
		{
			switch (_CacheUpdateType)
			{
				case AgentUpdateType.Time:
					{
						float currentTime = TimeUtility.CurrentTime(_CacheTimeType);
						if (currentTime - _BeginTime >= _NextInterval)
						{
							OnUpdateAgent();
							_BeginTime = currentTime;
							_NextInterval = _Interval.value;

							_IsStartExecuted = true;
						}
					}
					break;
				case AgentUpdateType.Done:
					{
						if (_IsStartExecuted)
						{
							if (!_IsDone)
							{
								AgentController agentController = cachedAgentController;
								if (agentController != null && agentController.isDone)
								{
									_IsDone = true;
									_BeginTime = TimeUtility.CurrentTime(_CacheTimeType);
									_NextInterval = _Interval.value;
								}
								else
								{
									_IsDone = false;
								}
							}
						}
						else
						{
							_IsDone = true;
							_IsStartExecuted = true;
							_NextInterval = 0f;
						}

						if (_IsDone)
						{
							float currentTime = TimeUtility.CurrentTime(_CacheTimeType);
							if (currentTime - _BeginTime >= _NextInterval)
							{
								OnUpdateAgent();
								_IsDone = false;
							}
						}
					}
					break;
				case AgentUpdateType.StartOnly:
					if (!_IsStartExecuted)
					{
						OnUpdateAgent();
						_IsStartExecuted = true;
					}
					break;
				case AgentUpdateType.Always:
					{
						OnUpdateAgent();

						_IsStartExecuted = true;
					}
					break;
			}
		}

		protected override void Reset()
		{
			base.Reset();

			_AgentPatrol_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_UpdateType = (FlexibleAgentUpdateType)_OldUpdateType;
			_TimeType = (FlexibleTimeType)_OldTimeType;
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

		public override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();

			Serialize();
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();

			Serialize();
		}
	}
}