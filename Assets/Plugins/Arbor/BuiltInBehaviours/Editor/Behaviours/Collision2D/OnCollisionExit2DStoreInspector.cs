﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(OnCollisionExit2DStore))]
	internal sealed class OnCollisionExit2DStoreInspector : CheckTagBehaviourBaseInspector
	{
		protected override void OnFieldGUI()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Parameter"));
		}
	}
}
