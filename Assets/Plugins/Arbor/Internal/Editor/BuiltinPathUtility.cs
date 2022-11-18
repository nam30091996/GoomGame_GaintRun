//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace ArborEditor
{
	using Arbor;

	public static class BuiltinPathUtility
	{
		private static List<BuiltinPath> _Paths = new List<BuiltinPath>();

		static BuiltinPathUtility()
		{
			System.Type builtInPathType = typeof(BuiltinPath);

			foreach (Assembly assembly in EditorGUITools.loadedAssemblies)
			{
				if (TypeUtility.IsReferenceableType(assembly, builtInPathType))
				{
					try
					{
						foreach (var attribute in assembly.GetCustomAttributes(builtInPathType, false))
						{
							BuiltinPath builtinPath = attribute as BuiltinPath;
							if (builtinPath != null)
							{
								_Paths.Add(builtinPath);
							}
						}
					}
					catch (System.Exception ex)
					{
						// An exception occurred in Unity 2019.3.0b1.
						// Issue Tracker : https://issuetracker.unity3d.com/issues/scripting-missingmethodexception-errors-are-thrown-on-selecting-object-after-updating-the-api
						Debug.LogWarning("There is a problem with the Unity version referenced by the Assembly. : " + assembly.FullName);
						Debug.LogException(ex);
					}
				}
			}
		}

		public static string GetBuiltinPath(System.Type classType)
		{
			int count = _Paths.Count;
			for (int i = 0; i < count; ++i)
			{
				BuiltinPath builtinPath = _Paths[i];
				if (classType == builtinPath.type || classType.IsSubclassOf(builtinPath.type))
				{
					return builtinPath.path;
				}
			}

			return "behaviours/";
		}
	}
}