﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Calculators
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 2つのfloat値から最大値を計算する。
	/// </summary>
#else
	/// <summary>
	/// Calculate the maximum value from two float values.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Mathf/Mathf.MaxFloat")]
	[BehaviourTitle("Mathf.MaxFloat")]
	[BuiltInBehaviour]
	public sealed class MathfMaxFloatCalculator : Calculator
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 値A
		/// </summary>
#else
		/// <summary>
		/// Value A
		/// </summary>
#endif
		[SerializeField] private FlexibleFloat _A = new FlexibleFloat();

#if ARBOR_DOC_JA
		/// <summary>
		/// 値B
		/// </summary>
#else
		/// <summary>
		/// Value B
		/// </summary>
#endif
		[SerializeField] private FlexibleFloat _B = new FlexibleFloat();

#if ARBOR_DOC_JA
		/// <summary>
		/// 結果出力
		/// </summary>
#else
		/// <summary>
		/// Result output
		/// </summary>
#endif
		[SerializeField] private OutputSlotFloat _Result = new OutputSlotFloat();

		#endregion // Serialize fields

		// Use this for calculate
		public override void OnCalculate()
		{
			_Result.SetValue(Mathf.Max(_A.value, _B.value));
		}
	}
}
