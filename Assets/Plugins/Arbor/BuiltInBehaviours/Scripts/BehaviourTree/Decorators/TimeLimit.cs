//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor.BehaviourTree.Decorators
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ノードに入ったタイミングから指定時間経過すると失敗を返す。
	/// </summary>
#else
	/// <summary>
	/// When a specified time elapses from the timing of entering the node, it returns failure.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("TimeLimit")]
	[BuiltInBehaviour]
	public sealed class TimeLimit : Decorator, INodeBehaviourSerializationCallbackReceiver
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 時間の種類。
		/// </summary>
#else
		/// <summary>
		/// Type of time.
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(TimeType))]
		private FlexibleTimeType _TimeType = new FlexibleTimeType(TimeType.Normal);

#if ARBOR_DOC_JA
		/// <summary>
		/// タイムリミットの秒数
		/// </summary>
#else
		/// <summary>
		/// Time limit in seconds
		/// </summary>
#endif
		[SerializeField]
		private FlexibleFloat _Seconds = new FlexibleFloat();

		[SerializeField]
		[HideInInspector]
		private int _SerializeVersion = 0;

		#region old

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_TimeType")]
		private TimeType _OldTimeType = TimeType.Normal;

		#endregion // old

		#endregion Serialize fields

		private const int kCurrentSerializeVersion = 1;

		private TimeType _CacheTimeType;

		public float currentTime
		{
			get
			{
				return TimeUtility.CurrentTime(_CacheTimeType);
			}
		}

		public float elapsedTime
		{
			get
			{
				return currentTime - _BeginTime;
			}
		}

		public float duration
		{
			get
			{
				return _DurationTime;
			}
		}

		private float _BeginTime;
		private float _DurationTime;

		protected override void OnStart()
		{
			_CacheTimeType = _TimeType.value;

			_BeginTime = currentTime;
			_DurationTime = _Seconds.value;
		}

		protected override bool OnConditionCheck()
		{
			return elapsedTime <= _DurationTime;
		}

		void Reset()
		{
			_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_TimeType = (FlexibleTimeType)_OldTimeType;
		}

		void Serialize()
		{
			while (_SerializeVersion != kCurrentSerializeVersion)
			{
				switch (_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_SerializeVersion++;
						break;
					default:
						_SerializeVersion = kCurrentSerializeVersion;
						break;
				}
			}
		}

		void INodeBehaviourSerializationCallbackReceiver.OnAfterDeserialize()
		{
			Serialize();
		}

		void INodeBehaviourSerializationCallbackReceiver.OnBeforeSerialize()
		{
			Serialize();
		}
	}
}