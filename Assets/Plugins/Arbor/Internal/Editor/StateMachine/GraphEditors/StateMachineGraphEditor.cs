//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Arbor;

namespace ArborEditor
{
	[CustomNodeGraphEditor(typeof(ArborFSMInternal))]
	public sealed class StateMachineGraphEditor : NodeGraphEditor
	{
		private static class Types
		{
			public static readonly System.Type SetParameterBehaviourType;

			static Types()
			{
				SetParameterBehaviourType = AssemblyHelper.GetTypeByName("Arbor.ParameterBehaviours.SetParameterBehaviour");
			}
		}

		private bool _DragStateBranchEnable = false;
		private Bezier2D _DragStateBranchBezier;
		private int _DragStateBranchNodeID = 0;
		private int _DragStateBranchHoverStateID = 0;

		private StateLinkSettingWindow _StateLinkSettingWindow = new StateLinkSettingWindow();

		public ArborFSMInternal stateMachine
		{
			get
			{
				return nodeGraph as ArborFSMInternal;
			}
			set
			{
				nodeGraph = value;
			}
		}

		public void BeginDragStateBranch(int nodeID)
		{
			_DragStateBranchEnable = true;
			_DragStateBranchNodeID = nodeID;
			_DragStateBranchHoverStateID = 0;
		}

		public void EndDragStateBranch()
		{
			_DragStateBranchEnable = false;
			_DragStateBranchNodeID = 0;
			_DragStateBranchHoverStateID = 0;

			hostWindow.Repaint();
		}

		public void DragStateBranchBezie(Bezier2D bezier)
		{
			_DragStateBranchBezier = bezier;
		}

		public void DragStateBranchHoverStateID(int stateID)
		{
			if (_DragStateBranchHoverStateID != stateID)
			{
				_DragStateBranchHoverStateID = stateID;
			}
		}

		void DrawDragStateBehaviour()
		{
			if (_DragStateBranchEnable)
			{
				EditorGUITools.DrawBranch(_DragStateBranchBezier, dragBezierColor, bezierShadowColor, 5.0f, true, false);
			}
		}

		public override void OnDrawDragBranchies()
		{
			DrawDragStateBehaviour();
		}

		public bool IsDragBranchHover(Node node)
		{
			return _DragStateBranchEnable && _DragStateBranchHoverStateID == node.nodeID;
		}

		public override bool IsDraggingBranch(Node node)
		{
			return base.IsDraggingBranch(node) ||
				_DragStateBranchEnable && _DragStateBranchNodeID == node.nodeID;
		}

		public override bool IsDragBranch()
		{
			return base.IsDragBranch() || _DragStateBranchEnable;
		}

		private sealed class HoverStateLink
		{
			public Node node;
			public StateBehaviour behaviour;
			public StateLink stateLink;
			public System.Reflection.FieldInfo fieldInfo;
		}

		private static readonly int s_DrawStateLinkBranchHash = "s_DrawStateLinkBranchHash".GetHashCode();
		private HoverStateLink _HoverStateLink = null;
		private HoverStateLink _NextHoverStateLink = null;

		protected override void OnBeginDrawBranch()
		{
			_NextHoverStateLink = null;
		}

		protected override bool OnHasHoverBranch()
		{
			return _NextHoverStateLink != null;
		}

		protected override void OnClearHoverBranch()
		{
			_NextHoverStateLink = null;
		}

		protected override bool OnEndDrawBranch()
		{
			if (_HoverStateLink != _NextHoverStateLink)
			{
				_HoverStateLink = _NextHoverStateLink;
				return true;
			}
			return false;
		}

		bool IsLinkedRerouteNode(State state, StateLinkRerouteNode rerouteNode)
		{
			StateEditor stateEditor = GetNodeEditor(state) as StateEditor;

			if (stateEditor == null)
			{
				return false;
			}

			int behaviourCount = state.behaviourCount;

			for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
			{
				StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);

				if (behaviourEditor != null)
				{
					foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
					{
						StateLinkRerouteNode currentRerouteNode = stateMachine.GetNodeFromID(stateLinkProperty.stateLink.stateID) as StateLinkRerouteNode;
						while (currentRerouteNode != null)
						{
							if (currentRerouteNode == rerouteNode)
							{
								return true;
							}

							currentRerouteNode = stateMachine.GetNodeFromID(currentRerouteNode.link.stateID) as StateLinkRerouteNode;
						}
					}
				}
			}

			return false;
		}

		List<State> GetParentStates(StateLinkRerouteNode rerouteNode)
		{
			List<State> states = new List<State>();

			int stateCount = stateMachine.stateCount;
			for (int i = 0; i < stateCount; i++)
			{
				State state = stateMachine.GetStateFromIndex(i);
				if (IsLinkedRerouteNode(state, rerouteNode))
				{
					states.Add(state);
				}
			}
			return states;
		}

		public static readonly Color reservedColor = new Color(0.5f, 0.0f, 1.0f);

		void DrawBranchStateLink(Node currentNode, StateBehaviour behaviour, StateLink stateLink, System.Reflection.FieldInfo stateLinkFieldInfo)
		{
			using (new ProfilerScope("DrawBranchStateLink"))
			{
				if (stateLink.stateID != 0)
				{
					bool editable = this.editable;

					Bezier2D bezier = stateLink.bezier;

					Node targetNode = stateMachine.GetNodeFromID(stateLink.stateID);

					int controlID = EditorGUIUtility.GetControlID(s_DrawStateLinkBranchHash, FocusType.Passive);

					Event currentEvent = Event.current;

					EventType eventType = currentEvent.GetTypeForControl(controlID);

					bool isHover = _HoverStateLink != null && _HoverStateLink.stateLink == stateLink;

					switch (eventType)
					{
						case EventType.MouseDown:
							if (currentEvent.button == 1 || Application.platform == RuntimePlatform.OSXEditor && currentEvent.control)
							{
								if (isHover)
								{
									GenericMenu menu = new GenericMenu();

									State prevState = currentNode as State;
									State nextState = stateMachine.GetState(stateLink);

									if (prevState != null)
									{
										menu.AddItem(EditorGUITools.GetTextContent(Localization.GetWord("Go to Previous State") + " : " + prevState.name), false, () =>
											{
												BeginFrameSelected(prevState);
											});
									}
									else
									{
										StateLinkRerouteNode rerouteNode = currentNode as StateLinkRerouteNode;
										if (rerouteNode != null)
										{
											List<State> parentStates = GetParentStates(rerouteNode);
											parentStates.Sort((a, b) =>
												{
													return a.position.y.CompareTo(b.position.y);
												});
											HashSet<string> names = new HashSet<string>();
											foreach (State state in parentStates)
											{
												State s = state;

												string stateName = s.name;
												while (names.Contains(stateName))
												{
													// dummy code 001f(US)
													stateName += '\u001f';
												}
												names.Add(stateName);

												menu.AddItem(EditorGUITools.GetTextContent(Localization.GetWord("Go to Previous State") + " : " + stateName), false, () =>
													{
														BeginFrameSelected(s);
													});
											}

											if (parentStates.Count > 1)
											{
												menu.AddSeparator("");
											}
										}
									}

									if (nextState != null)
									{
										menu.AddItem(EditorGUITools.GetTextContent(Localization.GetWord("Go to Next State") + " : " + nextState.name), false, () =>
											{
												BeginFrameSelected(nextState);
											});
									}

									menu.AddSeparator("");

									bool flag1 = false;

									if (prevState == null)
									{
										menu.AddItem(EditorContents.goToPreviousNode, false, () =>
											{
												BeginFrameSelected(currentNode);
											});
										flag1 = true;
									}

									if (targetNode != null && targetNode != nextState)
									{
										menu.AddItem(EditorContents.goToNextNode, false, () =>
											{
												BeginFrameSelected(targetNode);
											});
										flag1 = true;
									}

									if (flag1)
									{
										menu.AddSeparator("");
									}

									Vector2 mousePosition = currentEvent.mousePosition;

									if (editable)
									{
										menu.AddItem(EditorContents.reroute, false, () =>
											 {
												 int stateID = stateLink.stateID;

												 Undo.IncrementCurrentGroup();
												 int undoGroup = Undo.GetCurrentGroup();

												 StateLinkRerouteNode newStateLinkNode = CreateStateLinkRerouteNode(EditorGUITools.SnapToGrid(mousePosition - new Vector2(16f, 16f)), stateLink.lineColorChanged ? stateLink.lineColor : Color.white);

												 Undo.RecordObject(stateMachine, "Reroute");

												 float t = bezier.GetClosestParam(mousePosition);
												 newStateLinkNode.direction = bezier.GetTangent(t);
												 newStateLinkNode.link.stateID = stateID;

												 if (behaviour != null)
												 {
													 Undo.RecordObject(behaviour, "Reroute");
												 }

												 stateLink.stateID = newStateLinkNode.nodeID;

												 Undo.CollapseUndoOperations(undoGroup);

												 VisibleNode(currentNode);

												 EditorUtility.SetDirty(stateMachine);
												 if (behaviour != null)
												 {
													 EditorUtility.SetDirty(behaviour);
												 }
											 });

										menu.AddItem(EditorContents.disconnect, false, () =>
											{
												if (behaviour != null)
												{
													Undo.RecordObject(behaviour, "Disconnect StateLink");
												}
												else
												{
													Undo.RecordObject(stateMachine, "Disconnect StateLink");
												}

												stateLink.stateID = 0;

												if (behaviour != null)
												{
													EditorUtility.SetDirty(behaviour);
												}
												else
												{
													EditorUtility.SetDirty(stateMachine);
												}
											});
									}
									else
									{
										menu.AddDisabledItem(EditorContents.reroute);
										menu.AddDisabledItem(EditorContents.disconnect);
									}

									menu.AddSeparator("");

									if (editable)
									{
										Rect settingRect = new Rect();
										settingRect.position = mousePosition;
										settingRect = EditorGUITools.GUIToScreenRect(settingRect);

										menu.AddItem(EditorContents.settings, false, () =>
										 {
											 _StateLinkSettingWindow.Init(hostWindow, (Object)behaviour ?? (Object)nodeGraph, stateLink, stateLinkFieldInfo, currentNode is StateLinkRerouteNode);

											 settingRect = GUIUtility.ScreenToGUIRect(settingRect);
											 PopupWindowUtility.Show(settingRect, _StateLinkSettingWindow, false);
										 });
									}
									else
									{
										menu.AddDisabledItem(EditorContents.settings);
									}

									menu.ShowAsContext();
									currentEvent.Use();
								}
							}
							break;
						case EventType.MouseMove:
							{
								float distance = 0f;
								if (IsHoverBezier(currentEvent.mousePosition, bezier, true, EditorGUITools.kBranchArrowWidth, ref distance))
								{
									ClearHoverBranch();

									_NextHoverStateLink = new HoverStateLink()
									{
										node = currentNode,
										behaviour = behaviour,
										stateLink = stateLink,
										fieldInfo = stateLinkFieldInfo
									};
									_NextHoverBranchDistance = distance;
								}
							}
							break;
						case EventType.Repaint:
							{
								Color lineColor = Color.white;

								Color shadowColor = bezierShadowColor;
								float width = 5;
								if (Application.isPlaying)
								{
									int index = stateMachine.IndexOfStateLinkHistory(stateLink);
									if (index != -1)
									{
										float t = (float)index / 4.0f;

										shadowColor = Color.Lerp(new Color(0.0f, 0.5f, 0.5f, 1.0f), Color.black, t);
										lineColor *= Color.Lerp(Color.white, Color.gray, t);
										width = Mathf.Lerp(15, 5, t);
									}
									else
									{
										if (stateMachine.playState == PlayState.InactivePausing && stateMachine.reservedStateLink == stateLink)
										{
											lineColor *= reservedColor;
										}
										else
										{
											lineColor *= Color.gray;
										}
									}
								}

								if (stateLink.lineColorChanged)
								{
									lineColor *= stateLink.lineColor;
								}

								using (new ProfilerScope("DrawBezierArrow"))
								{
									EditorGUITools.DrawBranch(bezier, lineColor, shadowColor, width, targetNode is State, isHover);
								}
							}
							break;
					}
				}
			}
		}

		void DrawBehaviourBranches(StateEditor stateEditor)
		{
			using (new ProfilerScope("DrawBehaviourBranches"))
			{
				State state = stateEditor.state;
				int behaviourCount = state.behaviourCount;

				for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);
					if (behaviourEditor != null)
					{
						StateBehaviour behaviour = behaviourEditor.behaviourObj as StateBehaviour;
						if (behaviour != null)
						{
							foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
							{
								if (_HoverStateLink == null || _HoverStateLink.stateLink != stateLinkProperty.stateLink)
								{
									DrawBranchStateLink(state, behaviour, stateLinkProperty.stateLink, stateLinkProperty.fieldInfo);
								}
							}
						}
					}
				}
			}
		}

		void DrawStateLinkTransitionCount(StateLink stateLink)
		{
			if (stateLink.stateID != 0)
			{
				Vector2 startPosition = stateLink.bezier.startPosition;
				Vector2 startControl = stateLink.bezier.startControl;
				Vector2 endPosition = stateLink.bezier.endPosition;
				Vector2 endControl = stateLink.bezier.endControl;

				Vector2 v = (endPosition - endControl).normalized * EditorGUITools.kBranchArrowWidth;

				Vector2 pos = Bezier2D.GetPoint(startPosition, startControl, endPosition - v, endControl - v, 0.5f);

				GUIStyle style = Styles.countBadge;
				GUIContent content = new GUIContent(stateLink.transitionCount.ToString());
				Vector2 contentSize = style.CalcSize(content);

				Rect rect = new Rect(pos.x - contentSize.x / 2.0f, pos.y - contentSize.y / 2.0f, contentSize.x, contentSize.y);

				Color lineColor = Color.white;

				int index = stateMachine.IndexOfStateLinkHistory(stateLink);
				if (index != -1)
				{
					float t = (float)index / 4.0f;

					lineColor *= Color.Lerp(Color.white, Color.gray, t);
				}
				else
				{
					lineColor *= Color.gray;
				}

				lineColor = EditorGUITools.GetColorOnGUI(lineColor);

				Color savedColor = GUI.color;
				GUI.color = lineColor;

				EditorGUI.LabelField(rect, content, style);

				GUI.color = savedColor;
			}
		}

		void DrawBehaviourBranchesTransitionCount(StateEditor stateEditor)
		{
			using (new ProfilerScope("DrawBehaviourBranches"))
			{
				State state = stateEditor.state;
				int behaviourCount = state.behaviourCount;

				for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);
					if (behaviourEditor != null)
					{
						foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
						{
							if (_HoverStateLink == null || _HoverStateLink.stateLink != stateLinkProperty.stateLink)
							{
								DrawStateLinkTransitionCount(stateLinkProperty.stateLink);
							}
						}
					}
				}
			}
		}

		void DrawStateLinkBranchies()
		{
			using (new ProfilerScope("States"))
			{
				StateLinkRerouteNodeList stateLinkRerouteNodes = stateMachine.stateLinkRerouteNodes;
				int rerouteCount = stateLinkRerouteNodes.count;
				for (int i = 0; i < rerouteCount; i++)
				{
					StateLinkRerouteNode rerouteNode = stateLinkRerouteNodes[i];

					if (rerouteNode != null)
					{
						if (_HoverStateLink == null || _HoverStateLink.stateLink != rerouteNode.link)
						{
							DrawBranchStateLink(rerouteNode, null, rerouteNode.link, null);
						}
					}
				}

				int stateCount = stateMachine.stateCount;
				for (int i = 0; i < stateCount; i++)
				{
					State state = stateMachine.GetStateFromIndex(i);
					StateEditor stateEditor = GetNodeEditor(state) as StateEditor;

					if (stateEditor != null)
					{
						stateEditor.UpdateBehaviour();

						DrawBehaviourBranches(stateEditor);
					}
				}

				if (Application.isPlaying)
				{
					for (int i = 0; i < stateCount; i++)
					{
						State state = stateMachine.GetStateFromIndex(i);
						StateEditor stateEditor = GetNodeEditor(state) as StateEditor;

						if (stateEditor != null)
						{
							DrawBehaviourBranchesTransitionCount(stateEditor);
						}
					}
				}
			}
		}

		public override void OnDrawBranchies()
		{
			DrawStateLinkBranchies();
		}

		public override void OnDrawHoverBranch()
		{
			if (_HoverStateLink != null)
			{
				DrawBranchStateLink(_HoverStateLink.node, _HoverStateLink.behaviour, _HoverStateLink.stateLink, _HoverStateLink.fieldInfo);
				if (Application.isPlaying)
				{
					if (_HoverStateLink.node is State)
					{
						DrawStateLinkTransitionCount(_HoverStateLink.stateLink);
					}
				}
			}
		}

		public State CreateState(Vector2 position, bool resident)
		{
			Undo.IncrementCurrentGroup();

			State state = stateMachine.CreateState(resident);

			if (state != null)
			{
				Undo.RecordObject(stateMachine, "Created State");

				state.position = EditorGUITools.SnapPositionToGrid(new Rect(position.x, position.y, 300, 100));

				EditorUtility.SetDirty(stateMachine);

				CreateNodeEditor(state);
				UpdateNodeCommentControl(state);

				SetSelectNode(state);

				BeginRename(state.nodeID, state.name);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			Repaint();

			return state;
		}

		public StateLinkRerouteNode CreateStateLinkRerouteNode(Vector2 position, Color lineColor)
		{
			StateLinkRerouteNode stateLinkRerouteNode = stateMachine.CreateStateLinkRerouteNode(EditorGUITools.SnapToGrid(position), lineColor);

			if (stateLinkRerouteNode != null)
			{
				CreateNodeEditor(stateLinkRerouteNode);

				SetSelectNode(stateLinkRerouteNode);
			}

			Repaint();

			return stateLinkRerouteNode;
		}

		void CreateState(object obj)
		{
			Vector2 position = (Vector2)obj;

			CreateState(position, false);
		}

		void CreateResidentState(object obj)
		{
			Vector2 position = (Vector2)obj;

			CreateState(position, true);
		}

		protected override void SetCreateNodeContextMenu(GenericMenu menu, bool editable)
		{
			Event current = Event.current;

			if (editable)
			{
				menu.AddItem(EditorContents.createState, false, CreateState, current.mousePosition);
				menu.AddItem(EditorContents.createResidentState, false, CreateResidentState, current.mousePosition);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.createState);
				menu.AddDisabledItem(EditorContents.createResidentState);
			}
		}

		void ClearCount()
		{
			for (int stateIndex = 0, stateCount = stateMachine.stateCount; stateIndex < stateCount; stateIndex++)
			{
				State state = stateMachine.GetStateFromIndex(stateIndex);
				StateEditor stateEditor = GetNodeEditor(state) as StateEditor;

				state.transitionCount = 0;

				for (int behaviourIndex = 0, behaviourCount = state.behaviourCount; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviourEditorGUI behaviourEditor = stateEditor.GetBehaviourEditor(behaviourIndex);

					if (behaviourEditor != null)
					{
						foreach (StateEditor.StateLinkProperty stateLinkProperty in behaviourEditor.stateLinkProperties)
						{
							stateLinkProperty.stateLink.transitionCount = 0;
						}
					}
				}
			}
		}

		void SetBreakPoints()
		{
			Undo.RecordObject(stateMachine, "BreakPoint On");

			foreach (Node node in selection)
			{
				State state = node as State;
				if (state != null)
				{
					state.breakPoint = true;
				}
			}

			EditorUtility.SetDirty(stateMachine);
		}

		void ReleaseBreakPoints()
		{
			Undo.RecordObject(stateMachine, "BreakPoint Off");

			foreach (Node node in selection)
			{
				State state = node as State;
				if (state != null)
				{
					state.breakPoint = false;
				}
			}

			EditorUtility.SetDirty(stateMachine);
		}

		void ReleaseAllBreakPoints()
		{
			Undo.RecordObject(stateMachine, "Delete All BreakPoint");

			for (int stateIndex = 0, stateCount = stateMachine.stateCount; stateIndex < stateCount; stateIndex++)
			{
				State state = stateMachine.GetStateFromIndex(stateIndex);

				state.breakPoint = false;
			}

			EditorUtility.SetDirty(stateMachine);
		}

		internal static int InternalNodeListSortComparison(NodeEditor a, NodeEditor b)
		{
			StateEditor stateEditorA = a as StateEditor;
			StateEditor stateEditorB = b as StateEditor;
			if (stateEditorA == null || stateEditorB == null)
			{
				return NodeListGUI.Defaults.SortComparison(a, b);
			}

			ArborFSMInternal stateMachine = stateEditorA.graphEditor.nodeGraph as ArborFSMInternal;

			if (stateMachine.startStateID == stateEditorA.state.nodeID)
			{
				return -1;
			}
			else if (stateMachine.startStateID == stateEditorB.state.nodeID)
			{
				return 1;
			}
			if (!stateEditorA.state.resident && stateEditorB.state.resident)
			{
				return -1;
			}
			else if (stateEditorA.state.resident && !stateEditorB.state.resident)
			{
				return 1;
			}
			return stateEditorA.state.name.CompareTo(stateEditorB.state.name);
		}

		protected override int NodeListSortComparison(NodeEditor a, NodeEditor b)
		{
			return InternalNodeListSortComparison(a, b);
		}

		protected override bool HasViewMenu()
		{
			return true;
		}

		protected override void OnSetViewMenu(GenericMenu menu)
		{
			menu.AddItem(EditorContents.stateLinkShowNodeTop, ArborSettings.stateLinkShowMode == StateLinkShowMode.NodeTop, () =>
			{
				ArborSettings.stateLinkShowMode = StateLinkShowMode.NodeTop;
				Repaint();
			});
			menu.AddItem(EditorContents.stateLinkShowBehaviourTop, ArborSettings.stateLinkShowMode == StateLinkShowMode.BehaviourTop, () =>
			{
				ArborSettings.stateLinkShowMode = StateLinkShowMode.BehaviourTop;
				Repaint();
			});
			menu.AddItem(EditorContents.stateLinkShowBehaviourBottom, ArborSettings.stateLinkShowMode == StateLinkShowMode.BehaviourBottom, () =>
			{
				ArborSettings.stateLinkShowMode = StateLinkShowMode.BehaviourBottom;
				Repaint();
			});
			menu.AddItem(EditorContents.stateLinkShowNodeBottom, ArborSettings.stateLinkShowMode == StateLinkShowMode.NodeBottom, () =>
			{
				ArborSettings.stateLinkShowMode = StateLinkShowMode.NodeBottom;
				Repaint();
			});
		}

		protected override bool HasDebugMenu()
		{
			return true;
		}

		protected override void OnSetDebugMenu(GenericMenu menu)
		{
			bool isSelectionState = false;
			foreach (Node node in selection)
			{
				if (node is State)
				{
					isSelectionState = true;
					break;
				}
			}

			bool editable = this.editable;

			if (isSelectionState && editable)
			{
				menu.AddItem(EditorContents.setBreakPoints, false, SetBreakPoints);
				menu.AddItem(EditorContents.releaseBreakPoints, false, ReleaseBreakPoints);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.setBreakPoints);
				menu.AddDisabledItem(EditorContents.releaseBreakPoints);
			}

			if (editable)
			{
				menu.AddItem(EditorContents.releaseAllBreakPoints, false, ReleaseAllBreakPoints);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.releaseAllBreakPoints);
			}

			if (Application.isPlaying && editable)
			{
				menu.AddItem(EditorContents.clearCount, false, ClearCount);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.clearCount);
			}
		}

		public override GUIContent GetGraphLabel()
		{
			return EditorContents.stateMachine;
		}

		public override bool HasPlayState()
		{
			return true;
		}

		public override PlayState GetPlayState()
		{
			return stateMachine.playState;
		}

		bool CheckLoop(StateLinkRerouteNode current, StateLinkRerouteNode target)
		{
			if (current == null)
			{
				return false;
			}

			while (target != null)
			{
				StateLinkRerouteNode nextNode = stateMachine.GetNodeFromID(target.link.stateID) as StateLinkRerouteNode;
				if (nextNode == null)
				{
					break;
				}

				if (nextNode == current)
				{
					return true;
				}

				target = nextNode;
			}

			return false;
		}

		public Node GetTargetNodeFromPosition(Vector2 position, Node node)
		{
			for (int i = 0, count = stateMachine.stateCount; i < count; i++)
			{
				State state = stateMachine.GetStateFromIndex(i);
				if (!state.resident && state.position.Contains(position))
				{
					return state;
				}
			}

			StateLinkRerouteNode rerouteNode = node as StateLinkRerouteNode;

			StateLinkRerouteNodeList stateLinks = stateMachine.stateLinkRerouteNodes;
			for (int i = 0, count = stateLinks.count; i < count; i++)
			{
				StateLinkRerouteNode stateLinkNode = stateLinks[i];
				if (rerouteNode != stateLinkNode && stateLinkNode.position.Contains(position))
				{
					if (!CheckLoop(rerouteNode, stateLinkNode))
					{
						return stateLinkNode;
					}
				}
			}

			return null;
		}

		protected override Node GetActiveNode()
		{
			return stateMachine.currentState;
		}

		public StateBehaviour AddSetParameterBehaviour(State state, Parameter parameter)
		{
			Arbor.ParameterBehaviours.SetParameterBehaviourInternal setParameterBehaviour = state.AddBehaviour(Types.SetParameterBehaviourType) as Arbor.ParameterBehaviours.SetParameterBehaviourInternal;

			Undo.RecordObject(setParameterBehaviour, "Add Behaviour");

			setParameterBehaviour.SetParameter(parameter);

			EditorUtility.SetDirty(setParameterBehaviour);

			return setParameterBehaviour;
		}

		public StateBehaviour InsertSetParameterBehaviour(State state, int index, Parameter parameter)
		{
			Arbor.ParameterBehaviours.SetParameterBehaviourInternal setParameterBehaviour = state.InsertBehaviour(index, Types.SetParameterBehaviourType) as Arbor.ParameterBehaviours.SetParameterBehaviourInternal;

			Undo.RecordObject(setParameterBehaviour, "Insert Behaviour");

			setParameterBehaviour.SetParameter(parameter);

			EditorUtility.SetDirty(setParameterBehaviour);

			return setParameterBehaviour;
		}


		protected override void OnCreateSetParameter(Vector2 position, Parameter parameter)
		{
			Undo.IncrementCurrentGroup();
			int undoGroup = Undo.GetCurrentGroup();

			State state = CreateState(position, false);

			AddSetParameterBehaviour(state, parameter);

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(nodeGraph);
		}
	}
}