﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if !NETFX_CORE
	[System.Reflection.Obfuscation(Exclude = true)]
#endif
	[System.Serializable]
	internal sealed class DataLinkFieldInfo
	{
		public string fieldName = "";

		public DataLinkUpdateTiming updateTiming = DataLinkUpdateTiming.Enter | DataLinkUpdateTiming.Execute | DataLinkUpdateTiming.Manual;

		[HideSlotFields]
		public InputSlotTypable slot = new InputSlotTypable();

		[System.NonSerialized]
		internal DynamicReflection.DynamicField field;

		[System.NonSerialized]
		internal DataLinkAttribute attribute;

		public DataLinkUpdateTiming currentUpdateTiming
		{
			get
			{
				if (attribute.hasUpdateTiming)
				{
					return attribute.updateTiming;
				}

				return updateTiming;
			}
		}
	}
}