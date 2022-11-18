﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

namespace ArborEditor
{
	public class NodeBehaviourEditor : Editor
	{
		private NodeEditor _NodeEditor;

		public NodeEditor nodeEditor
		{
			get
			{
				return _NodeEditor;
			}
		}

		public NodeGraphEditor graphEditor
		{
			get
			{
				if (_NodeEditor != null)
				{
					return _NodeEditor.graphEditor;
				}
				return null;
			}
		}

		public static Editor CreateEditor(NodeEditor nodeEditor, Object obj)
		{
			Editor editor = Editor.CreateEditor(obj);
			NodeBehaviourEditor behaviourEditor = editor as NodeBehaviourEditor;
			if (behaviourEditor != null)
			{
				behaviourEditor._NodeEditor = nodeEditor;
			}

			return editor;
		}

#if UNITY_2018_1_OR_NEWER
		public virtual void OnPresetApplied()
		{
		}
#endif
	}
}