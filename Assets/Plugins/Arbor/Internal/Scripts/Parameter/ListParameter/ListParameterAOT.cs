//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Listを扱うパラメータのAOT対応クラス
	/// </summary>
#else
	/// <summary>
	/// AOT supported class for parameters that handle List
	/// </summary>
#endif
	public class ListParameterAOT : ListParameterBase, IValueContainer
	{
		private System.Type _ElementType;
		private ListAccessor _Accessor;
		private object _ListInstance;

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
				return _ListInstance;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ListParameterAOTコンストラクタ
		/// </summary>
		/// <param name="elementType">要素の型</param>
#else
		/// <summary>
		/// ListParameterAOT constructor
		/// </summary>
		/// <param name="elementType">Element type</param>
#endif
		public ListParameterAOT(System.Type elementType)
		{
			_ElementType = elementType;
			_Accessor = ListAccessor.Get(elementType);
			_ListInstance = _Accessor.NewList();
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
			if (value == null)
			{
				return false;
			}

			var valueType = value.GetType();
			if (!TypeUtility.IsAssignableFrom(ListUtility.GetIListType(_ElementType), valueType))
			{
				return false;
			}

			if (_Accessor.EqualsList(_ListInstance, value))
			{
				return false;
			}

			_Accessor.Clear(_ListInstance, ListInstanceType.Keep);
			int count = _Accessor.Count(value);
			for (int i = 0; i < count; i++)
			{
				_Accessor.AddElement(_ListInstance, _Accessor.GetElement(value, i), ListInstanceType.Keep);
			}

			return true;
		}

		object IValueContainer.GetValue()
		{
			return _ListInstance;
		}
	}
}