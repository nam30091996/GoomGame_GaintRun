//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なfloat型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible float type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleFloat : FlexiblePrimitiveBase
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private float _Value = 0f;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private FloatParameterReference _Parameter = new FloatParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private float _MinRange = 0f;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private float _MaxRange = 0f;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private InputSlotFloat _Slot = new InputSlotFloat();

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
		public float value
		{
			get
			{
				float value = 0;
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
		/// FlexibleFloatデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleFloat default constructor
		/// </summary>
#endif
		public FlexibleFloat()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFloatコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleFloat constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleFloat(float value)
		{
			_Type = FlexiblePrimitiveType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFloatコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleFloat constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleFloat(FloatParameterReference parameter)
		{
			_Type = FlexiblePrimitiveType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFloatコンストラクタ
		/// </summary>
		/// <param name="minRange">最小範囲。</param>
		/// <param name="maxRange">最大範囲。</param>
#else
		/// <summary>
		/// FlexibleFloat constructor
		/// </summary>
		/// <param name="minRange">Minimum range.</param>
		/// <param name="maxRange">Maximum range.</param>
#endif
		public FlexibleFloat(float minRange, float maxRange)
		{
			_Type = FlexiblePrimitiveType.Random;
			_MinRange = minRange;
			_MaxRange = maxRange;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFloatコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleFloat constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleFloat(InputSlotFloat slot)
		{
			_Type = FlexiblePrimitiveType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFloatをfloatにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleFloat</param>
#else
		/// <summary>
		/// Cast FlexibleFloat to float.
		/// </summary>
		/// <param name="flexible">FlexibleFloat</param>
#endif
		public static explicit operator float(FlexibleFloat flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// floatをFlexibleFloatにキャスト。
		/// </summary>
		/// <param name="value">float</param>
#else
		/// <summary>
		/// Cast float to FlexibleFloat.
		/// </summary>
		/// <param name="value">float</param>
#endif
		public static explicit operator FlexibleFloat(float value)
		{
			return new FlexibleFloat(value);
		}

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexiblePrimitiveType.DataSlot;
			_Slot.Copy(slot);
		}
	}
}
