﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor.StateMachine.StateBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Tweenを行う基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class for Tweening
	/// </summary>
#endif
	[AddComponentMenu("")]
	[HideBehaviour]
	public class TweenBase : StateBehaviour, INodeBehaviourSerializationCallbackReceiver
	{
		#region enum

		[Internal.Documentable]
		public enum Type
		{
#if ARBOR_DOC_JA
			/// <summary>
			/// １回のみ
			/// </summary>
#else
			/// <summary>
			/// Only once
			/// </summary>
#endif
			Once,

#if ARBOR_DOC_JA
			/// <summary>
			/// 繰り返し
			/// </summary>
#else
			/// <summary>
			/// Repeat
			/// </summary>
#endif
			Loop,

#if ARBOR_DOC_JA
			/// <summary>
			/// 終端で折り返し
			/// </summary>
#else
			/// <summary>
			/// Turn back at the end
			/// </summary>
#endif
			PingPong,
		};

		[System.Serializable]
		public sealed class FlexibleTweenType : FlexibleField<Type>
		{
			public FlexibleTweenType()
			{
			}

			public FlexibleTweenType(Type value) : base(value)
			{
			}

			public FlexibleTweenType(AnyParameterReference parameter) : base(parameter)
			{
			}

			public FlexibleTweenType(InputSlotAny slot) : base(slot)
			{
			}

			public static explicit operator Type(FlexibleTweenType flexible)
			{
				return flexible.value;
			}

			public static explicit operator FlexibleTweenType(Type value)
			{
				return new FlexibleTweenType(value);
			}
		}

		#endregion // enum

		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 再生タイプ。
		/// </summary>
#else
		/// <summary>
		/// Play type.
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentType(typeof(Type))]
		private FlexibleTweenType _Type = new FlexibleTweenType(Type.Once);

#if ARBOR_DOC_JA
		/// <summary>
		/// 再生時間。
		/// </summary>
#else
		/// <summary>
		/// Playback time.
		/// </summary>
#endif
		[SerializeField] private FlexibleFloat _Duration = new FlexibleFloat(1.0f);

#if ARBOR_DOC_JA
		/// <summary>
		/// 時間に対する適用度の変化曲線
		/// </summary>
#else
		/// <summary>
		/// Change curve of applicability with respect to time
		/// </summary>
#endif
		[SerializeField] private FlexibleAnimationCurve _Curve = new FlexibleAnimationCurve(AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f));

#if ARBOR_DOC_JA
		/// <summary>
		/// Time.timeScaleの影響を受けずリアルタイムに進行するフラグ。
		/// </summary>
#else
		/// <summary>
		/// Flag to progress in real time without the influence of Time.timeScale.
		/// </summary>
#endif
		[SerializeField] private FlexibleBool _UseRealtime = new FlexibleBool(false);

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移するまでの繰り返し回数(Loop、PingPongのみ)
		/// </summary>
#else
		/// <summary>
		/// Number of repetitions until the transition (Loop, PingPong only)
		/// </summary>
#endif
		[SerializeField] private FlexibleInt _RepeatUntilTransition = new FlexibleInt(1);

#if ARBOR_DOC_JA
		/// <summary>
		/// 時間経過後の遷移先。<br/>
		/// (Loop、Pingpongの場合、Repeat Until Transitionで指定した回数だけ繰り返してから遷移する)<br />
		/// 遷移メソッド : Update, FixedUpdate
		/// </summary>
#else
		/// <summary>
		/// Transition destination after time.<br/>
		/// (In the case of Loop and Pingpong, to transition from repeated as many times as specified in the Repeat Until Transition)<br />
		/// Transition Method : Update, FixedUpdate
		/// </summary>
#endif
		[Internal.DocumentOrder(1000)]
		[SerializeField] private StateLink _NextState = new StateLink();

		[SerializeField]
		[HideInInspector]
		private int _SerializeVersionBase = 0;

		#region old

		[SerializeField]
		[FormerlySerializedAs("_Duration")]
		[HideInInspector]
		private float _OldDuration = 1.0f;

		[SerializeField]
		[FormerlySerializedAs("_Curve")]
		[HideInInspector]
		private AnimationCurve _OldCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

		[SerializeField]
		[FormerlySerializedAs("_UseRealtime")]
		[HideInInspector]
		private bool _OldUseRealtime = false;

		[SerializeField]
		[FormerlySerializedAs("_RepeatUntilTransition")]
		[HideInInspector]
		private int _OldRepeatUntilTransition = 1;

		[SerializeField]
		[FormerlySerializedAs("_Type")]
		[HideInInspector]
		private Type _OldType = Type.Once;

		#endregion // old

		#endregion // Serialize fields

		private const int kCurrentSerializeVersionBase = 2;

		public virtual bool fixedUpdate
		{
			get
			{
				return false;
			}
		}

		public virtual bool forceRealtime
		{
			get
			{
				return false;
			}
		}

		private float _CurrentDuration = 0.0f;
		private AnimationCurve _CurrentCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		private bool _CurrentUseRealtime = false;
		private int _CurrentRepeatUntilTransition = 1;

		private float _BeginTime = 0.0f;

		private float _FromAdvance = 0.0f;
		private float _ToAdvance = 1.0f;

		public int repeatUntilTransition
		{
			get
			{
				return _CurrentRepeatUntilTransition;
			}
		}

		private int _RepeatCount = 0;
		public int repeatCount
		{
			get
			{
				return _RepeatCount;
			}
		}

		private float GetTime()
		{
			if (fixedUpdate)
			{
				return Time.fixedTime;
			}

			if (_CurrentUseRealtime)
			{
				return Time.realtimeSinceStartup;
			}

			return Time.time;
		}

		// Use this for enter state
		public override void OnStateBegin()
		{
			_CurrentDuration = _Duration.value;
			_CurrentCurve = _Curve.value;
			if (_CurrentCurve == null)
			{
				_CurrentCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
			}
			_CurrentUseRealtime = forceRealtime || _UseRealtime.value;
			_CurrentRepeatUntilTransition = _RepeatUntilTransition.value;

			_BeginTime = GetTime();

			_FromAdvance = 0.0f;
			_ToAdvance = 1.0f;
			_RepeatCount = 0;

			OnTweenBegin();

			OnTweenUpdate(_CurrentCurve.Evaluate(0.0f));
		}

		protected virtual void OnTweenBegin()
		{
		}
		protected virtual void OnTweenUpdate(float factor)
		{
		}

		void TweenUpdate()
		{
			float nowTime = GetTime();

			float t = 0.0f;

			if (_CurrentDuration > 0.0f)
			{
				t = (nowTime - _BeginTime) / _CurrentDuration;
			}
			else
			{
				t = 1.0f;
			}

			float factor = Mathf.Lerp(_FromAdvance, _ToAdvance, Mathf.Clamp01(t));
			float curveFactor = _CurrentCurve.Evaluate(factor);

			OnTweenUpdate(curveFactor);

			if (t >= 1.0f)
			{
				Type type = _Type.value;
				switch (type)
				{
					case Type.Once:
						break;
					case Type.Loop:
						_BeginTime = nowTime;
						break;
					case Type.PingPong:
						_BeginTime = nowTime;

						float temp = _FromAdvance;
						_FromAdvance = _ToAdvance;
						_ToAdvance = temp;
						break;
				}

				if (type == Type.Once)
				{
					Transition(_NextState);
				}
				else
				{
					_RepeatCount++;
					if (_RepeatCount >= _CurrentRepeatUntilTransition)
					{
						Transition(_NextState);
					}
				}
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (!fixedUpdate)
			{
				TweenUpdate();
			}
		}

		void FixedUpdate()
		{
			if (fixedUpdate)
			{
				TweenUpdate();
			}
		}

		protected virtual void Reset()
		{
			_SerializeVersionBase = kCurrentSerializeVersionBase;
		}

		void SerializeVer1()
		{
			_Duration = (FlexibleFloat)_OldDuration;
			_Curve = (FlexibleAnimationCurve)_OldCurve;
			_UseRealtime = (FlexibleBool)_OldUseRealtime;
			_RepeatUntilTransition = (FlexibleInt)_OldRepeatUntilTransition;
		}

		void SerializeVer2()
		{
			_Type = (FlexibleTweenType)_OldType;
		}

		void Serialize()
		{
			while (_SerializeVersionBase != kCurrentSerializeVersionBase)
			{
				switch (_SerializeVersionBase)
				{
					case 0:
						SerializeVer1();
						_SerializeVersionBase++;
						break;
					case 1:
						SerializeVer2();
						_SerializeVersionBase++;
						break;
					default:
						_SerializeVersionBase = kCurrentSerializeVersionBase;
						break;
				}
			}
		}

		public virtual void OnBeforeSerialize()
		{
			Serialize();
		}

		public virtual void OnAfterDeserialize()
		{
			Serialize();
		}
	}
}
