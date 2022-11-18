//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;

	public struct ParameterTypeMenuItem
	{
		public readonly static ParameterTypeMenuItem[] menuItems;
		private readonly static GUIContent[] s_DisplayOptions;

		static ParameterTypeMenuItem()
		{
			Parameter.Type[] rawParameterTypes = EnumUtility.GetValues<Parameter.Type>();
			GUIContent[] rawContents = EnumUtility.GetContents<Parameter.Type>();

			Parameter.Type[] parameterTypes = new Parameter.Type[rawParameterTypes.Length];
			System.Array.Copy(rawParameterTypes, parameterTypes, rawParameterTypes.Length);
			GUIContent[] contents = new GUIContent[rawContents.Length];
			System.Array.Copy(rawContents, contents, rawContents.Length);

			int longIndex = ArrayUtility.IndexOf(parameterTypes, Parameter.Type.Long);
			GUIContent longContent = contents[longIndex];

			ArrayUtility.RemoveAt(ref parameterTypes, longIndex);
			ArrayUtility.RemoveAt(ref contents, longIndex);

			ArrayUtility.Insert(ref parameterTypes, 1, Parameter.Type.Long);
			ArrayUtility.Insert(ref contents, 1, longContent);

			int gameObjectIndex = ArrayUtility.IndexOf(parameterTypes, Parameter.Type.GameObject);
			GUIContent gameObjectContent = contents[gameObjectIndex];

			ArrayUtility.RemoveAt(ref parameterTypes, gameObjectIndex);
			ArrayUtility.RemoveAt(ref contents, gameObjectIndex);

			int transformIndex = ArrayUtility.IndexOf(parameterTypes, Parameter.Type.Transform);
			ArrayUtility.Insert(ref parameterTypes, transformIndex, Parameter.Type.GameObject);
			ArrayUtility.Insert(ref contents, transformIndex, gameObjectContent);

			List<ParameterTypeMenuItem> menuItems = new List<ParameterTypeMenuItem>();
			for (int i = 0, count = parameterTypes.Length; i < count; ++i)
			{
				Parameter.Type parameterType = parameterTypes[i];

				if (parameterType == Parameter.Type.Vector2 || parameterType == Parameter.Type.GameObject || parameterType == Parameter.Type.Variable ||
					parameterType == Parameter.Type.IntList || parameterType == Parameter.Type.Vector2List || parameterType == Parameter.Type.GameObjectList || parameterType == Parameter.Type.VariableList)
				{
					menuItems.Add(new ParameterTypeMenuItem() { content = GUIContent.none, isSeparator = true });
				}

				menuItems.Add(new ParameterTypeMenuItem() { content = contents[i], type = parameterType });
			}

			s_DisplayOptions = new GUIContent[menuItems.Count];
			for (int i = 0; i < menuItems.Count; i++)
			{
				s_DisplayOptions[i] = menuItems[i].content;
			}

			ParameterTypeMenuItem.menuItems = menuItems.ToArray();
		}

		public static int GetIndex(Parameter.Type parameterType)
		{
			int selectedIndex = -1;
			for (int i = 0; i < menuItems.Length; i++)
			{
				var menuItem = menuItems[i];
				if (!menuItem.isSeparator && menuItem.type == parameterType)
				{
					selectedIndex = i;
					break;
				}
			}

			return selectedIndex;
		}

		public static Parameter.Type Popup(Rect rect, GUIContent label, Parameter.Type parameterType)
		{
			int selectedIndex = GetIndex(parameterType);

			EditorGUI.BeginChangeCheck();
			selectedIndex = EditorGUI.Popup(rect, label, selectedIndex, s_DisplayOptions);
			if (EditorGUI.EndChangeCheck() && selectedIndex >= 0)
			{
				parameterType = menuItems[selectedIndex].type;
			}

			return parameterType;
		}

		public Parameter.Type type;
		public GUIContent content;
		public bool isSeparator;
	}
}