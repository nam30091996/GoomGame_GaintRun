﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor
{
	using Arbor;

	[CustomPropertyDrawer(typeof(FlexibleFieldBase), true)]
	internal sealed class FlexibleFieldPropertyDrawer : PropertyEditorDrawer<FlexibleFieldPropertyEditor>
	{
	}
}
