﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(OnTriggerEnter2DTransition))]
	internal sealed class OnTriggerEnter2DTransitionInspector : CheckTagBehaviourBaseInspector
	{
		protected override void OnFieldGUI()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Collider2D"));
		}
	}
}
