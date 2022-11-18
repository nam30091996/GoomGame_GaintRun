//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なアセットObject型を扱うクラス。
	/// </summary>
#else
	/// <summary>
	/// Class to handle a flexible Asset Object type reference method there is more than one.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class FlexibleAssetObject : IFlexibleField
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		FlexibleType _Type = FlexibleType.Constant;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private Object _Value = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private AssetObjectParameterReference _Parameter = new AssetObjectParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[ClassAssetObject]
		private InputSlotUnityObject _Slot = new InputSlotUnityObject();

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
		public Object value
		{
			get
			{
				Object value = null;
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
		/// FlexibleAssetObjectデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleAssetObject default constructor
		/// </summary>
#endif
		public FlexibleAssetObject()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleAssetObjectコンストラクタ
		/// </summary>
		/// <param name="value">値</param>
#else
		/// <summary>
		/// FlexibleAssetObject constructor
		/// </summary>
		/// <param name="value">Value</param>
#endif
		public FlexibleAssetObject(Object value)
		{
			_Type = FlexibleType.Constant;
			_Value = value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleAssetObjectコンストラクタ
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// FlexibleAssetObject constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleAssetObject(AssetObjectParameterReference parameter)
		{
			_Type = FlexibleType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleAssetObjectコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleAssetObject constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleAssetObject(InputSlotUnityObject slot)
		{
			_Type = FlexibleType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleAssetObjectをObjectにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleAssetObject</param>
#else
		/// <summary>
		/// Cast FlexibleAssetObject to Object.
		/// </summary>
		/// <param name="flexible">FlexibleAssetObject</param>
#endif
		public static explicit operator Object(FlexibleAssetObject flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ObjectをFlexibleAssetObjectにキャスト。
		/// </summary>
		/// <param name="value">Object</param>
#else
		/// <summary>
		/// Cast Object to FlexibleAssetObject.
		/// </summary>
		/// <param name="value">Object</param>
#endif
		public static explicit operator FlexibleAssetObject(Object value)
		{
			return new FlexibleAssetObject(value);
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