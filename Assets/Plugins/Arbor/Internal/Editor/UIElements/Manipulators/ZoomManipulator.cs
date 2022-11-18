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
	internal sealed class ZoomManipulator : MouseManipulator
	{
		private VisualElement m_GraphUI;
		private ArborEditorWindow m_Window;
		private Vector2 m_Start;
		private Vector2 m_Last;
		private Vector2 m_ZoomCenter;

		public float zoomStep
		{
			get;
			set;
		}

		public bool isActive
		{
			get;
			private set;
		}

		public ZoomManipulator(VisualElement graphUI, ArborEditorWindow window) : base()
		{
			m_GraphUI = graphUI;
			m_Window = window;
			zoomStep = 0.01f;

			ManipulatorActivationFilter filter = new ManipulatorActivationFilter();
			filter.button = MouseButton.RightMouse;
			filter.modifiers = EventModifiers.Alt;
			activators.Add(filter);
		}

		protected override void RegisterCallbacksOnTarget()
		{
#if UNITY_2018_3_OR_NEWER
			target.RegisterCallback<WheelEvent>(OnScroll, TrickleDown.TrickleDown);
			target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
			target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
			target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#else
			target.RegisterCallback<WheelEvent>(OnScroll, Capture.Capture);
			target.RegisterCallback<MouseDownEvent>(OnMouseDown, Capture.Capture);
			target.RegisterCallback<MouseMoveEvent>(OnMouseMove, Capture.Capture);
			target.RegisterCallback<MouseUpEvent>(OnMouseUp, Capture.Capture);
#endif
		}

		protected override void UnregisterCallbacksFromTarget()
		{
#if UNITY_2018_3_OR_NEWER
			target.UnregisterCallback<WheelEvent>(OnScroll, TrickleDown.TrickleDown);
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
			target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
			target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#else
			target.UnregisterCallback<WheelEvent>(OnScroll, Capture.Capture);
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown, Capture.Capture);
			target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, Capture.Capture);
			target.UnregisterCallback<MouseUpEvent>(OnMouseUp, Capture.Capture);
#endif
		}

		private void OnScroll(WheelEvent e)
		{
			if (ArborSettings.mouseWheelMode == MouseWheelMode.Zoom)
			{
				m_Window.OnZoom(target.ChangeCoordinatesTo(m_GraphUI, e.localMousePosition), 1.0f - e.delta.y * zoomStep);
				e.StopPropagation();
			}
		}

		void OnMouseDown(MouseDownEvent e)
		{
			if (!CanStartManipulation(e))
			{
				return;
			}
			m_Start = m_Last = e.localMousePosition;
			m_ZoomCenter = target.ChangeCoordinatesTo(m_GraphUI, m_Start);
			isActive = true;
#if UNITY_2018_3_OR_NEWER
			target.CaptureMouse();
#elif UNITY_2018_1_OR_NEWER
			target.TakeMouseCapture();
#else
			target.TakeCapture();
#endif
			e.StopPropagation();
		}

		void OnMouseMove(MouseMoveEvent e)
		{
			if (!isActive ||
#if UNITY_2018_1_OR_NEWER
				!target.HasMouseCapture()
#else
				!target.HasCapture()
#endif
				)
			{
				return;
			}
			Vector2 vector2 = e.localMousePosition - m_Last;
			m_Window.OnZoom(m_ZoomCenter, 1.0f + (vector2.x + vector2.y) * zoomStep);
			e.StopPropagation();
			m_Last = e.localMousePosition;
		}

		void OnMouseUp(MouseUpEvent e)
		{
			if (!isActive || !CanStopManipulation(e))
			{
				return;
			}
			isActive = false;
#if UNITY_2018_3_OR_NEWER
			target.ReleaseMouse();
#elif UNITY_2018_1_OR_NEWER
			target.ReleaseMouseCapture();
#else
			target.ReleaseCapture();
#endif
			e.StopPropagation();
		}
	}
}

#endif