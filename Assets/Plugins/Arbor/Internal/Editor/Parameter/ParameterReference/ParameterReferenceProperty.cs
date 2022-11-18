//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	using Arbor;
	using Arbor.Internal;

	public sealed class ParameterReferenceProperty
	{
		// Paths
		private const string kTypePath = "_Type";
		private const string kContainerPath = "_Container";
		private const string kSlotPath = "_Slot";
		private const string kIdPath = "id";
		private const string kNamePath = "name";

		private SerializedProperty _Type;
		private SerializedProperty _Container;
		private InputSlotBaseProperty _Slot;
		private SerializedProperty _Id;
		private SerializedProperty _Name;

		private System.Reflection.FieldInfo _FieldInfo = null;
		private bool _IsInitializeConstraintInfo = false;

		private ParameterConstraintAttributeBase[] _ParameterConstraintAttributes = null;
		private ClassConstraintInfo _ClassConstraintInfo = null;

		public SerializedProperty property
		{
			get;
			private set;
		}

		public System.Reflection.FieldInfo fieldInfo
		{
			get
			{
				if (_FieldInfo == null)
				{
					System.Type fieldType;
					_FieldInfo = EditorGUITools.GetFieldInfoFromProperty(property, out fieldType);
				}
				return _FieldInfo;
			}
		}

		public SerializedProperty typeProperty
		{
			get
			{
				if (_Type == null)
				{
					_Type = property.FindPropertyRelative(kTypePath);
				}
				return _Type;
			}
		}

		public ParameterReferenceType type
		{
			get
			{
				return EnumUtility.GetValueFromIndex<ParameterReferenceType>(typeProperty.enumValueIndex);
			}
			set
			{
				typeProperty.enumValueIndex = EnumUtility.GetIndexFromValue(value);
			}
		}

		public SerializedProperty containerProperty
		{
			get
			{
				if (_Container == null)
				{
					_Container = property.FindPropertyRelative(kContainerPath);
				}
				return _Container;
			}
		}

		public ParameterContainerBase container
		{
			get
			{
				return containerProperty.objectReferenceValue as ParameterContainerBase;
			}
			set
			{
				containerProperty.objectReferenceValue = value;
			}
		}

		public InputSlotBaseProperty slotProperty
		{
			get
			{
				if (_Slot == null)
				{
					_Slot = new InputSlotBaseProperty(property.FindPropertyRelative(kSlotPath));
				}
				return _Slot;
			}
		}

		public SerializedProperty idProperty
		{
			get
			{
				if (_Id == null)
				{
					_Id = property.FindPropertyRelative(kIdPath);
				}
				return _Id;
			}
		}

		public int id
		{
			get
			{
				return idProperty.intValue;
			}
			set
			{
				idProperty.intValue = value;
			}
		}

		public SerializedProperty nameProperty
		{
			get
			{
				if (_Name == null)
				{
					_Name = property.FindPropertyRelative(kNamePath);
				}
				return _Name;
			}
		}

		public string name
		{
			get
			{
				return nameProperty.stringValue;
			}
			set
			{
				nameProperty.stringValue = value;
			}
		}

		public ParameterReferenceProperty(SerializedProperty property, System.Reflection.FieldInfo fieldInfo = null)
		{
			this.property = property;
			_FieldInfo = fieldInfo;
		}

		public ParameterContainerInternal GetDefaultContainer()
		{
			if (type != ParameterReferenceType.Constant)
			{
				return null;
			}

			ParameterContainerBase container = this.container;
			if (container == null)
			{
				return null;
			}

			return container.defaultContainer;
		}

		public Parameter GetParameter()
		{
			ParameterContainerInternal defaultContainer = GetDefaultContainer();
			if (defaultContainer != null)
			{
				return defaultContainer.GetParam(id);
			}

			return null;
		}

		public void SetParameter(Parameter parameter)
		{
			if (parameter == null)
			{
				return;
			}

			if (type == ParameterReferenceType.DataSlot)
			{
				slotProperty.Disconnect();
			}

			type = ParameterReferenceType.Constant;
			container = parameter.container;
			id = parameter.id;
			name = parameter.name;
		}

		public string GetParameterName()
		{
			Parameter parameter = GetParameter();
			if (parameter != null)
			{
				return parameter.name;
			}

			return name;
		}

		public void Clear()
		{
			type = ParameterReferenceType.Constant;
			container = null;
			slotProperty.Clear();
			id = 0;
			name = "";
		}

		public void Disconnect()
		{
			if (type == ParameterReferenceType.DataSlot)
			{
				slotProperty.Disconnect();
			}
		}

		void InitializeConstraintInfo()
		{
			if (_IsInitializeConstraintInfo)
			{
				return;
			}

			System.Reflection.FieldInfo fieldInfo = this.fieldInfo;

			System.Type elementType = Arbor.Serialization.SerializationUtility.ElementType(fieldInfo.FieldType);

			_ParameterConstraintAttributes = AttributeHelper.GetAttributes<ParameterConstraintAttributeBase>(elementType);

			foreach (var attr in _ParameterConstraintAttributes)
			{
				IConstraintableAttribute constraintableAttribute = attr as IConstraintableAttribute;
				if (constraintableAttribute != null)
				{
					ClassTypeConstraintAttribute classTypeConstraintAttribute = AttributeHelper.GetAttribute<ClassTypeConstraintAttribute>(fieldInfo);
					if (classTypeConstraintAttribute != null)
					{
						_ClassConstraintInfo = new ClassConstraintInfo() { constraintAttribute = classTypeConstraintAttribute, constraintFieldInfo = fieldInfo };
					}
					else
					{
						SlotTypeAttribute slotTypeAttribute = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
						if (slotTypeAttribute != null && constraintableAttribute.IsConstraintSatisfied(slotTypeAttribute.connectableType))
						{
							_ClassConstraintInfo = new ClassConstraintInfo() { slotTypeAttribute = slotTypeAttribute };
						}
					}
					break;
				}
			}

			_IsInitializeConstraintInfo = true;
		}

		ClassConstraintInfo GetConstraint()
		{
			ClassConstraintInfo constraint = property.GetStateData<ClassConstraintInfo>();
			if (constraint != null)
			{
				return constraint;
			}

			if (_ClassConstraintInfo != null)
			{
				return _ClassConstraintInfo;
			}

			return null;
		}

		public bool CheckType(Parameter parameter)
		{
			if (!_IsInitializeConstraintInfo)
			{
				InitializeConstraintInfo();
			}

			foreach (var attr in _ParameterConstraintAttributes)
			{
				if (!attr.IsConstraintSatisfied(parameter))
				{
					return false;
				}
			}

			ClassConstraintInfo constraintInfo = GetConstraint();
			if (constraintInfo != null)
			{
				return constraintInfo.IsConstraintSatisfied(parameter.valueType);
			}

			return true;
		}

		public bool IsShowOutsideSlot()
		{
			if (type == ParameterReferenceType.DataSlot)
			{
				return false;
			}

			switch (ArborSettings.dataSlotShowMode)
			{
				case DataSlotShowMode.Outside:
					return true;
				case DataSlotShowMode.Inside:
				case DataSlotShowMode.Flexibly:
					NodeBehaviour nodeBehaviour = property.serializedObject.targetObject as NodeBehaviour;
					if (nodeBehaviour != null)
					{
						if (DataSlotGUI.IsDragSlotConnectable(nodeBehaviour.node, slotProperty.dataSlotField, nodeBehaviour))
						{
							return true;
						}
					}
					break;
			}

			return false;
		}
	}
}