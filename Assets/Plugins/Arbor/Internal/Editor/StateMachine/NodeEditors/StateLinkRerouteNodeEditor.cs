//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[CustomNodeEditor(typeof(StateLinkRerouteNode))]
	internal sealed class StateLinkRerouteNodeEditor : NodeEditor
	{
		public StateLinkRerouteNode stateLinkRerouteNode
		{
			get
			{
				return node as StateLinkRerouteNode;
			}
		}

		protected override bool HasHeaderGUI()
		{
			return false;
		}

		public override GUIContent GetTitleContent()
		{
			return GUIContent.none;
		}

		public override string GetTitle()
		{
			return Localization.GetWord("StateLinkRerouteNode");
		}

		protected override float GetWidth()
		{
			return 32f;
		}

		protected override GUIStyle GetBackgroundStyle()
		{
			return GUIStyle.none;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			isNormalInvisibleStyle = true;
			isShowContextMenuInWindow = true;
			isUsedMouseDownOnMainGUI = false;
			isResizable = false;
		}

		private static readonly int s_StateLinkHash = "s_StateLinkHash".GetHashCode();
		private static readonly int s_DirectionFieldHash = "s_DirectionFieldHash".GetHashCode();

		Node _DragTargetNode;

		StateLinkSettingWindow _StateLinkSettingWindow;

		bool StateLinkField(Rect position, StateLink stateLink)
		{
			StateMachineGraphEditor graphEditor = this.graphEditor as StateMachineGraphEditor;

			GUIStyle style = Styles.reroutePin;
			Vector2 size = style.CalcSize(GUIContent.none);

			Rect pinPos = new Rect();
			pinPos.min = position.center - size * 0.5f;
			pinPos.max = position.center + size * 0.5f;

			int controlID = EditorGUIUtility.GetControlID(s_StateLinkHash, FocusType.Passive);

			Event currentEvent = Event.current;

			EventType eventType = currentEvent.GetTypeForControl(controlID);

			Node targetNode = graphEditor.nodeGraph.GetNodeFromID(stateLink.stateID);

			bool isActive = GUIUtility.hotControl == controlID;

			Vector2 nowPos = currentEvent.mousePosition;

			Bezier2D bezier = new Bezier2D();
			if (targetNode != null)
			{
				bezier = EditorGUITools.GetTargetBezier(node, targetNode, pinPos.center, pinPos.center);
			}
			else
			{
				bezier.startPosition = pinPos.center;
			}

			Bezier2D draggingBezier = new Bezier2D();
			if (isActive)
			{
				if (_DragTargetNode != null)
				{
					draggingBezier = EditorGUITools.GetTargetBezier(node, _DragTargetNode, pinPos.center, pinPos.center);
				}
				else
				{
					draggingBezier.startPosition = pinPos.center;
					draggingBezier.startControl = draggingBezier.startPosition + stateLinkRerouteNode.direction * EditorGUITools.kBezierTangent;
					draggingBezier.endPosition = nowPos;
					draggingBezier.endControl = draggingBezier.startControl;
				}
			}

			Vector2 statePosition = new Vector2(node.position.x, node.position.y);

			bezier.startPosition += statePosition;
			bezier.startControl += statePosition;
			bezier.endPosition += statePosition;
			bezier.endControl += statePosition;

			draggingBezier.startPosition += statePosition;
			draggingBezier.startControl += statePosition;
			draggingBezier.endPosition += statePosition;
			draggingBezier.endControl += statePosition;

			Color lineColor = stateLink.lineColorChanged ? stateLink.lineColor : Color.white;

			bool on = isActive || targetNode != null;

			switch (eventType)
			{
				case EventType.MouseDown:
					if (pinPos.Contains(nowPos))
					{
						if (currentEvent.button == 0)
						{
							GUIUtility.hotControl = GUIUtility.keyboardControl = controlID;

							_DragTargetNode = null;

							if (graphEditor != null)
							{
								graphEditor.BeginDragStateBranch(node.nodeID);
								graphEditor.DragStateBranchBezie(bezier);
							}

							currentEvent.Use();
						}
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID && currentEvent.button == 0)
					{
						DragAndDrop.PrepareStartDrag();

						Node hoverNode = graphEditor.GetTargetNodeFromPosition(nowPos + statePosition, node);

						if (hoverNode != null)
						{
							if (graphEditor != null)
							{
								graphEditor.DragStateBranchHoverStateID(hoverNode.nodeID);
							}

							_DragTargetNode = hoverNode;
						}
						else
						{
							if (graphEditor != null)
							{
								graphEditor.DragStateBranchHoverStateID(0);
							}
							_DragTargetNode = null;
						}

						currentEvent.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						if (currentEvent.button == 0)
						{
							GUI.UnfocusWindow();

							GUIUtility.hotControl = 0;

							if (_DragTargetNode == null)
							{
								GenericMenu menu = new GenericMenu();

								Vector2 currentMousePosition = currentEvent.mousePosition;
								Vector2 mousePosition = graphEditor.hostWindow.UnclipToGraph(currentMousePosition);
								Vector2 screenMousePosition = EditorGUIUtility.GUIToScreenPoint(currentMousePosition);

								menu.AddItem(EditorContents.createState, false, () =>
									{
										Undo.IncrementCurrentGroup();
										int undoGroup = Undo.GetCurrentGroup();

										mousePosition -= new Vector2(8f, 12f);

										State newState = graphEditor.CreateState(mousePosition, false);

										Undo.RecordObject(graphEditor.nodeGraph, "Link State");

										stateLink.stateID = newState.nodeID;
										stateLink.bezier = bezier;

										Undo.CollapseUndoOperations(undoGroup);

										EditorUtility.SetDirty(graphEditor.nodeGraph);
									});

								menu.AddItem(EditorContents.reroute, false, () =>
									{
										Undo.IncrementCurrentGroup();
										int undoGroup = Undo.GetCurrentGroup();

										mousePosition -= new Vector2(16f, 16f);

										StateLinkRerouteNode newStateLinkNode = graphEditor.CreateStateLinkRerouteNode(mousePosition, lineColor);

										Undo.RecordObject(graphEditor.nodeGraph, "Link State");

										stateLink.stateID = newStateLinkNode.nodeID;
										stateLink.bezier = bezier;

										Undo.CollapseUndoOperations(undoGroup);

										EditorUtility.SetDirty(graphEditor.nodeGraph);
									});

								menu.AddSeparator("");

								menu.AddItem(EditorContents.nodeListSelection, false, () =>
								{
									StateLink currentStateLink = stateLink;
									NodeGraph currentGraph = graphEditor.nodeGraph;

									StateLinkSelectorWindow.instance.Open(graphEditor, new Rect(screenMousePosition, Vector2.zero), currentStateLink.stateID,
										(targetNodeEditor) =>
										{
											Undo.RecordObject(currentGraph, "Link State");

											currentStateLink.stateID = targetNodeEditor.nodeID;
											currentStateLink.bezier = draggingBezier;

											EditorUtility.SetDirty(currentGraph);

											//graphEditor.BeginFrameSelected(targetNodeEditor.node);
										}
									);
								});

								if (stateLink.stateID != 0)
								{
									menu.AddSeparator("");
									menu.AddItem(EditorContents.disconnect, false, () =>
										{
											Undo.RecordObject(graphEditor.nodeGraph, "Disconect StateLink");

											stateLink.stateID = 0;

											EditorUtility.SetDirty(graphEditor.nodeGraph);
										});
								}
								menu.ShowAsContext();
							}
							else if (_DragTargetNode != targetNode)
							{
								Undo.RecordObject(graphEditor.nodeGraph, "Link State");

								stateLink.stateID = _DragTargetNode.nodeID;
								stateLink.bezier = draggingBezier;

								EditorUtility.SetDirty(graphEditor.nodeGraph);
							}

							if (graphEditor != null)
							{
								graphEditor.EndDragStateBranch();
							}

							_DragTargetNode = null;
						}

						currentEvent.Use();
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.hotControl == controlID && currentEvent.keyCode == KeyCode.Escape)
					{
						GUIUtility.hotControl = 0;
						if (graphEditor != null)
						{
							graphEditor.EndDragStateBranch();
						}

						_DragTargetNode = null;

						currentEvent.Use();
					}
					break;
				case EventType.Repaint:
					Vector2 iconSize = EditorGUIUtility.GetIconSize();
					EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));

					if (isActive)
					{
						if (graphEditor != null)
						{
							graphEditor.DragStateBranchBezie(draggingBezier);
						}
					}
					else if (targetNode != null)
					{
						if (stateLink.bezier == null || !stateLink.bezier.Equals(bezier))
						{
							stateLink.bezier = bezier;
							Repaint();
						}
					}

					bool isDragHover = graphEditor.IsDragBranchHover(stateLinkRerouteNode);

					Color slotColor = new Color(lineColor.r, lineColor.g, lineColor.b);
					Color slotPinBackgroundColor = (isActive || isDragHover) ? NodeGraphEditor.dragBezierColor : slotColor;

					Color backgroundColor = GUI.backgroundColor;
					GUI.backgroundColor = slotPinBackgroundColor;

					style.Draw(pinPos, GUIContent.none, controlID, on);

					GUI.backgroundColor = backgroundColor;

					EditorGUIUtility.SetIconSize(iconSize);
					break;
			}

			return on;
		}

		public bool isDragDirection
		{
			get;
			private set;
		}

		private Vector2 _BeginDirection;

		void DirectionField(Rect position, Color pinColor, bool isSelection)
		{
			int controlID = EditorGUIUtility.GetControlID(s_DirectionFieldHash, FocusType.Passive);

			Event currentEvent = Event.current;

			EventType eventType = currentEvent.GetTypeForControl(controlID);

			Vector2 center = position.center;

			Vector2 direction = stateLinkRerouteNode.direction;

			Vector2 arrowPosition = center + direction * 16f;

			float arrowWidth = 8f;
			float arrowWidthHalf = arrowWidth * 0.5f;
			Vector2 arrowCenter = arrowPosition - direction * arrowWidthHalf;
			Rect arrowRect = new Rect(arrowCenter.x - arrowWidthHalf, arrowCenter.y - arrowWidthHalf, arrowWidth, arrowWidth);

			switch (eventType)
			{
				case EventType.MouseDown:
					if (isSelection && arrowRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
					{
						isDragDirection = true;
						graphEditor.BeginDisableContextClick();
						_BeginDirection = stateLinkRerouteNode.direction;
						GUIUtility.hotControl = controlID;
						currentEvent.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						Undo.RecordObject(node.nodeGraph, "Change Reroute Direction");
						stateLinkRerouteNode.direction = (currentEvent.mousePosition - position.center).normalized;
						EditorUtility.SetDirty(node.nodeGraph);
						currentEvent.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID)
					{
						if (currentEvent.button == 0)
						{
							graphEditor.EndDisableContextClick();
							GUIUtility.hotControl = 0;
							isDragDirection = false;
						}
						currentEvent.Use();
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.hotControl == controlID && currentEvent.keyCode == KeyCode.Escape)
					{
						Undo.RecordObject(node.nodeGraph, "Change Reroute Direction");
						stateLinkRerouteNode.direction = _BeginDirection;
						EditorUtility.SetDirty(node.nodeGraph);

						GUIUtility.hotControl = 0;
						isDragDirection = false;
						currentEvent.Use();

						Repaint();
					}
					break;
				case EventType.Repaint:
					if (isSelection)
					{
						bool isHover = GUIUtility.hotControl == controlID || arrowRect.Contains(currentEvent.mousePosition);
						Color color = isHover ? Color.cyan : pinColor;
						EditorGUITools.DrawArrow(arrowPosition, direction, color, arrowWidth);
					}
					break;
			}
		}

		void StateLinkField(StateLink stateLink)
		{
			Rect position = GUILayoutUtility.GetRect(32f, 32f);

			bool on = StateLinkField(position, stateLink);

			Color lineColor = stateLink.lineColorChanged ? stateLink.lineColor : Color.white;

			DirectionField(position, lineColor, isSelection || !on);
		}

		void DeleteKeepConnection()
		{
			NodeGraph nodeGraph = this.node.nodeGraph;
			ArborFSMInternal stateMachine = nodeGraph as ArborFSMInternal;
			StateLinkRerouteNode rerouteNode = stateLinkRerouteNode;

			Undo.IncrementCurrentGroup();
			int undoGroup = Undo.GetCurrentGroup();

			int nextStateID = rerouteNode.link.stateID;
			Vector2 nextEndPosition = rerouteNode.link.bezier.endPosition;
			Vector2 nextEndControl = rerouteNode.link.bezier.endControl;

			StateLinkRerouteNodeList stateLinkRerouteNodes = stateMachine.stateLinkRerouteNodes;
			int rerouteCount = stateLinkRerouteNodes.count;
			for (int i = 0; i < rerouteCount; i++)
			{
				StateLinkRerouteNode node = stateLinkRerouteNodes[i];

				if (node != null && node.link.stateID == rerouteNode.nodeID)
				{
					Undo.RecordObject(stateMachine, "Delete Keep Connection");

					node.link.stateID = nextStateID;
					node.link.bezier.endPosition = nextEndPosition;
					node.link.bezier.endControl = nextEndControl;

					graphEditor.VisibleNode(node);
				}
			}

			for (int i = 0, count = stateMachine.stateCount; i < count; i++)
			{
				State state = stateMachine.GetStateFromIndex(i);
				StateEditor stateEditor = graphEditor.GetNodeEditor(state) as StateEditor;

				bool visible = false;

				for (int behaviourIndex = 0, behaviourCount = state.behaviourCount; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);

					if (behaviourEditor != null)
					{
						foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
						{
							if (stateLinkProperty.stateLink.stateID == rerouteNode.nodeID)
							{
								Undo.RecordObject(behaviourEditor.behaviourObj, "Delete Keep Connection");

								stateLinkProperty.stateLink.stateID = nextStateID;
								stateLinkProperty.stateLink.bezier.endPosition = nextEndPosition;
								stateLinkProperty.stateLink.bezier.endControl = nextEndControl;

								EditorUtility.SetDirty(behaviourEditor.behaviourObj);

								visible = true;
							}
						}
					}
				}

				if (visible)
				{
					graphEditor.VisibleNode(state);
				}
			}

			graphEditor.DeleteNodes(new Node[] { rerouteNode });

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(nodeGraph);

			Repaint();
		}

		bool IsConnected()
		{
			NodeGraph nodeGraph = this.node.nodeGraph;
			ArborFSMInternal stateMachine = nodeGraph as ArborFSMInternal;
			StateLinkRerouteNode rerouteNode = stateLinkRerouteNode;

			int nextStateID = rerouteNode.link.stateID;

			if (nextStateID == 0)
			{
				return false;
			}

			StateLinkRerouteNodeList stateLinkRerouteNodes = stateMachine.stateLinkRerouteNodes;
			int rerouteCount = stateLinkRerouteNodes.count;
			for (int i = 0; i < rerouteCount; i++)
			{
				StateLinkRerouteNode node = stateLinkRerouteNodes[i];

				if (node != null && node.link.stateID == rerouteNode.nodeID)
				{
					return true;
				}
			}

			for (int i = 0, count = stateMachine.stateCount; i < count; i++)
			{
				State state = stateMachine.GetStateFromIndex(i);
				StateEditor stateEditor = graphEditor.GetNodeEditor(state) as StateEditor;

				for (int behaviourIndex = 0, behaviourCount = state.behaviourCount; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);

					if (behaviourEditor != null)
					{
						foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
						{
							if (stateLinkProperty.stateLink.stateID == rerouteNode.nodeID)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		protected override void SetDeleteContextMenu(GenericMenu menu, bool deletable, bool editable)
		{
			if (deletable && IsConnected() && editable)
			{
				menu.AddItem(EditorContents.deleteKeepConnection, false, DeleteKeepConnection);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.deleteKeepConnection);
			}
		}

		protected override void SetContextMenu(GenericMenu menu, Rect headerPosition, bool editable)
		{
			Rect mousePosition = new Rect(0, 0, 0, 0);
			mousePosition.position = Event.current.mousePosition;
			Rect position = EditorGUITools.GUIToScreenRect(mousePosition);

			menu.AddItem(EditorContents.settings, false, () =>
			{
				if (_StateLinkSettingWindow == null)
				{
					_StateLinkSettingWindow = new StateLinkSettingWindow();
				}
				_StateLinkSettingWindow.Init(graphEditor != null ? graphEditor.hostWindow : null, graphEditor.nodeGraph, stateLinkRerouteNode.link, null, true);

				position = GUIUtility.ScreenToGUIRect(position);
				PopupWindowUtility.Show(position, _StateLinkSettingWindow, false);
			});
		}

		protected override void OnGUI()
		{
			using (new ProfilerScope("OnGUI"))
			{
				StateLinkField(stateLinkRerouteNode.link);
			}
		}

		public override bool IsDraggingVisible()
		{
			Node targetNode = graphEditor.nodeGraph.GetNodeFromID(stateLinkRerouteNode.link.stateID);
			if (targetNode != null)
			{
				if (graphEditor.IsDraggingNode(targetNode))
				{
					return true;
				}
				StateLinkRerouteNodeEditor rerouteNodeEditor = graphEditor.GetNodeEditor(targetNode) as StateLinkRerouteNodeEditor;
				if (rerouteNodeEditor != null && rerouteNodeEditor.isDragDirection)
				{
					return true;
				}
			}
			return false;
		}
	}
}
