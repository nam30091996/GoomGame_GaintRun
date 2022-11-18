//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	using Arbor;

	public class FlexibleSceneObjectProperty : FlexibleFieldPropertyBase
	{
		// Paths
		private const string kHierarchyTypePath = "_HierarchyType";

		private SerializedProperty _HierarchyTypeProperty;

		public FlexibleSceneObjectType type
		{
			get
			{
				return EnumUtility.GetValueFromIndex<FlexibleSceneObjectType>(typeProperty.enumValueIndex);
			}
			set
			{
				FlexibleSceneObjectType type = this.type;
				if (type == value)
				{
					return;
				}

				switch (type)
				{
					case FlexibleSceneObjectType.Parameter:
						if (parameterProperty.type == ParameterReferenceType.DataSlot)
						{
							parameterProperty.slotProperty.Disconnect();
						}
						break;
					case FlexibleSceneObjectType.DataSlot:
						FlexibleUtility.DisconectDataBranch(slotProperty.property);
						break;
				}

				typeProperty.enumValueIndex = EnumUtility.GetIndexFromValue(value);
			}
		}

		public SerializedProperty hierarchyTypeProperty
		{
			get
			{
				if (_HierarchyTypeProperty == null)
				{
					_HierarchyTypeProperty = property.FindPropertyRelative(kHierarchyTypePath);
				}
				return _HierarchyTypeProperty;
			}
		}

		public FlexibleHierarchyType hierarchyType
		{
			get
			{
				return EnumUtility.GetValueFromIndex<FlexibleHierarchyType>(hierarchyTypeProperty.enumValueIndex);
			}
			set
			{
				FlexibleHierarchyType hierarchyType = this.hierarchyType;
				if (hierarchyType == value)
				{
					return;
				}

				hierarchyTypeProperty.enumValueIndex = EnumUtility.GetIndexFromValue(value);
			}
		}

		public FlexibleSceneObjectProperty(SerializedProperty property) : base(property)
		{
		}

		protected override void ClearType()
		{
			type = FlexibleSceneObjectType.Constant;
		}

		public override void SetSlotType()
		{
			type = FlexibleSceneObjectType.DataSlot;
		}

		public override bool IsSlotType()
		{
			return type == FlexibleSceneObjectType.DataSlot;
		}

		public override bool IsParameterType()
		{
			return type == FlexibleSceneObjectType.Parameter;
		}
	}
}