﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.ParameterBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Parameterに値を設定する。
	/// </summary>
#else
	/// <summary>
	/// Set a value for Parameter.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[BehaviourTitle("SetParameter")]
	[AddBehaviourMenu("Parameter/SetParameter")]
	[BuiltInBehaviour]
	public sealed class SetParameterBehaviour : SetParameterBehaviourInternal
	{
	}
}