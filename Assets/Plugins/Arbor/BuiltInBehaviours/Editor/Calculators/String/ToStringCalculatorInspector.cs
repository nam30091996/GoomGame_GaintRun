﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

namespace ArborEditor.Calculators
{
	using Arbor.Calculators;

	[CustomEditor(typeof(ToStringCalculator))]
	public class ToStringCalculatorInspector : CalculatorBehaviourEditor
	{
		public override bool IsResizableNode()
		{
			return false;
		}

		public override float GetNodeWidth()
		{
			return 200f;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_Input"), GUILayout.Width(80f));
				GUILayout.FlexibleSpace();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_Output"), GUILayout.Width(80f));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}