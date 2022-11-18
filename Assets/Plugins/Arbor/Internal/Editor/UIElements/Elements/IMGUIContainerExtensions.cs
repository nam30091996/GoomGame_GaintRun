//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if UNITY_2017_3_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
#endif
using UnityEditor;

namespace ArborEditor.Internal
{
	internal static class IMGUIContainerExtensions
	{
		private static readonly PropertyInfo s_GUIDepthPropertyInfo;
		private static readonly MethodInfo s_HandleIMGUIEventMethod;

		static IMGUIContainerExtensions()
		{
			s_GUIDepthPropertyInfo = typeof(IMGUIContainer).GetProperty("GUIDepth", BindingFlags.Instance | BindingFlags.NonPublic);

			s_HandleIMGUIEventMethod = typeof(IMGUIContainer).GetMethod("HandleIMGUIEvent", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static int GetGUIDepth(this IMGUIContainer imguiContainer)
		{
			if (s_GUIDepthPropertyInfo == null)
			{
				return 0;
			}

			return (int)s_GUIDepthPropertyInfo.GetValue(imguiContainer,null);
		}

		public static void SetGUIDepth(this IMGUIContainer imguiContainer,int guiDepth)
		{
			if (s_GUIDepthPropertyInfo == null)
			{
				return;
			}

			s_GUIDepthPropertyInfo.SetValue(imguiContainer, guiDepth, null);
		}

		public static void HandleIMGUIEvent(this IMGUIContainer imguiContainer, Event e)
		{
			if (s_HandleIMGUIEventMethod == null)
			{
				return;
			}

			s_HandleIMGUIEventMethod.Invoke(imguiContainer, new object[] { e });
		}
	}
}

#endif