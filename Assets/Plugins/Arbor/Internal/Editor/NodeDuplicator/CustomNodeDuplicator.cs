//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System;

namespace ArborEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal sealed class CustomNodeDuplicator : CustomAttribute
	{
		public CustomNodeDuplicator(Type classType) : base(classType)
		{
		}
	}
}