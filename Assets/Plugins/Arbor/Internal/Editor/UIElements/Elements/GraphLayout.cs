//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if UNITY_2017_3_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace ArborEditor.Internal
{
	internal class GraphLayout : VisualElement
	{
		public event HandleEventDelegate handleEventDelegate;

		public GraphLayout() : base()
		{
		}

		public override void HandleEvent(EventBase evtBase)
		{
			if (handleEventDelegate != null)
			{
				handleEventDelegate(evtBase);
			}
			base.HandleEvent(evtBase);
		}

		public delegate void HandleEventDelegate(EventBase evtBase);
	}
}

#endif