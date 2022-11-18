//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ランタイムに生成したIList&lt;&gt;へのアクセスを行う。
	/// </summary>
#else
	/// <summary>
	/// Access IList&lt;&gt; generated at runtime.
	/// </summary>
#endif
	public abstract class ListAccessor
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// Listインスタンスを生成する。
		/// </summary>
		/// <returns>Listインスタンス</returns>
#else
		/// <summary>
		/// Create a List instance.
		/// </summary>
		/// <returns>List instance</returns>
#endif
		public abstract object NewList();

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を取得する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="index">インデックス</param>
		/// <returns>indexに格納されている要素を返す</returns>
#else
		/// <summary>
		/// Get an element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="index">Index</param>
		/// <returns>returns the element stored at index</returns>
#endif
		public abstract object GetElement(object instance, int index);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を設定する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="index">インデックス</param>
		/// <param name="element">設定する要素</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Set the element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="index">Index</param>
		/// <param name="element">The element to set</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object SetElement(object instance, int index, object element, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を追加する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">追加する要素</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Add an element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">The element to add</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object AddElement(object instance, object element, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を挿入する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="index">インデックス</param>
		/// <param name="element">挿入する要素</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Insert an element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="index">Index</param>
		/// <param name="element">The element to insert</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object InsertElement(object instance, int index, object element, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を削除する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">削除する要素</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Remove an element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">The element to remove</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object RemoveElement(object instance, object element, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を削除する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="index">インデックス</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Remove an element.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="index">Index</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object RemoveAtIndex(object instance, int index, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素をすべて削除する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="instanceType">インスタンスの変更方法</param>
		/// <returns>変更した結果のインスタンス</returns>
#else
		/// <summary>
		/// Remove all elements.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="instanceType">How to change the instance</param>
		/// <returns>The resulting instance of the change</returns>
#endif
		public abstract object Clear(object instance, ListInstanceType instanceType);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が含まれているか判断する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <returns>含まれている場合にtrueを返す。</returns>
#else
		/// <summary>
		/// To determine if it contains elements.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <returns>rReturns true if it contains.</returns>
#endif
		public abstract bool Contains(object instance, object element);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素数を取得する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <returns>要素数</returns>
#else
		/// <summary>
		/// Get the number of elements.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <returns>Element count</returns>
#endif
		public abstract int Count(object instance);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを取得する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Gets the index where the element is stored.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int IndexOf(object instance, object element);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを取得する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <param name="startIndex">開始インデックス</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Gets the index where the element is stored.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <param name="startIndex">Start index</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int IndexOf(object instance, object element, int startIndex);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを取得する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <param name="startIndex">開始インデックス</param>
		/// <param name="count">個数</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Gets the index where the element is stored.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <param name="startIndex">Start index</param>
		/// <param name="count">Count</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int IndexOf(object instance, object element, int startIndex, int count);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを末尾から検索する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Search the index where the element is stored from the end.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int LastIndexOf(object instance, object element);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを末尾から検索する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <param name="startIndex">開始インデックス</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Search the index where the element is stored from the end.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <param name="startIndex">Start index</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int LastIndexOf(object instance, object element, int startIndex);

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素が格納されているインデックスを末尾から検索する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <param name="element">要素</param>
		/// <param name="startIndex">開始インデックス</param>
		/// <param name="count">個数</param>
		/// <returns>要素が格納されているインデックス。要素がなかった場合は-1を返す。</returns>
#else
		/// <summary>
		/// Search the index where the element is stored from the end.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <param name="element">Element</param>
		/// <param name="startIndex">Start index</param>
		/// <param name="count">Count</param>
		/// <returns>The index where the element is stored. Returns -1 if there is no element.</returns>
#endif
		public abstract int LastIndexOf(object instance, object element, int startIndex, int count);

#if ARBOR_DOC_JA
		/// <summary>
		/// IList&lt;T&gt;が等しいかどうかを判断する。
		/// </summary>
		/// <param name="a">判定するIList&lt;T&gt;</param>
		/// <param name="b">判定するIList&lt;T&gt;</param>
		/// <returns>等しい場合にtrueを返す。</returns>
#else
		/// <summary>
		/// Determine if IList&lt;T&gt; are equal.
		/// </summary>
		/// <param name="a">IList&lt;T&gt; to judge</param>
		/// <param name="b">IList&lt;T&gt; to judge</param>
		/// <returns>Returns true if they are equal.</returns>
#endif
		public abstract bool EqualsList(object a, object b);

#if ARBOR_DOC_JA
		/// <summary>
		/// 配列に変換する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <returns>変換結果の配列</returns>
#else
		/// <summary>
		/// Convert to an array.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <returns>Array of conversion results</returns>
#endif
		public abstract object ToArray(object instance);

#if ARBOR_DOC_JA
		/// <summary>
		/// Listに変換する。
		/// </summary>
		/// <param name="instance">IList&lt;elementType&gt;型のインスタンス</param>
		/// <returns>変換結果のList</returns>
#else
		/// <summary>
		/// Convert to List.
		/// </summary>
		/// <param name="instance">Instance of type IList&lt;elementType&gt;</param>
		/// <returns>List of conversion results</returns>
#endif
		public abstract object ToList(object instance);

		static Dictionary<System.Type, ListAccessor> s_Accessors = new Dictionary<System.Type, ListAccessor>();

#if ARBOR_DOC_JA
		/// <summary>
		/// 指定した要素の型のListAccessorを取得する。
		/// </summary>
		/// <param name="elementType">要素の型</param>
		/// <returns>IList&lt;elementType&gt;にアクセスするListAccessor</returns>
#else
		/// <summary>
		/// Get ListAccessor of specified element type.
		/// </summary>
		/// <param name="elementType">Element type</param>
		/// <returns>ListAccessor to access IList&lt;elementType&gt;</returns>
#endif
		public static ListAccessor Get(System.Type elementType)
		{
			ListAccessor accessor = null;
			if (elementType != null && !s_Accessors.TryGetValue(elementType, out accessor))
			{
				accessor = new ListAccessorAOT(elementType);
				s_Accessors.Add(elementType, accessor);
			}

			return accessor;
		}
	}
}