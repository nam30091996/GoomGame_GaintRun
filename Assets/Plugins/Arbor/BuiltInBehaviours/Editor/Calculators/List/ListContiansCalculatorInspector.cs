//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

namespace ArborEditor.Calculators
{
	using Arbor;
	using Arbor.Calculators;
	using Arbor.Events;
	using ArborEditor.Events;

	[CustomEditor(typeof(ListContainsCalculator))]
	internal sealed class ListContiansCalculatorInspector : Editor
	{
		private const string kElementTypePath = "_ElementType";
		private const string kParameterTypePath = "_ParameterType";
		private const string kInputPath = "_Input";
		private const string kOutputPath = "_Output";
		private const string kParameterListPath = "_ParameterList";

		private ClassTypeReferenceProperty _ElementTypeProperty;
		private SerializedProperty _ParameterTypeProperty;
		private InputSlotTypableProperty _InputProperty;
		private OutputSlotBaseProperty _OutputProperty;
		private ParameterListProperty _ParameterListProperty;

		ParameterType parameterType
		{
			get
			{
				return EnumUtility.GetValueFromIndex<ParameterType>(_ParameterTypeProperty.enumValueIndex);
			}
			set
			{
				_ParameterTypeProperty.enumValueIndex = EnumUtility.GetIndexFromValue<ParameterType>(value);
			}
		}

		void OnEnable()
		{
			_ElementTypeProperty = new ClassTypeReferenceProperty(serializedObject.FindProperty(kElementTypePath));
			_ParameterTypeProperty = serializedObject.FindProperty(kParameterTypePath);
			_InputProperty = new InputSlotTypableProperty(serializedObject.FindProperty(kInputPath));
			_OutputProperty = new OutputSlotBaseProperty(serializedObject.FindProperty(kOutputPath));
			_ParameterListProperty = new ParameterListProperty(serializedObject.FindProperty(kParameterListPath));
		}

		void SetParameterType(ParameterType newParameterType)
		{
			if (parameterType == newParameterType)
			{
				return;
			}

			SerializedProperty oldParametersProperty = _ParameterListProperty.GetParametersProperty(parameterType);
			if (oldParametersProperty != null)
			{
				oldParametersProperty.ClearArray();
			}

			parameterType = newParameterType;

			SerializedProperty newParametersProperty = _ParameterListProperty.GetParametersProperty(parameterType);
			if (newParametersProperty != null)
			{
				newParametersProperty.arraySize = 1;

				if (parameterType == ParameterType.Slot)
				{
					SerializedProperty valueProperty = newParametersProperty.GetArrayElementAtIndex(0);

					InputSlotTypableProperty slotProperty = new InputSlotTypableProperty(valueProperty);
					slotProperty.type = _ElementTypeProperty.type;
				}
			}
		}

		void MigrationParameterType(ParameterType newParameterType)
		{
			if (newParameterType == ParameterType.Unknown)
			{
				return;
			}

			SerializedProperty parametersProperty = _ParameterListProperty.GetParametersProperty(parameterType);
			SerializedProperty newParametersProperty = _ParameterListProperty.GetParametersProperty(newParameterType);

			newParametersProperty.arraySize = parametersProperty.arraySize;

			for (int i = 0; i < newParametersProperty.arraySize; i++)
			{
				SerializedProperty newProperty = newParametersProperty.GetArrayElementAtIndex(i);
				SerializedProperty oldProperty = parametersProperty.GetArrayElementAtIndex(i);

				if (parameterType == ParameterType.Slot)
				{
					InputSlotTypableProperty oldValueProperty = new InputSlotTypableProperty(oldProperty);

					if (oldValueProperty.branchID != 0)
					{
						SerializedProperty newValueProperty = newProperty;

						FlexibleFieldPropertyBase flexiblePropertyBase = ParameterListProperty.GetFlexibleFieldProperty(newParameterType, newValueProperty);

						if (flexiblePropertyBase != null)
						{
							flexiblePropertyBase.SetSlotType();

							flexiblePropertyBase.slotProperty.nodeGraph = oldValueProperty.nodeGraph;
							flexiblePropertyBase.slotProperty.branchID = oldValueProperty.branchID;
						}
					}
				}
				else
				{
					InputSlotTypableProperty newValueProperty = new InputSlotTypableProperty(newProperty);

					SerializedProperty oldValueProperty = oldProperty;

					FlexibleFieldPropertyBase flexiblePropertyBase = ParameterListProperty.GetFlexibleFieldProperty(parameterType, oldValueProperty);

					if (flexiblePropertyBase != null && flexiblePropertyBase.IsSlotType())
					{
						newValueProperty.nodeGraph = flexiblePropertyBase.slotProperty.nodeGraph;
						newValueProperty.branchID = flexiblePropertyBase.slotProperty.branchID;
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			System.Type elementType = _ElementTypeProperty.type;

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(_ElementTypeProperty.property, GUIContent.none, true);
				if (EditorGUI.EndChangeCheck())
				{
					System.Type newType = _ElementTypeProperty.type;

					System.Type listType = ListUtility.GetIListType(newType);
					string listTypeName = TypeUtility.TidyAssemblyTypeName(listType);

					if (_InputProperty.typeProperty.assemblyTypeName.stringValue != listTypeName)
					{
						_InputProperty.Disconnect();

						_InputProperty.type = listType;
					}

					ParameterType newParameterType = ArborEventUtility.GetParameterType(newType, true);

					SetParameterType(newParameterType);

					elementType = newType;

					serializedObject.ApplyModifiedProperties();

					EditorGUIUtility.ExitGUI();
				}

				EditorGUILayout.PropertyField(_OutputProperty.property, EditorGUITools.GetTextContent(_OutputProperty.property.displayName), true, GUILayout.Width(70f));
			}

			bool disableInputField = false;
			bool disabledParameterArgument = false;

			bool hasAssemblyTypeName = !string.IsNullOrEmpty(_ElementTypeProperty.assemblyTypeName.stringValue);

			if (elementType == null)
			{
				if (hasAssemblyTypeName)
				{
					EditorGUILayout.HelpBox(Localization.GetWord("Array.Message.MissingType"), MessageType.Error);
					disabledParameterArgument = true;
				}
				disableInputField = true;
			}

			if (!disabledParameterArgument)
			{
				ParameterType newParameterType = ArborEventUtility.GetParameterType(elementType, true);

				if (parameterType != newParameterType)
				{
					disabledParameterArgument = true;

					SerializedProperty parametersProperty = _ParameterListProperty.GetParametersProperty(parameterType);

					if (parametersProperty.arraySize > 0)
					{
						EditorGUILayout.HelpBox(Localization.GetWord("ArborEvent.ChangedParameterType"), MessageType.Warning);
						if (GUILayout.Button(EditorContents.repair))
						{
							MigrationParameterType(newParameterType);

							SetParameterType(newParameterType);

							serializedObject.ApplyModifiedProperties();

							EditorGUIUtility.ExitGUI();
						}
					}
					else
					{
						SetParameterType(newParameterType);
					}
				}
			}

			using (new EditorGUI.DisabledGroupScope(disableInputField))
			{
				EditorGUILayout.PropertyField(_InputProperty.property);
			}

			using (new EditorGUI.DisabledGroupScope(disabledParameterArgument))
			{
				SerializedProperty valueProperty = _ParameterListProperty.GetValueProperty(parameterType, 0);
				if (valueProperty != null)
				{
					System.Type overrideType = _ElementTypeProperty.type;
					ParameterListProperty.SetOverrideType(parameterType, valueProperty, overrideType);

					EditorGUILayout.PropertyField(valueProperty, EditorGUITools.GetTextContent("Element"), true);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}