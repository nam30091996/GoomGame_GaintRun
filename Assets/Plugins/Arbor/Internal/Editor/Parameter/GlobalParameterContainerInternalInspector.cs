﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[CustomEditor(typeof(GlobalParameterContainerInternal), true)]
	internal sealed class GlobalParameterContainerInternalInspector : Editor
	{
		private GlobalParameterContainerInternal _Target;

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			_Target = target as GlobalParameterContainerInternal;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginDisabledGroup(Application.isPlaying);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Prefab"));

			if (Application.isPlaying)
			{
				EditorGUILayout.ObjectField("Instance", _Target.instance, typeof(ParameterContainerInternal), true);
			}

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();

		}
	}
}
