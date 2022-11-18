//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal sealed class FlexibleComponentPropertyEditor : FlexibleSceneObjectPropertyEditor
	{
		private ClassConstraintInfo _ConstraintInfo;

		FlexibleComponentProperty flexibleComponentProperty
		{
			get
			{
				return flexibleProperty as FlexibleComponentProperty;
			}
		}

		ClassConstraintInfo GetConstraint()
		{
			System.Type overrideType = flexibleComponentProperty.overrideConstraintType;
			if (overrideType != null)
			{
				return new ClassConstraintInfo() { baseType = overrideType };
			}

			ClassTypeConstraintAttribute constraint = AttributeHelper.GetAttribute<ClassTypeConstraintAttribute>(fieldInfo);
			if (constraint != null && typeof(Component).IsAssignableFrom(constraint.GetBaseType(fieldInfo)))
			{
				return new ClassConstraintInfo() { constraintAttribute = constraint, constraintFieldInfo = fieldInfo };
			}

			SlotTypeAttribute slotTypeAttribute = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
			if (slotTypeAttribute != null && typeof(Component).IsAssignableFrom(slotTypeAttribute.connectableType))
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
				if (connectableType != null && typeof(Component).IsAssignableFrom(connectableType))
				{
					return connectableType;
				}
			}

			return typeof(Component);
		}

		protected override System.Type GetConstantObjectType()
		{
			return GetConnectableBaseType();
		}

		protected override FlexibleSceneObjectProperty CreateProperty(SerializedProperty property)
		{
			return new FlexibleComponentProperty(property);
		}

		protected override void OnConstantGUI(Rect position, GUIContent label)
		{
			SerializedProperty valueProperty = flexibleProperty.valueProperty;

			System.Type type = GetConnectableBaseType();

			EditorGUI.BeginChangeCheck();

			Object objectReferenceValue = EditorGUI.ObjectField(position, label, valueProperty.objectReferenceValue, type, true);

			if (EditorGUI.EndChangeCheck() && (objectReferenceValue == null || _ConstraintInfo == null || _ConstraintInfo.IsConstraintSatisfied(objectReferenceValue.GetType())))
			{
				valueProperty.objectReferenceValue = objectReferenceValue;
			}
			else if (valueProperty.objectReferenceValue != null && _ConstraintInfo != null && !_ConstraintInfo.IsConstraintSatisfied(valueProperty.objectReferenceValue.GetType()))
			{
				valueProperty.objectReferenceValue = null;
			}
		}

		protected override void OnGUI(Rect position, GUIContent label)
		{
			_ConstraintInfo = GetConstraint();

			if (_ConstraintInfo != null)
			{
				flexibleProperty.parameterProperty.property.SetStateData(_ConstraintInfo);
			}

			DataSlotField slotField = flexibleProperty.slotProperty.dataSlotField;
			if (slotField != null && _ConstraintInfo != null)
			{
				slotField.overrideConstraint = _ConstraintInfo;
			}

			base.OnGUI(position, label);
		}
	}

	[CustomPropertyDrawer(typeof(FlexibleComponent))]
	internal sealed class FlexibleComponentPropertyDrawer : PropertyEditorDrawer<FlexibleComponentPropertyEditor>
	{
	}
}
