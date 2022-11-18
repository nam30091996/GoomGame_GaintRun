﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor.BehaviourTree.Actions
{
	using Arbor.BehaviourTree.Actions;

	[CustomEditor(typeof(AgentWarpToTransform))]
	internal sealed class AgentWarpToTransformInspector : AgentBaseInspector
	{
		SerializedProperty _TargetTransformProperty;

		protected override void OnEnable()
		{
			base.OnEnable();

			_TargetTransformProperty = serializedObject.FindProperty("_TargetTransform");
		}

		protected override void OnGUI()
		{
			base.OnGUI();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(_TargetTransformProperty);
		}
	}
}