﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;
	using Arbor.Serialization;

	public static class VariableEditorUtility
	{
		private class VariableTypeInfo
		{
			public System.Type variableType
			{
				get;
				private set;
			}

			public System.Type dataType
			{
				get;
				private set;
			}

			public bool isList
			{
				get;
				private set;
			}

			public System.Type valueType
			{
				get;
				private set;
			}

			public string menuName
			{
				get;
				private set;
			}

			public bool isSerializable
			{
				get;
				private set;
			}

			public VariableTypeInfo(System.Type variableType, System.Type dataType, string menuName)
			{
				this.variableType = variableType;
				this.dataType = dataType;
				this.menuName = menuName;

				this.isList = TypeUtility.IsGeneric(dataType, typeof(IList<>));
				this.valueType = this.isList ? TypeUtility.GetGenericArguments(dataType)[0] : dataType;
				this.isSerializable = SerializationUtility.IsSerializableFieldType(variableType);
			}
		}

		private static List<VariableTypeInfo> s_VariableTypeInfos = new List<VariableTypeInfo>();
		private static List<VariableTypeInfo> s_VariableListTypeInfos = new List<VariableTypeInfo>();

		static VariableEditorUtility()
		{
			HashSet<System.Type> alreadyTypes = new HashSet<System.Type>();

			foreach (MonoScript script in MonoImporter.GetAllRuntimeMonoScripts())
			{
				if (script == null || script.hideFlags != 0)
				{
					continue;
				}

				System.Type classType = script.GetClass();
				if (classType == null)
				{
					continue;
				}

				if (alreadyTypes.Contains(classType))
				{
					continue;
				}
				alreadyTypes.Add(classType);

				if (classType.IsSubclassOf(typeof(VariableBase)))
				{
					System.Type dataType = VariableBase.GetDataType(classType);
					if (dataType == null)
					{
						continue;
					}

					string menuName = GetVariableMenu(classType);
					if (string.IsNullOrEmpty(menuName))
					{
						continue;
					}

					s_VariableTypeInfos.Add(new VariableTypeInfo(classType, dataType, menuName));
				}
				else if (classType.IsSubclassOf(typeof(VariableListBase)))
				{
					System.Type dataType = VariableListBase.GetDataType(classType);
					if (dataType == null)
					{
						continue;
					}

					string menuName = GetVariableListMenu(classType);
					if (string.IsNullOrEmpty(menuName))
					{
						continue;
					}

					s_VariableListTypeInfos.Add(new VariableTypeInfo(classType, dataType, menuName));
				}
			}
		}

		public static bool IsVariableList(System.Type classType)
		{
			System.Type baseType = classType;
			while (baseType != null)
			{
				if (TypeUtility.IsGeneric(baseType, typeof(VariableList<>)))
				{
					return true;
				}
				baseType = baseType.BaseType;
			}

			return false;
		}

		public static string GetVariableName(System.Type classType)
		{
			System.Type dataType = VariableBase.GetDataType(classType);
			return TypeUtility.GetTypeName(dataType);
		}

		public static System.Type GetVariableTypeFromDataType(System.Type dataType)
		{
			foreach (var variableInfo in s_VariableTypeInfos)
			{
				if (variableInfo.dataType == dataType)
				{
					return variableInfo.variableType;
				}
			}

			return null;
		}

		public static string GetVariableMenu(System.Type classType)
		{
			AddVariableMenu addVariableMenu = AttributeHelper.GetAttribute<AddVariableMenu>(classType);

			string menuName = null;
			if (addVariableMenu != null)
			{
				menuName = addVariableMenu.menuName;
			}
			else
			{
				menuName = GetVariableName(classType);
			}

			if (string.IsNullOrEmpty(menuName))
			{
				return null;
			}

			return menuName;
		}

		public static void GenerateVariableMenus(GenericMenu genericMenu, System.Type currentDataType, bool inVariableGroup, System.Action<System.Type> onSelect)
		{
			foreach (var variableInfo in s_VariableTypeInfos)
			{
				string menuName = variableInfo.menuName;

				if (inVariableGroup)
				{
					menuName = "Variable/" + menuName;
				}

				if (variableInfo.isSerializable)
				{
					System.Type variableType = variableInfo.variableType;
					genericMenu.AddItem(EditorGUITools.GetTextContent(menuName), variableInfo.dataType == currentDataType, () =>
					{
						onSelect(variableType);
					});
				}
				else
				{
					genericMenu.AddDisabledItem(EditorGUITools.GetTextContent(menuName + " : not serializable"));
				}
			}
		}

		public static string GetVariableListName(System.Type classType)
		{
			System.Type dataType = VariableListBase.GetDataType(classType);

			System.Type valueType = TypeUtility.GetGenericArguments(dataType)[0];

			return TypeUtility.GetTypeName(valueType) + " List";
		}

		public static System.Type GetVariableListTypeFromDataType(System.Type dataType)
		{
			foreach (var variableInfo in s_VariableListTypeInfos)
			{
				if (variableInfo.dataType == dataType)
				{
					return variableInfo.variableType;
				}
			}

			return null;
		}

		public static string GetVariableListMenu(System.Type classType)
		{
			AddVariableMenu addVariableMenu = AttributeHelper.GetAttribute<AddVariableMenu>(classType);

			string menuName = null;
			if (addVariableMenu != null)
			{
				menuName = addVariableMenu.menuName;
			}
			else
			{
				menuName = GetVariableListName(classType);
			}

			if (string.IsNullOrEmpty(menuName))
			{
				return null;
			}

			return menuName;
		}

		public static void GenerateVariableListMenus(GenericMenu genericMenu, System.Type currentDataType, bool inVariableGroup, System.Action<System.Type> onSelect)
		{
			foreach (var variableInfo in s_VariableListTypeInfos)
			{
				string menuName = variableInfo.menuName;

				if (inVariableGroup)
				{
					menuName = "VariableList/" + menuName;
				}

				if (variableInfo.isSerializable)
				{
					System.Type variableType = variableInfo.variableType;
					genericMenu.AddItem(EditorGUITools.GetTextContent(menuName), variableInfo.dataType == currentDataType, () =>
					{
						onSelect(variableType);
					});
				}
				else
				{
					genericMenu.AddDisabledItem(EditorGUITools.GetTextContent(menuName + " : not serializable"));
				}
			}
		}
	}
}