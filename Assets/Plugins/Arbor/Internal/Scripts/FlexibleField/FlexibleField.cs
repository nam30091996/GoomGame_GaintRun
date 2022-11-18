//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟な型を扱うための基本クラス。
	/// 使用するには<see cref="FlexibleField{T}"/>を参照してください。
	/// </summary>
#else
	/// <summary>
	/// A base class for dealing with flexible types with multiple reference methods.
	/// See <see cref="FlexibleField{T}"/> for use.
	/// </summary>
#endif
	[System.Serializable]
	public abstract class FlexibleFieldBase : IFlexibleField
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 値の指定タイプ
		/// </summary>
#else
		/// <summary>
		/// Specified type of value
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		protected FlexibleType _Type = FlexibleType.Constant;

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
		/// フィールドの型を返す。
		/// </summary>
#else
		/// <summary>
		/// It returns a field type.
		/// </summary>
#endif
		public abstract System.Type fieldType
		{
			get;
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
		public abstract object GetValueObject();

		object IValueContainer.GetValue()
		{
			return GetValueObject();
		}
	}

#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟な型を扱うクラス。
	/// 使用する場合は、Tにユーザー定義クラスを指定して継承してください。
	/// </summary>
	/// <typeparam name="T">シリアライズ可能な型</typeparam>
#else
	/// <summary>
	/// A base class for dealing with flexible types with multiple reference methods.
	/// To use it, inherit T by specifying a user-defined class.
	/// </summary>
	/// <typeparam name="T">Serializable type</typeparam>
#endif
	[System.Serializable]
	public class FlexibleField<T> : FlexibleFieldBase
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 固定値
		/// </summary>
#else
		/// <summary>
		/// Constant value
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		protected T _Value = default(T);

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータ参照
		/// </summary>
#else
		/// <summary>
		/// Parameter reference
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[ClassGenericArgument(0)]
		protected AnyParameterReference _Parameter = new AnyParameterReference();

#if ARBOR_DOC_JA
		/// <summary>
		/// データ入力スロット
		/// </summary>
#else
		/// <summary>
		/// Data input slot
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[ClassGenericArgument(0)]
		protected InputSlotAny _Slot = new InputSlotAny();

#if ARBOR_DOC_JA
		/// <summary>
		/// フィールドの型を返す。
		/// </summary>
#else
		/// <summary>
		/// It returns a field type.
		/// </summary>
#endif
		public override System.Type fieldType
		{
			get
			{
				return typeof(T);
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
		public T value
		{
			get
			{
				T value = default(T);
				switch (_Type)
				{
					case FlexibleType.Constant:
						value = _Value;
						break;
					case FlexibleType.Parameter:
						try
						{
							if (_Parameter != null)
							{
								object parameterValue = _Parameter.value;
								if (parameterValue != null)
								{
									value = (T)parameterValue;
								}
							}
						}
						catch (System.InvalidCastException ex)
						{
							Debug.LogException(ex);
						}
						break;
					case FlexibleType.DataSlot:
						_Slot.GetValue<T>(ref value);
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
		/// FlexibleFieldデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleField default constructor
		/// </summary>
#endif
		public FlexibleField()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFieldコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleField constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleField(T value)
		{
			_Type = FlexibleType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFieldコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleField constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleField(AnyParameterReference parameter)
		{
			_Type = FlexibleType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFieldコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleField constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleField(InputSlotAny slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleFieldをTにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleField</param>
#else
		/// <summary>
		/// Cast FlexibleField to T.
		/// </summary>
		/// <param name="flexible">FlexibleField</param>
#endif
		public static explicit operator T(FlexibleField<T> flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// TをFlexibleFieldにキャスト。
		/// </summary>
		/// <param name="value">T</param>
#else
		/// <summary>
		/// Cast T to FlexibleField.
		/// </summary>
		/// <param name="value">T</param>
#endif
		public static explicit operator FlexibleField<T>(T value)
		{
			return new FlexibleField<T>(value);
		}

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot.Copy(slot);
		}
	}
}