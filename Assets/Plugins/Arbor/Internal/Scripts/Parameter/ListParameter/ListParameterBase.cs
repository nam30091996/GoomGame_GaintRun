//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Listを扱うパラメータの基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class for parameters that handle List
	/// </summary>
#endif
	[System.Serializable]
	public abstract class ListParameterBase
	{
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
		public abstract bool SetList(object value);

#if ARBOR_DOC_JA
		/// <summary>
		/// Listのインスタンスオブジェクト
		/// </summary>
#else
		/// <summary>
		/// List instance object
		/// </summary>
#endif
		public abstract object listObject
		{
			get;
		}
	}

#if ARBOR_DOC_JA
	/// <summary>
	/// Listを扱うパラメータの基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class for parameters that handle List
	/// </summary>
#endif
	[System.Serializable]
	public abstract class ListParameterBaseInternal<T> : ListParameterBase, IValueContainer
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// Listのインスタンス
		/// </summary>
#else
		/// <summary>
		/// List instance
		/// </summary>
#endif
		protected abstract List<T> listInstance
		{
			get;
		}

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
				return listInstance;
			}
		}

		private bool Internal_SetList(IList<T> value)
		{
			List<T> list = listInstance;

			if (value == null || ListUtility.Equals(list, value))
			{
				return false;
			}

			list.Clear();
			for (int i = 0; i < value.Count; i++)
			{
				list.Add(value[i]);
			}

			return true;
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
		public bool SetList(IList<T> value)
		{
			return Internal_SetList(value);
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
			return Internal_SetList((IList<T>)value);
		}

		object IValueContainer.GetValue()
		{
			return listInstance;
		}
	}

#if ARBOR_DOC_JA
	/// <summary>
	/// Listを扱うパラメータの基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class for parameters that handle List
	/// </summary>
#endif
	[System.Serializable]
	public class ListParameterBase<T> : ListParameterBaseInternal<T>
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// Listのインスタンス
		/// </summary>
#else
		/// <summary>
		/// List instance
		/// </summary>
#endif
		public List<T> list = new List<T>();

#if ARBOR_DOC_JA
		/// <summary>
		/// Listのインスタンス
		/// </summary>
#else
		/// <summary>
		/// List instance
		/// </summary>
#endif
		protected override sealed List<T> listInstance
		{
			get
			{
				return list;
			}
		}
	}
}
