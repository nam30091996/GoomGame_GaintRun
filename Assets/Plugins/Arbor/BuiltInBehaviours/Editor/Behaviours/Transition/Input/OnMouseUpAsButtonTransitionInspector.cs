﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
#pragma warning disable 0618
	[CustomEditor(typeof(OnMouseUpAsButtonTransition))]
#pragma warning restore 0618
	internal sealed class OnMouseUpAsButtonTransitionInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			serializedObject.ApplyModifiedProperties();
		}
	}
}