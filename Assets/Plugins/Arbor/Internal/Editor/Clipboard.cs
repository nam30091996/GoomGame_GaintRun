//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

using Arbor;
using Arbor.DynamicReflection;
using Arbor.BehaviourTree;

namespace ArborEditor
{
	public sealed class Clipboard : Arbor.ScriptableSingleton<Clipboard>
	{
		private static readonly DynamicField s_NodeGraph_IsEditorField;
		private static readonly DynamicField s_ParameterContainerInternal_IsEditorField;
		private static readonly DynamicField s_Node_NodeGraphField;
		private static readonly DynamicField s_NodeBehaviour_IsClipboardField;

		static Clipboard()
		{
			s_NodeGraph_IsEditorField = DynamicField.GetField(typeof(NodeGraph).GetField("_IsEditor", BindingFlags.NonPublic | BindingFlags.Instance));
			s_ParameterContainerInternal_IsEditorField = DynamicField.GetField(typeof(ParameterContainerInternal).GetField("_IsEditor", BindingFlags.NonPublic | BindingFlags.Instance));

			System.Type nodeType = typeof(Node);
			s_Node_NodeGraphField = DynamicField.GetField(nodeType.GetField("_NodeGraph", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

			System.Type nodeBehaviourType = typeof(NodeBehaviour);
			s_NodeBehaviour_IsClipboardField = DynamicField.GetField(nodeBehaviourType.GetField("_Internal_IsClipboard", BindingFlags.Instance | BindingFlags.NonPublic));
		}

		public static void CopyNodeGraph(NodeGraph source)
		{
			instance.CopyNodeGraphInternal(source);
		}

		public static void PasteNodeGraphValues(NodeGraph nodeGraph)
		{
			instance.PasteNodeGraphValuesInternal(nodeGraph);
		}

		public static void PasteNodeGraphAsNew(GameObject gameObject)
		{
			instance.PasteNodeGraphAsNewInternal(gameObject);
		}

		public static void CopyParameterContainer(ParameterContainerInternal source)
		{
			instance.CopyParameterContainerInternal(source);
		}

		public static void CopyBehaviour(NodeBehaviour source)
		{
			instance.CopyBehaviourInternal(source);
		}

		public static void PasteBehaviourValues(NodeBehaviour behaviour)
		{
			instance.PasteBehaviourValuesInternal(behaviour);
		}

		public static void PasteStateBehaviourAsNew(State state, int index)
		{
			instance.PasteStateBehaviourAsNewInternal(state, index);
		}

		public static void PasteStateBehaviourAsNew(State state, int index, StateBehaviour destBehaviour)
		{
			instance.PasteStateBehaviourAsNewInternal(state, index, destBehaviour);
		}

		public static void PasteDecoratorAsNew(TreeBehaviourNode behaviourNode, int index)
		{
			instance.PasteDecoratorAsNewInternal(behaviourNode, index);
		}

		public static void PasteDecoratorAsNew(TreeBehaviourNode behaviourNode, int index, Decorator destDecorator)
		{
			instance.PasteDecoratorAsNewInternal(behaviourNode, index, destDecorator);
		}

		public static void PasteServiceAsNew(TreeBehaviourNode behaviourNode, int index)
		{
			instance.PasteServiceAsNewInternal(behaviourNode, index);
		}

		public static void PasteServiceAsNew(TreeBehaviourNode behaviourNode, int index, Service destService)
		{
			instance.PasteServiceAsNewInternal(behaviourNode, index, destService);
		}

		public static bool CompareBehaviourType(System.Type behaviourType, bool inherit)
		{
			return instance.CompareBehaviourTypeInternal(behaviourType, inherit);
		}

		public static bool hasCopyedNodes
		{
			get
			{
				return instance.hasCopyedNodesInternal;
			}
		}

		public static void CopyNodes(Node[] nodes)
		{
			instance.CopyNodesInternal(nodes);
		}

		public static Node[] GetClippedNodes()
		{
			return instance.GetClippedNodesInternal();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private GameObject _GameObject = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeBehaviour _CopyBehaviour = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _CopyBehaviourSourceGraph = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _NodeClipboard = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private List<int> _CopyNodes = new List<int>();

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _SourceNodeGraph = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private Object _CopyComponent = null;

		private static GameObject gameObject
		{
			get
			{
				if (instance._GameObject == null)
				{
					instance._GameObject = EditorUtility.CreateGameObjectWithHideFlags("Clipboard", HideFlags.HideAndDontSave);
					instance._GameObject.tag = "EditorOnly";
				}

				instance._GameObject.SetActive(false);

				return instance._GameObject;
			}
		}

		private void ClearInternal()
		{
			bool saveEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			if (_CopyBehaviour != null)
			{
				DestroyImmediate(_CopyBehaviour);
				_CopyBehaviour = null;
				_CopyBehaviourSourceGraph = null;
			}

			if (_NodeClipboard != null)
			{
				NodeGraph.Destroy(_NodeClipboard);
				_NodeClipboard = null;
			}

			_CopyNodes.Clear();

			_SourceNodeGraph = null;

			ComponentUtility.useEditorProcessor = saveEnabled;
		}

		public static bool IsSameNodeGraph(NodeGraph sourceGraph, NodeGraph destGraph)
		{
			return instance.IsSameNodeGraphInternal(sourceGraph, destGraph);
		}

		public static bool IsSameNodeGraph(NodeBehaviour sourceBehaviour, NodeBehaviour destBehaviour)
		{
			return instance.IsSameNodeGraphInternal(sourceBehaviour, destBehaviour);
		}

		public static bool GetEditorNodeGraph(NodeGraph nodeGraph)
		{
			return (bool)s_NodeGraph_IsEditorField.GetValue(nodeGraph);
		}

		public static void SetEditorParameterContainer(ParameterContainerInternal parameterContainer, bool isEditor)
		{
			s_ParameterContainerInternal_IsEditorField.SetValue(parameterContainer, isEditor);
		}

		public static bool GetEditorParameterContainer(ParameterContainerInternal parameterContainer)
		{
			return (bool)s_ParameterContainerInternal_IsEditorField.GetValue(parameterContainer);
		}

		public static void SetEditorNodeGraph(NodeGraph nodeGraph, bool isEditor)
		{
			s_NodeGraph_IsEditorField.SetValue(nodeGraph, isEditor);
		}

		static void SetNodeGraph(Node node, NodeGraph graph)
		{
			s_Node_NodeGraphField.SetValue(node, graph);
		}

		internal static void SetNodeBehaviourIsClipboard(NodeBehaviour behaviour, bool isClipboard)
		{
			s_NodeBehaviour_IsClipboardField.SetValue(behaviour, isClipboard);
		}

		public static void CopyNodeBehaviour(NodeBehaviour source, NodeBehaviour dest, bool checkLink)
		{
			StateBehaviour sourceBehaviour = source as StateBehaviour;
			StateBehaviour destBehaviour = dest as StateBehaviour;
			if (sourceBehaviour != null && destBehaviour != null)
			{
				CopyBehaviour(sourceBehaviour, destBehaviour, checkLink, null);
				return;
			}

			CopyNodeBehaviourInternal(source, dest, checkLink);
		}

		static void CopyNodeGraphBehaviours(NodeGraph dest)
		{
			for (int count = dest.nodeCount, i = 0; i < count; i++)
			{
				Node node = dest.GetNodeFromIndex(i);

				if (node.nodeGraph != dest)
				{
					SetNodeGraph(node, dest);
				}

				INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
				if (behaviours != null)
				{
					int behaviourCount = behaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);
						if (behaviour != null)
						{
							NodeBehaviour copyBehaviour = NodeBehaviour.CreateNodeBehaviour(node, behaviour.GetType(), true);

							CopyNodeBehaviour(behaviour, copyBehaviour, false);

							behaviours.SetNodeBehaviour(behaviourIndex, copyBehaviour);
						}
					}
				}
			}
		}

		private static bool _IsCopyNodeGraph = false;

		static void CopyNodeGraph(NodeGraph source, NodeGraph dest)
		{
#if UNITY_2018_1_OR_NEWER
			using (new Presets.DisableApplyDefaultPresetScope(true))
#endif
			{
				bool tmpIsCopyNodeGraph = _IsCopyNodeGraph;
				_IsCopyNodeGraph = true;

				try
				{
					Object destOwnerBehaviour = dest.ownerBehaviourObject;

					dest.DestroySubComponents(false);

					bool isEditor = Clipboard.GetEditorNodeGraph(dest);
					Clipboard.SetEditorNodeGraph(dest, true);

					ParameterContainerInternal destParameterContainer = null;
					ParameterContainerInternal sourceParameterContainer = source.parameterContainer;
					if (sourceParameterContainer != null)
					{
						destParameterContainer = ParameterContainerInternal.Create(dest.gameObject, sourceParameterContainer.GetType(), dest);
						SetEditorParameterContainer(destParameterContainer, isEditor);

						CopyParameterContainer(sourceParameterContainer, destParameterContainer);
					}

					bool cachedEnabled = ComponentUtility.useEditorProcessor;
					ComponentUtility.useEditorProcessor = true;

					Object sourceOwnerBehaviour = source.ownerBehaviourObject;
					if (!isEditor)
					{
						source.ownerBehaviourObject = destOwnerBehaviour;
					}

					EditorUtility.CopySerialized(source, dest);

					source.ownerBehaviourObject = sourceOwnerBehaviour;

					for (int i = 0; i < dest.dataBranchRerouteNodes.count; i++)
					{
						var rerouteNode = dest.dataBranchRerouteNodes[i];
						if (rerouteNode != null)
						{
							RerouteSlot slot = rerouteNode.link;
							if (slot != null)
							{
								slot.nodeGraph = dest;
							}

							IInputSlot inputSlot = slot as IInputSlot;
							if (inputSlot != null)
							{
								DataBranch branch = inputSlot.GetBranch();
								if (branch != null)
								{
									branch.inBehaviour = dest;
									branch.inNodeID = rerouteNode.nodeID;
								}
							}

							IOutputSlot outputSlot = slot as IOutputSlot;
							if (outputSlot != null)
							{
								for (int j = 0; j < outputSlot.branchCount; j++)
								{
									DataBranch branch = outputSlot.GetBranch(j);
									if (branch != null)
									{
										branch.outBehaviour = dest;
										branch.outNodeID = rerouteNode.nodeID;
									}
								}
							}
						}
					}

					ComponentUtility.useEditorProcessor = cachedEnabled;

					CopyNodeGraphBehaviours(dest);

					ParameterContainerEditorUtility.SetParameterContainer(dest, destParameterContainer);
					if (sourceParameterContainer != null)
					{
						MoveParameterReference(dest, sourceParameterContainer);
					}

					dest.ownerBehaviourObject = destOwnerBehaviour;

					Clipboard.SetEditorNodeGraph(dest, isEditor);

				}
				finally
				{
					_IsCopyNodeGraph = tmpIsCopyNodeGraph;
				}
			}
		}

		public static GameObject SaveToPrefab(string path, NodeGraph nodeGraph)
		{
			GameObject prefabGameObject = null;
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(nodeGraph.graphName, HideFlags.HideInHierarchy);

			try
			{
				bool cachedEnabled = ComponentUtility.useEditorProcessor;
				ComponentUtility.useEditorProcessor = false;

				NodeGraph destGraph = NodeGraph.Create(gameObject, nodeGraph.GetType());
				SetEditorNodeGraph(destGraph, true);

				CopyNodeGraph(nodeGraph, destGraph);

				SetEditorNodeGraph(destGraph, false);
				destGraph.enabled = true;

				ComponentUtility.useEditorProcessor = cachedEnabled;

				prefabGameObject = EditorGUITools.SaveAsPrefabAsset(gameObject, path);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				Object.DestroyImmediate(gameObject);
			}

			return prefabGameObject;
		}

		static void CopyInternalVariable(InternalVariableBase source, InternalVariableBase dest)
		{
			if (dest == null)
			{
				return;
			}

			ParameterContainerInternal container = dest.parameterContainer;

			EditorUtility.CopySerialized(source, dest);

			dest.Initialize(container);
		}

		static void CopyParameterVariables(ParameterContainerInternal dest)
		{
			for (int count = dest.parameterCount, i = 0; i < count; i++)
			{
				Parameter parameter = dest.GetParameterFromIndex(i);

				if (parameter.container != dest)
				{
					parameter.container = dest;
				}

				switch (parameter.type)
				{
					case Parameter.Type.Variable:
						{
							VariableBase variable = parameter.variableObject;
							if (variable != null)
							{
								VariableBase copyVariable = VariableBase.Create(dest, variable.GetType());

								CopyInternalVariable(variable, copyVariable);

								parameter.variableObject = copyVariable;
							}
						}
						break;
					case Parameter.Type.VariableList:
						{
							VariableListBase variable = parameter.variableListObject;
							if (variable != null)
							{
								VariableListBase copyVariable = VariableListBase.Create(dest, variable.GetType());

								CopyInternalVariable(variable, copyVariable);

								parameter.variableListObject = copyVariable;
							}
						}
						break;
				}
			}
		}

		static void CopyParameterContainer(ParameterContainerInternal source, ParameterContainerInternal dest)
		{
			bool isEditor = Clipboard.GetEditorParameterContainer(dest);

			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = true;

			Object owner = dest.owner;

			EditorUtility.CopySerialized(source, dest);

			dest.owner = owner;

			ComponentUtility.useEditorProcessor = cachedEnabled;

			CopyParameterVariables(dest);

			Clipboard.SetEditorParameterContainer(dest, isEditor);
		}

		internal static void DestroyChildGraphs(NodeBehaviour dest)
		{
			INodeGraphContainer graphContainer = dest as INodeGraphContainer;
			if (graphContainer == null)
			{
				return;
			}

			int graphCount = graphContainer.GetNodeGraphCount();
			for (int i = 0; i < graphCount; i++)
			{
				NodeGraph nodeGraph = graphContainer.GetNodeGraph<NodeGraph>(i);

				if (nodeGraph != null)
				{
					if (!nodeGraph.external)
					{
						NodeGraph.Destroy(nodeGraph);
					}
				}
			}
		}

		static void MoveDataSlots(NodeBehaviour behaviour)
		{
			NodeGraph nodeGraph = behaviour.nodeGraph;

			for (int i = 0; i < behaviour.dataSlotFieldCount; i++)
			{
				var dataSlotField = behaviour.GetDataSlotField(i);
				if (dataSlotField != null)
				{
					DataSlot slot = dataSlotField.slot;
					if (slot != null)
					{
						slot.nodeGraph = nodeGraph;
					}

					IInputSlot inputSlot = slot as IInputSlot;
					if (inputSlot != null)
					{
						DataBranch branch = inputSlot.GetBranch();
						if (branch != null && branch.inBehaviour != behaviour)
						{
							branch.inBehaviour = behaviour;
						}
					}

					IOutputSlot outputSlot = slot as IOutputSlot;
					if (outputSlot != null)
					{
						for (int j = 0; j < outputSlot.branchCount; j++)
						{
							DataBranch branch = outputSlot.GetBranch(j);
							if (branch != null && branch.outBehaviour != behaviour)
							{
								branch.outBehaviour = behaviour;
							}
						}
					}
				}
			}
		}

		static void ClearDataSlots(NodeBehaviour behaviour)
		{
			for (int i = 0; i < behaviour.dataSlotFieldCount; i++)
			{
				var dataSlotField = behaviour.GetDataSlotField(i);
				if (dataSlotField != null)
				{
					DataSlot slot = dataSlotField.slot;
					if (slot != null)
					{
						slot.ClearBranch();
					}
				}
			}
		}

		static void ReconnectDataSlots(NodeBehaviour behaviour)
		{
			NodeGraph nodeGraph = behaviour.nodeGraph;

			for (int i = 0; i < behaviour.dataSlotFieldCount; i++)
			{
				DataSlotField slotField = behaviour.GetDataSlotField(i);
				if (slotField == null)
				{
					continue;
				}

				DataSlot slot = slotField.slot;
				if (slot == null)
				{
					continue;
				}

				bool clear = true;

				if (slot.slotType == SlotType.Input)
				{
					slot.nodeGraph = nodeGraph;

					IInputSlot inputSlot = slot as IInputSlot;
					if (inputSlot != null)
					{
						DataBranch branch = inputSlot.GetBranch();
						if (branch != null)
						{
							DataSlotField outputSlotField = branch.outputSlotField;
							if (outputSlotField != null && DataSlotField.IsConnectable(slotField, outputSlotField))
							{
								DataBranch newBranch = nodeGraph.ConnectDataBranch(behaviour.nodeID, behaviour, slot, branch.outNodeID, branch.outBehaviour, branch.outputSlot);
								if (newBranch != null)
								{
									clear = false;
									newBranch.enabled = true;
								}
							}
						}
					}
				}

				if (clear)
				{
					slot.ClearBranch();
				}
			}
		}

		internal static void CopyChildGraphs(NodeBehaviour dest)
		{
			INodeGraphContainer graphContainer = dest as INodeGraphContainer;
			if (graphContainer == null)
			{
				return;
			}

			NodeGraph parentGraph = dest.nodeGraph;
			bool isEditor = instance._CopyBehaviour == dest || (parentGraph != null && GetEditorNodeGraph(parentGraph));

			int graphCount = graphContainer.GetNodeGraphCount();
			for (int i = 0; i < graphCount; i++)
			{
				NodeGraph nodeGraph = graphContainer.GetNodeGraph<NodeGraph>(i);

				if (!nodeGraph.external)
				{
					NodeGraph destGraph = NodeGraph.Create(dest.gameObject, nodeGraph.GetType()) as NodeGraph;
#if !ARBOR_DEBUG
					destGraph.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif
					destGraph.ownerBehaviour = dest;

					SetEditorNodeGraph(destGraph, isEditor);

					CopyNodeGraph(nodeGraph, destGraph);

					SetEditorNodeGraph(destGraph, false);

					graphContainer.SetNodeGraph(i, destGraph);
				}
			}
		}

		internal static void CheckStateLink(StateLink stateLink, bool isSameNodeGraph, NodeGraph nodeGraph, List<NodeDuplicator> duplicators)
		{
			if (duplicators != null)
			{
				foreach (NodeDuplicator duplicator in duplicators)
				{
					if (duplicator.sourceNode.nodeID == stateLink.stateID)
					{
						stateLink.stateID = duplicator.destNode.nodeID;
						return;
					}
				}
			}

			if (!isSameNodeGraph || nodeGraph.GetNodeFromID(stateLink.stateID) == null)
			{
				stateLink.stateID = 0;
			}
		}

		internal static void CopyBehaviour(StateBehaviour source, StateBehaviour dest, bool checkLink, List<NodeDuplicator> duplicators)
		{
			if (dest == null)
			{
				return;
			}

			bool expanded = dest.expanded;

			CopyNodeBehaviourInternal(source, dest, checkLink);

			dest.expanded = expanded;

			if (checkLink)
			{
				Undo.RecordObject(dest, "Copy Behaviour");

				bool isSameNodeGraph = IsSameNodeGraph(source, dest);
				NodeGraph nodeGraph = dest.nodeGraph;

				for (int i = 0, count = dest.stateLinkCount; i < count; ++i)
				{
					StateLink s = dest.GetStateLink(i);
					CheckStateLink(s, isSameNodeGraph, nodeGraph, duplicators);
				}

				EditorUtility.SetDirty(dest);
			}
		}

		static void CopyNodeBehaviourInternal(NodeBehaviour source, NodeBehaviour dest, bool checkLink)
		{
			if (dest == null)
			{
				return;
			}

			NodeGraph nodeGraph = dest.nodeGraph;
			int nodeID = dest.nodeID;

			DestroyChildGraphs(dest);

			if (nodeGraph != null)
			{
				nodeGraph.DisconnectDataBranch(dest);
			}

			SetNodeBehaviourIsClipboard(dest, true);
			EditorUtility.CopySerialized(source, dest);
			SetNodeBehaviourIsClipboard(dest, false);

			dest.Initialize(nodeGraph, nodeID);

			bool isSameNodeGraph = IsSameNodeGraph(source, dest);

			if (_IsCopyNodeGraph)
			{
				MoveDataSlots(dest);
			}
			else if (!_IsDuplicateNode)
			{
				if (isSameNodeGraph)
				{
					ReconnectDataSlots(dest);
				}
				else if (checkLink)
				{
					ClearDataSlots(dest);
				}
				else
				{
					MoveDataSlots(dest);
				}
			}

			CopyChildGraphs(dest);
		}

		public static void MoveBehaviour(Node node, NodeBehaviour sourceBehaviour)
		{
			State state = node as State;
			StateBehaviour sourceStateBehaviour = sourceBehaviour as StateBehaviour;
			if (state != null && sourceStateBehaviour != null)
			{
				MoveStateBehaviour(state, sourceStateBehaviour);
				return;
			}

			CalculatorNode calculatorNode = node as CalculatorNode;
			Calculator sourceCalculator = sourceBehaviour as Calculator;
			if (calculatorNode != null && sourceCalculator != null)
			{
				MoveCalculator(calculatorNode, sourceCalculator);
				return;
			}

			TreeBehaviourNode treeBehaviourNode = node as TreeBehaviourNode;
			Decorator sourceDecorator = sourceBehaviour as Decorator;
			if (treeBehaviourNode != null && sourceDecorator != null)
			{
				MoveDecorator(treeBehaviourNode, sourceDecorator);
				return;
			}

			Service sourceService = sourceBehaviour as Service;
			if (treeBehaviourNode != null && sourceService != null)
			{
				MoveService(treeBehaviourNode, sourceService);
				return;
			}

			CompositeNode compositeNode = node as CompositeNode;
			CompositeBehaviour sourceCompositeBehaviour = sourceBehaviour as CompositeBehaviour;
			if (compositeNode != null && sourceCompositeBehaviour != null)
			{
				MoveCompositeBehaviour(compositeNode, sourceCompositeBehaviour);
				return;
			}

			ActionNode actionNode = node as ActionNode;
			ActionBehaviour sourceActionBehaviour = sourceBehaviour as ActionBehaviour;
			if (actionNode != null && sourceActionBehaviour != null)
			{
				MoveActionBehaviour(actionNode, sourceActionBehaviour);
				return;
			}
		}

		static void MoveStateBehaviour(State state, StateBehaviour sourceBehaviour)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			StateBehaviour destBehaviour = NodeBehaviour.CreateNodeBehaviour(state, sourceBehaviour.GetType(), true) as StateBehaviour;

			if (destBehaviour != null)
			{
				state.AddBehaviour(destBehaviour);
				Clipboard.CopyBehaviour(sourceBehaviour, destBehaviour, false, null);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveCalculator(CalculatorNode calculatorNode, Calculator sourceCalculator)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			Calculator destCalculator = calculatorNode.CreateCalculator(sourceCalculator.GetType());

			if (destCalculator != null)
			{
				Clipboard.CopyNodeBehaviourInternal(sourceCalculator, destCalculator, false);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveCompositeBehaviour(CompositeNode compositeNode, CompositeBehaviour sourceCompositeBehaviour)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			CompositeBehaviour destCompositeBehaviour = compositeNode.CreateCompositeBehaviour(sourceCompositeBehaviour.GetType());

			if (destCompositeBehaviour != null)
			{
				Clipboard.CopyNodeBehaviourInternal(sourceCompositeBehaviour, destCompositeBehaviour, false);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveActionBehaviour(ActionNode actionNode, ActionBehaviour sourceActionBehaviour)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			ActionBehaviour destActionBehaviour = actionNode.CreateActionBehaviour(sourceActionBehaviour.GetType());

			if (destActionBehaviour != null)
			{
				Clipboard.CopyNodeBehaviourInternal(sourceActionBehaviour, destActionBehaviour, false);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveDecorator(TreeBehaviourNode behaviourNode, Decorator sourceDecorator)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			Decorator destDecorator = NodeBehaviour.CreateNodeBehaviour(behaviourNode, sourceDecorator.GetType(), true) as Decorator;

			if (destDecorator != null)
			{
				behaviourNode.decoratorList.Add(destDecorator);
				Clipboard.CopyNodeBehaviourInternal(sourceDecorator, destDecorator, false);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveService(TreeBehaviourNode behaviourNode, Service sourceService)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			Service destService = NodeBehaviour.CreateNodeBehaviour(behaviourNode, sourceService.GetType(), true) as Service;

			if (destService != null)
			{
				behaviourNode.serviceList.Add(destService);
				Clipboard.CopyNodeBehaviourInternal(sourceService, destService, false);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		static void MoveParameterReference(NodeGraph nodeGraph, ParameterContainerInternal sourceParameterContainer)
		{
			ParameterContainerInternal destParameterContainer = nodeGraph.parameterContainer;

			int nodeCount = nodeGraph.nodeCount;
			for (int i = 0; i < nodeCount; i++)
			{
				var node = nodeGraph.GetNodeFromIndex(i);

				var behaviourContainer = node as INodeBehaviourContainer;
				if (behaviourContainer == null)
				{
					continue;
				}

				int behaviourCount = behaviourContainer.GetNodeBehaviourCount();
				for (int j = 0; j < behaviourCount; j++)
				{
					NodeBehaviour behaviour = behaviourContainer.GetNodeBehaviour<NodeBehaviour>(j);

					EachField<ParameterReference>.Find(behaviour, behaviour.GetType(), (r) =>
					{
						if (r.constantContainer == sourceParameterContainer)
						{
							r.constantContainer = destParameterContainer;
						}
					});
				}
			}
		}

		public static void MoveParameterContainer(NodeGraph nodeGraph)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			ParameterContainerInternal sourceParameterContainer = nodeGraph.parameterContainer;
			if (sourceParameterContainer != null)
			{
				if (sourceParameterContainer.gameObject == nodeGraph.gameObject && sourceParameterContainer.owner == null)
				{
					sourceParameterContainer.owner = nodeGraph;
				}
				else
				{
					ParameterContainerInternal destParameterContainer = ParameterContainerInternal.Create(nodeGraph.gameObject, sourceParameterContainer.GetType(), nodeGraph);

					CopyParameterContainer(sourceParameterContainer, destParameterContainer);

					ParameterContainerEditorUtility.SetParameterContainer(nodeGraph, destParameterContainer);

					MoveParameterReference(nodeGraph, sourceParameterContainer);
				}

				var scene = nodeGraph.gameObject.scene;
				if (scene.IsValid())
				{
					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
				}
				else
				{
					EditorUtility.SetDirty(nodeGraph);
				}
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		public static void MoveVariable(Parameter parameter, VariableBase sourceVariable)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			VariableBase destVariable = VariableBase.Create(parameter.container, sourceVariable.GetType()) as VariableBase;
			parameter.variableObject = destVariable;

			if (destVariable != null)
			{
				CopyInternalVariable(sourceVariable, destVariable);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		public static void MoveVariableList(Parameter parameter, VariableListBase sourceVariableList)
		{
			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			VariableListBase destVariableList = VariableListBase.Create(parameter.container, sourceVariableList.GetType()) as VariableListBase;
			parameter.variableListObject = destVariableList;

			if (destVariableList != null)
			{
				CopyInternalVariable(sourceVariableList, destVariableList);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		public static System.Type copiedComponentType
		{
			get
			{
				Object component = instance._CopyComponent;
				if (component == null)
				{
					return null;
				}

				return component.GetType();
			}
		}

		void DestroyCopyComponent()
		{
			if (_CopyComponent == null)
			{
				return;
			}

			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			if (_CopyComponent is NodeGraph)
			{
				NodeGraph.Destroy(_CopyComponent as NodeGraph);
			}
			else if (_CopyComponent is NodeBehaviour)
			{
				NodeBehaviour.Destroy(_CopyComponent as NodeBehaviour);
			}
			else
			{
				Object.DestroyImmediate(_CopyComponent);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;

			_CopyComponent = null;
		}

		private void CopyNodeGraphInternal(NodeGraph source)
		{
			DestroyCopyComponent();

			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			NodeGraph dest = NodeGraph.Create(gameObject, source.GetType());
			SetEditorNodeGraph(dest, true);

			CopyNodeGraph(source, dest);

			ComponentUtility.useEditorProcessor = cachedEnabled;

			_CopyComponent = dest;
		}

		private void PasteNodeGraphValuesInternal(NodeGraph nodeGraph)
		{
			NodeGraph source = _CopyComponent as NodeGraph;
			if (source == null || nodeGraph == null ||
				source.GetType() != nodeGraph.GetType())
			{
				return;
			}

			Undo.IncrementCurrentGroup();

			int currentGroup = Undo.GetCurrentGroup();

			Undo.RegisterCompleteObjectUndo(nodeGraph, "Paste " + nodeGraph.GetType() + " Values");

			CopyNodeGraph(source, nodeGraph);

			Undo.CollapseUndoOperations(currentGroup);

			EditorUtility.SetDirty(nodeGraph);
		}

		private void PasteNodeGraphAsNewInternal(GameObject gameObject)
		{
			NodeGraph source = _CopyComponent as NodeGraph;
			if (source == null)
			{
				return;
			}

			Undo.IncrementCurrentGroup();

			int currentGroup = Undo.GetCurrentGroup();

			NodeGraph destGraph = NodeGraph.Create(gameObject, source.GetType());

			CopyNodeGraph(source, destGraph);

			Undo.CollapseUndoOperations(currentGroup);

			EditorUtility.SetDirty(destGraph);
		}

		private void CopyParameterContainerInternal(ParameterContainerInternal source)
		{
			DestroyCopyComponent();

			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			ParameterContainerInternal dest = ParameterContainerInternal.Create(gameObject, source.GetType());

			SetEditorParameterContainer(dest, true);

			CopyParameterContainer(source, dest);

			ComponentUtility.useEditorProcessor = cachedEnabled;

			UnityEditorInternal.ComponentUtility.CopyComponent(dest);

			_CopyComponent = dest;
		}

		private void CopyBehaviourInternal(NodeBehaviour source)
		{
			ClearInternal();

			_CopyBehaviour = gameObject.AddComponent(source.GetType()) as NodeBehaviour;
			_CopyBehaviourSourceGraph = source.nodeGraph;

			CopyNodeBehaviour(source, _CopyBehaviour, false);
		}

		private void PasteBehaviourValuesInternal(NodeBehaviour behaviour)
		{
			CopyNodeBehaviour(_CopyBehaviour, behaviour, true);
		}

		private void PasteStateBehaviourAsNewInternal(State state, int index)
		{
			PasteStateBehaviourAsNewInternal(state, index, _CopyBehaviour);
		}

		private void PasteStateBehaviourAsNewInternal(State state, int index, NodeBehaviour destBehaviour)
		{
#if UNITY_2018_1_OR_NEWER
			using (new Presets.DisableApplyDefaultPresetScope(true))
#endif
			{
				StateBehaviour behaviour = NodeBehaviour.CreateNodeBehaviour(state, destBehaviour.GetType(), true) as StateBehaviour;

				if (index != -1)
				{
					state.InsertBehaviour(index, behaviour);
				}
				else
				{
					state.AddBehaviour(behaviour);
				}

				CopyNodeBehaviour(destBehaviour, behaviour, true);
			}
		}

		private void PasteDecoratorAsNewInternal(TreeBehaviourNode behaviourNode, int index)
		{
			PasteDecoratorAsNewInternal(behaviourNode, index, _CopyBehaviour);
		}

		private void PasteDecoratorAsNewInternal(TreeBehaviourNode behaviourNode, int index, NodeBehaviour destDecorator)
		{
			Decorator decorator = NodeBehaviour.CreateNodeBehaviour(behaviourNode, destDecorator.GetType(), true) as Decorator;

			if (index == -1)
			{
				behaviourNode.decoratorList.Add(decorator);
			}
			else
			{
				behaviourNode.decoratorList.Insert(index, decorator);
			}

			CopyNodeBehaviour(destDecorator, decorator, true);
		}

		private void PasteServiceAsNewInternal(TreeBehaviourNode behaviourNode, int index)
		{
			PasteServiceAsNewInternal(behaviourNode, index, _CopyBehaviour);
		}

		private void PasteServiceAsNewInternal(TreeBehaviourNode behaviourNode, int index, NodeBehaviour destService)
		{
			Service service = NodeBehaviour.CreateNodeBehaviour(behaviourNode, destService.GetType(), true) as Service;

			if (index == -1)
			{
				behaviourNode.serviceList.Add(service);
			}
			else
			{
				behaviourNode.serviceList.Insert(index, service);
			}

			CopyNodeBehaviour(destService, service, true);
		}

		private bool CompareBehaviourTypeInternal(System.Type behaviourType, bool inherit)
		{
			if (_CopyBehaviour == null)
			{
				return false;
			}

			System.Type copyType = _CopyBehaviour.GetType();

			return copyType == behaviourType || (inherit && behaviourType.IsAssignableFrom(copyType));
		}

		private bool hasCopyedNodesInternal
		{
			get
			{
				return _CopyNodes.Count != 0;
			}
		}

		private class ReconnectData
		{
			public DataBranch targetBranch = null;

			public int destInNodeID = 0;
			public NodeBehaviour destInBehaviour = null;
			public DataSlot destInputSlot = null;

			public int destOutNodeID = 0;
			public NodeBehaviour destOutBehaviour = null;
			public DataSlot destOutputSlot = null;
		}

		static void ReconnectDataBranch(NodeGraph nodeGraph, List<NodeDuplicator> duplicators, bool clip)
		{
			// Listup branchies
			HashSet<DataBranch> targetBranchies = new HashSet<DataBranch>();
			foreach (NodeDuplicator duplicator in duplicators)
			{
				Node sourceNode = duplicator.sourceNode;

				INodeBehaviourContainer behaviours = sourceNode as INodeBehaviourContainer;
				if (behaviours != null)
				{
					int behaviourCount = behaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);

						if (behaviour == null)
						{
							continue;
						}

						int slotCount = behaviour.dataSlotFieldCount;
						for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
						{
							DataSlotField slotField = behaviour.GetDataSlotField(slotIndex);
							if (slotField == null)
							{
								continue;
							}

							DataSlot slot = slotField.slot;

							IInputSlot inputSlot = slot as IInputSlot;
							if (inputSlot != null)
							{
								DataBranch branch = inputSlot.GetBranch();
								if (branch != null)
								{
									targetBranchies.Add(branch);
								}
							}
							else
							{
								IOutputSlot outputSlot = slot as IOutputSlot;
								if (outputSlot != null)
								{
									int branchCount = outputSlot.branchCount;
									for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
									{
										DataBranch branch = outputSlot.GetBranch(branchIndex);
										if (branch != null)
										{
											targetBranchies.Add(branch);
										}
									}
								}
							}
						}
					}
				}
				else
				{
					DataBranchRerouteNode rerouteNode = sourceNode as DataBranchRerouteNode;
					if (rerouteNode != null)
					{
						DataBranch inputBranch = rerouteNode.link.inputSlot.GetBranch();
						if (inputBranch != null)
						{
							targetBranchies.Add(inputBranch);
						}

						int branchCount = rerouteNode.link.outputSlot.branchCount;
						for (int i = 0; i < branchCount; i++)
						{
							DataBranch branch = rerouteNode.link.outputSlot.GetBranch(i);
							if (branch != null)
							{
								targetBranchies.Add(branch);
							}
						}
					}
				}
			}

			// listup reconnects
			List<ReconnectData> reconnects = new List<ReconnectData>();
			foreach (DataBranch targetBranch in targetBranchies)
			{
				NodeBehaviour inBehaviour = targetBranch.inBehaviour as NodeBehaviour;
				NodeBehaviour outBehaviour = targetBranch.outBehaviour as NodeBehaviour;

				int destInNodeID = 0;
				NodeBehaviour destInBehaviour = null;
				DataSlot destInputSlot = null;

				int destOutNodeID = 0;
				NodeBehaviour destOutBehaviour = null;
				DataSlot destOutputSlot = null;

				foreach (NodeDuplicator duplicator in duplicators)
				{
					if (destInNodeID == 0 && destInBehaviour == null)
					{
						if (inBehaviour != null)
						{
							destInBehaviour = duplicator.GetDestBehaviour(inBehaviour);
							if (destInBehaviour != null)
							{
								destInNodeID = destInBehaviour.nodeID;
								int slotCount = destInBehaviour.dataSlotFieldCount;
								for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
								{
									DataSlotField slotField = destInBehaviour.GetDataSlotField(slotIndex);
									if (slotField == null)
									{
										continue;
									}

									InputSlotBase slot = slotField.slot as InputSlotBase;
									if (slot != null && slot.IsConnected(targetBranch))
									{
										destInputSlot = slot;
										break;
									}
								}
							}
						}
						else
						{
							destInBehaviour = null;
							DataBranchRerouteNode rerouteNode = duplicator.destNode as DataBranchRerouteNode;
							if (rerouteNode != null && rerouteNode.link.inputSlot.IsConnected(targetBranch))
							{
								destInNodeID = rerouteNode.nodeID;
								destInputSlot = rerouteNode.link;
							}
						}
					}

					if (destOutNodeID == 0 && destOutBehaviour == null)
					{
						if (outBehaviour != null)
						{
							destOutBehaviour = duplicator.GetDestBehaviour(outBehaviour);
							if (destOutBehaviour != null)
							{
								destOutNodeID = destOutBehaviour.nodeID;
								int slotCount = destOutBehaviour.dataSlotFieldCount;
								for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
								{
									DataSlotField slotField = destOutBehaviour.GetDataSlotField(slotIndex);
									if (slotField == null)
									{
										continue;
									}
									OutputSlotBase slot = slotField.slot as OutputSlotBase;
									if (slot != null && slot.IsConnected(targetBranch))
									{
										destOutputSlot = slot;
										break;
									}
								}
							}
						}
						else
						{
							destOutBehaviour = null;
							DataBranchRerouteNode rerouteNode = duplicator.destNode as DataBranchRerouteNode;
							if (rerouteNode != null && rerouteNode.link.outputSlot.IsConnected(targetBranch))
							{
								destOutNodeID = rerouteNode.nodeID;
								destOutputSlot = rerouteNode.link;
							}
						}
					}

					if ((destInNodeID != 0 || destInBehaviour != null) && (destOutNodeID != 0 || destOutBehaviour != null))
					{
						break;
					}
				}

				ReconnectData reconnectData = new ReconnectData()
				{
					targetBranch = targetBranch,
					destInBehaviour = destInBehaviour,
					destInNodeID = destInNodeID,
					destInputSlot = destInputSlot,
					destOutBehaviour = destOutBehaviour,
					destOutNodeID = destOutNodeID,
					destOutputSlot = destOutputSlot,
				};

				reconnects.Add(reconnectData);
			}

			// clear branchies
			foreach (NodeDuplicator duplicator in duplicators)
			{
				Node destNode = duplicator.destNode;

				INodeBehaviourContainer behaviours = destNode as INodeBehaviourContainer;
				if (behaviours != null)
				{
					int behaviourCount = behaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);

						if (behaviour == null)
						{
							continue;
						}

						int slotCount = behaviour.dataSlotFieldCount;
						for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
						{
							DataSlotField slotField = behaviour.GetDataSlotField(slotIndex);
							if (slotField == null)
							{
								continue;
							}

							DataSlot slot = slotField.slot;

							if (slot != null)
							{
								slot.ClearBranch();
							}
						}
					}
				}
				else
				{
					DataBranchRerouteNode rerouteNode = destNode as DataBranchRerouteNode;
					if (rerouteNode != null)
					{
						rerouteNode.link.ClearBranch();
					}
				}
			}

			// reconnect
			foreach (var reconnectData in reconnects)
			{
				DataBranch targetBranch = reconnectData.targetBranch;

				NodeBehaviour outBehaviour = targetBranch.outBehaviour as NodeBehaviour;

				int destInNodeID = reconnectData.destInNodeID;
				NodeBehaviour destInBehaviour = reconnectData.destInBehaviour;
				DataSlot destInputSlot = reconnectData.destInputSlot;

				int destOutNodeID = reconnectData.destOutNodeID;
				NodeBehaviour destOutBehaviour = reconnectData.destOutBehaviour;
				DataSlot destOutputSlot = reconnectData.destOutputSlot;

				if ((destInNodeID != 0 || destInBehaviour != null) && (destOutNodeID != 0 || destOutBehaviour != null))
				{
					Bezier2D bezier = targetBranch.lineBezier;

					DataBranch branch = null;
					if (clip)
					{
						branch = nodeGraph.ConnectDataBranch(targetBranch.branchID, destInNodeID, destInBehaviour, destInputSlot, destOutNodeID, destOutBehaviour, destOutputSlot);
					}
					else
					{
						branch = nodeGraph.ConnectDataBranch(destInNodeID, destInBehaviour, destInputSlot, destOutNodeID, destOutBehaviour, destOutputSlot);
					}

					if (branch != null)
					{
						branch.lineBezier = new Bezier2D(bezier);
						branch.enabled = true;
					}
				}
				else
				{
					if (clip)
					{
						if (destInNodeID != 0 || destInBehaviour != null)
						{
							Bezier2D bezier = targetBranch.lineBezier;

							DataBranch branch = nodeGraph.ConnectDataBranch(targetBranch.branchID, destInNodeID, destInBehaviour, destInputSlot, targetBranch.outNodeID, targetBranch.outBehaviour, null);

							if (branch != null)
							{
								branch.lineBezier = new Bezier2D(bezier);
								branch.enabled = true;
							}
						}
					}
					else
					{
						if (destInputSlot != null)
						{
							NodeGraph outNodeGraph = null;
							if (outBehaviour != null)
							{
								outNodeGraph = outBehaviour.nodeGraph;
							}
							else
							{
								outNodeGraph = targetBranch.outBehaviour as NodeGraph;
							}

							if (IsSameNodeGraph(outNodeGraph, nodeGraph) && targetBranch.isValidOutputSlot)
							{
								Bezier2D bezier = targetBranch.lineBezier;

								DataBranch branch = nodeGraph.ConnectDataBranch(destInNodeID, destInBehaviour, destInputSlot, targetBranch.outNodeID, targetBranch.outBehaviour, targetBranch.outputSlot);

								if (branch != null)
								{
									branch.lineBezier = new Bezier2D(bezier);
									branch.enabled = true;
								}
							}
							else
							{
								IInputSlot destInSlot = destInputSlot as IInputSlot;
								if (destInSlot != null)
								{
									destInSlot.RemoveBranch(targetBranch);
								}
							}
						}
					}

					IOutputSlot destOutSlot = destOutputSlot as IOutputSlot;
					if (destOutSlot != null)
					{
						destOutSlot.RemoveBranch(targetBranch);
					}
				}
			}
		}

		private static bool _IsDuplicateNode = false;

		public static Node[] DuplicateNodes(Vector2 position, Node[] sourceNodes, NodeGraph nodeGraph, bool clip)
		{
#if UNITY_2018_1_OR_NEWER
			using (new Presets.DisableApplyDefaultPresetScope(true))
#endif
			{
				try
				{
					_IsDuplicateNode = true;

					List<Node> duplicateNodes = new List<Node>();

					List<NodeDuplicator> nodeDuplicators = new List<NodeDuplicator>();

					foreach (Node sourceNode in sourceNodes)
					{
						NodeDuplicator duplicator = NodeDuplicator.CreateDuplicator(nodeGraph, sourceNode, clip);
						if (duplicator != null)
						{
							Node node = duplicator.Duplicate(position);
							if (node != null)
							{
								duplicateNodes.Add(node);
								nodeDuplicators.Add(duplicator);
							}
							else
							{
								Object.DestroyImmediate(duplicator);
							}
						}
					}

					foreach (NodeDuplicator duplicator in nodeDuplicators)
					{
						duplicator.OnAfterDuplicate(nodeDuplicators);
					}

					ReconnectDataBranch(nodeGraph, nodeDuplicators, clip);

					nodeGraph.OnValidateNodes();

					foreach (NodeDuplicator duplicator in nodeDuplicators)
					{
						Object.DestroyImmediate(duplicator);
					}

					_IsDuplicateNode = false;

					return duplicateNodes.ToArray();
				}
				finally
				{
					_IsDuplicateNode = false;
				}
			}
		}

		private void CopyNodesInternal(Node[] nodes)
		{
			ClearInternal();

			if (nodes != null && nodes.Length > 0)
			{
				_SourceNodeGraph = nodes[0].nodeGraph;
			}

			bool cachedEnabled = ComponentUtility.useEditorProcessor;
			ComponentUtility.useEditorProcessor = false;

			_NodeClipboard = NodeGraph.Create(gameObject, _SourceNodeGraph.GetType());
			SetEditorNodeGraph(_NodeClipboard, true);

			foreach (Node node in DuplicateNodes(Vector2.zero, nodes, _NodeClipboard, true))
			{
				_CopyNodes.Add(node.nodeID);
			}

			ComponentUtility.useEditorProcessor = cachedEnabled;
		}

		private bool IsSameNodeGraphInternal(NodeGraph sourceGraph, NodeGraph destGraph)
		{
			if (sourceGraph == _NodeClipboard)
			{
				sourceGraph = _SourceNodeGraph;
			}

			return sourceGraph == destGraph;
		}

		private bool IsSameNodeGraphInternal(NodeBehaviour sourceBehaviour, NodeBehaviour destBehaviour)
		{
			NodeGraph sourceGraph = sourceBehaviour.nodeGraph;
			if (sourceBehaviour == _CopyBehaviour)
			{
				sourceGraph = _CopyBehaviourSourceGraph;
			}

			return IsSameNodeGraphInternal(sourceGraph, destBehaviour.nodeGraph);
		}

		private Node[] GetClippedNodesInternal()
		{
			List<Node> nodes = new List<Node>();

			if (_NodeClipboard != null)
			{
				for (int i = 0, count = _CopyNodes.Count; i < count; i++)
				{
					int nodeID = _CopyNodes[i];
					nodes.Add(_NodeClipboard.GetNodeFromID(nodeID));
				}
			}

			return nodes.ToArray();
		}
	}
}
