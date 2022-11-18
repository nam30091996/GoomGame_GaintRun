﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// EnumのListParameter
	/// </summary>
#else
	/// <summary>
	/// ListParameter of Enum
	/// </summary>
#endif
	[System.Serializable]
	public sealed class EnumListParameter : ListParameterBase<int>
	{
		[System.NonSerialized]
		private System.Type _EnumType;

		[System.NonSerialized]
		private ListParameterBase _EnumListParameter;

#if ARBOR_DOC_JA
		/// <summary>
		/// Listのインスタンスオブジェクト
		/// </summary>
#else
		/// <summary>
		/// List instance object
		/// </summary>
#endif
		public override object listObject
		{
			get
			{
				if (_EnumListParameter != null)
				{
					return _EnumListParameter.listObject;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Listを設定する。
		/// </summary>
		/// <param name="value">設定するList</param>
#else
		/// <summary>
		/// Set the List.
		/// </summary>
		/// <param name="value">The List to set</param>
#endif
		public override bool SetList(object value)
		{
			if (_EnumListParameter == null)
			{
				return false;
			}

			if (_EnumListParameter.SetList(value))
			{
				OnBeforeSerialize();
				return true;
			}

			return false;
		}

		internal void OnAfterDeserialize(System.Type valueType)
		{
			bool isEnum = valueType != null && TypeUtility.IsEnum(valueType);
			if (!isEnum)
			{
				return;
			}

			ListAccessor accessor = ListAccessor.Get(valueType);

			if (valueType != _EnumType)
			{
				_EnumType = valueType;

				_EnumListParameter = ListParameterUtility.CreateInstance(valueType);
			}

			object listObject = _EnumListParameter.listObject;
			accessor.Clear(listObject, ListInstanceType.Keep);

			for (int i = 0; i < list.Count; i++)
			{
				accessor.AddElement(listObject, System.Enum.ToObject(valueType, list[i]), ListInstanceType.Keep);
			}
		}

		internal void OnBeforeSerialize()
		{
			bool isEnum = _EnumType != null && TypeUtility.IsEnum(_EnumType);
			if (!isEnum)
			{
				return;
			}
			if (_EnumListParameter == null)
			{
				UnityEngine.Debug.LogWarning("enumListParameter == null");
				return;
			}

			list.Clear();

			ListAccessor accessor = ListAccessor.Get(_EnumType);
			object listObject = _EnumListParameter.listObject;
			int count = accessor.Count(listObject);
			for (int i = 0; i < count; i++)
			{
				object value = accessor.GetElement(listObject, i);
				list.Add(System.Convert.ToInt32(value));
			}
		}
	}
}