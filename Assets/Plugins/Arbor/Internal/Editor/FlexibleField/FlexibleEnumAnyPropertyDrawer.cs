//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal sealed class FlexibleEnumAnyPropertyEditor : FlexibleFieldPropertyEditor
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
			System.Type constraintType = constraint != null ? constraint.GetBaseType(fieldInfo) : null;
			if (EnumFieldUtility.IsEnum(constraintType))
			{
				return new ClassConstraintInfo() { constraintAttribute = constraint, constraintFieldInfo = fieldInfo };
			}

			SlotTypeAttribute slotTypeAttribute = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
			System.Type connectableType = slotTypeAttribute != null ? slotTypeAttribute.connectableType : null;
			if (slotTypeAttribute != null && EnumFieldUtility.IsEnum(connectableType))
			{
				return new ClassConstraintInfo() { slotTypeAttribute = slotTypeAttribute };
			}

			return new ClassConstraintInfo() { constraintAttribute = ClassTypeConstraintEditorUtility.enumField, constraintFieldInfo = fieldInfo };
		}

		System.Type GetConnectableBaseType()
		{
			if (_ConstraintInfo != null)
			{
				System.Type connectableType = _ConstraintInfo.GetConstraintBaseType();
				if (EnumFieldUtility.IsEnum(connectableType))
				{
					return connectableType;
				}
			}

			return typeof(System.Enum);
		}

		protected override void OnConstantGUI(Rect position, SerializedProperty valueProperty, GUIContent label)
		{
			System.Type type = GetConnectableBaseType();
			if (EnumFieldUtility.IsEnum(type))
			{
				object enumValue = System.Enum.ToObject(type, valueProperty.intValue);

				EditorGUI.BeginChangeCheck();

				if (AttributeHelper.HasAttribute<System.FlagsAttribute>(type))
				{
					enumValue = EditorGUITools.EnumMaskField(position, label, (System.Enum)enumValue);
				}
				else
				{
					enumValue = EditorGUI.EnumPopup(position, label, (System.Enum)enumValue);
				}

				if (EditorGUI.EndChangeCheck())
				{
					valueProperty.intValue = (int)enumValue;
				}
			}
			else
			{
				base.OnConstantGUI(position, valueProperty, label);
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

	[CustomPropertyDrawer(typeof(FlexibleEnumAny))]
	internal sealed class FlexibleEnumAnyPropertyDrawer : PropertyEditorDrawer<FlexibleEnumAnyPropertyEditor>
	{
	}
}
