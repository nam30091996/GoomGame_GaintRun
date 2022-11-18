//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	public sealed class FlexibleEnumProperty<T> : FlexibleFieldProperty where T : struct
	{
		public T value
		{
			get
			{
				return EnumUtility.GetValueFromIndex<T>(valueProperty.enumValueIndex);
			}
			set
			{
				valueProperty.enumValueIndex = EnumUtility.GetIndexFromValue<T>(value);
			}
		}

		public FlexibleEnumProperty(SerializedProperty property) : base(property)
		{
		}
	}
}