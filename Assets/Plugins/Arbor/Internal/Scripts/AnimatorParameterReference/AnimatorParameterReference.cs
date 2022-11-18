//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Animatorパラメータの参照。
	/// </summary>
#else
	/// <summary>
	/// Reference Animator parameters.
	/// </summary>
#endif
	[System.Serializable]
	public class AnimatorParameterReference
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータが格納されているAnimator
		/// </summary>
#else
		/// <summary>
		/// Animator parameters are stored.
		/// </summary>
#endif
		public Animator animator;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの名前
		/// </summary>
#else
		/// <summary>
		/// Parameter name.
		/// </summary>
#endif
		public string name;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータのタイプ
		/// </summary>
#else
		/// <summary>
		/// Parameter type.
		/// </summary>
#endif
		public int type;
	}
}
