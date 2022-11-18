//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なint型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible int type reference method there is more than one.
	/// </summary>
#endif

	[System.Serializable]
	public sealed class FlexibleInt : FlexiblePrimitiveBase
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private int _Value = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private IntParameterReference _Parameter = new IntParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private int _MinRange = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private int _MaxRange = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private InputSlotInt _Slot = new InputSlotInt();

#if ARBOR_DOC_JA
		/// <summary>
		/// Parameterを返す。TypeがParameter以外の場合はnull。
		/// </summary>
#else
		/// <summary>
		/// It return a Paramter. It is null if Type is other than Parameter.
		/// </summary>
#endif
		public Parameter parameter
		{
			get
			{
				if (_Type == FlexiblePrimitiveType.Parameter)
				{
					return _Parameter.parameter;
				}
				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を返す
		/// </summary>
#else
		/// <summary>
		/// It returns a value
		/// </summary>
#endif
		public int value
		{
			get
			{
				int value = 0;
				switch (_Type)
				{
					case FlexiblePrimitiveType.Constant:
						value = _Value;
						break;
					case FlexiblePrimitiveType.Parameter:
						value = _Parameter.value;
						break;
					case FlexiblePrimitiveType.Random:
						value = Random.Range(_MinRange, _MaxRange);
						break;
					case FlexiblePrimitiveType.DataSlot:
						_Slot.GetValue(ref value);
						break;
				}

				return value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットを返す
		/// </summary>
#else
		/// <summary>
		/// It returns a slot
		/// </summary>
#endif
		public InputSlotInt slot
		{
			get
			{
				return _Slot;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値をobjectで返す。
		/// </summary>
		/// <returns>値のobject</returns>
#else
		/// <summary>
		/// Return the value as object.
		/// </summary>
		/// <returns>The value object</returns>
#endif
		public override object GetValueObject()
		{
			return value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleInt default constructor
		/// </summary>
#endif
		public FlexibleInt()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleInt constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleInt(int value)
		{
			_Type = FlexiblePrimitiveType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleInt constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleInt(IntParameterReference parameter)
		{
			_Type = FlexiblePrimitiveType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntコンストラクタ
		/// </summary>
		/// <param name="minRange">最小範囲。</param>
		/// <param name="maxRange">最大範囲。</param>
#else
		/// <summary>
		/// FlexibleInt constructor
		/// </summary>
		/// <param name="minRange">Minimum range.</param>
		/// <param name="maxRange">Maximum range.</param>
#endif
		public FlexibleInt(int minRange, int maxRange)
		{
			_Type = FlexiblePrimitiveType.Random;
			_MinRange = minRange;
			_MaxRange = maxRange;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleInt constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleInt(InputSlotInt slot)
		{
			_Type = FlexiblePrimitiveType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleIntをintにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleInt</param>
#else
		/// <summary>
		/// Cast FlexibleInt to int.
		/// </summary>
		/// <param name="flexible">FlexibleInt</param>
#endif
		public static explicit operator int(FlexibleInt flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// intをFlexibleIntにキャスト。
		/// </summary>
		/// <param name="value">int</param>
#else
		/// <summary>
		/// Cast int to FlexibleInt.
		/// </summary>
		/// <param name="value">int</param>
#endif
		public static explicit operator FlexibleInt(int value)
		{
			return new FlexibleInt(value);
		}

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexiblePrimitiveType.DataSlot;
			_Slot.Copy(slot);
		}
	}
}
