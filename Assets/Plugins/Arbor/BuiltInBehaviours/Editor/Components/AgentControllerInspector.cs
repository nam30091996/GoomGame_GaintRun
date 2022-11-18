//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[CustomEditor(typeof(AgentController))]
	internal sealed class AgentControllerInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Agent"));

			SerializedProperty animatorProperty = serializedObject.FindProperty("_Animator");

			EditorGUILayout.PropertyField(animatorProperty);

			Animator animator = animatorProperty.objectReferenceValue as Animator;

			SerializedProperty movingParameterProperty = serializedObject.FindProperty("_MovingParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Moving", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, movingParameterProperty, null, EditorGUITools.GetTextContent(movingParameterProperty.displayName), AnimatorControllerParameterType.Bool);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovingSpeedThreshold"));

			SerializedProperty speedParameterProperty = serializedObject.FindProperty("_SpeedParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Speed", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, speedParameterProperty, null, EditorGUITools.GetTextContent(speedParameterProperty.displayName), AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_IsDivAgentSpeed"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_SpeedDampTime"));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovementType"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovementDivValue"));

			SerializedProperty movementXParameterProperty = serializedObject.FindProperty("_MovementXParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MovementX", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, movementXParameterProperty, null, EditorGUITools.GetTextContent(movementXParameterProperty.displayName), AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovementXDampTime"));

			SerializedProperty movementYParameterProperty = serializedObject.FindProperty("_MovementYParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MovementY", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, movementYParameterProperty, null, EditorGUITools.GetTextContent(movementYParameterProperty.displayName), AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovementYDampTime"));

			SerializedProperty movementZParameterProperty = serializedObject.FindProperty("_MovementZParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("MovementZ", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, movementZParameterProperty, null, EditorGUITools.GetTextContent(movementZParameterProperty.displayName), AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_MovementZDampTime"));

			SerializedProperty turnParameterProperty = serializedObject.FindProperty("_TurnParameter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Turn", EditorStyles.boldLabel);
			EditorGUITools.AnimatorParameterField(animator, turnParameterProperty, null, EditorGUITools.GetTextContent(turnParameterProperty.displayName), AnimatorControllerParameterType.Float);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_TurnType"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_TurnDampTime"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}