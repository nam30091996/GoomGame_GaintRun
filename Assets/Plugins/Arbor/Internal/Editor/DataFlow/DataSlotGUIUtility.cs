//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;

	internal static class DataSlotGUIUtility
	{
		static System.Type GetInterfaceIListOfT(System.Type type)
		{
			var interfaces = type.GetInterfaces();
			if (interfaces == null)
			{
				return null;
			}

			for (int i = 0; i < interfaces.Length; i++)
			{
				var inter = interfaces[i];
				System.Type listType = Internal_GetIListOfT(inter);
				if (listType != null)
				{
					return listType;
				}
			}

			return null;
		}

		static System.Type Internal_GetIListOfT(System.Type type)
		{
			if (type.IsInterface)
			{
				if (TypeUtility.IsGeneric(type, typeof(IList<>)))
				{
					return type;
				}

				return GetInterfaceIListOfT(type);
			}
			else
			{
				for (var current = type; current != null; current = current.BaseType)
				{
					System.Type listType = GetInterfaceIListOfT(current);
					if (listType != null)
					{
						return listType;
					}
				}
			}

			return null;
		}

		static System.Type GetInterfaceIList(System.Type type)
		{
			var interfaces = type.GetInterfaces();
			if (interfaces == null)
			{
				return null;
			}

			for (int i = 0; i < interfaces.Length; i++)
			{
				var inter = interfaces[i];
				System.Type listType = Internal_GetIList(inter);
				if (listType != null)
				{
					return listType;
				}
			}

			return null;
		}

		static System.Type Internal_GetIList(System.Type type)
		{
			if (type.IsInterface)
			{
				if (type == typeof(IList))
				{
					return type;
				}

				return GetInterfaceIList(type);
			}
			else
			{
				for (var current = type; current != null; current = current.BaseType)
				{
					System.Type listType = GetInterfaceIList(current);
					if (listType != null)
					{
						return listType;
					}
				}
			}

			return null;
		}

		public static System.Type GetIList(System.Type type)
		{
			System.Type listType = Internal_GetIListOfT(type);
			if (listType != null)
			{
				return listType;
			}

			return Internal_GetIList(type);
		}

		public static bool IsList(System.Type type)
		{
			return GetIList(type) != null;
		}

		public static System.Type ElementType(System.Type type)
		{
			System.Type listType = GetIList(type);
			if (listType == null)
			{
				return type;
			}

			if (TypeUtility.IsGeneric(listType, typeof(IList<>)))
			{
				return TypeUtility.GetGenericArguments(listType)[0];
			}
			else
			{
				return typeof(object);
			}
		}
	}
}