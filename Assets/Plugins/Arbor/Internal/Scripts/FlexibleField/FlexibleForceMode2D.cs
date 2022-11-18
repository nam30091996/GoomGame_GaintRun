//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なForceMode2D型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible ForceMode2D type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleForceMode2D : FlexibleField<ForceMode2D>
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleForceMode2Dデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleForceMode2D default constructor
		/// </summary>
#endif
		public FlexibleForceMode2D()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleForceMode2Dコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleForceMode2D constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleForceMode2D(ForceMode2D value) : base(value)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleForceMode2Dコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleForceMode2D constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleForceMode2D(AnyParameterReference parameter) : base(parameter)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleForceMode2Dコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleForceMode2D constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleForceMode2D(InputSlotAny slot) : base(slot)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleForceMode2DをForceMode2Dにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleForceMode2D</param>
#else
		/// <summary>
		/// Cast FlexibleForceMode2D to ForceMode2D.
		/// </summary>
		/// <param name="flexible">FlexibleForceMode2D</param>
#endif
		public static explicit operator ForceMode2D(FlexibleForceMode2D flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ForceMode2DをFlexibleForceMode2Dにキャスト。
		/// </summary>
		/// <param name="value">ForceMode2D</param>
#else
		/// <summary>
		/// Cast ForceMode2D to FlexibleForceMode2D.
		/// </summary>
		/// <param name="value">ForceMode2D</param>
#endif
		public static explicit operator FlexibleForceMode2D(ForceMode2D value)
		{
			return new FlexibleForceMode2D(value);
		}
	}
}