﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[CustomPropertyDrawer(typeof(AnimatorParameterReference), true)]
	internal sealed class AnimatorParameterReferencePropertyDrawer : PropertyDrawer
	{
		AnimatorControllerParameterType GetParameterType()
		{
			System.Type elementType = Arbor.Serialization.SerializationUtility.ElementType(fieldInfo.FieldType);
			Arbor.Internal.AnimatorParameterTypeAttribute parameterTypeAttribute = AttributeHelper.GetAttribute<Arbor.Internal.AnimatorParameterTypeAttribute>(elementType);
			if (parameterTypeAttribute != null)
			{
				return parameterTypeAttribute.parameterType;
			}
			return AnimatorControllerParameterType.Bool;
		}

		Rect DoGUI(Rect position, SerializedProperty property, GUIContent label, bool isDraw)
		{
#if UNITY_2019_3_OR_NEWER
			if (property.IsInvalidManagedReference())
			{
				position.height = EditorGUI.GetPropertyHeight(property, label);
				if (isDraw)
				{
					EditorGUI.PropertyField(position, property, label);
				}
				return position;
			}
#endif

			SerializedProperty animatorProperty = property.FindPropertyRelative("animator");
			SerializedProperty nameProperty = property.FindPropertyRelative("name");
			SerializedProperty typeProperty = property.FindPropertyRelative("type");

			Rect lineRect = new Rect(position);

			lineRect.height = EditorGUIUtility.singleLineHeight;

			if (isDraw)
			{
				EditorGUI.LabelField(lineRect, label);
			}

			lineRect.y += lineRect.height + EditorGUIUtility.standardVerticalSpacing;
			lineRect.height = 0f;

			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;

			lineRect.height = EditorGUIUtility.singleLineHeight;

			if (isDraw)
			{
				EditorGUI.PropertyField(lineRect, animatorProperty);
			}

			lineRect.y += lineRect.height + EditorGUIUtility.standardVerticalSpacing;
			lineRect.height = 0f;

			System.Type elementType = Arbor.Serialization.SerializationUtility.ElementType(fieldInfo.FieldType);
			Arbor.Internal.AnimatorParameterTypeAttribute parameterTypeAttribute = AttributeHelper.GetAttribute<Arbor.Internal.AnimatorParameterTypeAttribute>(elementType);

			bool hasType = parameterTypeAttribute != null;
			AnimatorControllerParameterType parameterType = (parameterTypeAttribute != null) ? parameterTypeAttribute.parameterType : AnimatorControllerParameterType.Bool;

			Animator animator = animatorProperty.objectReferenceValue as Animator;
			lineRect.height = EditorGUITools.GetAnimatorParameterFieldHeight(animator, nameProperty, typeProperty, hasType);

			if (isDraw)
			{
				EditorGUITools.AnimatorParameterField(lineRect, animator, nameProperty, typeProperty, EditorGUITools.GetTextContent("Parameter"), hasType, parameterType);
			}

			lineRect.y += lineRect.height + EditorGUIUtility.standardVerticalSpacing;
			lineRect.height = 0f;

			EditorGUI.indentLevel = indentLevel;

			position.yMax = Mathf.Max(position.yMax, lineRect.yMax - EditorGUIUtility.standardVerticalSpacing);

			return position;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			DoGUI(position, property, label, true);
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Rect position = DoGUI(new Rect(), property, label, false);

			return position.height;
		}
	}
}
