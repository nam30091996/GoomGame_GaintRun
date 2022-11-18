//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なenum型を扱うクラス。
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="ClassTypeConstraintAttribute" /></description></item>
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Class to handle a flexible enum type reference method there is more than one.
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="ClassTypeConstraintAttribute" /></description></item>
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class FlexibleEnumAny : IFlexibleField
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private FlexibleType _Type = FlexibleType.Constant;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private int _Value = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private AnyParameterReference _Parameter = new AnyParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private InputSlotAny _Slot = new InputSlotAny();

#if ARBOR_DOC_JA
		/// <summary>
		/// Typeを返す
		/// </summary>
#else
		/// <summary>
		/// It returns a type
		/// </summary>
#endif
		public FlexibleType type
		{
			get
			{
				return _Type;
			}
		}

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
				if (_Type == FlexibleType.Parameter)
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
					case FlexibleType.Constant:
						value = _Value;
						break;
					case FlexibleType.Parameter:
						Parameter parameter = _Parameter.parameter;
						if (parameter != null)
						{
							switch (parameter.type)
							{
								case Parameter.Type.Enum:
									value = parameter.enumIntValue;
									break;
								case Parameter.Type.Variable:
									object valueObj = parameter.GetVariable();
									if (valueObj != null)
									{
										value = (int)valueObj;
									}
									break;
							}
						}
						break;
					case FlexibleType.DataSlot:
						object valueObject = null;
						_Slot.GetValue(ref valueObject);
						if (valueObject != null && EnumFieldUtility.IsEnum(valueObject.GetType()))
						{
							value = (int)valueObject;
						}
						break;
				}

				return value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleEnumAny default constructor
		/// </summary>
#endif
		public FlexibleEnumAny()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleEnumAny constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleEnumAny(int value)
		{
			_Type = FlexibleType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleEnumAny constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleEnumAny(System.Enum value) : this(System.Convert.ToInt32(value))
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleEnumAny constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleEnumAny(AnyParameterReference parameter)
		{
			_Type = FlexibleType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleEnumAny constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleEnumAny(InputSlotAny slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleEnumAnyをintにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleEnumAny</param>
#else
		/// <summary>
		/// Cast FlexibleEnumAny to int.
		/// </summary>
		/// <param name="flexible">FlexibleEnumAny</param>
#endif
		public static explicit operator int(FlexibleEnumAny flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// intをFlexibleEnumAnyにキャスト。
		/// </summary>
		/// <param name="value">int</param>
#else
		/// <summary>
		/// Cast int to FlexibleEnumAny.
		/// </summary>
		/// <param name="value">int</param>
#endif
		public static explicit operator FlexibleEnumAny(int value)
		{
			return new FlexibleEnumAny(value);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// System.EnumをFlexibleEnumAnyにキャスト。
		/// </summary>
		/// <param name="value">System.Enum</param>
#else
		/// <summary>
		/// Cast System.Enum to FlexibleEnumAny.
		/// </summary>
		/// <param name="value">System.Enum</param>
#endif
		public static explicit operator FlexibleEnumAny(System.Enum value)
		{
			return new FlexibleEnumAny(value);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// enum型の値を返す。
		/// </summary>
		/// <typeparam name="T">enumの型</typeparam>
		/// <returns>enum型の値</returns>
#else
		/// <summary>
		/// Returns the enum type value.
		/// </summary>
		/// <typeparam name="T">Type of enum</typeparam>
		/// <returns>Value of enum type</returns>
#endif
		public T GetEnumValue<T>() where T : struct
		{
			return (T)System.Enum.ToObject(typeof(T), value);
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
		public object GetValueObject()
		{
			return value;
		}

		object IValueContainer.GetValue()
		{
			return GetValueObject();
		}

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot.Copy(slot);
		}
	}
}