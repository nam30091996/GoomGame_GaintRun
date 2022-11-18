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
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
#endif
using UnityEditor;

namespace ArborEditor.Internal
{
	internal class VisualSplitter :
#if UNITY_2019_1_OR_NEWER
		ImmediateModeElement
#else
		VisualElement
#endif
	{
		private const int kDefalutSplitSize = 6;
		public int splitSize = kDefalutSplitSize;

		public VisualSplitter() : base()
		{
			this.AddManipulator(new SplitManipulator());
		}

		public VisualElement[] GetAffectedVisualElements()
		{
			List<VisualElement> visualElementList = new List<VisualElement>();
#if UNITY_2019_1_OR_NEWER
			for (int index = 0; index < hierarchy.childCount; ++index)
			{
				VisualElement visualElement = hierarchy[index];
				if (visualElement.resolvedStyle.position == Position.Relative)
				{
					visualElementList.Add(visualElement);
				}
			}
#else
			for (int index = 0; index < this.shadow.childCount; ++index)
			{
				VisualElement visualElement = this.shadow[index];
				if (visualElement.style.positionType == PositionType.Relative)
				{
					visualElementList.Add(visualElement);
				}
			}
#endif
			return visualElementList.ToArray();
		}

		static bool IsRowDirection(FlexDirection flexDirection)
		{
			return flexDirection == FlexDirection.Row || flexDirection == FlexDirection.RowReverse;
		}

		public static float CalcFlex(Vector2 position, Vector2 min1, Vector2 minSize1, Vector2 minSize2, Vector2 totalSize, FlexDirection flexDirection, float totalFlex)
		{
			bool isRowDirection = IsRowDirection(flexDirection);

#if UNITY_2018_2_OR_NEWER
			position -= (min1 + minSize1);
			totalSize -= (minSize1 + minSize2);
#endif

			Vector2 val = new Vector2(position.x / totalSize.x, position.y / totalSize.y);

			float num1 = Mathf.Max(0f, Mathf.Min(1f, isRowDirection ? val.x : val.y));

			return num1 * totalFlex;
		}

		private Dictionary<VisualElement, float> _FlexCache = new Dictionary<VisualElement, float>();

		public void SetFlex(VisualElement element, float flex)
		{
#if UNITY_2018_3_OR_NEWER
			element.style.flexGrow = flex;
#else
			element.style.flex = flex;
#endif
			_FlexCache[element] = flex;
		}

		float GetFlex(VisualElement element)
		{
			float flex = 0f;
			if (!_FlexCache.TryGetValue(element, out flex))
			{
#if UNITY_2019_1_OR_NEWER
				flex = element.style.flexGrow.value;
#elif UNITY_2018_3_OR_NEWER
				flex = element.style.flexGrow;
#else
				flex = element.style.flex;
#endif
				_FlexCache[element] = flex;
			}

			return flex;
		}


#if !UNITY_2018_2_OR_NEWER
		protected override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);

#if UNITY_2018_2_OR_NEWER
			if (evt.GetEventTypeId() == GeometryChangedEvent.TypeId())
			{
				OnPostLayout(true);
			}
#else
			if (evt.GetEventTypeId() == PostLayoutEvent.TypeId())
			{
				var postLayoutEvt = (PostLayoutEvent)evt;
				OnPostLayout(postLayoutEvt.hasNewLayout);
			}
#endif
		}

		private Rect _LastLayout = new Rect();

		private void OnPostLayout(bool hasNewLayout)
		{
			if (!hasNewLayout)
				return;

			Rect layout = this.layout;

			if (_LastLayout == layout)
			{
				return;
			}

			_LastLayout = layout;

			VisualElement[] visualElements = GetAffectedVisualElements();

			FlexDirection flexDirection = (FlexDirection)style.flexDirection;

			bool isRowDirection = IsRowDirection(flexDirection);

			float totalSize = isRowDirection ? layout.width : layout.height;

			float sumSize = 0f;

			float totalFlex = 0f;
			for (int index = 0; index < visualElements.Length; ++index)
			{
				VisualElement element = visualElements[index];

				totalFlex += GetFlex(element);

				float size = isRowDirection ? element.layout.width : element.layout.height;
				
				sumSize += size;
			}

			if (totalSize < sumSize)
			{
				return;
			}

			float sumFlex = 0f;

			for (int index = 0; index < visualElements.Length; ++index)
			{
				VisualElement element = visualElements[index];

				float flex = GetFlex(element);
				float size = flex / totalFlex * totalSize;
				float minSize = isRowDirection ? element.style.minWidth : element.style.minHeight;
				float newSize = Mathf.Max(Mathf.Floor(size), minSize);
				
				float newFlex = (index < visualElements.Length - 1)? newSize / totalSize * totalFlex : totalFlex - sumFlex;

				if (newFlex != flex)
				{
#if UNITY_2018_3_OR_NEWER
					element.style.flexGrow = newFlex;
#else
					element.style.flex = newFlex;
#endif
				}

				sumFlex += newFlex;
			}
		}
#endif

#if UNITY_2019_1_OR_NEWER
		protected override void ImmediateRepaint()
		{
#elif UNITY_2018_3_OR_NEWER
		protected override void DoRepaint(IStylePainter painter)
		{
			base.DoRepaint(painter);
#else
		public override void DoRepaint()
		{
			base.DoRepaint();
#endif

#if UNITY_2019_1_OR_NEWER
			for (int index = 0; index < this.hierarchy.childCount - 1; ++index)
			{
				EditorGUIUtility.AddCursorRect(GetSplitterRect(hierarchy[index]), IsRowDirection(resolvedStyle.flexDirection) ? MouseCursor.SplitResizeLeftRight : MouseCursor.ResizeVertical);
			}
#else
			for (int index = 0; index < this.shadow.childCount - 1; ++index)
			{
				EditorGUIUtility.AddCursorRect(GetSplitterRect(shadow[index]), IsRowDirection(style.flexDirection)? MouseCursor.SplitResizeLeftRight : MouseCursor.ResizeVertical);
			}
#endif
		}

		public Rect GetSplitterRect(VisualElement visualElement)
		{
			Rect layout = visualElement.layout;
#if UNITY_2019_1_OR_NEWER
			FlexDirection flexDirection = resolvedStyle.flexDirection;
#else
			FlexDirection flexDirection = (FlexDirection)style.flexDirection;
#endif
			switch (flexDirection)
			{
				case FlexDirection.Row:
					layout.xMin = visualElement.layout.xMax - splitSize * 0.5f;
					layout.xMax = visualElement.layout.xMax + splitSize * 0.5f;
					break;
				case FlexDirection.RowReverse:
					layout.xMin = visualElement.layout.xMin - splitSize * 0.5f;
					layout.xMax = visualElement.layout.xMin + splitSize * 0.5f;
					break;
				case FlexDirection.Column:
					layout.yMin = visualElement.layout.yMax - splitSize * 0.5f;
					layout.yMax = visualElement.layout.yMax + splitSize * 0.5f;
					break;
				case FlexDirection.ColumnReverse:
					layout.yMin = visualElement.layout.yMin - splitSize * 0.5f;
					layout.yMax = visualElement.layout.yMin + splitSize * 0.5f;
					break;
			}
			return layout;
		}

		private sealed class SplitManipulator : MouseManipulator
		{
			private int m_ActiveVisualElementIndex;
			private int m_NextVisualElementIndex;
			private VisualElement[] m_AffectedElements;

			public SplitManipulator() : base()
			{
				m_ActiveVisualElementIndex = -1;
				m_NextVisualElementIndex = -1;

				ManipulatorActivationFilter activationFilter = new ManipulatorActivationFilter();
				activationFilter.button = MouseButton.LeftMouse;
				activators.Add(activationFilter);
			}
			protected override void RegisterCallbacksOnTarget()
			{
#if UNITY_2018_3_OR_NEWER
				target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
				target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
				target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#else
				target.RegisterCallback<MouseDownEvent>(OnMouseDown, Capture.Capture);
				target.RegisterCallback<MouseMoveEvent>(OnMouseMove, Capture.Capture);
				target.RegisterCallback<MouseUpEvent>(OnMouseUp, Capture.Capture);
#endif
			}

			protected override void UnregisterCallbacksFromTarget()
			{
#if UNITY_2018_3_OR_NEWER
				target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
				target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
				target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
#else
				target.UnregisterCallback<MouseDownEvent>(OnMouseDown, Capture.Capture);
				target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, Capture.Capture);
				target.UnregisterCallback<MouseUpEvent>(OnMouseUp, Capture.Capture);
#endif
			}

			void OnMouseDown(MouseDownEvent e)
			{
				if (!CanStartManipulation(e))
				{
					return;
				}

				VisualSplitter target = this.target as VisualSplitter;
#if UNITY_2019_1_OR_NEWER
				FlexDirection flexDirection = target.resolvedStyle.flexDirection;
#else
				FlexDirection flexDirection = (FlexDirection)target.style.flexDirection;
#endif
				m_AffectedElements = target.GetAffectedVisualElements();
				for (int index = 0; index < m_AffectedElements.Length - 1; ++index)
				{
					VisualElement affectedElement = m_AffectedElements[index];
					if (target.GetSplitterRect(affectedElement).Contains(e.localMousePosition))
					{
						if (flexDirection == FlexDirection.RowReverse || flexDirection == FlexDirection.ColumnReverse)
						{
							m_ActiveVisualElementIndex = index + 1;
							m_NextVisualElementIndex = index;
						}
						else
						{
							m_ActiveVisualElementIndex = index;
							m_NextVisualElementIndex = index + 1;
						}
#if UNITY_2018_3_OR_NEWER
						target.CaptureMouse();
#elif UNITY_2018_1_OR_NEWER
						target.TakeMouseCapture();
#else
						target.TakeCapture();
#endif
						e.StopPropagation();
					}
				}
			}

			void OnMouseMove(MouseMoveEvent e)
			{
#if UNITY_2018_1_OR_NEWER
				if (!this.target.HasMouseCapture())
#else
				if (!this.target.HasCapture())
#endif
				{
					return;
				}

				VisualSplitter target = this.target as VisualSplitter;
				VisualElement affectedElement1 = m_AffectedElements[m_ActiveVisualElementIndex];
				VisualElement affectedElement2 = m_AffectedElements[m_NextVisualElementIndex];
#if UNITY_2019_1_OR_NEWER
				FlexDirection flexDirection = target.resolvedStyle.flexDirection;
#else
				FlexDirection flexDirection = (FlexDirection)target.style.flexDirection;
#endif

				float totalFlex = 0f;

#if UNITY_2019_1_OR_NEWER
				totalFlex = affectedElement1.resolvedStyle.flexGrow + affectedElement2.resolvedStyle.flexGrow;
#elif UNITY_2018_3_OR_NEWER
				totalFlex = affectedElement1.style.flexGrow + affectedElement2.style.flexGrow;
#else
				totalFlex = affectedElement1.style.flex + affectedElement2.style.flex;
#endif

				Vector2 minSize1 = new Vector2();
				Vector2 minSize2 = new Vector2();

#if UNITY_2019_1_OR_NEWER
				minSize1.x = !(affectedElement1.resolvedStyle.minWidth == (StyleFloat)StyleKeyword.Auto) ? affectedElement1.resolvedStyle.minWidth.value : 0.0f;
				minSize1.y = !(affectedElement1.resolvedStyle.minHeight == (StyleFloat)StyleKeyword.Auto) ? affectedElement1.resolvedStyle.minHeight.value : 0.0f;

				minSize2.x = !(affectedElement2.resolvedStyle.minWidth == (StyleFloat)StyleKeyword.Auto) ? affectedElement2.resolvedStyle.minWidth.value : 0.0f;
				minSize2.y = !(affectedElement2.resolvedStyle.minHeight == (StyleFloat)StyleKeyword.Auto) ? affectedElement2.resolvedStyle.minHeight.value : 0.0f;
#else
				minSize1.x = affectedElement1.style.minWidth;
				minSize1.y = affectedElement1.style.minHeight;

				minSize2.x = affectedElement2.style.minWidth;
				minSize2.y = affectedElement2.style.minHeight;
#endif

				Vector2 totalSize = affectedElement1.layout.size + affectedElement2.layout.size;

				float flex = CalcFlex(e.localMousePosition, affectedElement1.layout.position, minSize1, minSize2, totalSize, flexDirection, totalFlex);

				target.SetFlex(affectedElement1, flex);
				target.SetFlex(affectedElement2, totalFlex - flex);

				e.StopPropagation();
			}

			void OnMouseUp(MouseUpEvent e)
			{
				if (!CanStopManipulation(e))
				{
					return;
				}

#if UNITY_2018_3_OR_NEWER
				target.ReleaseMouse();
#elif UNITY_2018_1_OR_NEWER
				target.ReleaseMouseCapture();
#else
				target.ReleaseCapture();
#endif
				e.StopPropagation();
				m_ActiveVisualElementIndex = -1;
				m_NextVisualElementIndex = -1;
			}
		}
	}
}

#endif