//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor.StateMachine.StateBehaviours
{
	[AddComponentMenu("")]
	[HideBehaviour()]
	public abstract class AgentIntervalUpdate : AgentBase
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
		/// 移動先を変更するまでのインターバル(秒)。(UpdateTypeがTime、Doneの時のみ使用) <br />
		/// AgentUpdateType.Doneの場合は到達後のインターバル。
		/// </summary>
#else
		/// <summary>
		/// Interval (seconds) before moving destination is changed. (Used only when UpdateType is Time, Done)<br />
		/// The interval after arrival for AgentUpdateType.Done.
		/// </summary>
#endif
		[SerializeField]
		private FlexibleFloat _Interval = new FlexibleFloat();

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートから抜けるときに停止するかどうか
		/// </summary>
#else
		/// <summary>
		/// Whether to stop when leaving the state.
		/// </summary>
#endif
		[SerializeField]
		private FlexibleBool _StopOnStateEnd = new FlexibleBool(false);

		[SerializeField]
		[HideInInspector]
		private int _IntervalUpdate_SerializeVersion = 0;

		#region old

		[SerializeField]
		[HideInInspector]
		private float _MinInterval = 0f;

		[SerializeField]
		[HideInInspector]
		private float _MaxInterval = 0f;

		[SerializeField]
		[FormerlySerializedAs("_UpdateType")]
		[HideInInspector]
		private AgentUpdateType _OldUpdateType = AgentUpdateType.Time;

		[SerializeField]
		[FormerlySerializedAs("_TimeType")]
		[HideInInspector]
		private TimeType _OldTimeType = TimeType.Normal;

		[SerializeField]
		[FormerlySerializedAs("_StopOnStateEnd")]
		[HideInInspector]
		private bool _OldStopOnStateEnd = false;

		#endregion // old

		#endregion // Serialize fields

		private const int kIntervalUpdate_SerializeVersion = 2;

		private bool _IsStartExecuted = false;
		private float _BeginTime = 0f;
		private float _NextInterval = 0f;
		private bool _IsDone = false;

		private AgentUpdateType _CacheUpdateType;
		private TimeType _CacheTimeType;

		// Use this for enter state
		public override void OnStateBegin()
		{
			_IsStartExecuted = false;
			_BeginTime = 0f;
			_NextInterval = 0f;
			_IsDone = false;

			_CacheUpdateType = _UpdateType.value;
			_CacheTimeType = _TimeType.value;

			AgentUpdate();
		}

		// Use this for exit state
		public override void OnStateEnd()
		{
			AgentController agentController = cachedAgentController;
			if (_StopOnStateEnd.value && agentController != null)
			{
				agentController.Stop();
			}
		}

		protected abstract void OnUpdateAgent();
		protected virtual void OnDone()
		{
		}

		void AgentUpdate()
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

		// Update is called once per frame
		public override void OnStateUpdate()
		{
			AgentController agentController = cachedAgentController;

			AgentUpdate();

			if (agentController != null && agentController.isDone)
			{
				OnDone();
			}
		}

		void SerializeVer1()
		{
			_Interval = new FlexibleFloat(_MinInterval, _MaxInterval);
		}

		void SerializeVer2()
		{
			_UpdateType = (FlexibleAgentUpdateType)_OldUpdateType;
			_TimeType = (FlexibleTimeType)_OldTimeType;
			_StopOnStateEnd = (FlexibleBool)_OldStopOnStateEnd;
		}

		void Serialize()
		{
			while (_IntervalUpdate_SerializeVersion != kIntervalUpdate_SerializeVersion)
			{
				switch (_IntervalUpdate_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_IntervalUpdate_SerializeVersion++;
						break;
					case 1:
						SerializeVer2();
						_IntervalUpdate_SerializeVersion++;
						break;
					default:
						_IntervalUpdate_SerializeVersion = kIntervalUpdate_SerializeVersion;
						break;
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();

			_IntervalUpdate_SerializeVersion = kIntervalUpdate_SerializeVersion;
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