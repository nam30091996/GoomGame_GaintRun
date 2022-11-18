//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

using Arbor;

namespace ArborEditor
{
	internal abstract class NodeDuplicator : ScriptableObject
	{
		public NodeGraph targetGraph;
		public bool isClip;
		public Node sourceNode;
		public Node destNode;

		public static bool HasDuplicator(Node node)
		{
			if (node == null)
			{
				return false;
			}

			System.Type classType = node.GetType();
			System.Type editorType = CustomAttributes<CustomNodeDuplicator>.FindEditorType(classType);

			return editorType != null && editorType.IsSubclassOf(typeof(NodeDuplicator));
		}

		public static NodeDuplicator CreateDuplicator(NodeGraph targetGraph, Node sourceNode, bool clip)
		{
			if (sourceNode == null)
			{
				return null;
			}

			System.Type classType = sourceNode.GetType();
			System.Type editorType = CustomAttributes<CustomNodeDuplicator>.FindEditorType(classType);

			if (editorType == null || !editorType.IsSubclassOf(typeof(NodeDuplicator)))
			{
				return null;
			}

			NodeDuplicator nodeDuplicator = CreateInstance(editorType) as NodeDuplicator;
			nodeDuplicator.hideFlags = HideFlags.HideAndDontSave;
			nodeDuplicator.Initialize(targetGraph, sourceNode, clip);

			return nodeDuplicator;
		}

		private Dictionary<NodeBehaviour, NodeBehaviour> _DuplicateBehaviours = new Dictionary<NodeBehaviour, NodeBehaviour>();

		public void Initialize(NodeGraph targetGraph, Node sourceNode, bool clip)
		{
			this.targetGraph = targetGraph;
			this.sourceNode = sourceNode;
			this.isClip = clip;
		}

		protected abstract Node OnDuplicate();

		public Node Duplicate(Vector2 position)
		{
			destNode = OnDuplicate();

			if (destNode != null)
			{
				destNode.position = sourceNode.position;
				destNode.position.position += position;

				destNode.position.position = EditorGUITools.SnapToGrid(destNode.position.position);

				destNode.showComment = sourceNode.showComment;
				destNode.nodeComment = sourceNode.nodeComment;
			}

			return destNode;
		}

		protected void RegisterBehaviour(NodeBehaviour source, NodeBehaviour dest)
		{
			_DuplicateBehaviours.Add(source, dest);
		}

		public NodeBehaviour GetDestBehaviour(NodeBehaviour sourceBehaviour)
		{
			NodeBehaviour destBehaviour = null;
			if (_DuplicateBehaviours.TryGetValue(sourceBehaviour, out destBehaviour))
			{
				return destBehaviour;
			}
			return null;
		}

		public virtual void OnAfterDuplicate(List<NodeDuplicator> duplicators)
		{
		}
	}
}