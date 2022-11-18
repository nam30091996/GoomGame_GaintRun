//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Arbor
{
	using DynamicReflection;
	internal sealed class ListAccessorAOT : ListAccessor
	{
		private static class ExceptionMessage
		{
			public const string ArgumentOutOfRange_NeedNonNegNum = "Non-negative number required.";
			public const string ArgumentOutOfRange_BiggerThanCollection = "Larger than collection size.";
		}

		private System.Type _ElementType;
		private DynamicMethod _GetCountMethod;
		private DynamicMethod _GetElementMethod;
		private DynamicMethod _SetElementMethod;
		private DynamicMethod _AddMethod;
		private DynamicMethod _InsertMethod;
		private DynamicMethod _RemoveAtMethod;
		private DynamicMethod _ClearMethod;
		private DynamicMethod _ContainsMethod;
		private IEqualityComparer _Comparer;

		internal ListAccessorAOT(System.Type elementType)
		{
			_ElementType = elementType;
			var typeIList = ListUtility.GetIListType(_ElementType);
			var typeICollection = ListUtility.GetICollectionType(_ElementType);

			var countProperty = typeICollection.GetProperty("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			var getCountMethod = countProperty.GetGetMethod();
			_GetCountMethod = DynamicMethod.GetMethod(getCountMethod);

			var itemProperty = typeIList.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			var getElementMethod = itemProperty.GetGetMethod();
			_GetElementMethod = DynamicMethod.GetMethod(getElementMethod);
			var setElementMethod = itemProperty.GetSetMethod();
			_SetElementMethod = DynamicMethod.GetMethod(setElementMethod);

			var addMethod = typeICollection.GetMethod("Add", new System.Type[] { _ElementType });
			_AddMethod = DynamicMethod.GetMethod(addMethod);

			var insertMethod = typeIList.GetMethod("Insert", new System.Type[] { typeof(int), _ElementType });
			_InsertMethod = DynamicMethod.GetMethod(insertMethod);

			var removeAtMethod = typeIList.GetMethod("RemoveAt", new System.Type[] { typeof(int) });
			_RemoveAtMethod = DynamicMethod.GetMethod(removeAtMethod);

			var clearMethod = typeICollection.GetMethod("Clear", new System.Type[] { });
			_ClearMethod = DynamicMethod.GetMethod(clearMethod);

			var containsMethod = typeICollection.GetMethod("Contains", new System.Type[] { _ElementType });
			_ContainsMethod = DynamicMethod.GetMethod(containsMethod);

			var comparerType = typeof(EqualityComparer<>).MakeGenericType(_ElementType);
			var defaultProperty = comparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
			_Comparer = (IEqualityComparer)defaultProperty.GetValue(null, null);
		}

		private int Internal_GetCount(object instance)
		{
			try
			{
				return (int)_GetCountMethod.Invoke(instance, null);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private object Internal_GetElement(object instance, int index)
		{
			try
			{
				return _GetElementMethod.Invoke(instance, new object[] { index });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Internal_SetElement(object instance, int index, object element)
		{
			try
			{
				_SetElementMethod.Invoke(instance, new object[] { index, element });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Internal_AddElement(object instance, object element)
		{
			try
			{
				_AddMethod.Invoke(instance, new object[] { element });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Internal_InsertElement(object instance, int index, object element)
		{
			try
			{
				_InsertMethod.Invoke(instance, new object[] { index, element });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Internal_RemoveAt(object instance, int index)
		{
			try
			{
				_RemoveAtMethod.Invoke(instance, new object[] { index });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Internal_Clear(object instance)
		{
			try
			{
				_ClearMethod.Invoke(instance, null);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private bool Internal_Contains(object instance, object element)
		{
			try
			{
				return (bool)_ContainsMethod.Invoke(instance, new object[] { element });
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Copy(object sourceList, int sourceIndex, object destinationList, int destinationIndex, int length)
		{
			int sourceCount = Internal_GetCount(sourceList);
			if (sourceIndex < 0)
			{
				throw new System.ArgumentOutOfRangeException("sourceIndex", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (sourceIndex >= sourceCount)
			{
				throw new System.ArgumentOutOfRangeException("sourceIndex", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			int destinationCount = Internal_GetCount(destinationList);
			if (destinationIndex < 0)
			{
				throw new System.ArgumentOutOfRangeException("destinationIndex", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (destinationIndex >= destinationCount)
			{
				throw new System.ArgumentOutOfRangeException("destinationIndex", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			if (length < 0)
			{
				throw new System.ArgumentOutOfRangeException("length", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (length > sourceCount - sourceIndex ||
				length > destinationCount - destinationIndex)
			{
				throw new System.ArgumentOutOfRangeException("length", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			for (int i = 0; i < length; i++)
			{
				var element = Internal_GetElement(sourceList, i + sourceIndex);
				Internal_SetElement(destinationList, i + destinationIndex, element );
			}
		}

		private void CheckInstance(object instance, string instanceName)
		{
			if (instance == null || !TypeUtility.IsAssignableFrom(ListUtility.GetIListType(_ElementType), instance.GetType()))
			{
				throw new System.ArgumentNullException(instanceName);
			}
		}

		private void CheckIndex(object instance, int index)
		{
			if (index < 0)
			{
				throw new System.ArgumentOutOfRangeException("index", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}

			int count = Internal_GetCount(instance);
			if (index >= count)
			{
				throw new System.ArgumentOutOfRangeException("index", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}
		}

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
		public override object NewList()
		{
			return System.Activator.CreateInstance(ListUtility.GetListType(_ElementType));
		}

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
		public override object GetElement(object instance, int index)
		{
			CheckInstance(instance, "instance");
			CheckIndex(instance, index);

			return Internal_GetElement(instance, index);
		}

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
		public override object SetElement(object instance, int index, object element, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");
			CheckIndex(instance, index);

			switch (instanceType)
			{
				case ListInstanceType.Keep:
					{
						Internal_SetElement(instance, index, element);
						return instance;
					}
				case ListInstanceType.NewArray:
					{
						object newArray = Internal_ToArray(instance);
						Internal_SetElement(newArray, index, element);
						return newArray;
					}
				case ListInstanceType.NewList:
					{
						object newList = Internal_ToList(instance);
						Internal_SetElement(newList, index, element);
						return newList;
					}
			}

			return instance;
		}

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
		public override object AddElement(object instance, object element, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");

			switch (instanceType)
			{
				case ListInstanceType.Keep:
					{
						Internal_AddElement(instance, element);
						return instance;
					}
				case ListInstanceType.NewArray:
					{
						int count = Internal_GetCount(instance);
						object newArray = System.Array.CreateInstance(_ElementType, count + 1);

						Copy(instance, 0, newArray, 0, count);

						Internal_SetElement(newArray, count, element);
						return newArray;
					}
				case ListInstanceType.NewList:
					{
						object newList = Internal_ToList(instance);

						Internal_AddElement(newList, element);
						return newList;
					}
			}

			return instance;
		}

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
		public override object InsertElement(object instance, int index, object element, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");

			switch (instanceType)
			{
				case ListInstanceType.Keep:
					{
						Internal_InsertElement(instance, index, element);
						return instance;
					}
				case ListInstanceType.NewArray:
					{
						int count = Internal_GetCount(instance);
						object newArray = System.Array.CreateInstance(_ElementType, count + 1);
						Copy(instance, 0, newArray, 0, index);
						Internal_SetElement(newArray, index, element);
						if (index < count)
						{
							Copy(instance, index, newArray, index + 1, count - index);
						}

						return newArray;
					}
				case ListInstanceType.NewList:
					{
						object newList = Internal_ToList(instance);
						Internal_InsertElement(newList, index, element);
						return newList;
					}
			}

			return instance;
		}

		private object Internal_RemoveAtIndex(object instance, int index, ListInstanceType instanceType)
		{
			switch (instanceType)
			{
				case ListInstanceType.Keep:
					{
						Internal_RemoveAt(instance, index);
						return instance;
					}
				case ListInstanceType.NewArray:
					{
						CheckIndex(instance, index);

						int count = Internal_GetCount(instance);
						int newCount = count - 1;
						object array = System.Array.CreateInstance(_ElementType, newCount);
						Copy(instance, 0, array, 0, index);
						if (index + 1 < count)
						{
							Copy(instance, index + 1, array, index, count - (index + 1));
						}

						return array;
					}
				case ListInstanceType.NewList:
					{
						object newList = Internal_ToList(instance);
						Internal_RemoveAt(newList, index);
						return newList;
					}
			}

			return instance;
		}

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
		public override object RemoveElement(object instance, object element, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");

			int index = IndexOf(instance, element);
			if (index >= 0)
			{
				return Internal_RemoveAtIndex(instance, index, instanceType);
			}
			else
			{
				switch (instanceType)
				{
					case ListInstanceType.Keep:
						return instance;
					case ListInstanceType.NewArray:
						return Internal_ToArray(instance);
					case ListInstanceType.NewList:
						return Internal_ToList(instance);
				}
			}

			return instance;
		}

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
		public override object RemoveAtIndex(object instance, int index, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");

			return Internal_RemoveAtIndex(instance, index, instanceType);
		}

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
		public override object Clear(object instance, ListInstanceType instanceType)
		{
			CheckInstance(instance, "instance");

			switch (instanceType)
			{
				case ListInstanceType.Keep:
					{
						Internal_Clear(instance);
						return instance;
					}
				case ListInstanceType.NewArray:
					{
						int count = Internal_GetCount(instance);
						var newArray = System.Array.CreateInstance(_ElementType, count);
						System.Array.Clear(newArray, 0, count);
						return newArray;
					}
				case ListInstanceType.NewList:
					{
						return NewList();
					}
			}

			return instance;
		}

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
		public override bool Contains(object instance, object element)
		{
			CheckInstance(instance, "instance");

			return Internal_Contains(instance, element);
		}

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
		public override int Count(object instance)
		{
			CheckInstance(instance, "instance");

			return Internal_GetCount(instance);
		}

		private int Internal_IndexOf(object instance, object element, int startIndex, int count)
		{
			int listCount = Internal_GetCount(instance);
			if (listCount == 0)
			{
				return -1;
			}

			CheckIndex(instance, startIndex);
			if (count < 0)
			{
				throw new System.ArgumentOutOfRangeException("count", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count > listCount - startIndex)
			{
				throw new System.ArgumentOutOfRangeException("count", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			int num = startIndex + count;
			for (int i = startIndex; i < num; ++i)
			{
				var value = Internal_GetElement(instance, i);
				if (_Comparer.Equals(value, element))
				{
					return i;
				}
			}

			return -1;
		}

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
		public override int IndexOf(object instance, object element)
		{
			CheckInstance(instance, "instance");

			return Internal_IndexOf(instance, element, 0, Internal_GetCount(instance));
		}

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
		public override int IndexOf(object instance, object element, int startIndex)
		{
			CheckInstance(instance, "instance");

			return Internal_IndexOf(instance, element, startIndex, Internal_GetCount(instance) - startIndex);
		}

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
		public override int IndexOf(object instance, object element, int startIndex, int count)
		{
			CheckInstance(instance, "instance");

			return Internal_IndexOf(instance, element, startIndex, count);
		}

		private int Internal_LastIndexOf(object instance, object element, int startIndex, int count)
		{
			int listCount = Internal_GetCount(instance);
			if (listCount == 0)
			{
				return -1;
			}

			if (startIndex < 0)
			{
				throw new System.ArgumentOutOfRangeException("index", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (startIndex >= listCount)
			{
				throw new System.ArgumentOutOfRangeException("index", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			if (count < 0)
			{
				throw new System.ArgumentOutOfRangeException("count", ExceptionMessage.ArgumentOutOfRange_NeedNonNegNum);
			}
			if (count > startIndex + 1)
			{
				throw new System.ArgumentOutOfRangeException("count", ExceptionMessage.ArgumentOutOfRange_BiggerThanCollection);
			}

			int num = startIndex - count + 1;
			for (int i = startIndex; i >= num; --i)
			{
				object value = Internal_GetElement(instance, i);
				if (_Comparer.Equals(value, element))
				{
					return i;
				}
			}

			return -1;
		}

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
		public override int LastIndexOf(object instance, object element)
		{
			CheckInstance(instance, "instance");

			int count = Internal_GetCount(instance);

			return Internal_LastIndexOf(instance, element, count - 1, count);
		}

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
		public override int LastIndexOf(object instance, object element, int startIndex)
		{
			return Internal_LastIndexOf(instance, element, startIndex, startIndex + 1);
		}

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
		public override int LastIndexOf(object instance, object element, int startIndex, int count)
		{
			CheckInstance(instance, "instance");

			return Internal_LastIndexOf(instance, element, startIndex, count);
		}

		public override bool EqualsList(object a, object b)
		{
			if (object.ReferenceEquals(a, b))
			{
				return true;
			}

			if (a == null || b == null)
			{
				return false;
			}

			CheckInstance(a, "a");
			CheckInstance(b, "b");

			int aCount = Internal_GetCount(a);
			int bCount = Internal_GetCount(b);
			if(aCount != bCount)
			{
				return false;
			}

			for (int i = 0; i < aCount; i++)
			{
				object aValue = Internal_GetElement(a, i);
				object bValue = Internal_GetElement(b, i);
				if (!_Comparer.Equals(aValue, bValue))
				{
					return false;
				}
			}

			return true;
		}

		private object Internal_ToArray(object instance)
		{
			int count = Internal_GetCount(instance);
			var newArray = System.Array.CreateInstance(_ElementType, count);
			Copy(instance, 0, newArray, 0, count);

			return newArray;
		}

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
		public override object ToArray(object instance)
		{
			CheckInstance(instance, "instance");

			return Internal_ToArray(instance);
		}

		private object Internal_ToList(object instance)
		{
			return System.Activator.CreateInstance(ListUtility.GetListType(_ElementType), instance);
		}

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
		public override object ToList(object instance)
		{
			CheckInstance(instance, "instance");

			return Internal_ToList(instance);
		}
	}
}
