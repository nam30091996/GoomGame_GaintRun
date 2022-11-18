//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;

	[System.Serializable]
	internal abstract class BehaviourEditorList<TEditor> where TEditor : BehaviourEditorGUI, new()
	{
		public NodeEditor nodeEditor = null;
		public NodeGraphEditor graphEditor
		{
			get
			{
				if (nodeEditor == null)
				{
					return null;
				}
				return nodeEditor.graphEditor;
			}
		}
		public Node node
		{
			get
			{
				if (nodeEditor == null)
				{
					return null;
				}
				return nodeEditor.node;
			}
		}

		public virtual Color backgroundColor
		{
			get
			{
				return Color.white;
			}
		}
		public virtual GUIStyle backgroundStyle
		{
			get
			{
				return GUIStyle.none;
			}
		}

		public abstract System.Type targetType
		{
			get;
		}

		public virtual bool isDroppableParameter
		{
			get
			{
				return false;
			}
		}

		private List<TEditor> _BehaviourEditors = null;
		private List<TEditor> _NewBehaviourEditors = null;

		private List<TEditor> behaviourEditors
		{
			get
			{
				if (_BehaviourEditors == null || GetCount() != _BehaviourEditors.Count)
				{
					RebuildBehaviourEditors();
				}

				return _BehaviourEditors;
			}
		}

		public abstract int GetCount();
		public abstract Object GetObject(int behaviourIndex);
		public abstract void InsertBehaviour(int index, System.Type classType);
		public abstract void MoveBehaviour(Node fromNode, int fromIndex, Node toNode, int toIndex, bool isCopy);
		public abstract void OpenBehaviourMenu(Rect buttonRect, int index);
		public abstract void PasteBehaviour(int index);
		public abstract GUIContent GetInsertButtonContent();
		public abstract GUIContent GetAddBehaviourContent();
		public abstract GUIContent GetPasteBehaviourContent();

		public void RebuildBehaviourEditors()
		{
			if (_NewBehaviourEditors == null)
			{
				_NewBehaviourEditors = new List<TEditor>();
			}
			List<TEditor> newBehaviourEditors = _NewBehaviourEditors;

			int behaviourCount = GetCount();
			for (int i = 0, count = behaviourCount; i < count; i++)
			{
				Object behaviourObj = GetObject(i);

				TEditor behaviourEditor = null;

				if (_BehaviourEditors != null)
				{
					foreach (var e in _BehaviourEditors)
					{
						if (e.behaviourObj == behaviourObj)
						{
							behaviourEditor = e;
							_BehaviourEditors.Remove(e);
							break;
						}
					}
				}

				if (behaviourEditor == null)
				{
					behaviourEditor = new TEditor();
					behaviourEditor.Initialize(nodeEditor, behaviourObj);
				}

				behaviourEditor.behaviourIndex = i;
				newBehaviourEditors.Add(behaviourEditor);
			}

			if (_BehaviourEditors != null)
			{
				foreach (var e in _BehaviourEditors)
				{
					e.DestroyEditor();
				}
				_BehaviourEditors.Clear();
			}

			_NewBehaviourEditors = _BehaviourEditors;

			_BehaviourEditors = newBehaviourEditors;
		}

		public TEditor GetBehaviourEditor(int behaviourIndex)
		{
			using (new ProfilerScope("GetBehaviourEditor"))
			{
				TEditor behaviourEditor = behaviourEditors[behaviourIndex];

				if (!ComponentUtility.IsValidObject(behaviourEditor.behaviourObj))
				{
					Object behaviourObj = GetObject(behaviourIndex);
					behaviourEditor.Repair(behaviourObj);
				}

				if (behaviourEditor != null)
				{
#if ARBOR_DEBUG
					if (behaviourEditor.behaviourIndex != behaviourIndex)
					{
						Debug.Log(behaviourEditor.behaviourIndex + " -> " + behaviourIndex);
					}
#endif
					behaviourEditor.behaviourIndex = behaviourIndex;
				}

				return behaviourEditor;
			}
		}

		public void Validate()
		{
			RebuildBehaviourEditors();

			for (int i = 0, count = _BehaviourEditors.Count; i < count; i++)
			{
				TEditor behaviourEditor = _BehaviourEditors[i];
				behaviourEditor.Validate();
			}
		}

		public void MoveBehaviourEditor(int fromIndex, int toIndex)
		{
			List<TEditor> behaviourEditors = this.behaviourEditors;

			TEditor tempEditor = behaviourEditors[toIndex];
			behaviourEditors[toIndex] = behaviourEditors[fromIndex];
			behaviourEditors[fromIndex] = tempEditor;

			behaviourEditors[fromIndex].behaviourIndex = fromIndex;
			behaviourEditors[toIndex].behaviourIndex = toIndex;
		}

		public void RemoveBehaviourEditor(int behaviourIndex)
		{
			List<TEditor> behaviourEditors = this.behaviourEditors;

			TEditor behaviourEditor = behaviourEditors[behaviourIndex];

			if (behaviourEditor != null)
			{
				behaviourEditor.DestroyEditor();
			}

			behaviourEditors.RemoveAt(behaviourIndex);

			for (int i = behaviourIndex, count = behaviourEditors.Count; i < count; i++)
			{
				TEditor e = behaviourEditors[i];
				e.behaviourIndex = i;
			}
		}

		public void DestroyEditors()
		{
			if (_BehaviourEditors == null)
			{
				return;
			}

			int editorCount = _BehaviourEditors.Count;
			for (int editorIndex = 0; editorIndex < editorCount; editorIndex++)
			{
				_BehaviourEditors[editorIndex].DestroyEditor();
			}
			_BehaviourEditors.Clear();
		}

		public TEditor InsertBehaviourEditor(int behaviourIndex, Object behaviourObj)
		{
			TEditor behaviourEditor = new TEditor();
			behaviourEditor.Initialize(nodeEditor, behaviourObj);
			behaviourEditor.behaviourIndex = behaviourIndex;

			List<TEditor> behaviourEditors = this.behaviourEditors;

			behaviourEditors.Insert(behaviourIndex, behaviourEditor);

			for (int i = behaviourIndex + 1, count = behaviourEditors.Count; i < count; i++)
			{
				TEditor e = behaviourEditors[i];
				e.behaviourIndex = i;
			}

			return behaviourEditor;
		}

		public virtual void InsertSetParameter(int index, Parameter parameter)
		{
		}

		private static readonly int s_DoDragBehaviourHash = "DoDragBehaviour".GetHashCode();

		public void DropBehaviour(Rect position, int index, bool top, bool last)
		{
			bool editable = graphEditor.editable;
			if (!editable)
			{
				return;
			}

			bool isDragAndDrop = false;
			bool draggable = false;
			BehaviourDragInfo behaviourDragInfo = BehaviourDragInfo.GetBehaviourDragInfo();
			Node fromNode = null;
			int fromIndex = -1;

			bool isDroppableParameter = this.isDroppableParameter;

			Rect insertBarRect = new Rect(position);
			insertBarRect.height = EditorGUIUtility.singleLineHeight * 0.5f;
			insertBarRect.xMin = position.center.x - 16f;
			insertBarRect.xMax = position.center.x + 16f;
			if (last)
			{
				insertBarRect.yMin = position.yMax - insertBarRect.height * 0.5f;
				insertBarRect.yMax = position.yMax;
			}
			else
			{
				insertBarRect.y -= insertBarRect.height * 0.5f;
			}

			GUIStyle insertionStyle = last ? Styles.insertion : Styles.insertionAbove;

			float styleHeight = Styles.kInsertionHeight;

			if (last)
			{
				position.yMin -= styleHeight;
			}
			position.height = styleHeight;

			Rect dropRect = new Rect(position);
			dropRect.height = EditorGUIUtility.singleLineHeight;
			if (last)
			{
				dropRect.yMin = dropRect.yMax - dropRect.height * 0.5f;
			}
			else if (top)
			{
				dropRect.yMax = dropRect.yMin + dropRect.height * 0.5f;
			}
			else
			{
				dropRect.y -= dropRect.height * 0.5f;
			}

			Rect insertButtonRect = new Rect(position);
			insertButtonRect.height = EditorGUIUtility.singleLineHeight;
			insertButtonRect.width = 16.0f;

			Vector2 center = insertButtonRect.center;
			center.x = Mathf.Clamp(Event.current.mousePosition.x, insertBarRect.xMin, insertBarRect.xMax);
			insertButtonRect.center = center;

			if (last)
			{
				insertButtonRect.y += insertButtonRect.height * 0.5f;
			}
			else
			{
				insertButtonRect.y -= insertButtonRect.height * 0.5f;
			}

			int controlId = GUIUtility.GetControlID(s_DoDragBehaviourHash, FocusType.Passive, position);

			Event current = Event.current;

			bool isCopy = (Application.platform == RuntimePlatform.OSXEditor) ? current.alt : current.control;

			if (behaviourDragInfo != null && behaviourDragInfo.behaviourEditor != null && behaviourDragInfo.dragging)
			{
				BehaviourEditorGUI behaviourEditor = behaviourDragInfo.behaviourEditor;

				if (typeof(TEditor).IsAssignableFrom(behaviourEditor.GetType()))
				{
					if (behaviourEditor.nodeEditor.graphEditor.nodeGraph == node.nodeGraph)
					{
						fromNode = behaviourEditor.nodeEditor.node;
					}
					else
					{
						fromNode = node;
					}

					fromIndex = behaviourEditor.behaviourIndex;
					if (fromIndex >= 0)
					{
						if (!isCopy && fromNode == node)
						{
							if (fromIndex < index)
							{
								index--;
							}
						}

						if (fromNode == node && !isCopy)
						{
							draggable = fromIndex != index;
						}
						else if (behaviourEditor.behaviourObj != null)
						{
							System.Type classType = behaviourEditor.behaviourObj.GetType();
							draggable = classType.IsSubclassOf(targetType);
						}
					}
				}
			}
			else
			{
				foreach (Object draggedObject in DragAndDrop.objectReferences)
				{
					MonoScript script = draggedObject as MonoScript;
					if (script != null)
					{
						System.Type classType = script.GetClass();

						if (classType != null && classType.IsSubclassOf(targetType))
						{
							isDragAndDrop = true;
							break;
						}
					}
					else if (isDroppableParameter)
					{
						ParameterDraggingObject parameterDraggingObject = draggedObject as ParameterDraggingObject;
						if (parameterDraggingObject != null)
						{
							isDragAndDrop = true;
							break;
						}
					}
				}

				draggable = isDragAndDrop;
			}

			EventType typeForControl = current.GetTypeForControl(controlId);
			switch (typeForControl)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					bool perform = typeForControl == EventType.DragPerform;
					if (dropRect.Contains(current.mousePosition))
					{
						if (draggable)
						{
							bool isAccepted = false;

							if (isDragAndDrop)
							{
								int insertIndex = index;
								foreach (Object draggedObject in DragAndDrop.objectReferences)
								{
									MonoScript script = draggedObject as MonoScript;
									if (script != null)
									{
										System.Type classType = script.GetClass();

										if (classType.IsSubclassOf(targetType))
										{
											DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

											if (perform)
											{
												InsertBehaviour(insertIndex, classType);
												insertIndex++;

												isAccepted = true;
											}

											current.Use();
										}
									}
									else if (isDroppableParameter)
									{
										ParameterDraggingObject parameterDraggingObject = draggedObject as ParameterDraggingObject;
										if (parameterDraggingObject != null)
										{
											DragAndDrop.visualMode = DragAndDropVisualMode.Link;

											if (perform)
											{
												InsertSetParameter(insertIndex, parameterDraggingObject.parameter);
												insertIndex++;

												isAccepted = true;
											}

											current.Use();
										}
									}
								}
							}
							else
							{
								if (isCopy)
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
								}
								else
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Move;
								}

								if (perform)
								{
									MoveBehaviour(fromNode, fromIndex, node, index, isCopy);

									isAccepted = true;
								}

								current.Use();
							}

							if (isAccepted)
							{
								DragAndDrop.AcceptDrag();
								DragAndDrop.activeControlID = 0;

								EditorGUIUtility.ExitGUI();
							}
						}
						else
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}
					}
					break;
				case EventType.MouseMove:
					if (!(draggable && (isDragAndDrop || behaviourDragInfo.dragging)) && insertBarRect.Contains(current.mousePosition))
					{
						GUIStyle style = Styles.addBehaviourButton;
						GUIContent content = GetInsertButtonContent();

						Rect popupButtonRect = nodeEditor.NodeToGraphRect(insertButtonRect);
						popupButtonRect.size = style.CalcSize(content);

						Rect buttonRect = EditorGUITools.GUIToScreenRect(position);
						if (!last)
						{
							buttonRect.height = 0f;
						}

						graphEditor.ShowPopupButtonControl(popupButtonRect, content, controlId, style, (Rect rect) =>
						{
							GenericMenu menu = new GenericMenu();
							menu.AddItem(GetAddBehaviourContent(), false, () =>
							{
								OpenBehaviourMenu(buttonRect, index);
							});

							GUIContent pasteContent = GetPasteBehaviourContent();

							if (Clipboard.CompareBehaviourType(typeof(StateBehaviour), true))
							{
								menu.AddItem(pasteContent, false, () =>
								{
									PasteBehaviour(index);
								});
							}
							else
							{
								menu.AddDisabledItem(pasteContent);
							}

							menu.DropDown(rect);
						});
					}
					break;
				case EventType.Repaint:
					{
						bool draw = false;
						if (draggable && (isDragAndDrop || behaviourDragInfo.dragging))
						{
							if (dropRect.Contains(current.mousePosition))
							{
								draw = true;
							}
						}
						else if (graphEditor.GetPopupButtonActiveControlID() == controlId)
						{
							draw = true;
						}

						if (draw)
						{
							insertionStyle.Draw(position, true, true, true, false);
						}

						//EditorGUI.DrawRect(position, new Color(0f, 1f, 0f, 0.5f));
						//EditorGUI.DrawRect(insertBarRect, new Color(1f, 0f, 0f, 0.5f));
					}
					break;
			}
		}

		public void OnGUI()
		{
			int behaviourCount = GetCount();
			Color savedBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = backgroundColor;
			GUIStyle style = behaviourCount > 0 ? backgroundStyle : GUIStyle.none;
			using (EditorGUILayout.VerticalScope listScope = new EditorGUILayout.VerticalScope(style))
			{
				GUI.backgroundColor = savedBackgroundColor;

				GUILayout.Space(0);

				for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					using (EditorGUILayout.VerticalScope behavioiurScope = new EditorGUILayout.VerticalScope())
					{
						Rect dropRect = behavioiurScope.rect;
						dropRect.height = 0f;

						DropBehaviour(dropRect, behaviourIndex, behaviourIndex == 0, false);

						TEditor behaviourEditor = GetBehaviourEditor(behaviourIndex);
						if (behaviourEditor != null)
						{
							behaviourEditor.backgroundColor = backgroundColor;
							BehaviourInfo behaviourInfo = BehaviourInfoUtility.GetBehaviourInfo(behaviourEditor.behaviourObj);
							GUIContent titleContent = behaviourInfo.titleContent;

							using (new ProfilerScope(titleContent.text))
							{
								behaviourEditor.OnGUI();
							}
						}
					}
				}

				if (behaviourCount > 0)
				{
					GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
				}

				Rect lastRect = listScope.rect;
				lastRect.yMin = lastRect.yMax;
				DropBehaviour(lastRect, behaviourCount, false, true);
			}
		}

		public bool IsVisibleDataLinkGUI()
		{
			int behaviourCount = GetCount();
			for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
			{
				TEditor behaviourEditor = GetBehaviourEditor(behaviourIndex);
				if (behaviourEditor != null && behaviourEditor.IsVisibleDataLinkGUI())
				{
					return true;
				}
			}

			return false;
		}

		public void OnDataLinkGUI(RectOffset outsideOffset)
		{
			int behaviourCount = GetCount();
			for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
			{
				TEditor behaviourEditor = GetBehaviourEditor(behaviourIndex);
				if (behaviourEditor != null)
				{
					behaviourEditor.DataLinkGUI(outsideOffset);
				}
			}
		}
	}
}