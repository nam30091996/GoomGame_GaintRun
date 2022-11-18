﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

namespace ArborEditor
{
	using Arbor;

	[CustomPropertyDrawer(typeof(DataLinkAttribute))]
	internal sealed class DataLinkPropertyDrawer : PropertyDrawer
	{
		SerializedProperty GetLinkProperty(SerializedProperty property)
		{
			SerializedObject serializedObject = property.serializedObject;
			NodeBehaviour nodeBehaviour = serializedObject.targetObject as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				SerializedProperty linksProperty = serializedObject.FindProperty("_DataSlotFieldLinks");
				for (int i = 0; i < linksProperty.arraySize; i++)
				{
					SerializedProperty linkProperty = linksProperty.GetArrayElementAtIndex(i);
					SerializedProperty nameProperty = linkProperty.FindPropertyRelative("fieldName");
					if (nameProperty.stringValue == property.propertyPath)
					{
						return linkProperty;
					}
				}
			}

			return null;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty linkProperty = GetLinkProperty(property);

			if (ArborEditorWindow.isInNodeEditor && linkProperty != null)
			{
				InputSlotTypableProperty slotProperty = new InputSlotTypableProperty(linkProperty.FindPropertyRelative("slot"));

				bool on = slotProperty.branchID != 0;

				if (on)
				{
					return EditorGUIUtility.singleLineHeight;
				}
				else
				{
					return EditorGUI.GetPropertyHeight(property, label, true);
				}
			}
			else
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty linkProperty = GetLinkProperty(property);

			if (ArborEditorWindow.isInNodeEditor && linkProperty != null)
			{
				InputSlotTypableProperty slotProperty = new InputSlotTypableProperty(linkProperty.FindPropertyRelative("slot"));

				BehaviourEditorGUI.AddInputSlotLink(position, slotProperty.property);

				bool on = slotProperty.branchID != 0;

				if (on)
				{
					DataLinkAttribute dataLinkAttribute = attribute as DataLinkAttribute;
					if (dataLinkAttribute.hasUpdateTiming)
					{
						EditorGUI.LabelField(position, label);
					}
					else
					{
						SerializedProperty updateTimingProperty = linkProperty.FindPropertyRelative("updateTiming");
						DataLinkUpdateTiming updateTiming = (DataLinkUpdateTiming)updateTimingProperty.intValue;
						EditorGUI.BeginChangeCheck();
						updateTiming = (DataLinkUpdateTiming)EditorGUITools.EnumMaskField(position, label, updateTiming);
						if (EditorGUI.EndChangeCheck())
						{
							updateTimingProperty.intValue = (int)updateTiming;
						}
					}
				}
				else
				{
					EditorGUI.PropertyField(position, property, label, true);
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
		}
	}
}