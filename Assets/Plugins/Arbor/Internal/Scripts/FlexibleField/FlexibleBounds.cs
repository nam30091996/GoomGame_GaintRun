//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なBounds型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible Bounds type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleBounds : FlexibleField<Bounds>
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleBoundsデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleBounds default constructor
		/// </summary>
#endif
		public FlexibleBounds() : base()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleBoundsコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleBounds constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleBounds(Bounds value) : base(value)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleBoundsコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleBounds constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleBounds(BoundsParameterReference parameter) : base(new AnyParameterReference(parameter))
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleBoundsコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleBounds constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleBounds(InputSlotBounds slot) : base(new InputSlotAny(slot))
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleBoundsをBoundsにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleBounds</param>
#else
		/// <summary>
		/// Cast FlexibleBounds to Bounds.
		/// </summary>
		/// <param name="flexible">FlexibleBounds</param>
#endif
		public static explicit operator Bounds(FlexibleBounds flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// BoundsをFlexibleBoundsにキャスト。
		/// </summary>
		/// <param name="value">Bounds</param>
#else
		/// <summary>
		/// Cast Bounds to FlexibleBounds.
		/// </summary>
		/// <param name="value">Bounds</param>
#endif
		public static explicit operator FlexibleBounds(Bounds value)
		{
			return new FlexibleBounds(value);
		}
	}
}
