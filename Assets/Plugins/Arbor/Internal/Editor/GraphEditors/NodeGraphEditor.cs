﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Arbor;

namespace ArborEditor
{
	public class NodeGraphEditor : ScriptableObject
	{
		private static class Types
		{
			public static readonly System.Type ParameterContainerType;
			public static readonly System.Type GetParameterCalculatorType;

			static Types()
			{
				ParameterContainerType = AssemblyHelper.GetTypeByName("Arbor.ParameterContainer");
				GetParameterCalculatorType = AssemblyHelper.GetTypeByName("Arbor.ParameterBehaviours.GetParameterCalculator");
			}
		}

		public static bool HasEditor(NodeGraph nodeGraph)
		{
			if (nodeGraph == null)
			{
				return false;
			}

			System.Type classType = nodeGraph.GetType();
			System.Type editorType = CustomAttributes<CustomNodeGraphEditor>.FindEditorType(classType);

			return editorType != null && editorType.IsSubclassOf(typeof(NodeGraphEditor));
		}

		public static NodeGraphEditor CreateEditor(ArborEditorWindow window, NodeGraph nodeGraph)
		{
			if (nodeGraph == null)
			{
				return null;
			}

			System.Type classType = nodeGraph.GetType();
			System.Type editorType = CustomAttributes<CustomNodeGraphEditor>.FindEditorType(classType);

			if (editorType == null || !editorType.IsSubclassOf(typeof(NodeGraphEditor)))
			{
				editorType = typeof(NodeGraphEditor);
			}

			NodeGraphEditor nodeGraphEditor = CreateInstance(editorType) as NodeGraphEditor;
			nodeGraphEditor.hideFlags = HideFlags.HideAndDontSave;
			nodeGraphEditor.hostWindow = window;
			nodeGraphEditor.nodeGraph = nodeGraph;

			return nodeGraphEditor;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private ArborEditorWindow _HostWindow;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _NodeGraph;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private int _NodeGraphInstanceID = 0;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private List<int> _Selection = new List<int>();

		private List<NodeEditor> _NodeEditors = new List<NodeEditor>();
		private Dictionary<int, NodeEditor> _DicNodeEditors = new Dictionary<int, NodeEditor>();

		private ControlLayer _HighlightLayer = new ControlLayer();
		private Dictionary<int, HighlightControl> _HighlightControls = new Dictionary<int, HighlightControl>();

		private ControlLayer _PopupButtonLayer = new ControlLayer();
		private PopupButtonControl _PopupButtonControl = null;

		private ControlLayer _NodeCommentLayer = new ControlLayer();
		private Dictionary<Node, NodeCommentControl> _DicNodeCommentControls = new Dictionary<Node, NodeCommentControl>();

		private ControlLayer _GroupLayer = new ControlLayer();
		private Dictionary<GroupNode, GroupControl> _DicGroupControls = new Dictionary<GroupNode, GroupControl>();

		static Dictionary<Object, Rect> _BehaviourPosition = new Dictionary<Object, Rect>();

#if UNITY_2019_3_OR_NEWER
		[InitializeOnEnterPlayMode]
		static void OnEnterPlayMode()
		{
			_BehaviourPosition.Clear();
		}
#endif

		private Dictionary<int, Mesh> _DataBranchMeshes = new Dictionary<int, Mesh>();

		private RenameOverlay _RenameOverlay;
		private bool _RenameOverlayMouseDown;

		private static readonly int s_HandleGraphViewControlID = "s_HandleGraphViewControlID".GetHashCode();

		private Vector2 _DragBeginPos;
		private List<int> _OldSelection;

		private bool _IsDragSelection = false;

		private bool _DragDataBranchEnable = false;
		private int _DragDataBranchNodeID = 0;
		private Bezier2D _DragDataBranchBezier;

		private static readonly int s_DragNodesControlID = "DragNodes".GetHashCode();

		private Dictionary<Node, Rect> _DragNodePositions = new Dictionary<Node, Rect>();
		private Dictionary<Node, Rect> _SaveNodePositions = new Dictionary<Node, Rect>();
		private Vector2 _BeginMousePosition;
		private Vector2 _DragNodeDistance;
		private Rect _ResizeNodePosition;

		[System.Flags]
		public enum ResizeControlPointFlags
		{
			Top = 0x01,
			Bottom = 0x02,
			Left = 0x04,
			Right = 0x08,

			TopLeft = Top | Left,
			TopRight = Top | Right,
			BottomLeft = Bottom | Left,
			BottomRight = Bottom | Right,
		}
		private ResizeControlPointFlags _ResizeControlPoint;

		public delegate void DropNodesCallback(Node[] nodes);

		private sealed class DropNodesElement
		{
			public Rect position;
			public DropNodesCallback callback;
		}

		private List<DropNodesElement> _DropNodesElements = new List<DropNodesElement>();

		public void AddDropNodesListener(Rect position, DropNodesCallback callback)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			DropNodesElement element = new DropNodesElement();
			element.position = EditorGUITools.GUIToScreenRect(position);
			element.callback = callback;

			_DropNodesElements.Add(element);
		}


		public bool isDragNodes
		{
			get
			{
				return _DragNodePositions.Count > 0;
			}
		}

		public ArborEditorWindow hostWindow
		{
			get
			{
				return _HostWindow;
			}
			set
			{
				_HostWindow = value;
			}
		}

		public NodeGraph nodeGraph
		{
			get
			{
				return _NodeGraph;
			}
			set
			{
				if (_NodeGraph != value)
				{
					_NodeGraph = value;
					if (_NodeGraph != null)
					{
						_NodeGraphInstanceID = _NodeGraph.GetInstanceID();
					}
					else
					{
						_NodeGraphInstanceID = 0;
					}

					RaiseOnChangedGraph();
				}
			}
		}

		public int nodeEditorCount
		{
			get
			{
				return _NodeEditors.Count;
			}
		}

		public Node[] selection
		{
			get
			{
				List<Node> nodes = new List<Node>();

				int selectionCount = _Selection.Count;
				for (int selectionIndex = 0; selectionIndex < selectionCount; selectionIndex++)
				{
					int nodeID = _Selection[selectionIndex];
					Node node = nodeGraph.GetNodeFromID(nodeID);

					if (node != null)
					{
						nodes.Add(node);
					}
				}

				return nodes.ToArray();
			}
		}

		public NodeEditor[] selectionNodeEditors
		{
			get
			{
				List<NodeEditor> selectionNodeEditors = new List<NodeEditor>();

				int selectionCount = _Selection.Count;
				for (int selectionIndex = 0; selectionIndex < selectionCount; selectionIndex++)
				{
					int nodeID = _Selection[selectionIndex];
					Node node = nodeGraph.GetNodeFromID(nodeID);

					NodeEditor nodeEditor = GetNodeEditor(node);

					if (nodeEditor != null)
					{
						selectionNodeEditors.Add(nodeEditor);
					}
				}

				return selectionNodeEditors.ToArray();
			}
		}

		public bool editable
		{
			get
			{
				return (nodeGraph.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable;
			}
		}

		public event System.Action onChangedGraph;
		public event System.Action onChangedNodes;

		public bool ReatachIfNecessary()
		{
			if (_NodeGraph == null && _NodeGraphInstanceID != 0)
			{
#if ARBOR_DEBUG
				Debug.Log("Reatach");
#endif
				_NodeGraph = EditorUtility.InstanceIDToObject(_NodeGraphInstanceID) as NodeGraph;

				RaiseOnChangedGraph();

				return true;
			}
			return false;
		}

		public void Repaint()
		{
			_HostWindow.DoRepaint();
		}

		protected void CreateNodeEditor(Node node)
		{
			if (!NodeEditor.HasEditor(node))
			{
				return;
			}

			NodeEditor editor = NodeEditor.CreateEditors(this, node);

			if (editor == null)
			{
				return;
			}

			_NodeEditors.Add(editor);
			_DicNodeEditors.Add(node.nodeID, editor);

			_HostWindow.DirtyGraphExtents();

			RaiseOnChangedNodes();
		}

		public NodeEditor GetNodeEditor(int index)
		{
			return _NodeEditors[index];
		}

		public NodeEditor GetNodeEditorFromID(int nodeID)
		{
			NodeEditor result;
			if (_DicNodeEditors.TryGetValue(nodeID, out result))
			{
				return result;
			}

			for (int i = 0, count = _NodeEditors.Count; i < count; i++)
			{
				NodeEditor nodeEditor = _NodeEditors[i];
				if (nodeEditor.nodeID == nodeID)
				{
					_DicNodeEditors.Add(nodeID, nodeEditor);
					return nodeEditor;
				}
			}

			return null;
		}

		public NodeEditor GetNodeEditor(Node node)
		{
			return GetNodeEditorFromID(node.nodeID);
		}

		void DeleteNodeEditor(Node node)
		{
			NodeEditor nodeEditor = GetNodeEditor(node);

			if (nodeEditor != null)
			{
				_NodeEditors.Remove(nodeEditor);
				_DicNodeEditors.Remove(node.nodeID);

				Object.DestroyImmediate(nodeEditor);
			}

			RaiseOnChangedNodes();
		}

		void InitNodeEditor()
		{
			if (nodeGraph == null)
			{
				return;
			}

			int nodeCount = nodeGraph.nodeCount;
			for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
			{
				Node node = nodeGraph.GetNodeFromIndex(nodeIndex);

				CreateNodeEditor(node);
			}
		}

		void FinalizeNodeEditor()
		{
			for (int i = 0, count = _NodeEditors.Count; i < count; i++)
			{
				NodeEditor nodeEditor = _NodeEditors[i];
				if (nodeEditor != null)
				{
					Object.DestroyImmediate(nodeEditor);
				}
			}

			_NodeEditors.Clear();
			_DicNodeEditors.Clear();
		}

		bool RebuildNodeEditor()
		{
			bool rebuilded = false;
			bool removed = false;

			System.Action onChangeNodes = this.onChangedNodes;

			this.onChangedNodes = null;

			for (int count = _NodeEditors.Count, i = count - 1; i >= 0; i--)
			{
				NodeEditor nodeEditor = _NodeEditors[i];
				if (nodeEditor == null)
				{
					_NodeEditors.RemoveAt(i);

					rebuilded = true;
					removed = true;
				}
				else
				{
					Node node = nodeGraph != null ? nodeGraph.GetNodeFromID(nodeEditor.nodeID) : null;

					if (nodeEditor.IsValidNode(node))
					{
						nodeEditor.Validate(node);
					}
					else
					{
						_NodeEditors.RemoveAt(i);
						Object.DestroyImmediate(nodeEditor);

						rebuilded = true;
						removed = true;
					}
				}
			}

			if (removed)
			{
				_DicNodeEditors.Clear();
			}

			if (nodeGraph != null)
			{
				int nodeCount = nodeGraph.nodeCount;
				for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
				{
					Node node = nodeGraph.GetNodeFromIndex(nodeIndex);

					NodeEditor nodeEditor = GetNodeEditor(node);
					if (nodeEditor == null)
					{
						CreateNodeEditor(node);

						rebuilded = true;
					}
				}
			}

			this.onChangedNodes = onChangeNodes;

			if (rebuilded)
			{
				RaiseOnChangedNodes();
			}

			return rebuilded;
		}

		public void RebuildIfNecessary()
		{
			bool rebuilded = RebuildNodeEditor();

			RebuildNodeCommentLayer();
			RebuildGroupLayer();

			if (rebuilded)
			{
				EditorGUITools.ClearPropertyDrawerCache();
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnValidate()
		{
			_DicNodeEditors.Clear();
			_DicGroupControls.Clear();
			_DicNodeCommentControls.Clear();

			RebuildIfNecessary();
		}

		public Rect UpdateGraphExtents()
		{
			if (nodeGraph.nodeCount > 0)
			{
				Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 max = new Vector2(float.MinValue, float.MinValue);

				for (int i = 0, count = nodeGraph.nodeCount; i < count; i++)
				{
					Node node = nodeGraph.GetNodeFromIndex(i);
					min = Vector2.Min(min, node.position.min);
					max = Vector2.Max(max, node.position.max);
				}

				return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
			}
			else
			{
				return new Rect(0, 0, 1, 1);
			}
		}

		public bool OverlapsViewArea(Rect position)
		{
			return _HostWindow.OverlapsVewArea(position);
		}

		public virtual GUIContent GetGraphLabel()
		{
			return null;
		}

		public virtual bool HasPlayState()
		{
			return false;
		}

		public virtual PlayState GetPlayState()
		{
			return PlayState.Stopping;
		}

		void AutoLayout()
		{
			for (int i = 0, count = nodeEditorCount; i < count; i++)
			{
				GroupNodeEditor groupNodeEditor = GetNodeEditor(i) as GroupNodeEditor;
				if (groupNodeEditor != null)
				{
					groupNodeEditor.AutoLayout();
				}
			}
		}

		private bool _IsDraggingSlot = false;

		internal void BeginDisableContextClick()
		{
			_IsDraggingSlot = true;
		}

		internal void EndDisableContextClick()
		{
			_IsDraggingSlot = false;
		}

		bool IsDisableContextClick()
		{
			return IsDragBranch() || _IsDraggingSlot;
		}

		public void BeginGraphGUI(bool useOverlayLayer)
		{
			if (Event.current.type == EventType.Repaint)
			{
				_DropNodesElements.Clear();
			}

			if (IsDisableContextClick() && Event.current.type == EventType.MouseDown && (Event.current.button == 1 || Application.platform == RuntimePlatform.OSXEditor && Event.current.control))
			{
				Event.current.Use();
			}

			BeginRenameOverlayGUI();

			if (Event.current.type == EventType.Layout)
			{
				AutoLayout();
			}

			for (int i = 0, count = nodeEditorCount; i < count; i++)
			{
				NodeEditor nodeEditor = GetNodeEditor(i);
				if (nodeEditor != null)
				{
					nodeEditor.UpdateStyles();
				}
			}

			BeginLayer(useOverlayLayer);

			Node hoverNode = GetHoverNode(Event.current.mousePosition);
			isHoverNode = hoverNode != null;

			BeginDrawBranch();

			BeginDropParameter();
		}

		public virtual void OnDrawDragBranchies()
		{
		}

		[System.NonSerialized]
		private Parameter _DraggingParameter = null;

		public Parameter draggingParameter
		{
			get
			{
				return _DraggingParameter;
			}
		}

		public bool IsDragObject()
		{
			return _DraggingParameter != null;
		}

		public bool IsDragScroll()
		{
			bool draggingNodes = _DragNodePositions.Count > 0;
			bool isDragBranchScroll = IsDragBranchScroll();

			return draggingNodes || isDragBranchScroll || _IsDragSelection;
		}

		protected virtual void OnCreateSetParameter(Vector2 position, Parameter parameter)
		{
		}

		void OnCreateGetParameter(Vector2 position, Parameter parameter)
		{
			Undo.IncrementCurrentGroup();

			CalculatorNode calculatorNode = CreateCalculatorInternal(position, Types.GetParameterCalculatorType);
			Arbor.ParameterBehaviours.GetParameterCalculatorInternal getParameterCalculator = calculatorNode.calculator as Arbor.ParameterBehaviours.GetParameterCalculatorInternal;

			Undo.RecordObject(getParameterCalculator, "Created Calculator");

			getParameterCalculator.SetParameter(parameter);

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			EditorUtility.SetDirty(getParameterCalculator);

			CalculatorEditor calculatorEditor = GetNodeEditor(calculatorNode) as CalculatorEditor;
			if (calculatorEditor != null)
			{
				calculatorEditor.SetupResizable();
			}
		}

		void BeginDropParameter()
		{
			Event current = Event.current;
			switch (current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					{
						Parameter nextDraggingParameter = null;
						foreach (Object draggedObject in DragAndDrop.objectReferences)
						{
							ParameterDraggingObject draggingObject = draggedObject as ParameterDraggingObject;
							if (draggingObject != null)
							{
								nextDraggingParameter = draggingObject.parameter;

								break;
							}
						}

						if (_DraggingParameter != nextDraggingParameter)
						{
							_DraggingParameter = nextDraggingParameter;
							_InVisibleNodes.Clear();
							Repaint();
						}
					}
					break;
				case EventType.DragExited:
					{
						if (_DraggingParameter != null)
						{
							_DraggingParameter = null;
							_InVisibleNodes.Clear();
							Repaint();
						}
					}
					break;
			}
		}

		void EndDropParameter()
		{
			bool editable = this.editable;

			Event current = Event.current;
			switch (current.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (_DraggingParameter != null)
					{
						if (editable)
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Link;

							if (current.type == EventType.DragPerform)
							{
								MousePosition mousePosition = new MousePosition(current.mousePosition);

								GenericMenu menu = new GenericMenu();

								menu.AddItem(EditorContents.get, false, () =>
								{
									OnCreateGetParameter(mousePosition.guiPoint, _DraggingParameter);
								});

								menu.AddItem(EditorContents.set, false, () =>
								{
									OnCreateSetParameter(mousePosition.guiPoint, _DraggingParameter);
								});

								menu.ShowAsContext();

								DragAndDrop.AcceptDrag();
								DragAndDrop.activeControlID = 0;
							}
						}
						else
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}

						current.Use();
					}
					break;
			}
		}

		public void EndGraphGUI(bool useOverlayLayer)
		{
			EndDropParameter();

			EndDrawBranch();

			EndLayer(useOverlayLayer);

			OnDrawDragBranchies();

			DrawDragDataBranch();

			EndRenameOverlayGUI();

			HandleGraphView();

			if (Event.current.type == EventType.Repaint)
			{
				ClearVisibleDataSlot();
			}
		}

		public void ShowPopupButtonControl(Rect position, GUIContent content, int activeControlID, GUIStyle style, System.Action<Rect> onClick)
		{
			if (_PopupButtonControl == null)
			{
				_PopupButtonControl = ScriptableObject.CreateInstance<PopupButtonControl>();
				_PopupButtonControl.hideFlags = HideFlags.HideAndDontSave;
				_PopupButtonControl.graphEditor = this;

				_PopupButtonLayer.Add(_PopupButtonControl);
			}

			_PopupButtonControl.position = position;
			_PopupButtonControl.content = content;
			_PopupButtonControl.style = style;
			_PopupButtonControl.activeControlID = activeControlID;
			_PopupButtonControl.onClick = onClick;

			Repaint();
		}

		internal void ClosePopupButtonControl(PopupButtonControl popupControl)
		{
			if (_PopupButtonControl != popupControl)
			{
				return;
			}

			_PopupButtonLayer.Remove(_PopupButtonControl);
			_PopupButtonControl = null;

			Repaint();
		}

		public void CloseAllPopupButtonControl()
		{
			if (_PopupButtonLayer.controlCount == 0)
			{
				return;
			}

			_PopupButtonLayer.Clear();
			_PopupButtonControl = null;

			Repaint();
		}

		public int GetPopupButtonActiveControlID()
		{
			return _PopupButtonControl != null ? _PopupButtonControl.activeControlID : 0;
		}

		void FinalizePopupLayer()
		{
			_PopupButtonLayer.Clear();
			_PopupButtonControl = null;
		}

		public void ShowHightlightControl(Rect position, int controlID, GUIStyle style)
		{
			HighlightControl highlightControl = null;
			if (!_HighlightControls.TryGetValue(controlID, out highlightControl))
			{
				highlightControl = ScriptableObject.CreateInstance<HighlightControl>();
				highlightControl.hideFlags = HideFlags.HideAndDontSave;
				highlightControl.graphEditor = this;

				_HighlightControls.Add(controlID, highlightControl);

				_HighlightLayer.Add(highlightControl);
			}

			if (position != highlightControl.position || highlightControl.style != style)
			{
				highlightControl.position = position;
				highlightControl.style = style;

				Repaint();
			}
		}

		public void CloseHighlightControl(int controlID)
		{
			HighlightControl highlightControl = null;
			if (_HighlightControls.TryGetValue(controlID, out highlightControl))
			{
				_HighlightLayer.Remove(highlightControl);
				_HighlightControls.Remove(controlID);
				highlightControl = null;

				Repaint();
			}
		}

		void FinalizeHighlightLayer()
		{
			_HighlightLayer.Clear();
			_HighlightControls.Clear();
		}

		public void UpdateOverlayLayer()
		{
			_HighlightLayer.Update();
			_PopupButtonLayer.Update();
			if (!ArborEditorWindow.nodeCommentAffectsZoom)
			{
				_NodeCommentLayer.Update();
			}
		}

		public void BeginOverlayLayer()
		{
			_HighlightLayer.BeginLayer();
			_PopupButtonLayer.BeginLayer();
			if (!ArborEditorWindow.nodeCommentAffectsZoom)
			{
				_NodeCommentLayer.BeginLayer();
			}
		}

		public void EndOverlayLayer()
		{
			if (!ArborEditorWindow.nodeCommentAffectsZoom)
			{
				_NodeCommentLayer.EndLayer();
			}
			_PopupButtonLayer.EndLayer();
			_HighlightLayer.EndLayer();
		}

		public bool ContainsOverlayLayer(Vector2 position)
		{
			return _NodeCommentLayer.Contains(position) || _PopupButtonLayer.Contains(position) || _HighlightLayer.Contains(position);
		}

		static bool IsShowNodeComment(Node node)
		{
			switch (ArborSettings.nodeCommentViewMode)
			{
				case NodeCommentViewMode.Normal:
					return NodeEditorUtility.GetShowComment(node);
				case NodeCommentViewMode.ShowAll:
					return true;
				case NodeCommentViewMode.ShowCommentedOnly:
					return !string.IsNullOrEmpty(node.nodeComment);
				case NodeCommentViewMode.HideAll:
					return false;
			}

			return false;
		}

		public void UpdateNodeCommentControl(Node node)
		{
			if (IsShowNodeComment(node))
			{
				CreateNodeCommentControl(node);
			}
			else
			{
				DeleteNodeCommentControl(node);
			}
		}

		void CreateNodeCommentControl(Node node)
		{
			if (GetNodeCommentControl(node) != null)
			{
				return;
			}

			NodeCommentControl nodeCommentControl = ScriptableObject.CreateInstance<NodeCommentControl>();
			nodeCommentControl.hideFlags = HideFlags.HideAndDontSave;
			nodeCommentControl.node = node;
			nodeCommentControl.graphEditor = this;
			nodeCommentControl.focusType = FocusType.Keyboard;

			_NodeCommentLayer.Add(nodeCommentControl);
			_DicNodeCommentControls.Add(node, nodeCommentControl);
		}

		NodeCommentControl GetNodeCommentControl(Node node)
		{
			NodeCommentControl result;
			if (_DicNodeCommentControls.TryGetValue(node, out result))
			{
				return result;
			}

			for (int i = 0, count = _NodeCommentLayer.controlCount; i < count; i++)
			{
				NodeCommentControl nodeCommentControl = _NodeCommentLayer.GetControlFromIndex(i) as NodeCommentControl;
				if (nodeCommentControl.node == node)
				{
					_DicNodeCommentControls.Add(node, nodeCommentControl);
					return nodeCommentControl;
				}
			}
			return null;
		}

		void DeleteNodeCommentControl(Node node)
		{
			NodeCommentControl nodeCommentControl = GetNodeCommentControl(node);
			if (nodeCommentControl != null)
			{
				_NodeCommentLayer.Remove(nodeCommentControl);
				_DicNodeCommentControls.Remove(node);
			}
		}

		void InitNodeCommentLayer()
		{
			if (nodeGraph == null)
			{
				return;
			}

			int nodeCount = nodeGraph.nodeCount;
			for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
			{
				Node node = nodeGraph.GetNodeFromIndex(nodeIndex);

				UpdateNodeCommentControl(node);
			}
		}

		void RebuildNodeCommentLayer()
		{
			bool removed = false;

			for (int count = _NodeCommentLayer.controlCount, i = count - 1; i >= 0; i--)
			{
				NodeCommentControl nodeCommentControl = _NodeCommentLayer.GetControlFromIndex(i) as NodeCommentControl;
				Node node = nodeGraph != null ? nodeGraph.GetNodeFromID(nodeCommentControl.nodeID) : null;

				if (node == null || !IsShowNodeComment(node))
				{
					_NodeCommentLayer.Remove(nodeCommentControl);
					removed = true;
				}
				else
				{
					nodeCommentControl.node = node;
				}
			}

			if (removed)
			{
				_DicNodeCommentControls.Clear();
			}

			if (nodeGraph != null)
			{
				int nodeCount = nodeGraph.nodeCount;
				for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
				{
					Node node = nodeGraph.GetNodeFromIndex(nodeIndex);

					NodeCommentControl nodeCommentControl = GetNodeCommentControl(node);
					if (nodeCommentControl == null)
					{
						UpdateNodeCommentControl(node);
					}
				}
			}
		}

		void FinalizeNodeCommentLayer()
		{
			_NodeCommentLayer.Clear();
			_DicNodeCommentControls.Clear();
		}

		internal void AddGroupControl(GroupControl groupControl)
		{
			_GroupLayer.Add(groupControl);
			_DicGroupControls.Add(groupControl.groupNode, groupControl);
		}

		GroupControl GetGroupControl(GroupNode groupNode)
		{
			GroupControl result;
			if (_DicGroupControls.TryGetValue(groupNode, out result))
			{
				return result;
			}

			for (int i = 0, count = _GroupLayer.controlCount; i < count; i++)
			{
				GroupControl groupControl = _GroupLayer.GetControlFromIndex(i) as GroupControl;
				if (groupControl.groupNode == groupNode)
				{
					_DicGroupControls.Add(groupNode, groupControl);
					return groupControl;
				}
			}

			return null;
		}

		internal void DeleteGroupControl(GroupControl groupControl)
		{
			if (groupControl != null)
			{
				GroupNode groupNode = groupControl.groupNode;

				_GroupLayer.Remove(groupControl);
				_DicGroupControls.Remove(groupNode);
			}
		}

		void InitGroupLayer()
		{
			if (nodeGraph == null)
			{
				return;
			}

			_GroupLayer.order = -1000;
		}

		void RebuildGroupLayer()
		{
			bool removed = false;

			for (int count = _GroupLayer.controlCount, i = count - 1; i >= 0; i--)
			{
				GroupControl groupControl = _GroupLayer.GetControlFromIndex(i) as GroupControl;
				GroupNode groupNode = nodeGraph != null ? nodeGraph.GetGroupFromID(groupControl.groupNodeID) : null;

				if (groupNode == null)
				{
					_GroupLayer.Remove(groupControl);

					removed = true;
				}
			}

			if (removed)
			{
				_DicGroupControls.Clear();
			}
		}

		void FinalizeGroupLayer()
		{
			_GroupLayer.Clear();
			_DicGroupControls.Clear();
		}

		public void UpdateLayer(bool useOverlayLayer)
		{
			_GroupLayer.Update();
			if (useOverlayLayer)
			{
				_NodeCommentLayer.Update();
				_PopupButtonLayer.Update();
				_HighlightLayer.Update();
			}
			else if (ArborEditorWindow.nodeCommentAffectsZoom)
			{
				_NodeCommentLayer.Update();
			}
		}

		void BeginLayer(bool useOverlayLayer)
		{
			_GroupLayer.BeginLayer();
			if (useOverlayLayer)
			{
				BeginOverlayLayer();
			}
			else if (ArborEditorWindow.nodeCommentAffectsZoom)
			{
				_NodeCommentLayer.BeginLayer();
			}
		}

		void EndLayer(bool useOverlayLayer)
		{
			if (useOverlayLayer)
			{
				EndOverlayLayer();
			}
			else
			{
				if (ArborEditorWindow.nodeCommentAffectsZoom)
				{
					_NodeCommentLayer.EndLayer();
				}
			}
			_GroupLayer.EndLayer();
		}

		public void SetBehaviourPosition(Object obj, Rect position)
		{
			_BehaviourPosition[obj] = position;
		}

		private static readonly int s_DrawDataBranchHash = "s_DrawDataBranchHash".GetHashCode();
		private DataBranch _HoverDataBranch = null;
		private DataBranch _NextHoverDataBranch = null;
		protected float _NextHoverBranchDistance = float.MaxValue;

		private bool _IsDrawBranchMouseMove = false;

		void BeginDrawBranch()
		{
			_IsDrawBranchMouseMove = Event.current.type == EventType.MouseMove;
			_NextHoverDataBranch = null;

			OnBeginDrawBranch();

			DrawDataBranchies();

			OnDrawBranchies();
		}

		protected bool HasHoverBranch()
		{
			return _NextHoverDataBranch != null || OnHasHoverBranch();
		}

		protected void ClearHoverBranch()
		{
			_NextHoverDataBranch = null;
			OnClearHoverBranch();
		}

		void EndDrawBranch()
		{
			OnDrawHoverBranch();

			if (_HoverDataBranch != null)
			{
				DrawDataBranch(_HoverDataBranch);
			}

			if (_IsDrawBranchMouseMove)
			{
				bool changed = false;
				if (_HoverDataBranch != _NextHoverDataBranch)
				{
					_HoverDataBranch = _NextHoverDataBranch;
					changed = true;
				}
				if (OnEndDrawBranch())
				{
					changed = true;
				}

				if (changed)
				{
					Repaint();
				}
			}
		}

		protected virtual void OnBeginDrawBranch()
		{
		}

		protected virtual bool OnHasHoverBranch()
		{
			return false;
		}

		protected virtual void OnClearHoverBranch()
		{
		}

		protected virtual bool OnEndDrawBranch()
		{
			return false;
		}

		protected bool IsHoverBezier(Vector2 mousePosition, Bezier2D bezier, bool arrow, float arrowWidth, ref float distance)
		{
			if (!isHoverNode)
			{
				bool isHover = false;
				Vector2 v = Vector2.zero;
				if (arrow)
				{
					v = (bezier.endPosition - bezier.endControl).normalized * arrowWidth;
				}
				float d = HandleUtility.DistancePointBezier(mousePosition, bezier.startPosition, bezier.endPosition - v, bezier.startControl, bezier.endControl - v);
				if (d <= 15.0f)
				{
					isHover = (!HasHoverBranch() || _NextHoverBranchDistance > d);
				}
				if (arrow)
				{
					float arrowDistance = HandleUtility.DistancePointLine(mousePosition, bezier.endPosition - v, bezier.endPosition);
					if (arrowDistance <= 15.0f)
					{
						if ((!HasHoverBranch() || _NextHoverBranchDistance > arrowDistance))
						{
							isHover = true;
							if (d > arrowDistance)
							{
								d = arrowDistance;
							}
						}
					}
				}

				if (isHover)
				{
					distance = d;
					return true;
				}
			}

			return false;
		}

		protected bool IsHoverBezier(Vector2 mousePosition, Bezier2D bezier, ref float distance)
		{
			return IsHoverBezier(mousePosition, bezier, false, 0, ref distance);
		}

		private sealed class EnumInfo
		{
			public readonly int[] values;
			public readonly string[] names;

			public EnumInfo(System.Type enumType)
			{
				if (!enumType.IsEnum)
				{
					throw new System.ArgumentException("The type `" + enumType.Name + "' must be convertible to `enum' in order to use it as parameter `enumType'", "enumType");
				}

				List<System.Reflection.FieldInfo> enumFields = EnumUtility.GetFields(enumType);

				values = enumFields.Select(f => (int)System.Enum.Parse(enumType, f.Name)).ToArray();
				names = enumFields.Select(f => f.Name).ToArray();
			}
		}

		private static Dictionary<System.Type, EnumInfo> s_EnumInfos = new Dictionary<System.Type, EnumInfo>();
		private static System.Text.StringBuilder s_DataValueStringBuilder = new System.Text.StringBuilder();

		static string ToDataValueString(object value)
		{
			if (value == null)
			{
				return "null";
			}

			System.Type valueType = value.GetType();
			if (EnumFieldUtility.IsEnumFlags(valueType))
			{
				EnumInfo enumInfo = null;
				if (!s_EnumInfos.TryGetValue(valueType, out enumInfo))
				{
					enumInfo = new EnumInfo(valueType);
					s_EnumInfos.Add(valueType, enumInfo);
				}

				s_DataValueStringBuilder.Length = 0;
				int currentValue = (int)value;
				for (int i = 0, count = enumInfo.values.Length; i < count; i++)
				{
					int intValue = enumInfo.values[i];
					if ((currentValue & intValue) == intValue)
					{
						if (s_DataValueStringBuilder.Length > 0)
						{
							s_DataValueStringBuilder.Append(", ");
						}
						s_DataValueStringBuilder.Append(enumInfo.names[i]);
					}
				}

				return s_DataValueStringBuilder.Length == 0 ? "<Nothing>" : s_DataValueStringBuilder.ToString();
			}

			IList list = value as IList;
			if (list != null)
			{
				s_DataValueStringBuilder.Length = 0;
				s_DataValueStringBuilder.AppendFormat("{0} : Length {1}", TypeUtility.GetSlotTypeName(valueType), list.Count);

				return s_DataValueStringBuilder.ToString();
			}

			return value.ToString();
		}

		static string ToDataValueDetailsString(object value)
		{
			if (value == null)
			{
				return null;
			}

			IList list = value as IList;
			if (list == null)
			{
				return null;
			}

			s_DataValueStringBuilder.Length = 0;
			for (int i = 0; i < list.Count; i++)
			{
				object element = list[i];
				if (i != 0)
				{
					s_DataValueStringBuilder.AppendLine();
				}
				s_DataValueStringBuilder.AppendFormat("\t[{0}] {1}", i, (element == null) ? "null" : element.ToString());
			}

			return s_DataValueStringBuilder.ToString();
		}

		static string GetDataValueLogString(string valueString, string detailsString)
		{
			s_DataValueStringBuilder.Length = 0;

			s_DataValueStringBuilder.Append(valueString);
			if (!string.IsNullOrEmpty(detailsString))
			{
				s_DataValueStringBuilder.AppendLine();
				s_DataValueStringBuilder.Append(detailsString);
			}

			return s_DataValueStringBuilder.ToString();
		}

		void DrawDataBranch(DataBranch branch)
		{
			using (new ProfilerScope("DrawDataBranch"))
			{
				bool editable = this.editable;

				Event currentEvent = Event.current;

				int controlID = EditorGUIUtility.GetControlID(s_DrawDataBranchHash, FocusType.Passive);

				EventType eventType = currentEvent.GetTypeForControl(controlID);

				bool isHover = _HoverDataBranch == branch;

				switch (eventType)
				{
					case EventType.MouseDown:
						if (currentEvent.button == 1 || Application.platform == RuntimePlatform.OSXEditor && currentEvent.control)
						{
							if (isHover)
							{
								GenericMenu menu = new GenericMenu();

								NodeGraph nodeGraph = this.nodeGraph;

								if (editable)
								{
									menu.AddItem(EditorContents.showDataValue, branch.showDataValue, () =>
										{
											Undo.RecordObject(nodeGraph, "Change ShowDataValue");
											branch.showDataValue = !branch.showDataValue;
											EditorUtility.SetDirty(nodeGraph);
										});
								}
								else
								{
									menu.AddDisabledItem(EditorContents.showDataValue);
								}

								menu.AddSeparator("");

								if (editable)
								{
									Vector2 mousePosition = currentEvent.mousePosition;

									menu.AddItem(EditorContents.reroute, false, () =>
										{
											int inNodeID = branch.inNodeID;
											Object inBehaviour = branch.inBehaviour;
											DataSlot inputSlot = branch.inputSlot;
											int outNodeID = branch.outNodeID;
											Object outBehaviour = branch.outBehaviour;
											DataSlot outputSlot = branch.outputSlot;
											Bezier2D lineBezier = branch.lineBezier;
											System.Type outputType = branch.outputType;

											Undo.IncrementCurrentGroup();
											int undoGroup = Undo.GetCurrentGroup();

											_NodeGraph.DeleteDataBranch(branch);

											DataBranchRerouteNode newRerouteNode = nodeGraph.CreateDataBranchRerouteNode(EditorGUITools.SnapToGrid(mousePosition - new Vector2(16f, 16f)), outputType);

											Undo.RecordObject(_NodeGraph, "Reroute");

											RerouteSlot rerouteSlot = newRerouteNode.link;

											float t = lineBezier.GetClosestParam(mousePosition);
											newRerouteNode.direction = lineBezier.GetTangent(t);

											DataBranch outBranch = _NodeGraph.ConnectDataBranch(newRerouteNode.nodeID, null, rerouteSlot, outNodeID, outBehaviour, outputSlot);
											if (outBranch != null)
											{
												outBranch.enabled = true;
											}

											DataBranch inBranch = _NodeGraph.ConnectDataBranch(inNodeID, inBehaviour, inputSlot, newRerouteNode.nodeID, null, rerouteSlot);
											if (inBranch != null)
											{
												inBranch.enabled = true;
											}

											Undo.CollapseUndoOperations(undoGroup);

											EditorUtility.SetDirty(_NodeGraph);

											VisibleNode(inNodeID);
											VisibleNode(outNodeID);
										});

									menu.AddItem(EditorContents.disconnect, false, () =>
										{
											nodeGraph.DeleteDataBranch(branch);
										});
								}
								else
								{
									menu.AddDisabledItem(EditorContents.reroute);
									menu.AddDisabledItem(EditorContents.disconnect);
								}

								menu.ShowAsContext();
								currentEvent.Use();
							}
						}
						break;
					case EventType.MouseMove:
						{
							float distance = 0f;
							if (IsHoverBezier(currentEvent.mousePosition, branch.lineBezier, ref distance))
							{
								ClearHoverBranch();
								_NextHoverDataBranch = branch;
								_NextHoverBranchDistance = distance;
							}
						}
						break;
					case EventType.Repaint:
						{
							DataSlot inputSlot = branch.inputSlot;
							DataSlot outputSlot = branch.outputSlot;

							if (inputSlot != null && outputSlot != null)
							{
								bool changed = false;

								DataSlotField inputSlotField = branch.inputSlotField;
								DataSlotField outputSlotField = branch.outputSlotField;

								if (!inputSlotField.isVisible)
								{
									Rect inRect = new Rect();
									if (_BehaviourPosition.TryGetValue(branch.inBehaviour, out inRect))
									{
										Vector2 endPosition = new Vector2(inRect.x, inRect.center.y);
										branch.lineBezier.endPosition = endPosition;
										branch.lineBezier.endControl = endPosition - EditorGUITools.kBezierTangentOffset;
									}
									else
									{
										DataBranchRerouteNode rerouteNode = nodeGraph.dataBranchRerouteNodes.GetFromID(branch.inNodeID);
										if (rerouteNode != null)
										{
											Vector2 endPosition = rerouteNode.position.center;
											branch.lineBezier.endPosition = endPosition;
											branch.lineBezier.endControl = endPosition - EditorGUITools.kBezierTangentOffset;
										}
									}
								}

								if (!outputSlotField.isVisible)
								{
									Rect outRect = new Rect();
									if (_BehaviourPosition.TryGetValue(branch.outBehaviour, out outRect))
									{
										Vector2 startPosition = new Vector2(outRect.xMax, outRect.center.y);
										branch.lineBezier.startPosition = startPosition;
										branch.lineBezier.startControl = startPosition + EditorGUITools.kBezierTangentOffset;
									}
									else
									{
										DataBranchRerouteNode rerouteNode = nodeGraph.dataBranchRerouteNodes.GetFromID(branch.outNodeID);
										if (rerouteNode != null)
										{
											Vector2 startPosition = rerouteNode.position.center;
											branch.lineBezier.startPosition = startPosition;
											branch.lineBezier.startControl = startPosition + EditorGUITools.kBezierTangentOffset;
										}
									}
								}

								Mesh mesh = null;
								if (!_DataBranchMeshes.TryGetValue(branch.branchID, out mesh))
								{
									mesh = new Mesh();
									mesh.name = "DataBranch";
									mesh.hideFlags |= HideFlags.HideAndDontSave;
									mesh.MarkDynamic();

									_DataBranchMeshes.Add(branch.branchID, mesh);

									changed = true;
								}

								Color outputSlotColor = EditorGUITools.GetTypeColor(outputSlotField.connectableType);
								Color inputSlotColor = EditorGUITools.GetTypeColor(inputSlotField.connectableType);

								float alpha = 1.0f;
								if (!branch.enabled)
								{
									alpha = 0.1f;
								}

								outputSlotColor.a = alpha;
								inputSlotColor.a = alpha;

								if (Application.isPlaying && !branch.isUsed)
								{
									outputSlotColor *= Color.gray;
									inputSlotColor *= Color.gray;
								}

								outputSlotColor = EditorGUITools.GetColorOnGUI(outputSlotColor);
								inputSlotColor = EditorGUITools.GetColorOnGUI(inputSlotColor);

								Color shadowColor = new Color(0, 0, 0, alpha);

								if (changed || branch.lineBezier.isChanged || branch.outputSlotColor != outputSlotColor || branch.inputSlotColor != inputSlotColor)
								{
									branch.outputSlotColor = outputSlotColor;
									branch.inputSlotColor = inputSlotColor;
									Vector2 shadowPos = Vector2.one * 3;
									EditorGUITools.GenerateBezierDottedQuadMesh(branch.lineBezier, branch.outputSlotColor, branch.inputSlotColor, 16.0f, 10.0f, shadowPos, shadowColor, mesh);
								}

								if (isHover)
								{
									EditorGUITools.DrawBezier(branch.lineBezier, Vector2.zero, Styles.selectedConnectionTexture, Color.cyan, 11.0f);
								}

								EditorGUITools.DrawMesh(mesh, Styles.dataConnectionTexture);
							}
						}
						break;
				}

				if (Application.isPlaying && branch.isUsed)
				{
					bool isVisible = ArborSettings.showDataValue || branch.showDataValue || isHover;
					if (isVisible)
					{
						Bezier2D bezier = branch.lineBezier;
						Vector2 pos = bezier.GetPoint(0.5f);

						object value = branch.currentValue;

						GUIStyle style = Styles.countBadgeLarge;
						GUIContent content = GUIContent.none;
						Vector2 size = Vector2.zero;
						string valueString = ToDataValueString(value);

						bool isColor = value is Color;
						if (isColor)
						{
							size = style.CalcScreenSize(new Vector2(16, 16));
						}
						else
						{
							content = new GUIContent(valueString);
							size = style.CalcSize(content);
						}

						Rect rect = new Rect(pos.x - size.x / 2, pos.y - size.y / 2, size.x, size.y);

						if (EditorGUITools.ButtonMouseDown(rect, content, FocusType.Passive, style))
						{
							if (value is Object)
							{
								EditorGUIUtility.PingObject(value as Object);
							}
							else
							{
								// Output current value to console
								string detailsString = ToDataValueDetailsString(value);
								Debug.Log(GetDataValueLogString(valueString, detailsString));
							}
						}

						if (isColor && Event.current.type == EventType.Repaint)
						{
							Rect contentRect = style.padding.Remove(rect);
							EditorGUIUtility.DrawColorSwatch(contentRect, (Color)value);
							Styles.colorPickerBox.Draw(contentRect, GUIContent.none, false, false, false, false);
						}
					}
				}
			}
		}

		public void DrawDataBranchies()
		{
			using (new ProfilerScope("DrawDataBranchies"))
			{
				for (int count = nodeGraph.dataBranchCount, i = count - 1; i >= 0; i--)
				{
					DataBranch branch = nodeGraph.GetDataBranchFromIndex(i);
					if (branch != _HoverDataBranch)
					{
						DrawDataBranch(branch);
					}
				}
			}
		}

		void FinalizeDataBranch()
		{
			foreach (KeyValuePair<int, Mesh> pair in _DataBranchMeshes)
			{
				Object.DestroyImmediate(pair.Value);
			}
			_DataBranchMeshes.Clear();
		}

		void FinalizeParameterContainerEditor()
		{
			if (_ParameterContainerEditor != null)
			{
				Object.DestroyImmediate(_ParameterContainerEditor);
				_ParameterContainerEditor = null;
			}
		}

		public void InitializeGraph()
		{
			EndRename();

			FinalizeHighlightLayer();
			FinalizePopupLayer();
			FinalizeGroupLayer();
			FinalizeNodeCommentLayer();
			FinalizeNodeEditor();
			FinalizeParameterContainerEditor();

			EditorGUITools.ClearPropertyDrawerCache();

			InitNodeEditor();
			InitNodeCommentLayer();
			InitGroupLayer();

			InitializeVisibleNodes();
		}

		public void OnUndoRedoPerformed()
		{
			RebuildIfNecessary();

			InitializeVisibleNodes();
		}

		public void FinalizeGraph()
		{
			FinalizeDataBranch();
			FinalizeNodeEditor();

			FinalizeHighlightLayer();
			FinalizePopupLayer();
			FinalizeGroupLayer();
			FinalizeNodeCommentLayer();

			FinalizeParameterContainerEditor();
		}

		public void RaiseOnChangedGraph()
		{
			if (onChangedGraph != null)
			{
				onChangedGraph();
			}
		}

		public void RaiseOnChangedNodes()
		{
			if (onChangedNodes != null)
			{
				onChangedNodes();
			}
		}

		public Rect GetHeaderContentRect(Node node, Rect nodePosition)
		{
			NodeEditor nodeEditor = GetNodeEditor(node);
			if (nodeEditor != null)
			{
				return nodeEditor.GetNameRect(nodePosition.position);
			}

			return nodePosition;
		}

		private RenameOverlay GetRenameOverlay()
		{
			if (_RenameOverlay == null)
			{
				_RenameOverlay = new RenameOverlay();
			}
			return _RenameOverlay;
		}

		public void BeginRename(int nodeID, string name)
		{
			hostWindow.Focus();

			RenameOverlay renameOverlay = GetRenameOverlay();
			renameOverlay.BeginRename(name, nodeID, 0.0f);
		}

		private void RenameEnded()
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.userAcceptedRename)
			{
				string name = !string.IsNullOrEmpty(renameOverlay.name) ? renameOverlay.name : renameOverlay.originalName;
				int nodeID = renameOverlay.userData;
				Node node = nodeGraph.GetNodeFromID(nodeID);
				if (node != null)
				{
					NodeEditor nodeEditor = GetNodeEditor(node);
					if (nodeEditor != null)
					{
						nodeEditor.OnRename(name);

						RaiseOnChangedNodes();
					}
				}
			}
			renameOverlay.Clear();
		}

		void BeginRenameOverlayGUI()
		{
			_RenameOverlayMouseDown = false;
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.IsRenaming())
			{
				int nodeID = renameOverlay.userData;
				Node node = nodeGraph.GetNodeFromID(nodeID);
				if (node != null)
				{
					renameOverlay.editFieldRect = GetHeaderContentRect(node, node.position);
				}

				Event current = Event.current;
				if (current.type == EventType.MouseDown && renameOverlay.editFieldRect.Contains(current.mousePosition))
				{
					_RenameOverlayMouseDown = true;
					current.Use();
				}
			}
		}

		void EndRenameOverlayGUI()
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.IsRenaming())
			{
				if (_RenameOverlayMouseDown)
				{
					Event current = Event.current;
					current.type = EventType.MouseDown;
				}

				if (renameOverlay.OnGUI(Styles.renameTextField))
				{
					return;
				}
				RenameEnded();
				GUIUtility.ExitGUI();
			}
		}

		void EndRename()
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.IsRenaming())
			{
				renameOverlay.EndRename(false);
			}
		}

		public void OnRenameEvent()
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			renameOverlay.OnEvent();
		}

		public bool IsRenaming()
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.IsRenaming())
				return !renameOverlay.isWaitingForDelay;
			return false;
		}

		public bool IsRenaming(int instanceID)
		{
			RenameOverlay renameOverlay = GetRenameOverlay();
			if (renameOverlay.IsRenaming() && renameOverlay.userData == instanceID)
				return !renameOverlay.isWaitingForDelay;
			return false;
		}

		public void BeginDragDataBranch(int nodeID)
		{
			_DragDataBranchEnable = true;
			_DragDataBranchNodeID = nodeID;
		}

		public void EndDragDataBranch()
		{
			_DragDataBranchEnable = false;
			_DragDataBranchNodeID = 0;

			hostWindow.Repaint();
		}

		public void DragDataBranchBezier(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent)
		{
			if (_DragDataBranchBezier == null)
			{
				_DragDataBranchBezier = new Bezier2D(start, startTangent, end, endTangent);
			}
			else
			{
				_DragDataBranchBezier.startPosition = start;
				_DragDataBranchBezier.startControl = startTangent;
				_DragDataBranchBezier.endPosition = end;
				_DragDataBranchBezier.endControl = endTangent;
			}
		}

		public static readonly Color dragBezierColor = new Color(1.0f, 0.8f, 0.8f, 1.0f);
		public static readonly Color bezierShadowColor = new Color(0, 0, 0, 1.0f);

		void DrawDragDataBranch()
		{
			if (_DragDataBranchEnable)
			{
				Vector2 shadowPos = Vector2.one * 3;

				EditorGUITools.BezierQuad(_DragDataBranchBezier, shadowPos, bezierShadowColor, 16.0f, 10.0f, Styles.dataConnectionTexture);
				EditorGUITools.BezierQuad(_DragDataBranchBezier, Vector2.zero, dragBezierColor, 16.0f, 10.0f, Styles.dataConnectionTexture);
			}
		}

		public virtual bool IsDraggingBranch(Node node)
		{
			return _DragDataBranchEnable && _DragDataBranchNodeID == node.nodeID;
		}

		public virtual bool IsDragBranch()
		{
			return _DragDataBranchEnable;
		}

		public virtual bool IsDragBranchScroll()
		{
			return IsDragBranch();
		}

		public void SelectAll()
		{
			Undo.IncrementCurrentGroup();

			Undo.RecordObject(this, "Selection State");

			_Selection.Clear();

			for (int i = 0, count = nodeGraph.nodeCount; i < count; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);
				_Selection.Add(node.nodeID);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			EditorUtility.SetDirty(this);
		}

		public bool HasSelection()
		{
			return _Selection != null && _Selection.Count > 0;
		}

		public bool IsSelection(Node node)
		{
			return _Selection != null && _Selection.Contains(node.nodeID);
		}

		public void SetSelectNode(Node node)
		{
			int nodeID = node.nodeID;

			Undo.RecordObject(this, "Selection Node");

			_Selection.Clear();
			_Selection.Add(nodeID);

			EditorUtility.SetDirty(this);

			GroupNode group = node as GroupNode;
			if (group != null)
			{
				GroupControl groupControl = GetGroupControl(group);
				if (groupControl != null)
				{
					_GroupLayer.Focus(groupControl);
				}
			}
		}

		public void AddSelectNode(Node node)
		{
			int nodeID = node.nodeID;

			Undo.RecordObject(this, "Selection Node");

			if (_Selection.Contains(nodeID))
			{
				_Selection.Remove(nodeID);
			}
			else
			{
				_Selection.Add(nodeID);

				GroupNode group = node as GroupNode;
				if (group != null)
				{
					GroupControl groupControl = GetGroupControl(group);
					if (groupControl != null)
					{
						_GroupLayer.Focus(groupControl);
					}
				}
			}

			EditorUtility.SetDirty(this);
		}

		public void ChangeSelectNode(Node node, bool add)
		{
			int nodeID = node.nodeID;

			if (add)
			{
				Undo.IncrementCurrentGroup();

				AddSelectNode(node);

				Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
			}
			else
			{
				if (!_Selection.Contains(nodeID))
				{
					Undo.IncrementCurrentGroup();

					SetSelectNode(node);

					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
				}
				HandleUtility.Repaint();
			}
		}

		bool IsNodeInPoint(Node node, Vector2 point)
		{
			NodeEditor nodeEditor = GetNodeEditor(node);
			if (nodeEditor != null)
			{
				return nodeEditor.IsSelectPoint(point);
			}

			Rect position = node.position;
			return position.Contains(point);
		}

		public bool ContainsNodes(Vector2 point)
		{
			for (int i = 0, count = nodeGraph.nodeCount; i < count; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);

				if (IsNodeInPoint(node, point))
				{
					return true;
				}
			}

			return false;
		}

		bool IsNodeInRect(Node node, Rect rect)
		{
			NodeEditor nodeEditor = GetNodeEditor(node);
			if (nodeEditor != null)
			{
				return nodeEditor.IsSelectRect(rect);
			}

			Rect position = node.position;
			return (position.xMax >= rect.x && position.x <= rect.xMax &&
				position.yMax >= rect.y && position.y <= rect.yMax);
		}

		public List<Node> GetNodesInRect(Rect rect)
		{
			List<Node> nodes = new List<Node>();

			for (int i = 0, count = nodeGraph.nodeCount; i < count; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);

				if (IsNodeInRect(node, rect))
				{
					nodes.Add(node);
				}
			}

			return nodes;
		}

		public void SelectNodesInRect(Rect rect)
		{
			Undo.RecordObject(this, "Selection State");

			_Selection.Clear();

			for (int i = 0, count = nodeGraph.nodeCount; i < count; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);

				if (IsNodeInRect(node, rect))
				{
					_Selection.Add(node.nodeID);
				}
			}

			EditorUtility.SetDirty(this);
		}

		void DragSelection(int controlId, EventType eventType)
		{
			Event current = Event.current;

			switch (eventType)
			{
				case EventType.MouseDown:
					if (current.button == 0 && !(current.clickCount == 2 || current.alt))
					{
						Undo.IncrementCurrentGroup();

						GUIUtility.hotControl = GUIUtility.keyboardControl = controlId;
						_IsDragSelection = true;
						_DragBeginPos = current.mousePosition;
						_OldSelection = new List<int>(_Selection);
						if (!EditorGUI.actionKey && !current.shift)
						{
							Undo.RecordObject(this, "Selection State");

							_Selection.Clear();

							EditorUtility.SetDirty(this);
						}
						_HostWindow.BeginSelection();
						current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlId)
					{
						if (current.button == 0)
						{
							GUIUtility.hotControl = 0;
							_IsDragSelection = false;
							_OldSelection.Clear();
							_HostWindow.EndSelection();
						}

						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlId)
					{
						Rect selectionRect = EditorGUITools.FromToRect(_DragBeginPos, current.mousePosition);
						SelectNodesInRect(selectionRect);
						_HostWindow.SetSelectionRect(selectionRect);
						current.Use();
					}
					break;
				case EventType.Repaint:
					if (GUIUtility.hotControl == controlId)
					{
						Rect selectionRect = EditorGUITools.FromToRect(_DragBeginPos, current.mousePosition);
						SelectNodesInRect(selectionRect);
						_HostWindow.SetSelectionRect(selectionRect);
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.hotControl == controlId && current.keyCode == KeyCode.Escape)
					{
						Undo.RecordObject(this, "Selection State");

						_Selection = _OldSelection;

						GUIUtility.hotControl = 0;
						_IsDragSelection = false;
						_OldSelection.Clear();

						_HostWindow.EndSelection();

						Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

						EditorUtility.SetDirty(this);

						current.Use();
					}
					break;
			}
		}

		public void SelectNodes(Node[] nodes)
		{
			_Selection.Clear();

			foreach (Node node in nodes)
			{
				_Selection.Add(node.nodeID);

				INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
				if (behaviours != null)
				{
					int behaviourCount = behaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);
						EditorUtility.SetDirty(behaviour);
					}
				}
			}
		}

		public void OnAutoLayoutNode(Node node, Rect newPosition)
		{
			if (_DragNodePositions.Count == 0)
			{
				return;
			}

			if (_DragNodePositions.ContainsKey(node))
			{
				newPosition.position -= _DragNodeDistance;
				_DragNodePositions[node] = newPosition;
			}
			else if (!_SaveNodePositions.ContainsKey(node))
			{
				_SaveNodePositions[node] = node.position;
			}
		}

		public void RegisterDragNode(Node node)
		{
			_SaveNodePositions[node] = _DragNodePositions[node] = node.position;
		}

		public bool OnBeginDragNodes(Event current, Node selectTargetNode, Rect nodePosition)
		{
			int nodeID = selectTargetNode.nodeID;
			if (IsRenaming(nodeID))
			{
				Rect headerContentRect = GetHeaderContentRect(selectTargetNode, nodePosition);
				if (headerContentRect.Contains(current.mousePosition))
				{
					return false;
				}
			}

			ChangeSelectNode(selectTargetNode, EditorGUI.actionKey || current.shift);

			Undo.IncrementCurrentGroup();

			_BeginMousePosition = _HostWindow.UnclipToGraph(current.mousePosition);

			_DragNodeDistance = Vector2.zero;
			foreach (Node node in selection)
			{
				NodeEditor nodeEditor = GetNodeEditor(node);

				if (nodeEditor != null)
				{
					nodeEditor.OnBeginDrag(current);
				}
			}

			return true;
		}

		public void OnEndDragNodes(Event current)
		{
			if (_DropNodesElements.Count > 0)
			{
				if (editable)
				{
					Vector2 mousePosition = EditorGUIUtility.GUIToScreenPoint(current.mousePosition);

					foreach (var element in _DropNodesElements)
					{
						if (element.position.Contains(mousePosition))
						{
							List<Node> nodes = new List<Node>(_DragNodePositions.Keys);

							OnEscapeDragNodes(current);

							element.callback(nodes.ToArray());
							break;
						}
					}
				}

				_DropNodesElements.Clear();
			}
			_DragNodePositions.Clear();
			_SaveNodePositions.Clear();
		}

		protected virtual void OnDragNodes()
		{
		}

		public void OnDragNodes(Event current, Node selectTargetNode)
		{
			if (!editable)
			{
				return;
			}

			Vector2 currentPosition = _HostWindow.UnclipToGraph(current.mousePosition);
			_DragNodeDistance = currentPosition - _BeginMousePosition;

			bool isMove = false;

			foreach (KeyValuePair<Node, Rect> pair in _DragNodePositions)
			{
				Node node = pair.Key;
				Rect rect = pair.Value;

				Rect position = node.position;
				position.x = rect.x + _DragNodeDistance.x;
				position.y = rect.y + _DragNodeDistance.y;

				position = EditorGUITools.SnapPositionToGrid(position);

				if ((position.x != node.position.x || position.y != node.position.y))
				{
					isMove = true;

					Undo.RecordObject(nodeGraph, "Move Node");

					node.position = position;

					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

					EditorUtility.SetDirty(nodeGraph);
				}
			}

			if (isMove)
			{
				OnDragNodes();
				_HostWindow.DirtyGraphExtents();
			}
		}

		public void OnEscapeDragNodes(Event current)
		{
			if (editable)
			{
				bool isMove = false;

				foreach (KeyValuePair<Node, Rect> pair in _SaveNodePositions)
				{
					Node node = pair.Key;
					Rect position = pair.Value;

					position = EditorGUITools.SnapPositionToGrid(position);

					if ((position.x != node.position.x || position.y != node.position.y))
					{
						isMove = true;

						Undo.RecordObject(nodeGraph, "Move Node");

						node.position = position;

						Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

						EditorUtility.SetDirty(nodeGraph);
					}
				}

				if (isMove)
				{
					OnDragNodes();
					_HostWindow.DirtyGraphExtents();
				}
			}

			_DropNodesElements.Clear();
			_DragNodePositions.Clear();
			_SaveNodePositions.Clear();
		}

		public void DragNodes(Node selectTargetNode, Rect selectRect, RectOffset overflowOffset)
		{
			int controlId = GUIUtility.GetControlID(s_DragNodesControlID, FocusType.Passive);

			DragNodes(selectTargetNode, selectRect, overflowOffset, controlId);
		}

		public void DragNodes(Node selectTargetNode, Rect selectRect, RectOffset overflowOffset, int controlId)
		{
			Event current = Event.current;

			EventType eventType = current.GetTypeForControl(controlId);
			switch (eventType)
			{
				case EventType.MouseDown:
					if (current.button == 0 && selectRect.Contains(current.mousePosition))
					{
						Rect nodePosition = selectTargetNode.position;
						nodePosition.position = new Vector2(overflowOffset.left, overflowOffset.top);

						if (OnBeginDragNodes(current, selectTargetNode, nodePosition))
						{
							GUIUtility.hotControl = controlId;
							GUIUtility.keyboardControl = controlId;

							current.Use();
						}
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlId)
					{
						if (current.button == 0)
						{
							OnEndDragNodes(current);
							GUIUtility.hotControl = 0;
						}
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlId)
					{
						OnDragNodes(current, selectTargetNode);

						current.Use();
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.hotControl == controlId && current.keyCode == KeyCode.Escape)
					{
						OnEscapeDragNodes(current);

						GUIUtility.hotControl = 0;
						current.Use();

						Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
					}
					break;
				case EventType.Repaint:
					if (IsDragScroll() && GUIUtility.hotControl == controlId)
					{
						OnDragNodes(current, selectTargetNode);
					}
					break;
			}
		}

		public bool IsDraggingNode(Node node)
		{
			return _DragNodePositions.ContainsKey(node);
		}

		public void OnBeginResizeNode(Event current, Node node, ResizeControlPointFlags controlPoint)
		{
			_BeginMousePosition = _HostWindow.UnclipToGraph(current.mousePosition);

			ChangeSelectNode(node, false);

			_ResizeNodePosition = node.position;
			_ResizeControlPoint = controlPoint;
		}

		public ResizeControlPointFlags GetResizeControlPoint()
		{
			return _ResizeControlPoint;
		}

		public void OnResizeNode(Event current, Node node, Vector2 minSize)
		{
			Vector2 mousePosition = _HostWindow.UnclipToGraph(current.mousePosition);

			_DragNodeDistance = mousePosition - _BeginMousePosition;

			Undo.RecordObject(nodeGraph, "Move Node");

			Rect nodePosition = node.position;

			if ((_ResizeControlPoint & ResizeControlPointFlags.Top) == ResizeControlPointFlags.Top)
			{
				nodePosition.yMin = _ResizeNodePosition.yMin + _DragNodeDistance.y;
				if (nodePosition.size.y <= minSize.y)
				{
					nodePosition.yMin = nodePosition.yMax - minSize.y;
				}
				nodePosition.yMin = EditorGUITools.SnapToGrid(nodePosition.yMin);
			}
			if ((_ResizeControlPoint & ResizeControlPointFlags.Bottom) == ResizeControlPointFlags.Bottom)
			{
				nodePosition.yMax = _ResizeNodePosition.yMax + _DragNodeDistance.y;
				if (nodePosition.size.y <= minSize.y)
				{
					nodePosition.yMax = nodePosition.yMin + minSize.y;
				}
				nodePosition.yMax = EditorGUITools.SnapToGrid(nodePosition.yMax);
			}
			if ((_ResizeControlPoint & ResizeControlPointFlags.Left) == ResizeControlPointFlags.Left)
			{
				nodePosition.xMin = _ResizeNodePosition.xMin + _DragNodeDistance.x;
				if (nodePosition.size.x <= minSize.x)
				{
					nodePosition.xMin = nodePosition.xMax - minSize.x;
				}
				nodePosition.xMin = EditorGUITools.SnapToGrid(nodePosition.xMin);
			}
			if ((_ResizeControlPoint & ResizeControlPointFlags.Right) == ResizeControlPointFlags.Right)
			{
				nodePosition.xMax = _ResizeNodePosition.xMax + _DragNodeDistance.x;
				if (nodePosition.size.x <= minSize.x)
				{
					nodePosition.xMax = nodePosition.xMin + minSize.x;
				}
				nodePosition.xMax = EditorGUITools.SnapToGrid(nodePosition.xMax);
			}

			node.position = nodePosition;

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			EditorUtility.SetDirty(nodeGraph);

			_HostWindow.DirtyGraphExtents();
		}

		private HashSet<int> _InVisibleNodes = new HashSet<int>();

		void InitializeVisibleNodes()
		{
			if (nodeGraph == null)
			{
				return;
			}

			_InVisibleNodes.Clear();
		}

		public void UpdateVisibleNodes()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			HashSet<int> oldInVisibleNodes = new HashSet<int>(_InVisibleNodes);

			_InVisibleNodes.Clear();

			int nodeCount = nodeGraph.nodeCount;
			for (int i = 0; i < nodeCount; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);

				NodeEditor nodeEditor = GetNodeEditor(node);

				Rect nodePosition = node.position;

				if (nodeEditor != null)
				{
					RectOffset overflow = nodeEditor.GetOverflowOffset();
					nodePosition = overflow.Add(nodePosition);
				}

				if (!IsDraggingNode(node) && !OverlapsViewArea(nodePosition) && !IsDraggingBranch(node) && nodeEditor != null && !nodeEditor.IsDraggingVisible())
				{
					_InVisibleNodes.Add(node.nodeID);
				}
			}

			if (!oldInVisibleNodes.SetEquals(_InVisibleNodes))
			{
				_HostWindow.Repaint();
			}
		}

		public void VisibleNode(Node node)
		{
			VisibleNode(node.nodeID);
		}

		public void VisibleNode(int nodeID)
		{
			_InVisibleNodes.Remove(nodeID);
		}

		public bool IsVisibleNode(int nodeID)
		{
			return !_InVisibleNodes.Contains(nodeID);
		}

		public void BeginFrameSelected(Vector2 frameSelectTarget)
		{
			_HostWindow.FrameSelected(frameSelectTarget);
		}

		public void BeginFrameSelected()
		{
			if (!HasSelection())
			{
				return;
			}

			Node[] selectionNodes = selection;

			Vector2 frameSelectTarget = Vector2.zero;
			foreach (Node node in selectionNodes)
			{
				frameSelectTarget += node.position.center;
			}
			frameSelectTarget /= (float)selectionNodes.Length;

			BeginFrameSelected(frameSelectTarget);
		}

		public void BeginFrameSelected(Node node, bool select = true)
		{
			if (select)
			{
				SetSelectNode(node);
			}

			BeginFrameSelected(node.position.center);
		}

		public virtual void OnDrawBranchies()
		{
		}

		public virtual void OnDrawHoverBranch()
		{
		}

		public bool isHoverNode
		{
			get;
			private set;
		}

		public void OnGraphGUI()
		{
			using (new ProfilerScope("OnGraphGUI"))
			{
				using (new ProfilerScope("Nodes"))
				{
					for (int i = 0, count = nodeEditorCount; i < count; i++)
					{
						NodeEditor nodeEditor = GetNodeEditor(i);
						if (nodeEditor != null && nodeEditor.IsWindow() && (_HostWindow.isCapture || IsVisibleNode(nodeEditor.node.nodeID)))
						{
							nodeEditor.DoWindow();
						}
					}
				}
			}
		}

		CalculatorNode CreateCalculatorInternal(Vector2 position, System.Type calculatorType)
		{
			CalculatorNode calculator = nodeGraph.CreateCalculator(calculatorType);

			if (calculator != null)
			{
				Undo.RecordObject(nodeGraph, "Created Calculator");

				calculator.position = EditorGUITools.SnapPositionToGrid(new Rect(position.x, position.y, 300, 100));

				EditorUtility.SetDirty(nodeGraph);

				CreateNodeEditor(calculator);
				UpdateNodeCommentControl(calculator);

				SetSelectNode(calculator);
			}

			Repaint();

			return calculator;
		}

		public void CreateCalculator(Vector2 position, System.Type calculatorType)
		{
			Undo.IncrementCurrentGroup();

			CreateCalculatorInternal(position, calculatorType);

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
		}

		void CreateComment(Vector2 position)
		{
			Undo.IncrementCurrentGroup();

			CommentNode comment = nodeGraph.CreateComment();

			if (comment != null)
			{
				Undo.RecordObject(nodeGraph, "Created Comment");

				comment.position = EditorGUITools.SnapPositionToGrid(new Rect(position.x, position.y, 300, 100));

				EditorUtility.SetDirty(nodeGraph);

				CreateNodeEditor(comment);

				SetSelectNode(comment);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			Repaint();
		}

		public void CreateGroup(Vector2 position)
		{
			Undo.IncrementCurrentGroup();

			GroupNode group = nodeGraph.CreateGroup();

			if (group != null)
			{
				Undo.RecordObject(nodeGraph, "Created Group");

				group.position = EditorGUITools.SnapPositionToGrid(new Rect(position.x, position.y, 300, 100));

				EditorUtility.SetDirty(nodeGraph);

				CreateNodeEditor(group);
				UpdateNodeCommentControl(group);

				SetSelectNode(group);

				BeginRename(group.nodeID, group.name);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			Repaint();
		}

		public DataBranchRerouteNode CreateDataBranchRerouteNode(Vector2 position, System.Type type)
		{
			Undo.IncrementCurrentGroup();

			DataBranchRerouteNode rerouteNode = nodeGraph.CreateDataBranchRerouteNode(EditorGUITools.SnapToGrid(position), type);

			if (rerouteNode != null)
			{
				CreateNodeEditor(rerouteNode);

				SetSelectNode(rerouteNode);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			Repaint();

			return rerouteNode;
		}

		void CreateCalculator(object obj)
		{
			MousePosition mousePosition = (MousePosition)obj;

			Rect buttonRect = new Rect(mousePosition.screenPoint, Vector2.zero);

			CalculateMenuWindow.instance.Init(this, mousePosition.guiPoint, buttonRect);
		}

		void CreateComment(object obj)
		{
			Vector2 position = (Vector2)obj;

			CreateComment(position);
		}

		void CreateGroup(object obj)
		{
			Vector2 position = (Vector2)obj;

			CreateGroup(position);
		}

		void CopyNodes()
		{
			Clipboard.CopyNodes(selection);
		}

		void CutNodes()
		{
			Clipboard.CopyNodes(selection);
			DeleteNodes();
		}

		public void DuplicateNodes(Vector2 position, Node[] sourceNodes, string undoName = "Duplicate Nodes")
		{
			Undo.IncrementCurrentGroup();

			Undo.RegisterCompleteObjectUndo(nodeGraph, undoName);

			Vector2 minPosition = new Vector2(float.MaxValue, float.MaxValue);

			foreach (Node sourceNode in sourceNodes)
			{
				minPosition.x = Mathf.Min(sourceNode.position.x, minPosition.x);
				minPosition.y = Mathf.Min(sourceNode.position.y, minPosition.y);
			}

			position -= minPosition;

			Node[] duplicateNodes = Clipboard.DuplicateNodes(position, sourceNodes, nodeGraph, false);

			if (duplicateNodes != null && duplicateNodes.Length > 0)
			{
				EditorUtility.SetDirty(nodeGraph);

				Undo.RecordObject(this, undoName);

				SelectNodes(duplicateNodes);

				EditorUtility.SetDirty(this);
			}

			Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

			_HostWindow.Repaint();
		}

		void DuplicateNodes(object obj)
		{
			Vector2 position = (Vector2)obj;

			DuplicateNodes(position, selection);
		}

		void PasteNodes(object obj)
		{
			Vector2 position = (Vector2)obj;

			DuplicateNodes(position, Clipboard.GetClippedNodes(), "Paste Nodes");
		}

		protected string GetNodeTitle(Node node)
		{
			NodeEditor nodeEditor = GetNodeEditor(node);
			if (nodeEditor != null)
			{
				return nodeEditor.GetTitle();
			}

			return "Node";
		}

		public void DeleteNodes(Node[] deleteNodes)
		{
			Undo.IncrementCurrentGroup();
			int undoGroup = Undo.GetCurrentGroup();

			List<int> deleteNodeIDs = new List<int>();

			foreach (Node deleteNode in deleteNodes)
			{
				if (deleteNode.IsDeletable())
				{
					if (nodeGraph.DeleteNode(deleteNode))
					{
						deleteNodeIDs.Add(deleteNode.nodeID);
						DeleteNodeCommentControl(deleteNode);
						DeleteNodeEditor(deleteNode);
					}
					else
					{
						string name = GetNodeTitle(deleteNode);
						Debug.LogErrorFormat(Localization.GetWord("DeleteError"), name);
					}
				}
			}

			nodeGraph.OnValidateNodes();

			Undo.RecordObject(this, "Delete Nodes");

			foreach (int deleteNodeID in deleteNodeIDs)
			{
				_Selection.Remove(deleteNodeID);
			}

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(this);

			_HostWindow.DirtyGraphExtents();
		}

		void DeleteNodes()
		{
			DeleteNodes(selection);
		}

		void ExpandAll(bool expanded, Node[] nodes)
		{
			int nodeCount = nodes.Length;
			for (int i = 0; i < nodeCount; i++)
			{
				NodeEditor nodeEditor = GetNodeEditor(nodes[i]);
				if (nodeEditor != null)
				{
					nodeEditor.ExpandAll(expanded);
				}
			}
		}

		void ExpandAll(bool expanded)
		{
			for (int i = 0, count = nodeEditorCount; i < count; i++)
			{
				NodeEditor nodeEditor = GetNodeEditor(i);
				if (nodeEditor != null)
				{
					nodeEditor.ExpandAll(expanded);
				}
			}
		}

		void ExpandAll()
		{
			ExpandAll(true);
		}

		void FoldAll()
		{
			ExpandAll(false);
		}

		void ExpandAllInSelectionNodes()
		{
			ExpandAll(true, selection);
		}

		void FoldAllInSelectionNodes()
		{
			ExpandAll(false, selection);
		}

		protected virtual void SetCreateNodeContextMenu(GenericMenu menu, bool editable)
		{
		}

		void HandleContextMenu(int controlId, EventType eventType)
		{
			Event current = Event.current;

			if (!IsDragBranch() && eventType == EventType.ContextClick)
			{
				bool editable = this.editable;

				GenericMenu menu = new GenericMenu();

				SetCreateNodeContextMenu(menu, editable);

				menu.AddSeparator("");

				if (editable)
				{
					menu.AddItem(EditorContents.createCalculator, false, CreateCalculator, new MousePosition(current.mousePosition));
				}
				else
				{
					menu.AddDisabledItem(EditorContents.createCalculator);
				}

				menu.AddSeparator("");

				if (editable)
				{
					menu.AddItem(EditorContents.createGroup, false, CreateGroup, current.mousePosition);
					menu.AddItem(EditorContents.createComment, false, CreateComment, current.mousePosition);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.createGroup);
					menu.AddDisabledItem(EditorContents.createComment);
				}

				menu.AddSeparator("");

				bool isCopyable = false;
				foreach (var nodeID in _Selection)
				{
					NodeEditor nodeEditor = GetNodeEditorFromID(nodeID);
					if (nodeEditor != null && nodeEditor.IsCopyable())
					{
						isCopyable = true;
						break;
					}
				}

				bool isDeletable = false;
				foreach (var nodeID in _Selection)
				{
					Node node = nodeGraph.GetNodeFromID(nodeID);
					if (node != null && node.IsDeletable())
					{
						isDeletable = true;
						break;
					}
				}

				if (isCopyable && isDeletable && editable)
				{
					menu.AddItem(EditorContents.cut, false, CutNodes);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.cut);
				}

				if (isCopyable)
				{
					menu.AddItem(EditorContents.copy, false, CopyNodes);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.copy);
				}

				if (Clipboard.hasCopyedNodes && editable)
				{
					menu.AddItem(EditorContents.paste, false, PasteNodes, current.mousePosition);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.paste);
				}

				menu.AddSeparator("");

				if (isCopyable && editable)
				{
					menu.AddItem(EditorContents.duplicate, false, DuplicateNodes, current.mousePosition);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.duplicate);
				}

				if (isDeletable && editable)
				{
					menu.AddItem(EditorContents.delete, false, DeleteNodes);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.delete);
				}

				menu.AddSeparator("");

				if (HasSelection())
				{
					menu.AddItem(EditorContents.expandAll, false, ExpandAllInSelectionNodes);
					menu.AddItem(EditorContents.collapseAll, false, FoldAllInSelectionNodes);
				}
				else
				{
					menu.AddDisabledItem(EditorContents.expandAll);
					menu.AddDisabledItem(EditorContents.collapseAll);
				}

				menu.ShowAsContext();

				current.Use();
			}
		}

		Vector2 _MousePosition;

		void HandleGraphView()
		{
			int controlId = GUIUtility.GetControlID(s_HandleGraphViewControlID, FocusType.Keyboard);
			Event current = Event.current;

			EventType eventType = current.GetTypeForControl(controlId);

			HandleContextMenu(controlId, eventType);
			HandleCommand(controlId, eventType);
			DragSelection(controlId, eventType);
		}

		void HandleCommand(int controlId, EventType eventType)
		{
			Event current = Event.current;

			bool editable = this.editable;

			switch (eventType)
			{
				case EventType.MouseMove:
					_MousePosition = current.mousePosition;
					break;
				case EventType.ValidateCommand:
					switch (current.commandName)
					{
						case "Cut":
						case "Duplicate":
						case "Delete":
						case "SoftDelete":
							if (HasSelection() && editable)
							{
								current.Use();
							}
							break;
						case "Copy":
						case "FrameSelected":
							if (HasSelection())
							{
								current.Use();
							}
							break;
						case "Paste":
							if (Clipboard.hasCopyedNodes && editable)
							{
								current.Use();
							}
							break;
						case "SelectAll":
							if (nodeGraph.nodeCount > 0)
							{
								current.Use();
							}
							break;
					}
					break;
				case EventType.ExecuteCommand:
					switch (current.commandName)
					{
						case "Copy":
							CopyNodes();
							current.Use();
							break;
						case "Cut":
							CutNodes();
							current.Use();
							break;
						case "Paste":
							PasteNodes(_MousePosition);
							current.Use();
							break;
						case "Duplicate":
							DuplicateNodes(_MousePosition);
							current.Use();
							break;
						case "FrameSelected":
							BeginFrameSelected();
							current.Use();
							break;
						case "Delete":
						case "SoftDelete":
							DeleteNodes();
							current.Use();
							break;
						case "SelectAll":
							SelectAll();
							current.Use();
							break;
					}
					break;
			}
		}

		void ClearVisibleDataSlot()
		{
			bool changed = false;

			int nodeCount = nodeGraph.nodeCount;
			for (int i = 0; i < nodeCount; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);

				if (!IsVisibleNode(node.nodeID))
				{
					continue;
				}

				INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
				if (behaviours != null)
				{
					for (int behaviourCount = behaviours.GetNodeBehaviourCount(), behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);
						if (behaviour != null)
						{
							for (int slotCount = behaviour.dataSlotFieldCount, slotIndex = 0; slotIndex < slotCount; slotIndex++)
							{
								DataSlotField slotField = behaviour.GetDataSlotField(slotIndex);
								if (slotField == null)
								{
									continue;
								}

								bool oldVisible = slotField.isVisible;

								slotField.ClearVisible();

								if (oldVisible != slotField.isVisible)
								{
									changed = true;
								}
							}
						}
					}
				}

				DataBranchRerouteNode rerouteNode = node as DataBranchRerouteNode;
				if (rerouteNode != null)
				{
					DataSlotField slotField = rerouteNode.slotField;
					if (slotField != null)
					{
						bool oldVisible = slotField.isVisible;
						slotField.ClearVisible();
						if (oldVisible != slotField.isVisible)
						{
							changed = true;
						}
					}
				}
			}

			if (changed)
			{
				Repaint();
			}
		}

		private NodeListGUI _NodeListGUI;

		protected virtual int NodeListSortComparison(NodeEditor a, NodeEditor b)
		{
			return NodeListGUI.Defaults.SortComparison(a, b);
		}

		void OnNodeListSelect(NodeEditor nodeEditor)
		{
			List<NodeEditor> viewNodes = _NodeListGUI.viewNodes;

			int nodeIndex = viewNodes.IndexOf(nodeEditor);

			if (Event.current.shift)
			{
				int lastSelectIndex = 0;
				if (HasSelection())
				{
					for (int i = _Selection.Count - 1; i >= 0; i--)
					{
						Node lastSelectNode = _NodeGraph.GetNodeFromID(_Selection[i]);
						NodeEditor lastSelectNodeEditor = GetNodeEditor(lastSelectNode);
						if (viewNodes.Contains(lastSelectNodeEditor))
						{
							lastSelectIndex = viewNodes.IndexOf(lastSelectNodeEditor);
							break;
						}
					}
				}

				int minIndex = Mathf.Min(nodeIndex, lastSelectIndex);
				int maxIndex = Mathf.Max(nodeIndex, lastSelectIndex);

				bool add = EditorGUI.actionKey;

				Undo.IncrementCurrentGroup();

				Undo.RecordObject(this, "Selection Node");

				if (!add)
				{
					_Selection.Clear();
				}

				for (int i = minIndex; i <= maxIndex; i++)
				{
					NodeEditor nodeEditorSelect = viewNodes[i];
					if (!IsSelection(nodeEditorSelect.node))
					{
						AddSelectNode(nodeEditorSelect.node);
					}
				}
			}
			else if (EditorGUI.actionKey)
			{
				ChangeSelectNode(nodeEditor.node, true);
			}
			else
			{
				SetSelectNode(nodeEditor.node);
			}

			if (IsSelection(nodeEditor.node))
			{
				BeginFrameSelected(nodeEditor.node, false);
			}
		}

		public void NodeListPanelGUI()
		{
			_NodeListGUI.OnGUI();
		}

		void CreateParameterContainer()
		{
			int undoGroup = Undo.GetCurrentGroup();

			ParameterContainerInternal parameterContainer = ParameterContainerInternal.Create(_NodeGraph.gameObject, ParameterContainerEditorUtility.ParameterContainerType, _NodeGraph);
#if !ARBOR_DEBUG
			parameterContainer.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif

			Undo.RecordObject(_NodeGraph, "Create ParameterContainer");

			ParameterContainerEditorUtility.SetParameterContainer(_NodeGraph, parameterContainer);

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(_NodeGraph);
		}

		private Editor _ParameterContainerEditor = null;

		public void ParametersPanelGUI()
		{
			ParameterContainerInternal parameterContainer = _NodeGraph.parameterContainer;

			bool editable = this.editable;

			if (parameterContainer == null)
			{
				EditorGUI.BeginDisabledGroup(!editable);

				if (GUILayout.Button(EditorContents.create, (GUILayoutOption[])null))
				{
					CreateParameterContainer();
				}

				EditorGUI.EndDisabledGroup();
			}
			else
			{
				if (_ParameterContainerEditor != null && (_ParameterContainerEditor.target == null || _ParameterContainerEditor.target != parameterContainer))
				{
					FinalizeParameterContainerEditor();
				}

				if (_ParameterContainerEditor == null)
				{
					_ParameterContainerEditor = Editor.CreateEditor(parameterContainer);
				}

				try
				{
					_ParameterContainerEditor.OnInspectorGUI();
				}
				catch (System.Exception ex)
				{
					if (EditorGUITools.ShouldRethrowException(ex))
					{
						throw;
					}
					else
					{
						Debug.LogException(ex);
					}
				}
			}
		}

		void ShowDataBranchValueAll(object visible)
		{
			Undo.RecordObject(nodeGraph, "Change Visible Values");
			int branchCount = nodeGraph.dataBranchCount;
			for (int i = 0; i < branchCount; i++)
			{
				DataBranch branch = nodeGraph.GetDataBranchFromIndex(i);
				if (branch != null)
				{
					branch.showDataValue = (bool)visible;
				}
			}
			EditorUtility.SetDirty(nodeGraph);
		}

		protected virtual bool HasViewMenu()
		{
			return false;
		}

		protected virtual void OnSetViewMenu(GenericMenu menu)
		{
		}

		public void SetViewMenu(GenericMenu menu)
		{
			menu.AddItem(EditorContents.expandAll, false, ExpandAll);
			menu.AddItem(EditorContents.collapseAll, false, FoldAll);
			menu.AddSeparator("");
			menu.AddItem(EditorContents.nodeCommentViewModeNormal, ArborSettings.nodeCommentViewMode == NodeCommentViewMode.Normal, () =>
			{
				ArborSettings.nodeCommentViewMode = NodeCommentViewMode.Normal;
				Repaint();
			});
			menu.AddItem(EditorContents.nodeCommentViewModeShowAll, ArborSettings.nodeCommentViewMode == NodeCommentViewMode.ShowAll, () =>
			{
				ArborSettings.nodeCommentViewMode = NodeCommentViewMode.ShowAll;
				Repaint();
			});
			menu.AddItem(EditorContents.nodeCommentViewModeShowCommentedOnly, ArborSettings.nodeCommentViewMode == NodeCommentViewMode.ShowCommentedOnly, () =>
			{
				ArborSettings.nodeCommentViewMode = NodeCommentViewMode.ShowCommentedOnly;
				Repaint();
			});
			menu.AddItem(EditorContents.nodeCommentViewModeHideAll, ArborSettings.nodeCommentViewMode == NodeCommentViewMode.HideAll, () =>
			{
				ArborSettings.nodeCommentViewMode = NodeCommentViewMode.HideAll;
				Repaint();
			});
			menu.AddItem(EditorContents.dataSlotShowOutsideNode, ArborSettings.dataSlotShowMode == DataSlotShowMode.Outside, () =>
			{
				ArborSettings.dataSlotShowMode = DataSlotShowMode.Outside;
				Repaint();
			});
			menu.AddItem(EditorContents.dataSlotShowInsideNode, ArborSettings.dataSlotShowMode == DataSlotShowMode.Inside, () =>
			{
				ArborSettings.dataSlotShowMode = DataSlotShowMode.Inside;
				Repaint();
			});
			menu.AddItem(EditorContents.dataSlotShowFlexibly, ArborSettings.dataSlotShowMode == DataSlotShowMode.Flexibly, () =>
			{
				ArborSettings.dataSlotShowMode = DataSlotShowMode.Flexibly;
				Repaint();
			});

			if (HasViewMenu())
			{
				menu.AddSeparator("");
				OnSetViewMenu(menu);
			}
		}

		protected virtual bool HasDebugMenu()
		{
			return false;
		}

		protected virtual void OnSetDebugMenu(GenericMenu menu)
		{
		}

		public void SetDenugMenu(GenericMenu menu)
		{
			menu.AddItem(EditorContents.showAllDataValuesAlways, ArborSettings.showDataValue, () => ArborSettings.showDataValue = !ArborSettings.showDataValue);
			menu.AddSeparator("");
			if (editable)
			{
				menu.AddItem(EditorContents.showAllDataValues, false, ShowDataBranchValueAll, true);
				menu.AddItem(EditorContents.hideAllDataValues, false, ShowDataBranchValueAll, false);
			}
			else
			{
				menu.AddDisabledItem(EditorContents.showAllDataValues);
				menu.AddDisabledItem(EditorContents.hideAllDataValues);
			}
			if (HasDebugMenu())
			{
				menu.AddSeparator("");
				OnSetDebugMenu(menu);
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			if (_NodeListGUI == null)
			{
				_NodeListGUI = new NodeListGUI();
			}

			_NodeListGUI.sortComparisonCallback = NodeListSortComparison;
			_NodeListGUI.onSelectCallback = OnNodeListSelect;

			_NodeListGUI.Initialize(this);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDisable()
		{
			_NodeListGUI.Dispose();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDestroy()
		{
			FinalizeGraph();
		}

		public Node GetHoverNode(Vector2 position)
		{
			int nodeCount = nodeGraph.nodeCount;
			for (int i = 0; i < nodeCount; i++)
			{
				Node node = nodeGraph.GetNodeFromIndex(i);
				if (node is GroupNode)
				{
					continue;
				}

				NodeEditor nodeEditor = GetNodeEditor(node);
				if (nodeEditor != null && nodeEditor.IsHover(position))
				{
					return node;
				}
			}

			return null;
		}

		protected virtual Node GetActiveNode()
		{
			return null;
		}

		public void LiveTracking()
		{
			if (nodeGraph == null)
			{
#if ARBOR_DEBUG
				Debug.LogWarning("nodeGraph == null");
#endif
				return;
			}

			if (!ArborSettings.liveTracking)
			{
				return;
			}

			Node activeNode = GetActiveNode();
			if (activeNode != null)
			{
				if (ArborSettings.liveTrackingHierarchy)
				{
					NodeGraph childGraph = null;

					INodeBehaviourContainer container = activeNode as INodeBehaviourContainer;
					if (container != null)
					{
						for (int i = 0, count = container.GetNodeBehaviourCount(); i < count; i++)
						{
							NodeBehaviour behaviour = container.GetNodeBehaviour<NodeBehaviour>(i);

							if (behaviour != null && behaviour.enabled)
							{
								INodeGraphContainer graphContainer = behaviour as INodeGraphContainer;
								if (graphContainer != null && graphContainer.GetNodeGraphCount() > 0)
								{
									childGraph = graphContainer.GetNodeGraph<NodeGraph>(0);
									break;
								}
							}
						}
					}

					if (childGraph != null)
					{
						_HostWindow.ChangeCurrentNodeGraph(childGraph, true);
						return;
					}
				}

				BeginFrameSelected(activeNode, false);
			}
			else if (ArborSettings.liveTrackingHierarchy && (!HasPlayState() || GetPlayState() == PlayState.Stopping))
			{
				NodeGraph parentGraph = nodeGraph.parentGraph;
				if (parentGraph != null)
				{
					_HostWindow.ChangeCurrentNodeGraph(parentGraph, true);
				}
			}
		}
	}
}