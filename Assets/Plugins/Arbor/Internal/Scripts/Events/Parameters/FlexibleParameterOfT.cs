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
	internal class FlexibleParameter<T> : IValueContainer where T : IFlexibleField, new()
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private T _Value = new T();

		public T GetFlexibleField()
		{
			return _Value;
		}

		public object GetValue()
		{
			return _Value.GetValueObject();
		}
	}
}