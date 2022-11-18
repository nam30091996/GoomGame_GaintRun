﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace ArborEditor
{
	internal sealed class MousePosition
	{
		public Vector2 guiPoint
		{
			get;
			private set;
		}
		public Vector2 screenPoint
		{
			get;
			private set;
		}

		public MousePosition(Vector2 mousePosition)
		{
			guiPoint = mousePosition;
			screenPoint = GUIUtility.GUIToScreenPoint(guiPoint);
		}
	}
}