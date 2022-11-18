//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Listのユーティリティクラス
	/// </summary>
#else
	/// <summary>
	/// List utility class
	/// </summary>
#endif
	public static class ListUtility
	{
		static Dictionary<System.Type, System.Type> s_IListTypeCache = new Dictionary<System.Type, System.Type>();
		static Dictionary<System.Type, System.Type> s_ICollectionTypeCache = new Dictionary<System.Type, System.Type>();
		static Dictionary<System.Type, System.Type> s_ListTypeCache = new Dictionary<System.Type, System.Type>();

#if ARBOR_DOC_JA
		/// <summary>
		/// IList&lt;T&gt;が等しいかどうかを判断する。
		/// </summary>
		/// <typeparam name="T">要素の型</typeparam>
		/// <param name="a">判定するIList&lt;T&gt;</param>
		/// <param name="b">判定するIList&lt;T&gt;</param>
		/// <param name="comparer">等価比較を行うインターフェイス。nullを指定した場合はデフォルトの判定を使用する。</param>
		/// <returns>等しい場合にtrueを返す。</returns>
#else
		/// <summary>
		/// Determine if IList&lt;T&gt; are equal.
		/// </summary>
		/// <typeparam name="T">Element type</typeparam>
		/// <param name="a">IList&lt;T&gt; to judge</param>
		/// <param name="b">IList&lt;T&gt; to judge</param>
		/// <param name="comparer">Interface for equality comparison. When null is specified, default judgment is used.</param>
		/// <returns>Returns true if they are equal.</returns>
#endif
		public static bool Equals<T>(IList<T> a, IList<T> b, IEqualityComparer<T> comparer = null)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}

			if (a == null || b == null || a.Count != b.Count)
			{
				return false;
			}

			if (comparer == null)
			{
				comparer = EqualityComparer<T>.Default;
			}

			for (int i = 0; i < a.Count; i++)
			{
				if (!comparer.Equals(a[i], b[i]))
				{
					return false;
				}
			}

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素の型を取得する。
		/// </summary>
		/// <param name="listType">リストの型</param>
		/// <returns>要素の型を返す。</returns>
#else
		/// <summary>
		/// Get the element type.
		/// </summary>
		/// <param name="listType">List type</param>
		/// <returns>Returns the element type.</returns>
#endif
		public static System.Type GetElementType(System.Type listType)
		{
			if (TypeUtility.IsGeneric(listType, typeof(IList<>)) || TypeUtility.IsGeneric(listType, typeof(List<>)))
			{
				return TypeUtility.GetGenericArguments(listType)[0];
			}
			else if (listType.IsArray && listType.GetArrayRank() == 1)
			{
				return listType.GetElementType();
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// IList&lt;elementType&gt;の型を取得する。
		/// </summary>
		/// <param name="elementType">要素の型。</param>
		/// <returns>IList&lt;elementType&gt;の型を返す。</returns>
#else
		/// <summary>
		/// Get the type of IList&lt;elementType&gt;.
		/// </summary>
		/// <param name="elementType">Element type</param>
		/// <returns>Returns the type of IList&lt;elementType&gt;.</returns>
#endif
		public static System.Type GetIListType(System.Type elementType)
		{
			if (elementType == null)
			{
				return null;
			}

			System.Type type = null;
			if (!s_IListTypeCache.TryGetValue(elementType, out type))
			{
				type = typeof(IList<>).MakeGenericType(elementType);
				s_IListTypeCache.Add(elementType, type);
			}

			return type;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ICollection&lt;elementType&gt;の型を取得する。
		/// </summary>
		/// <param name="elementType">要素の型。</param>
		/// <returns>ICollection&lt;elementType&gt;の型を返す。</returns>
#else
		/// <summary>
		/// Get the type of ICollection&lt;elementType&gt;.
		/// </summary>
		/// <param name="elementType">Element type</param>
		/// <returns>Returns the type of ICollection&lt;elementType&gt;.</returns>
#endif
		public static System.Type GetICollectionType(System.Type elementType)
		{
			if (elementType == null)
			{
				return null;
			}

			System.Type type = null;
			if (!s_ICollectionTypeCache.TryGetValue(elementType, out type))
			{
				type = typeof(ICollection<>).MakeGenericType(elementType);
				s_ICollectionTypeCache.Add(elementType, type);
			}

			return type;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// List&lt;elementType&gt;の型を取得する。
		/// </summary>
		/// <param name="elementType">要素の型。</param>
		/// <returns>List&lt;elementType&gt;の型を返す。</returns>
#else
		/// <summary>
		/// Get the type of List&lt;elementType&gt;.
		/// </summary>
		/// <param name="elementType">Element type</param>
		/// <returns>Returns the type of List&lt;elementType&gt;.</returns>
#endif
		public static System.Type GetListType(System.Type elementType)
		{
			if (elementType == null)
			{
				return null;
			}

			System.Type type = null;
			if (!s_ListTypeCache.TryGetValue(elementType, out type))
			{
				type = typeof(List<>).MakeGenericType(elementType);
				s_ListTypeCache.Add(elementType, type);
			}

			return type;
		}
	}
}