//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;
using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(CalcParameter))]
	internal sealed class CalcParameterInspector : Editor
	{
		private ParameterReferenceProperty _ReferenceProperty;
		private SerializedProperty _ParameterTypeProperty;
		private ClassTypeReferenceProperty _ReferenceTypeProperty;
		private FlexibleEnumProperty<CalcFunction> _FunctionProperty;

		private FlexibleNumericProperty _IntValueProperty;
		private FlexibleNumericProperty _FloatValueProperty;
		private FlexibleBoolProperty _BoolValueProperty;
		private FlexibleFieldProperty _StringValueProperty;
		private FlexibleFieldProperty _EnumValueProperty;
		private FlexibleSceneObjectProperty _GameObjectValueProperty;
		private FlexibleFieldProperty _Vector2ValueProperty;
		private FlexibleFieldProperty _Vector3ValueProperty;
		private FlexibleFieldProperty _QuaternionValueProperty;
		private FlexibleFieldProperty _RectValueProperty;
		private FlexibleFieldProperty _BoundsValueProperty;
		private FlexibleFieldProperty _ColorValueProperty;
		private FlexibleSceneObjectProperty _TransformValueProperty;
		private FlexibleSceneObjectProperty _RectTransformValueProperty;
		private FlexibleSceneObjectProperty _RigidbodyValueProperty;
		private FlexibleSceneObjectProperty _Rigidbody2DValueProperty;
		private FlexibleComponentProperty _ComponentValueProperty;
		private FlexibleNumericProperty _LongValueProperty;

		void OnEnable()
		{
			_ReferenceProperty = new ParameterReferenceProperty(serializedObject.FindProperty("_Reference"));
			_ParameterTypeProperty = serializedObject.FindProperty("_ParameterType");
			_ReferenceTypeProperty = new ClassTypeReferenceProperty(serializedObject.FindProperty("_ReferenceType"));
			_FunctionProperty = new FlexibleEnumProperty<CalcFunction>(serializedObject.FindProperty("_Function"));

			_IntValueProperty = new FlexibleNumericProperty(serializedObject.FindProperty("_IntValue"));
			_FloatValueProperty = new FlexibleNumericProperty(serializedObject.FindProperty("_FloatValue"));
			_BoolValueProperty = new FlexibleBoolProperty(serializedObject.FindProperty("_BoolValue"));
			_StringValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_StringValue"));
			_EnumValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_EnumValue"));
			_GameObjectValueProperty = new FlexibleSceneObjectProperty(serializedObject.FindProperty("_GameObjectValue"));
			_Vector2ValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_Vector2Value"));
			_Vector3ValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_Vector3Value"));
			_QuaternionValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_QuaternionValue"));
			_RectValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_RectValue"));
			_BoundsValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_BoundsValue"));
			_ColorValueProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_ColorValue"));
			_TransformValueProperty = new FlexibleSceneObjectProperty(serializedObject.FindProperty("_TransformValue"));
			_RectTransformValueProperty = new FlexibleSceneObjectProperty(serializedObject.FindProperty("_RectTransformValue"));
			_RigidbodyValueProperty = new FlexibleSceneObjectProperty(serializedObject.FindProperty("_RigidbodyValue"));
			_Rigidbody2DValueProperty = new FlexibleSceneObjectProperty(serializedObject.FindProperty("_Rigidbody2DValue"));
			_ComponentValueProperty = new FlexibleComponentProperty(serializedObject.FindProperty("_ComponentValue"));
			_LongValueProperty = new FlexibleNumericProperty(serializedObject.FindProperty("_LongValue"));
		}

		Parameter.Type GetParameterType()
		{
			ParameterReferenceType parameterReferenceType = _ReferenceProperty.type;
			Parameter.Type parameterType = Parameter.Type.Int;
			switch (parameterReferenceType)
			{
				case ParameterReferenceType.Constant:
					{
						Parameter parameter = _ReferenceProperty.GetParameter();
						if (parameter != null)
						{
							parameterType = parameter.type;
						}
					}
					break;
				case ParameterReferenceType.DataSlot:
					{
						parameterType = EnumUtility.GetValueFromIndex<Parameter.Type>(_ParameterTypeProperty.enumValueIndex);
					}
					break;
			}

			return parameterType;
		}

		System.Type GetReferenceType()
		{
			ParameterReferenceType parameterReferenceType = _ReferenceProperty.type;
			System.Type referenceType = null;
			switch (parameterReferenceType)
			{
				case ParameterReferenceType.Constant:
					{
						Parameter parameter = _ReferenceProperty.GetParameter();
						if (parameter != null)
						{
							referenceType = parameter.referenceType;
						}
					}
					break;
				case ParameterReferenceType.DataSlot:
					{
						referenceType = _ReferenceTypeProperty.type;
					}
					break;
			}

			return referenceType;
		}

		void DeleteOldBranch(Parameter.Type parameterType)
		{
			serializedObject.ApplyModifiedProperties();

			switch (parameterType)
			{
				case Parameter.Type.Int:
					{
						_FunctionProperty.Disconnect();
						_IntValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Long:
					{
						_FunctionProperty.Disconnect();
						_LongValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Float:
					{
						_FunctionProperty.Disconnect();
						_FloatValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Bool:
					{
						_BoolValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.String:
					{
						_FunctionProperty.Disconnect();
						_StringValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Enum:
					{
						_EnumValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.GameObject:
					{
						_GameObjectValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Vector2:
					{
						_Vector2ValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Vector3:
					{
						_Vector3ValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Quaternion:
					{
						_QuaternionValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Rect:
					{
						_RectValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Bounds:
					{
						_BoundsValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Color:
					{
						_ColorValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Transform:
					{
						_TransformValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.RectTransform:
					{
						_RectTransformValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Rigidbody:
					{
						_RigidbodyValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Rigidbody2D:
					{
						_Rigidbody2DValueProperty.Disconnect();
					}
					break;
				case Parameter.Type.Component:
					{
						_ComponentValueProperty.Disconnect();
					}
					break;
			}

			GUIUtility.ExitGUI();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Parameter.Type oldParameterType = GetParameterType();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_ReferenceProperty.property);
			if (EditorGUI.EndChangeCheck())
			{
				Parameter.Type newParameterType = GetParameterType();
				if (newParameterType != oldParameterType)
				{
					DeleteOldBranch(oldParameterType);
				}
				oldParameterType = newParameterType;
			}

			ParameterReferenceType parameterReferenceType = _ReferenceProperty.type;
			if (parameterReferenceType == ParameterReferenceType.DataSlot)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_ParameterTypeProperty);
				if (EditorGUI.EndChangeCheck())
				{
					Parameter.Type newParameterType = GetParameterType();
					if (newParameterType != oldParameterType)
					{
						DeleteOldBranch(oldParameterType);

						_ReferenceTypeProperty.type = null;
					}
					oldParameterType = newParameterType;
				}

				switch (oldParameterType)
				{
					case Parameter.Type.Component:
						{
							System.Type oldReferenceType = _ReferenceTypeProperty.type;

							_ReferenceTypeProperty.property.SetStateData<ClassTypeConstraintAttribute>(ClassTypeConstraintEditorUtility.component);

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField(_ReferenceTypeProperty.property);
							if (EditorGUI.EndChangeCheck())
							{
								System.Type referenceType = _ReferenceTypeProperty.type;
								if (referenceType != oldReferenceType)
								{
									DeleteOldBranch(oldParameterType);
								}
								oldReferenceType = referenceType;
							}
						}
						break;
					case Parameter.Type.Enum:
						{
							System.Type oldReferenceType = _ReferenceTypeProperty.type;

							_ReferenceTypeProperty.property.SetStateData<ClassTypeConstraintAttribute>(ClassTypeConstraintEditorUtility.enumField);

							EditorGUI.BeginChangeCheck();
							EditorGUILayout.PropertyField(_ReferenceTypeProperty.property);
							if (EditorGUI.EndChangeCheck())
							{
								System.Type referenceType = _ReferenceTypeProperty.type;
								if (referenceType != oldReferenceType)
								{
									DeleteOldBranch(oldParameterType);
								}
								oldReferenceType = referenceType;
							}
						}
						break;
				}
			}

			Parameter.Type parameterType = GetParameterType();

			switch (parameterType)
			{
				case Parameter.Type.Int:
					{
						EditorGUILayout.PropertyField(_FunctionProperty.property);

						EditorGUILayout.PropertyField(_IntValueProperty.property);
					}
					break;
				case Parameter.Type.Long:
					{
						EditorGUILayout.PropertyField(_FunctionProperty.property);

						EditorGUILayout.PropertyField(_LongValueProperty.property);
					}
					break;
				case Parameter.Type.Float:
					{
						EditorGUILayout.PropertyField(_FunctionProperty.property);

						EditorGUILayout.PropertyField(_FloatValueProperty.property);
					}
					break;
				case Parameter.Type.Bool:
					{
						EditorGUILayout.PropertyField(_BoolValueProperty.property);
					}
					break;
				case Parameter.Type.String:
					{
						EditorGUILayout.PropertyField(_FunctionProperty.property);

						EditorGUILayout.PropertyField(_StringValueProperty.property);
					}
					break;
				case Parameter.Type.Enum:
					{
						SerializedProperty enumValueProperty = _EnumValueProperty.property;
						enumValueProperty.SetStateData(GetReferenceType());

						EditorGUILayout.PropertyField(enumValueProperty);
					}
					break;
				case Parameter.Type.GameObject:
					{
						EditorGUILayout.PropertyField(_GameObjectValueProperty.property);
					}
					break;
				case Parameter.Type.Vector2:
					{
						EditorGUILayout.PropertyField(_Vector2ValueProperty.property);
					}
					break;
				case Parameter.Type.Vector3:
					{
						EditorGUILayout.PropertyField(_Vector3ValueProperty.property);
					}
					break;
				case Parameter.Type.Quaternion:
					{
						EditorGUILayout.PropertyField(_QuaternionValueProperty.property);
					}
					break;
				case Parameter.Type.Rect:
					{
						EditorGUILayout.PropertyField(_RectValueProperty.property);
					}
					break;
				case Parameter.Type.Bounds:
					{
						EditorGUILayout.PropertyField(_BoundsValueProperty.property);
					}
					break;
				case Parameter.Type.Color:
					{
						EditorGUILayout.PropertyField(_ColorValueProperty.property);
					}
					break;
				case Parameter.Type.Transform:
					{
						EditorGUILayout.PropertyField(_TransformValueProperty.property);
					}
					break;
				case Parameter.Type.RectTransform:
					{
						EditorGUILayout.PropertyField(_RectTransformValueProperty.property);
					}
					break;
				case Parameter.Type.Rigidbody:
					{
						EditorGUILayout.PropertyField(_RigidbodyValueProperty.property);
					}
					break;
				case Parameter.Type.Rigidbody2D:
					{
						EditorGUILayout.PropertyField(_Rigidbody2DValueProperty.property);
					}
					break;
				case Parameter.Type.Component:
					{
						_ComponentValueProperty.overrideConstraintType = GetReferenceType();

						EditorGUILayout.PropertyField(_ComponentValueProperty.property);
					}
					break;
				case Parameter.Type.Variable:
					{
						Parameter parameter = _ReferenceProperty.GetParameter();
						string valueTypeName = (parameter != null && parameter.valueType != null) ? parameter.valueType.ToString() : "Variable";
						string message = string.Format(Localization.GetWord("CalcParameter.NotSupportVariable"), valueTypeName);
						EditorGUILayout.HelpBox(message, MessageType.Warning);
					}
					break;
				case Parameter.Type.VariableList:
					{
						Parameter parameter = _ReferenceProperty.GetParameter();
						string valueTypeName = (parameter != null && parameter.valueType != null) ? parameter.valueType.ToString() : "VariableList";
						string message = string.Format(Localization.GetWord("CalcParameter.NotSupportVariableList"), valueTypeName);
						EditorGUILayout.HelpBox(message, MessageType.Warning);
					}
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

