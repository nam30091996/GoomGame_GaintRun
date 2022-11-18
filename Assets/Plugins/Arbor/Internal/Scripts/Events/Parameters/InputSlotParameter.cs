//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Events
{
#if !NETFX_CORE
	[System.Reflection.Obfuscation(Exclude = true)]
#endif
	[System.Serializable]
	internal sealed class InputSlotParameter : IValueContainer
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideSlotFields]
		private InputSlotTypable _Value = new InputSlotTypable();

		public InputSlotTypable GetSlot()
		{
			return _Value;
		}

		public object GetValue()
		{
			object value = null;
			if (_Value.GetValue(ref value))
			{
				return value;
			}
			return null;
		}
	}
}