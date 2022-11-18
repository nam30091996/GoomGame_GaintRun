//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System;

namespace ArborEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal abstract class CustomAttribute : Attribute
	{
		public Type classType
		{
			get;
			private set;
		}

		public CustomAttribute(Type classType)
		{
			this.classType = classType;
		}
	}
}