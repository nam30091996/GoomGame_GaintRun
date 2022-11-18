//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;
	using Arbor.Serialization;

	[System.Serializable]
	public sealed class StateBehaviourEditorGUI : BehaviourEditorGUI
	{
		public StateEditor stateEditor
		{
			get
			{
				return nodeEditor as StateEditor;
			}
		}

		public State state
		{
			get
			{
				return (nodeEditor != null) ? nodeEditor.node as State : null;
			}
		}

		public List<StateEditor.StateLinkProperty> stateLinkProperties = new List<StateEditor.StateLinkProperty>();

		private StateLinkSettingWindow _StateLinkSettingWindow = null;

		private static System.Text.StringBuilder s_StateLinkPropertyBuilder = new System.Text.StringBuilder();

		void AddStateLinkProperty(SerializedProperty property, System.Reflection.FieldInfo fieldInfo, StateLink stateLink)
		{
			if (stateLink == null)
			{
				return;
			}

			StateEditor.StateLinkProperty stateLinkProperty = new StateEditor.StateLinkProperty();
			if (!string.IsNullOrEmpty(stateLink.name))
			{
				stateLinkProperty.label = EditorGUITools.GetTextContent(stateLink.name);
			}
			else
			{
				stateLinkProperty.label = EditorGUITools.GetTextContent(s_StateLinkPropertyBuilder.ToString());
			}
			stateLinkProperty.property = property.Copy();
			stateLinkProperty.stateLink = stateLink;
			stateLinkProperty.fieldInfo = fieldInfo;

			stateLinkProperties.Add(stateLinkProperty);
		}

		void UpdateStateLinkProperty(SerializedProperty property, System.Reflection.FieldInfo fieldInfo, System.Type fieldType, object value)
		{
			if (property.isArray)
			{
				System.Type elementType = SerializationUtility.ElementTypeOfArray(fieldType);

				int currentLength = s_StateLinkPropertyBuilder.Length;

				IList list = (IList)value;

				for (int i = 0; i < property.arraySize; i++)
				{
					s_StateLinkPropertyBuilder.Append("[");
					s_StateLinkPropertyBuilder.Append(i);
					s_StateLinkPropertyBuilder.Append("]");

					SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

					object elementValue = list[i];

					if (elementType == typeof(StateLink))
					{
						AddStateLinkProperty(elementProperty, fieldInfo, elementValue as StateLink);
					}
					else
					{
						UpdateStateLinkProperty(elementProperty, fieldInfo, elementType, elementValue);
					}

					s_StateLinkPropertyBuilder.Length = currentLength;
				}
			}
			else if (fieldType == typeof(StateLink))
			{
				AddStateLinkProperty(property, fieldInfo, value as StateLink);
			}
			else
			{
#if UNITY_2019_3_OR_NEWER
				if (property.propertyType == SerializedPropertyType.ManagedReference)
				{
					if(value == null)
					{
						return;
					}

					fieldType = property.GetTypeFromManagedReferenceFullTypeName();
					if (fieldType == null)
					{
						return;
					}
				}
#endif

				int currentLength = s_StateLinkPropertyBuilder.Length;

				foreach (Arbor.DynamicReflection.DynamicField dynamicField in EachField<StateLink>.GetFields(fieldType))
				{
					System.Reflection.FieldInfo fi = dynamicField.fieldInfo;

					SerializedProperty p = property.FindPropertyRelative(fi.Name);
					object elementValue = dynamicField.GetValue(value);

					s_StateLinkPropertyBuilder.Append("/");
					s_StateLinkPropertyBuilder.Append(p.displayName);

					UpdateStateLinkProperty(p, fi, fi.FieldType, elementValue);

					s_StateLinkPropertyBuilder.Length = currentLength;
				}
			}
		}

		private void UpdateStateLinkInternal()
		{
			using (new ProfilerScope("UpdateStateLink"))
			{
				if (editor == null)
				{
					return;
				}

				SerializedObject serializedObject = editor.serializedObject;

				Object targetObject = serializedObject.targetObject;

				System.Type classType = targetObject.GetType();

				if (stateLinkProperties == null)
				{
					stateLinkProperties = new List<StateEditor.StateLinkProperty>();
				}

				stateLinkProperties.Clear();

				foreach (Arbor.DynamicReflection.DynamicField dynamicField in EachField<StateLink>.GetFields(classType))
				{
					System.Reflection.FieldInfo fieldInfo = dynamicField.fieldInfo;

					object value = dynamicField.GetValue(targetObject);

					SerializedProperty property = serializedObject.FindProperty(fieldInfo.Name);

					s_StateLinkPropertyBuilder.Length = 0;
					s_StateLinkPropertyBuilder.Append(property.displayName);

					UpdateStateLinkProperty(property, fieldInfo, fieldInfo.FieldType, value);
				}
			}
		}

		public void UpdateStateLink()
		{
			if (editor == null)
			{
				return;
			}

			SerializedObject serializedObject = editor.serializedObject;

			serializedObject.Update();

			UpdateStateLinkInternal();

			serializedObject.ApplyModifiedProperties();
		}

		protected override bool HasTitlebar()
		{
			return true;
		}

		public override bool GetExpanded()
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;
			return (behaviour != null) ? BehaviourEditorUtility.GetExpanded(behaviour, behaviour.expanded) : this.expanded;
		}

		public override void SetExpanded(bool expanded)
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;
			if (behaviour != null)
			{
				if ((behaviour.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable)
				{
					behaviour.expanded = expanded;
					EditorUtility.SetDirty(behaviour);
				}
				BehaviourEditorUtility.SetExpanded(behaviour, expanded);
			}
			else
			{
				this.expanded = expanded;
			}
		}

		protected override bool HasBehaviourEnable()
		{
			return true;
		}

		protected override bool GetBehaviourEnable()
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;
			return behaviour.behaviourEnabled;
		}

		protected override void SetBehaviourEnable(bool enable)
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;
			behaviour.behaviourEnabled = enable;
		}

		protected override void SetPopupMenu(GenericMenu menu)
		{
			bool editable = nodeEditor.graphEditor.editable;

			int behaviourCount = state.behaviourCount;

			if (behaviourIndex >= 1 && editable)
			{
				menu.AddItem(EditorContents.moveUp, false, MoveUpBehaviourContextMenu);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.moveUp);
			}

			if (behaviourIndex < behaviourCount - 1 && editable)
			{
				menu.AddItem(EditorContents.moveDown, false, MoveDownBehaviourContextMenu);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.moveDown);
			}

			StateBehaviour behaviour = behaviourObj as StateBehaviour;
			if (behaviour != null)
			{
				menu.AddItem(EditorContents.copy, false, CopyBehaviourContextMenu);
				if (Clipboard.CompareBehaviourType(behaviourObj.GetType(), false) && editable)
				{
					menu.AddItem(EditorContents.paste, false, PasteBehaviourContextMenu);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.paste);
				}
			}

			if (editable)
			{
				menu.AddItem(EditorContents.delete, false, DeleteBehaviourContextMenu);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.delete);
			}
		}

		void MoveUpBehaviourContextMenu()
		{
			if (stateEditor != null)
			{
				stateEditor.MoveBehaviour(behaviourIndex, behaviourIndex - 1);
			}
		}

		void MoveDownBehaviourContextMenu()
		{
			if (stateEditor != null)
			{
				stateEditor.MoveBehaviour(behaviourIndex, behaviourIndex + 1);
			}
		}

		void CopyBehaviourContextMenu()
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;

			Clipboard.CopyBehaviour(behaviour);
		}

		void PasteBehaviourContextMenu()
		{
			StateBehaviour behaviour = behaviourObj as StateBehaviour;

			Undo.IncrementCurrentGroup();

			Undo.RecordObject(behaviour, "Paste Behaviour");

			Clipboard.PasteBehaviourValues(behaviour);

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			EditorUtility.SetDirty(behaviour);
		}

		void DeleteBehaviourContextMenu()
		{
			if (stateEditor != null)
			{
				stateEditor.RemoveBehaviour(behaviourIndex);
			}
		}

		public bool StateLinkGUI(GUIStyle headerStyle)
		{
			if (editor == null)
			{
				return false;
			}

			editor.serializedObject.Update();

			UpdateStateLinkInternal();

			bool hasStateLinks = false;

			if (stateLinkProperties != null)
			{
				int stateLinkCount = stateLinkProperties.Count;
				if (stateLinkCount > 0)
				{
					hasStateLinks = true;

					HeaderSpace();

					if (headerStyle != null && headerStyle != GUIStyle.none)
					{
						BehaviourInfo behaviourInfo = BehaviourInfoUtility.GetBehaviourInfo(behaviourObj);
						GUILayout.Box(behaviourInfo.titleContent, headerStyle);
					}

					EditorGUILayout.BeginVertical(Styles.stateLinkMargin);

					for (int i = 0; i < stateLinkCount; i++)
					{
						StateEditor.StateLinkProperty stateLinkProperty = stateLinkProperties[i];
						SingleStateLinkField(stateLinkProperty);
					}

					EditorGUILayout.EndVertical();
				}
			}

			editor.serializedObject.ApplyModifiedProperties();

			return hasStateLinks;
		}

		public override void OnTopGUI()
		{
			if (ArborSettings.stateLinkShowMode == StateLinkShowMode.BehaviourTop)
			{
				StateLinkGUI(null);
			}
		}

		public override void OnBottomGUI()
		{
			if (ArborSettings.stateLinkShowMode == StateLinkShowMode.BehaviourBottom)
			{
				StateLinkGUI(null);
			}
		}

		private static Node _DragTargetNode = null;

		private static readonly int s_StateLinkHash = "s_StateLinkHash".GetHashCode();

		static TransitionTiming GetTransitionTiming(SerializedProperty property, System.Reflection.FieldInfo stateLinkFieldInfo)
		{
			FixedTransitionTiming fixedTransitionTiming = AttributeHelper.GetAttribute<FixedTransitionTiming>(stateLinkFieldInfo);
			FixedImmediateTransition fixedImmediateTransition = AttributeHelper.GetAttribute<FixedImmediateTransition>(stateLinkFieldInfo);

			TransitionTiming transitionTiming = TransitionTiming.LateUpdateDontOverwrite;

			if (fixedTransitionTiming != null)
			{
				transitionTiming = fixedTransitionTiming.transitionTiming;
			}
			else if (fixedImmediateTransition != null)
			{
				transitionTiming = fixedImmediateTransition.immediate ? TransitionTiming.Immediate : TransitionTiming.LateUpdateOverwrite;
			}
			else
			{
				SerializedProperty transitionTimingProperty = property.FindPropertyRelative("transitionTiming");
				transitionTiming = EnumUtility.GetValueFromIndex<TransitionTiming>(transitionTimingProperty.enumValueIndex);
			}

			return transitionTiming;
		}

		static Bezier2D GetTargetBezier(Vector2 targetPos, Vector2 leftPos, Vector2 rightPos, ref bool isRight)
		{
			bool right = (targetPos - leftPos).magnitude > (targetPos - rightPos).magnitude;

			Vector2 startPos;
			Vector2 startTangent;

			if (right)
			{
				isRight = true;
				startPos = rightPos;
				startTangent = rightPos + EditorGUITools.kBezierTangentOffset;
			}
			else
			{
				isRight = false;
				startPos = leftPos;
				startTangent = leftPos - EditorGUITools.kBezierTangentOffset;
			}

			return new Bezier2D(startPos, startTangent, targetPos, startTangent);
		}

		void SingleStateLinkField(StateEditor.StateLinkProperty stateLinkProperty)
		{
			using (new ProfilerScope("SingleStateLinkField"))
			{
				EditorGUI.BeginDisabledGroup(nodeEditor.graphEditor != null && !nodeEditor.graphEditor.editable);

				SerializedProperty property = stateLinkProperty.property;
				StateBehaviour behaviour = property.serializedObject.targetObject as StateBehaviour;

				if (behaviour == null || behaviour.nodeID == 0 || behaviour.stateMachine == null || property.isArray)
				{
					GUIContent helpContent = EditorGUITools.GetHelpBoxContent("StateLink can only be used with ArborEditor.", MessageType.Error);

					Rect position = GUILayoutUtility.GetRect(helpContent, EditorStyles.helpBox);
					EditorGUI.HelpBox(position, helpContent.text, MessageType.Error);
				}
				else
				{
					int controlID = EditorGUIUtility.GetControlID(s_StateLinkHash, FocusType.Passive);

					bool isActive = GUIUtility.hotControl == controlID;

					GUIStyle style = isActive ? Styles.nodeLinkSlotActive : Styles.nodeLinkSlot;

					GUIContent label = stateLinkProperty.label;
					StateLink stateLink = stateLinkProperty.stateLink;

					TransitionTiming transitionTiming = GetTransitionTiming(property, stateLinkProperty.fieldInfo);

					label.image = Icons.GetTransitionTimingIcon(transitionTiming);

					Rect position = GUILayoutUtility.GetRect(label, style, GUILayout.Height(18f));

					Rect nodePosition = nodeEditor.node.position;
					nodePosition.position = Vector2.zero;

					ArborFSMInternal stateMachine = behaviour.stateMachine;
					State state = stateMachine.GetStateFromID(behaviour.nodeID);

					Event currentEvent = Event.current;

					Node targetNode = stateMachine.GetNodeFromID(stateLink.stateID);

					Vector2 nowPos = currentEvent.mousePosition;

					Vector2 leftPos = new Vector2(nodePosition.xMin, position.center.y);
					Vector2 rightPos = new Vector2(nodePosition.xMax, position.center.y);

					Bezier2D bezier = new Bezier2D();
					bool isRight = true;
					if (targetNode != null)
					{
						bezier = EditorGUITools.GetTargetBezier(state, targetNode, leftPos, rightPos, ref isRight);
					}
					else
					{
						bezier.startPosition = rightPos;
					}

					Bezier2D draggingBezier = new Bezier2D();
					bool isDraggingRight = true;
					if (isActive)
					{
						if (_DragTargetNode != null)
						{
							draggingBezier = EditorGUITools.GetTargetBezier(state, _DragTargetNode, leftPos, rightPos, ref isDraggingRight);
						}
						else
						{
							draggingBezier = GetTargetBezier(nowPos, leftPos, rightPos, ref isDraggingRight);
						}
					}

					GUIStyle stateLinkPinStyle = isRight ? Styles.stateLinkRightPin : Styles.stateLinkLeftPin;
					Vector2 pinSize = stateLinkPinStyle.CalcSize(GUIContent.none);

					Rect boxRect = new Rect(isRight ? position.xMax - pinSize.x : position.xMin, position.y, pinSize.x, pinSize.y);
					boxRect.y += Mathf.Floor((position.height - boxRect.height) / 2f);

					GUIStyle draggingStateLinkPinStyle = isDraggingRight ? Styles.stateLinkRightPin : Styles.stateLinkLeftPin;
					Vector2 draggingPinSize = draggingStateLinkPinStyle.CalcSize(GUIContent.none);

					Rect draggingBoxRect = new Rect(isDraggingRight ? position.xMax - draggingPinSize.x : position.xMin, position.y, draggingPinSize.x, draggingPinSize.y);
					draggingBoxRect.y += Mathf.Floor((position.height - draggingBoxRect.height) / 2f);

					Rect settingRect = position;

					GUIStyle settingStyle = Styles.popupIconButton;
					GUIContent settingContent = EditorContents.stateLinkPopupIcon;
					Vector2 settingButtonSize = settingStyle.CalcSize(settingContent);

					settingRect.x += position.width - settingButtonSize.x - pinSize.x - 4f;
					settingRect.y += Mathf.Floor((position.height - settingButtonSize.y) / 2);
					settingRect.height = settingButtonSize.x;
					settingRect.width = settingButtonSize.y;

					Vector2 bezierStartPosition = bezier.startPosition;

					Vector2 statePosition = new Vector2(state.position.x, state.position.y);

					if (nodeEditor != null)
					{
						bezier.startPosition = nodeEditor.NodeToGraphPoint(bezier.startPosition);
						bezier.startControl = nodeEditor.NodeToGraphPoint(bezier.startControl);
					}
					else
					{
						bezier.startPosition += statePosition;
						bezier.startControl += statePosition;
					}
					bezier.endPosition += statePosition;
					bezier.endControl += statePosition;

					Vector2 draggingBezierStartPosition = draggingBezier.startPosition;

					if (nodeEditor != null)
					{
						draggingBezier.startPosition = nodeEditor.NodeToGraphPoint(draggingBezier.startPosition);
						draggingBezier.startControl = nodeEditor.NodeToGraphPoint(draggingBezier.startControl);
					}
					else
					{
						draggingBezier.startPosition = statePosition;
						draggingBezier.startControl = statePosition;
					}
					draggingBezier.endPosition += statePosition;
					draggingBezier.endControl += statePosition;

					StateMachineGraphEditor graphEditor = (stateEditor != null) ? stateEditor.graphEditor as StateMachineGraphEditor : null;

					Color lineColor = stateLink.lineColorChanged ? stateLink.lineColor : Color.white;

					EventType eventType = currentEvent.GetTypeForControl(controlID);
					switch (eventType)
					{
						case EventType.MouseDown:
							if (position.Contains(nowPos) && !settingRect.Contains(nowPos))
							{
								if (currentEvent.button == 0)
								{
									GUIUtility.hotControl = GUIUtility.keyboardControl = controlID;

									_DragTargetNode = null;

									if (graphEditor != null)
									{
										graphEditor.BeginDragStateBranch(state.nodeID);
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

								Node hoverNode = !position.Contains(nowPos) ? graphEditor.GetTargetNodeFromPosition(nowPos + statePosition, state) : null;

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

											Undo.RecordObject(behaviour, "Link State");

											stateLink.stateID = newState.nodeID;
											stateLink.bezier = bezier;

											Undo.CollapseUndoOperations(undoGroup);

											EditorUtility.SetDirty(behaviour);
										});

										menu.AddItem(EditorContents.reroute, false, () =>
										{
											Undo.IncrementCurrentGroup();
											int undoGroup = Undo.GetCurrentGroup();

											mousePosition -= new Vector2(16f, 16f);

											StateLinkRerouteNode newStateLinkNode = graphEditor.CreateStateLinkRerouteNode(mousePosition, lineColor);

											Undo.RecordObject(behaviour, "Link State");

											stateLink.stateID = newStateLinkNode.nodeID;
											stateLink.bezier = bezier;

											Undo.CollapseUndoOperations(undoGroup);

											EditorUtility.SetDirty(behaviour);
										});

										menu.AddSeparator("");

										menu.AddItem(EditorContents.nodeListSelection, false, () =>
										{
											StateLink currentStateLink = stateLink;
											StateBehaviour currentBehaviour = behaviour;

											StateLinkSelectorWindow.instance.Open(graphEditor, new Rect(screenMousePosition, Vector2.zero), currentStateLink.stateID,
												(targetNodeEditor) =>
												{
													Undo.RecordObject(currentBehaviour, "Link State");

													currentStateLink.stateID = targetNodeEditor.nodeID;
													currentStateLink.bezier = draggingBezier;

													EditorUtility.SetDirty(currentBehaviour);

													//graphEditor.BeginFrameSelected(targetNodeEditor.node);
												}
											);
										});

										if (stateLink.stateID != 0)
										{
											menu.AddSeparator("");
											menu.AddItem(EditorContents.disconnect, false, () =>
											{
												Undo.RecordObject(behaviour, "Disconect StateLink");

												stateLink.stateID = 0;

												EditorUtility.SetDirty(behaviour);
											});
										}
										menu.ShowAsContext();
									}
									else if (_DragTargetNode != targetNode)
									{
										Undo.RecordObject(behaviour, "Link State");

										stateLink.stateID = _DragTargetNode.nodeID;
										stateLink.bezier = draggingBezier;

										EditorUtility.SetDirty(behaviour);
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
									if (graphEditor != null)
									{
										graphEditor.Repaint();
									}
								}
							}

							bool isConnected = targetNode != null;

							bool on = isActive || isConnected;

							Color slotColor = isActive ? Color.white : new Color(lineColor.r, lineColor.g, lineColor.b);

							Color slotBackgroundColor = EditorGUITools.GetSlotBackgroundColor(slotColor, isActive, on);

							Color backgroundColor = GUI.backgroundColor;
							GUI.backgroundColor = slotBackgroundColor;
							style.Draw(position, label, controlID, on);
							GUI.backgroundColor = backgroundColor;

							if (isConnected)
							{
								EditorGUITools.DrawLines(Styles.outlineConnectionTexture, lineColor, 8.0f, boxRect.center, bezierStartPosition);
							}

							if (isConnected || !isActive)
							{
								GUI.backgroundColor = new Color(lineColor.r, lineColor.g, lineColor.b);
								stateLinkPinStyle.Draw(boxRect, GUIContent.none, controlID, on);
								GUI.backgroundColor = backgroundColor;
							}

							if (isActive)
							{
								EditorGUITools.DrawLines(Styles.outlineConnectionTexture, NodeGraphEditor.dragBezierColor, 8.0f, draggingBoxRect.center, draggingBezierStartPosition);

								GUI.backgroundColor = NodeGraphEditor.dragBezierColor;
								draggingStateLinkPinStyle.Draw(draggingBoxRect, GUIContent.none, controlID, on);
								GUI.backgroundColor = backgroundColor;
							}
							break;
					}

					if (EditorGUITools.ButtonMouseDown(settingRect, settingContent, FocusType.Passive, settingStyle))
					{
						if (_StateLinkSettingWindow == null)
						{
							_StateLinkSettingWindow = new StateLinkSettingWindow();
						}
						_StateLinkSettingWindow.Init(graphEditor != null ? graphEditor.hostWindow : null, behaviourObj, stateLink, stateLinkProperty.fieldInfo, false);

						PopupWindowUtility.Show(settingRect, _StateLinkSettingWindow, true);
					}
				}

				EditorGUI.EndDisabledGroup();
			}
		}

		protected override void OnUnderlayGUI(Rect rect)
		{
			Event currentEvent = Event.current;
			if (currentEvent.type != EventType.Repaint || !Application.isPlaying)
			{
				return;
			}

			StateBehaviour stateBehaviour = behaviourObj as StateBehaviour;

			if (stateBehaviour == null || !stateBehaviour.behaviourEnabled)
			{
				return;
			}

			ArborFSMInternal stateMachine = stateBehaviour.stateMachine;
			if (stateMachine == null ||
				stateMachine.playState == PlayState.Stopping ||
				stateMachine.currentState != state)
			{
				return;
			}

			if (!stateBehaviour.isCalledActivate)
			{
				Color conditionColor = StateMachineGraphEditor.reservedColor;

				rect.width = 5f;
				EditorGUI.DrawRect(rect, conditionColor);
			}
		}
	}
}