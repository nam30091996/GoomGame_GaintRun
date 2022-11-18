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
#endif

namespace ArborEditor
{
	using Arbor.DynamicReflection;

	internal static class UIElementsUtility
	{
#if UNITY_2019_1_OR_NEWER
		private static readonly DynamicMethod _IsLayoutManual;
		private static readonly DynamicMethod _SetLayout;

		static UIElementsUtility()
		{
			System.Type typeVisualElement = typeof(VisualElement);

			PropertyInfo propertyIsLayoutManual = typeVisualElement.GetProperty("isLayoutManual", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo methodIsLayoutManual = propertyIsLayoutManual.GetGetMethod(true);
			_IsLayoutManual = DynamicMethod.GetMethod(methodIsLayoutManual);

			PropertyInfo propertyLayout = typeVisualElement.GetProperty("layout", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo methodSetLayout = propertyLayout.GetSetMethod(true);
			_SetLayout = DynamicMethod.GetMethod(methodSetLayout);
		}

		public static bool IsLayoutManual(VisualElement element)
		{
			return (bool)_IsLayoutManual.Invoke(element, null);
		}

		public static void SetLayout(VisualElement element, Rect layout)
		{
			_SetLayout.Invoke(element, new object[] { layout });
		}
#endif

		public static bool IsLayoutEvent(EventBase evt)
		{
#if UNITY_2019_1_OR_NEWER
			long eventTypeId = evt.eventTypeId;
#else
			long eventTypeId = evt.GetEventTypeId();
#endif

#if UNITY_2018_2_OR_NEWER
			if (evt.propagationPhase == PropagationPhase.DefaultAction && eventTypeId == GeometryChangedEvent.TypeId())
			{
				return true;
			}
#else
			if (evt.propagationPhase == PropagationPhase.DefaultAction && eventTypeId == PostLayoutEvent.TypeId())
			{
				return true;
			}
#endif

			return false;
		}
	}
}

#endif