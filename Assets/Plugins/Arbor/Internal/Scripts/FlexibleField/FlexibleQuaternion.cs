//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なQuaternion型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible Quaternion type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleQuaternion : FlexibleFieldBase
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[EulerAngles]
		[SerializeField]
		private Quaternion _Value = Quaternion.identity;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private QuaternionParameterReference _Parameter = new QuaternionParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private InputSlotQuaternion _Slot = new InputSlotQuaternion();

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
				return typeof(Quaternion);
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
		public Quaternion value
		{
			get
			{
				Quaternion value = Quaternion.identity;
				switch (_Type)
				{
					case FlexibleType.Constant:
						value = _Value;
						break;
					case FlexibleType.Parameter:
						value = _Parameter.value;
						break;
					case FlexibleType.DataSlot:
						_Slot.GetValue(ref value);
						break;
				}

				return value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleQuaternionデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleQuaternion default constructor
		/// </summary>
#endif
		public FlexibleQuaternion()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleQuaternionコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleQuaternion constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleQuaternion(Quaternion value)
		{
			_Type = FlexibleType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleQuaternionコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleQuaternion constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleQuaternion(QuaternionParameterReference parameter)
		{
			_Type = FlexibleType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleQuaternionコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleQuaternion constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleQuaternion(InputSlotQuaternion slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleQuaternionをQuaternionにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleQuaternion</param>
#else
		/// <summary>
		/// Cast FlexibleQuaternion to Quaternion.
		/// </summary>
		/// <param name="flexible">FlexibleQuaternion</param>
#endif
		public static explicit operator Quaternion(FlexibleQuaternion flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// QuaternionをFlexibleQuaternionにキャスト。
		/// </summary>
		/// <param name="value">Quaternion</param>
#else
		/// <summary>
		/// Cast Quaternion to FlexibleQuaternion.
		/// </summary>
		/// <param name="value">Quaternion</param>
#endif
		public static explicit operator FlexibleQuaternion(Quaternion value)
		{
			return new FlexibleQuaternion(value);
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

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot.Copy(slot);
		}
	}
}
