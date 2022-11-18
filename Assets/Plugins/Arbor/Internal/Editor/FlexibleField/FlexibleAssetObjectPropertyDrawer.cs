﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal sealed class FlexibleAssetObjectPropertyEditor : FlexibleFieldPropertyEditor
	{
		private ClassConstraintInfo _ConstraintInfo;

		ClassConstraintInfo GetConstraint()
		{
			System.Type overrideType = property.GetStateData<System.Type>();
			if (overrideType != null)
			{
				return new ClassConstraintInfo() { baseType = overrideType };
			}

			ClassTypeConstraintAttribute constraint = AttributeHelper.GetAttribute<ClassTypeConstraintAttribute>(fieldInfo);
			if (constraint != null && typeof(Object).IsAssignableFrom(constraint.GetBaseType(fieldInfo)))
			{
				return new ClassConstraintInfo() { constraintAttribute = constraint, constraintFieldInfo = fieldInfo };
			}

			SlotTypeAttribute slotTypeAttribute = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
			if (slotTypeAttribute != null && typeof(Object).IsAssignableFrom(slotTypeAttribute.connectableType))
			{
				return new ClassConstraintInfo() { slotTypeAttribute = slotTypeAttribute };
			}

			return null;
		}

		System.Type GetConnectableBaseType()
		{
			if (_ConstraintInfo != null)
			{
				System.Type connectableType = _ConstraintInfo.GetConstraintBaseType();
				if (connectableType != null && typeof(Object).IsAssignableFrom(connectableType))
				{
					return connectableType;
				}
			}

			return typeof(Object);
		}

		System.Type GetConstantObjectType()
		{
			return GetConnectableBaseType();
		}

		protected override void OnConstantGUI(Rect position, SerializedProperty valueProperty, GUIContent label)
		{
			System.Type type = GetConnectableBaseType();

			EditorGUI.BeginChangeCheck();

			Object objectReferenceValue = EditorGUI.ObjectField(position, label, valueProperty.objectReferenceValue, type, false);

			if (EditorGUI.EndChangeCheck() && (objectReferenceValue == null || _ConstraintInfo == null || _ConstraintInfo.IsConstraintSatisfied(objectReferenceValue.GetType())))
			{
				valueProperty.objectReferenceValue = objectReferenceValue;
			}
			else if (valueProperty.objectReferenceValue != null && _ConstraintInfo != null && !_ConstraintInfo.IsConstraintSatisfied(valueProperty.objectReferenceValue.GetType()))
			{
				valueProperty.objectReferenceValue = null;
			}
		}

		Object ValidateObjectFieldAssignment(Object obj)
		{
			if (obj.GetType() == typeof(ParameterDraggingObject))
			{
				return null;
			}

			System.Type objType = GetConstantObjectType();
			if (objType.IsAssignableFrom(obj.GetType()))
			{
				return obj;
			}
			else if (obj is GameObject && typeof(Object).IsAssignableFrom(objType))
			{
				GameObject go = obj as GameObject;
				return go.GetComponent(objType);
			}

			return null;
		}

		bool IsHoverableObject()
		{
			foreach (Object draggedObject in DragAndDrop.objectReferences)
			{
				if (draggedObject != null && ValidateObjectFieldAssignment(draggedObject) != null)
				{
					return true;
				}
			}

			return false;
		}

		protected override float GetDropConstantHeight()
		{
			if (!IsHoverableObject())
			{
				return 0f;
			}

			return Styles.dropField.CalcSize(EditorContents.dropObject).y;
		}

		protected override void OnDropConstant(Rect position)
		{
			if (!IsHoverableObject())
			{
				return;
			}

			position = EditorGUI.IndentedRect(position);

			GUIContent content = EditorContents.dropObject;

			Event current = Event.current;
			switch (current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (position.Contains(current.mousePosition))
					{
						foreach (Object draggedObject in DragAndDrop.objectReferences)
						{
							if (draggedObject != null)
							{
								Object obj = ValidateObjectFieldAssignment(draggedObject);
								if (obj != null)
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Link;

									if (current.type == EventType.DragPerform)
									{
										flexibleFieldProperty.valueProperty.objectReferenceValue = obj;

										GUI.changed = true;

										DragAndDrop.AcceptDrag();
										DragAndDrop.activeControlID = 0;
									}
								}
								else
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
								}

								current.Use();

								break;
							}
						}
					}
					break;
				case EventType.Repaint:
					{
						Styles.dropField.Draw(position, content, false, false, position.Contains(current.mousePosition), false);
					}
					break;
			}
		}

		protected override void OnGUI(Rect position, GUIContent label)
		{
			_ConstraintInfo = GetConstraint();

			if (_ConstraintInfo != null)
			{
				flexibleFieldProperty.parameterProperty.property.SetStateData(_ConstraintInfo);
			}

			DataSlotField slotField = flexibleFieldProperty.slotProperty.dataSlotField;
			if (slotField != null && _ConstraintInfo != null)
			{
				slotField.overrideConstraint = _ConstraintInfo;
			}

			base.OnGUI(position, label);
		}
	}

	[CustomPropertyDrawer(typeof(FlexibleAssetObject))]
	internal sealed class FlexibleAssetObjectPropertyDrawer : PropertyEditorDrawer<FlexibleAssetObjectPropertyEditor>
	{
	}
}
