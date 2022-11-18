﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	using Arbor;

	public class FlexibleFieldProperty : FlexibleFieldPropertyBase
	{
		public FlexibleType type
		{
			get
			{
				return EnumUtility.GetValueFromIndex<FlexibleType>(typeProperty.enumValueIndex);
			}
			set
			{
				FlexibleType type = this.type;
				if (type == value)
				{
					return;
				}

				switch (type)
				{
					case FlexibleType.Parameter:
						if (parameterProperty.type == ParameterReferenceType.DataSlot)
						{
							parameterProperty.slotProperty.Disconnect();
						}
						break;
					case FlexibleType.DataSlot:
						FlexibleUtility.DisconectDataBranch(slotProperty.property);
						break;
				}

				typeProperty.enumValueIndex = EnumUtility.GetIndexFromValue(value);
			}
		}

		public FlexibleFieldProperty(SerializedProperty property) : base(property)
		{
		}

		protected override void ClearType()
		{
			type = FlexibleType.Constant;
		}

		public override void SetSlotType()
		{
			type = FlexibleType.DataSlot;
		}

		public override bool IsSlotType()
		{
			return type == FlexibleType.DataSlot;
		}

		public override bool IsParameterType()
		{
			return type == FlexibleType.Parameter;
		}
	}
}