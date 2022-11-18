﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.StateMachine.StateBehaviours
{
	using Arbor.Utilities;

#if ARBOR_DOC_JA
	/// <summary>
	/// OnCollisionEnterが呼ばれたときにステートを遷移する。
	/// </summary>
	/// <remarks>Unity 2018.3.0以降 : Collisionを出力する場合は、Physics Settingsの「Reuse Collision Callbacks」を無効にする必要があります。</remarks>
#else
	/// <summary>
	/// It will transition the state when the OnCollisionEnter is called.
	/// </summary>
	/// <remarks>Unity 2018.3.0 or later : If you want to output Collision, you need to disable "Reuse Collision Callbacks" in Physics Settings.</remarks>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Transition/Collision/OnCollisionEnterTransition")]
	[BuiltInBehaviour]
	public sealed class OnCollisionEnterTransition : CheckTagBehaviourBase
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// Collisionの出力。
		/// </summary>
#else
		/// <summary>
		/// Collision output.
		/// </summary>
#endif
		[SerializeField]
		private OutputSlotCollision _Collision = new OutputSlotCollision();

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移先ステート。<br />
		/// 遷移メソッド : OnCollisionEnter
		/// </summary>
#else
		/// <summary>
		/// Transition destination state.<br />
		/// Transition Method : OnCollisionEnter
		/// </summary>
#endif
		[SerializeField]
		private StateLink _NextState = new StateLink();

		#endregion // Serialize fields

		void OnCollisionEnter(Collision collision)
		{
			if (!enabled)
			{
				return;
			}

			if (CheckTag(collision.gameObject))
			{
				PhysicsUtility.CheckReuseCollision(_Collision);
				_Collision.SetValue(collision);
				Transition(_NextState);
			}
		}
	}
}
