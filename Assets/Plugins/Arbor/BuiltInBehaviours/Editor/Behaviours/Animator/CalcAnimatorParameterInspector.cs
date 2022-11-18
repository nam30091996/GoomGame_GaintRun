//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(CalcAnimatorParameter))]
	internal sealed class CalcAnimatorParameterInspector : Editor
	{
		private SerializedProperty _ReferenceProperty;
		private FlexibleEnumProperty<CalcAnimatorParameter.Function> _FunctionProperty;
		private FlexibleNumericProperty _FloatValueProperty;
		private FlexibleNumericProperty _IntValueProperty;
		private FlexibleBoolProperty _BoolValueProperty;

		void OnEnable()
		{
			_ReferenceProperty = serializedObject.FindProperty("_Reference");
			_FloatValueProperty = new FlexibleNumericProperty(serializedObject.FindProperty("_FloatValue"));
			_IntValueProperty = new FlexibleNumericProperty(serializedObject.FindProperty("_IntValue"));
			_BoolValueProperty = new FlexibleBoolProperty(serializedObject.FindProperty("_BoolValue"));
			_FunctionProperty = new FlexibleEnumProperty<CalcAnimatorParameter.Function>(serializedObject.FindProperty("_Function"));
		}

		static AnimatorControllerParameter GetParameter(Animator animator, string name)
		{
			if (animator == null)
			{
				return null;
			}

			AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
			if (animatorController == null)
			{
				return null;
			}

			foreach (AnimatorControllerParameter parameter in animatorController.parameters)
			{
				if (parameter.name == name)
				{
					return parameter;
				}
			}

			return null;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SerializedProperty animatorProperty = _ReferenceProperty.FindPropertyRelative("animator");

			Animator animator = animatorProperty.objectReferenceValue as Animator;
			SerializedProperty nameProperty = _ReferenceProperty.FindPropertyRelative("name");

			AnimatorControllerParameter selectParameter = GetParameter(animator, nameProperty.stringValue);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_ReferenceProperty);
			if (EditorGUI.EndChangeCheck())
			{
				Animator newAnimator = animatorProperty.objectReferenceValue as Animator;
				AnimatorControllerParameter newSelectParameter = GetParameter(newAnimator, nameProperty.stringValue);
				if (selectParameter != newSelectParameter &&
					(newSelectParameter == null || selectParameter == null || selectParameter.type != newSelectParameter.type))
				{
					serializedObject.ApplyModifiedProperties();

					if (newSelectParameter == null || newSelectParameter.type != AnimatorControllerParameterType.Float)
					{
						_FunctionProperty.Disconnect();
						_FloatValueProperty.Disconnect();
					}
					if (newSelectParameter == null || newSelectParameter.type != AnimatorControllerParameterType.Int)
					{
						_FunctionProperty.Disconnect();
						_IntValueProperty.Disconnect();
					}
					if (newSelectParameter == null || newSelectParameter.type != AnimatorControllerParameterType.Bool)
					{
						_BoolValueProperty.Disconnect();
					}

					GUIUtility.ExitGUI();
				}
				animator = newAnimator;
				selectParameter = newSelectParameter;
			}

			if (selectParameter != null)
			{
				switch (selectParameter.type)
				{
					case AnimatorControllerParameterType.Float:
						{
							EditorGUILayout.PropertyField(_FunctionProperty.property);
							EditorGUILayout.PropertyField(_FloatValueProperty.property, EditorGUITools.GetTextContent("Float Value"));
						}
						break;
					case AnimatorControllerParameterType.Int:
						{
							EditorGUILayout.PropertyField(_FunctionProperty.property);
							EditorGUILayout.PropertyField(_IntValueProperty.property, EditorGUITools.GetTextContent("Int Value"));
						}
						break;
					case AnimatorControllerParameterType.Bool:
						{
							EditorGUILayout.PropertyField(_BoolValueProperty.property, EditorGUITools.GetTextContent("Bool Value"));
						}
						break;
					case AnimatorControllerParameterType.Trigger:
						break;
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
