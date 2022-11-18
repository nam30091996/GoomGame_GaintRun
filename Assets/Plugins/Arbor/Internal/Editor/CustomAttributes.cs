//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

using Arbor;

namespace ArborEditor
{
	internal static class CustomAttributes<T> where T : CustomAttribute
	{
		private sealed class EditorType
		{
			public Type objectType;
			public Type editorType;
		}
		private static readonly List<EditorType> s_CustomEditors;
		private static bool s_Initialized;

		static CustomAttributes()
		{
			s_CustomEditors = new List<EditorType>();
		}

		public static void Rebuild(Assembly assembly)
		{
			foreach (Type type in TypeUtility.GetTypesFromAssembly(assembly))
			{
				foreach (T customAttribute in type.GetCustomAttributes(typeof(T), false))
				{
					if (customAttribute.classType != null)
					{
						EditorType editorType = new EditorType();
						editorType.objectType = customAttribute.classType;
						editorType.editorType = type;
						s_CustomEditors.Add(editorType);
					}
					else
					{
						Debug.Log("Can't load custom editor " + type.Name + " because the class type is null.");
					}
				}
			}
		}

		public static Type FindEditorType(System.Type objectType)
		{
			if (!s_Initialized)
			{
				Assembly[] assemblies = EditorGUITools.loadedAssemblies;
				for (int i = assemblies.Length - 1; i >= 0; --i)
				{
					Rebuild(assemblies[i]);
				}
				s_Initialized = true;
			}

			foreach (EditorType editorType in s_CustomEditors)
			{
				if (editorType.objectType == objectType)
				{
					return editorType.editorType;
				}
			}

			foreach (EditorType editorType in s_CustomEditors)
			{
				if (objectType.IsSubclassOf(editorType.objectType))
				{
					return editorType.editorType;
				}
			}

			return null;
		}
	}
}