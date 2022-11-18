//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	public sealed class FlexibleEnumAnyProperty<T> : FlexibleFieldProperty where T : struct
	{
		public T value
		{
			get
			{
				return (T)System.Enum.ToObject(typeof(T), valueProperty.intValue);
			}
			set
			{
				valueProperty.intValue = System.Convert.ToInt32(value);
			}
		}

		public FlexibleEnumAnyProperty(SerializedProperty property) : base(property)
		{
		}
	}
}