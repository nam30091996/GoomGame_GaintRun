﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;

namespace Arbor
{
	public sealed partial class Parameter
	{
		#region EnumList

		object Internal_GetEnumList()
		{
			if (type == Type.EnumList)
			{
				return container._EnumListParameters[_ParameterIndex].listObject;
			}

			return null;
		}

		void Internal_SetEnumList(object value, bool changeType)
		{
			if (changeType)
			{
				if (value == null)
				{
					return;
				}

				System.Type valueType = value.GetType();
				System.Type elementType = ListUtility.GetElementType(valueType);

				Internal_EnumListUpdateTypeIfNecessary(elementType);
			}
			if (container._EnumListParameters[_ParameterIndex].SetList(value))
			{
				DoChanged();
			}
		}

		void Internal_EnumListUpdateTypeIfNecessary(System.Type elementType)
		{
			if (elementType == null || !TypeUtility.IsEnum(elementType))
			{
				return;
			}

			var enumType = referenceType.type ?? typeof(System.Enum);
			if (elementType == enumType)
			{
				return;
			}

			var parameters = container._EnumListParameters[_ParameterIndex];

			parameters.OnBeforeSerialize();

			referenceType.type = elementType;

			parameters.OnAfterDeserialize(elementType);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// EnumList型の値を設定する。
		/// </summary>
		/// <param name="value">値。</param>
		/// <returns>値を設定できた場合にtrueを返す。</returns>
#else
		/// <summary>
		/// It wants to set the value of the EnumList type.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Return true if the value could be set.</returns>
#endif
		public bool SetEnumList<TEnum>(IList<TEnum> value) where TEnum : struct
		{
			if (type == Type.EnumList)
			{
				Internal_EnumListUpdateTypeIfNecessary(typeof(TEnum));
				Internal_SetEnumList(value, false);
				return true;
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// EnumList型の値を取得する。
		/// </summary>
		/// <param name="value">取得する値。</param>
		///  <returns>値を取得できた場合にtrueを返す。</returns>
#else
		/// <summary>
		/// Get the value of the EnumList type.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Return true if the value can be obtained.</returns>
#endif
		public bool TryGetEnumList<TEnum>(out IList<TEnum> value) where TEnum : struct
		{
			var enumType = referenceType.type ?? typeof(System.Enum);

			if (type == Type.EnumList && typeof(TEnum) == enumType)
			{
				value = (IList<TEnum>)container._EnumListParameters[_ParameterIndex].listObject;
				return true;
			}

			value = null;
			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// EnumList型の値を取得する。
		/// </summary>
		/// <returns>パラメータの値。パラメータがない場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get the value of the EnumList type.
		/// </summary>
		/// <returns>The value of the parameter. If there is no parameter, it returns null.</returns>
#endif
		public IList<TEnum> GetEnumList<TEnum>() where TEnum : struct
		{
			IList<TEnum> value;
			if (TryGetEnumList(out value))
			{
				return value;
			}
			return null;
		}

		#endregion //EnumList
	}
}