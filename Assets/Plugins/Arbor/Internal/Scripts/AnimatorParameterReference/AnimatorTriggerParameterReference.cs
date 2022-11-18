﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Animatorのbool型パラメータの参照。
	/// </summary>
#else
	/// <summary>
	/// A reference to the Boolean parameter of Animator.
	/// </summary>
#endif
	[System.Serializable]
	[Internal.AnimatorParameterType(AnimatorControllerParameterType.Trigger)]
	public sealed class AnimatorTriggerParameterReference : AnimatorParameterReference
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// トリガーをセットする。
		/// </summary>
#else
		/// <summary>
		/// Set the trigger.
		/// </summary>
#endif
		public void Set()
		{
			if (animator != null)
			{
				animator.SetTrigger(name);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// トリガーをリセットする。
		/// </summary>
#else
		/// <summary>
		/// Reset the trigger.
		/// </summary>
#endif
		public void Reset()
		{
			if (animator != null)
			{
				animator.ResetTrigger(name);
			}
		}
	}
}
