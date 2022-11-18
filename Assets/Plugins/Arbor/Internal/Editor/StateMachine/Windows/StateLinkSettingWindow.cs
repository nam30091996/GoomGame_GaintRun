﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	internal sealed class StateLinkSettingWindow : PopupWindowContent
	{
		private ArborEditorWindow _HostWindow;
		private Object _OwnerObject;
		private StateLink _StateLink;
		private System.Reflection.FieldInfo _FieldInfo;
		private bool _IsRerouteNode;

		private LayoutArea _LayoutArea = new LayoutArea();

		public void Init(ArborEditorWindow hostWindow, Object ownerObject, StateLink stateLink, System.Reflection.FieldInfo fieldInfo, bool isReroute)
		{
			_HostWindow = hostWindow;
			_OwnerObject = ownerObject;
			_StateLink = stateLink;
			_FieldInfo = fieldInfo;
			_IsRerouteNode = isReroute;
		}

		void DoGUI()
		{
			ArborEditorWindow window = _HostWindow;

			if (!_IsRerouteNode)
			{
				EditorGUI.BeginChangeCheck();
				string name = _LayoutArea.TextField("name", _StateLink.name);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_OwnerObject, "Change StateLink Settings");

					_StateLink.name = name;

					if (window != null)
					{
						window.Repaint();
					}

					EditorUtility.SetDirty(_OwnerObject);
				}

				bool isFixedTransitionTiming = false;

				TransitionTiming tempTransitionTiming = _StateLink.transitionTiming;
				if (_FieldInfo != null)
				{
					FixedTransitionTiming fixedTransitionTiming = AttributeHelper.GetAttribute<FixedTransitionTiming>(_FieldInfo);
					FixedImmediateTransition fixedImmediateTransition = AttributeHelper.GetAttribute<FixedImmediateTransition>(_FieldInfo);

					if (fixedTransitionTiming != null)
					{
						tempTransitionTiming = fixedTransitionTiming.transitionTiming;
						isFixedTransitionTiming = true;
						GUI.enabled = false;
					}
					else if (fixedImmediateTransition != null)
					{
						tempTransitionTiming = fixedImmediateTransition.immediate ? TransitionTiming.Immediate : TransitionTiming.LateUpdateOverwrite;
						isFixedTransitionTiming = true;
						GUI.enabled = false;
					}
				}

				EditorGUI.BeginChangeCheck();
				TransitionTiming transitionTiming = (TransitionTiming)_LayoutArea.EnumPopup("Transition Timing", tempTransitionTiming);

				if (isFixedTransitionTiming)
				{
					transitionTiming = tempTransitionTiming;
					GUI.enabled = true;
				}

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_OwnerObject, "Change StateLink Settings");

					_StateLink.transitionTiming = transitionTiming;

					if (window != null)
					{
						window.Repaint();
					}

					EditorUtility.SetDirty(_OwnerObject);
				}
			}

			Color lineColor = _StateLink.lineColorChanged ? _StateLink.lineColor : Color.white;

			EditorGUI.BeginChangeCheck();
			lineColor = _LayoutArea.ColorField("Line Color", lineColor);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_OwnerObject, "Change StateLink Settings");

				_StateLink.lineColor = lineColor;
				_StateLink.lineColorChanged = true;

				if (window != null)
				{
					window.Repaint();
				}

				EditorUtility.SetDirty(_OwnerObject);
			}

			Event current = Event.current;
			if (!_LayoutArea.isLayout && current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed")
			{
				HandleUtility.Repaint();
			}
		}

		public override void OnGUI(Rect rect)
		{
			_LayoutArea.Begin(rect, false);

			DoGUI();

			_LayoutArea.End();
		}

		public override Vector2 GetWindowSize()
		{
			Rect rect = new Rect(0, 0, 300, 0);

			_LayoutArea.Begin(rect, true);

			DoGUI();

			_LayoutArea.End();

			return _LayoutArea.rect.size;
		}
	}
}
