﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;
using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(SendMessageUpwardsGameObject))]
	internal sealed class SendMessageUpwardsGameObjectInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Target"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MethodName"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Argument"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}
