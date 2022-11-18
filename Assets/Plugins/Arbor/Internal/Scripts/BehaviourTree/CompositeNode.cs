//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor.BehaviourTree
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 子ノードの実行を制御するノード。
	/// </summary>
#else
	/// <summary>
	/// This node controls the execution of child nodes.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class CompositeNode : TreeBehaviourNode
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 親ノードへのリンク
		/// </summary>
#else
		/// <summary>
		/// Link to parent node
		/// </summary>
#endif
		public NodeLinkSlot parentLink = new NodeLinkSlot();

#if ARBOR_DOC_JA
		/// <summary>
		/// 子ノードへのリンク
		/// </summary>
#else
		/// <summary>
		/// Link to child nodes.
		/// </summary>
#endif
		public List<NodeLinkSlot> childrenLink = new List<NodeLinkSlot>();

#if ARBOR_DOC_JA
		/// <summary>
		/// ノード名。
		/// </summary>
#else
		/// <summary>
		/// Node name.
		/// </summary>
#endif
		public string name = "New Composite";

		#endregion // Serialize fields

		private int _CurrentIndex = 0;

#if ARBOR_DOC_JA
		/// <summary>
		/// CompositeNodeの生成は<see cref="BehaviourTreeInternal.CreateComposite(Vector2, System.Type)"/>を使用してください。
		/// </summary>
#else
		/// <summary>
		/// Please use the <see cref = "BehaviourTreeInternal.CreateComposite(Vector2, System.Type)" /> CompositeNode creating.
		/// </summary>
#endif
		public CompositeNode(NodeGraph nodeGraph, int nodeID, System.Type classType)
			: base(nodeGraph, nodeID)
		{
			CreateCompositeBehaviour(classType);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CompositeBehaviourを作成する。エディタで使用する。
		/// </summary>
#else
		/// <summary>
		/// Create a CompositeBehaviour. Use it in the editor.
		/// </summary>
#endif
		public CompositeBehaviour CreateCompositeBehaviour(System.Type classType)
		{
			CompositeBehaviour behaviour = CompositeBehaviour.Create(this, classType);
			SetBehaviour(behaviour);
			return behaviour;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 親へのNodeLinkSlotを持っているかどうか
		/// </summary>
		/// <returns>持っている場合はtrue、なければfalse。</returns>
#else
		/// <summary>
		/// Whether this node has a NodeLinkSlot to parent.
		/// </summary>
		/// <returns>True if it has a NodeLinkSlot to parent, false otherwise.</returns>
#endif
		public override bool HasParentLinkSlot()
		{
			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 親へのNodeLinkSlotを取得。
		/// </summary>
		/// <returns>親へのNodeLinkSlot</returns>
#else
		/// <summary>
		/// Get NodeLinkSlot to parent.
		/// </summary>
		/// <returns>NodeLinkSlot to parent</returns>
#endif
		public override NodeLinkSlot GetParentLinkSlot()
		{
			return parentLink;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 子へのNodeLinkSlotを持っているかどうか
		/// </summary>
		/// <returns>持っている場合はtrue、なければfalse。</returns>
#else
		/// <summary>
		/// Whether this node has a NodeLinkSlot to child.
		/// </summary>
		/// <returns>True if it has a NodeLinkSlot to child, false otherwise.</returns>
#endif
		public override bool HasChildLinkSlot()
		{
			return true;
		}

		internal override void ConnectChildLinkSlot(int branchID)
		{
			NodeLinkSlot childNodeLink = new NodeLinkSlot();
			childNodeLink.branchID = branchID;
			childrenLink.Add(childNodeLink);
		}

		internal override void DisconnectChildLinkSlot(int branchID)
		{
			NodeLinkSlot childNodeLink = null;
			foreach (var slot in childrenLink)
			{
				if (slot.branchID == branchID)
				{
					childNodeLink = slot;
					break;
				}
			}

			if (childNodeLink != null)
			{
				childrenLink.Remove(childNodeLink);
			}
		}

		internal override int OnCalculateChildPriority(int order)
		{
			NodeLinkSlot currentLink = (isActive && childrenLink.Count > 0) ? childrenLink[_CurrentIndex] : null;

			childrenLink.Sort((a, b) =>
			{
				TreeNodeBase a_node = null;
				TreeNodeBase b_node = null;

				if (a.branchID == b.branchID)
				{
					return 0;
				}

				NodeBranch a_branch = behaviourTree.nodeBranchies.GetFromID(a.branchID);
				if (a_branch != null)
				{
					a_node = behaviourTree.GetNodeFromID(a_branch.childNodeID) as TreeNodeBase;
				}

				NodeBranch b_branch = behaviourTree.nodeBranchies.GetFromID(b.branchID);
				if (b_branch != null)
				{
					b_node = behaviourTree.GetNodeFromID(b_branch.childNodeID) as TreeNodeBase;
				}

				if (a_node == null || b_node == null)
				{
					return 0;
				}

				if (a_node.position.x == b_node.position.x)
				{
					return -1;
				}

				return a_node.position.x.CompareTo(b_node.position.x);
			});

			foreach (var slot in childrenLink)
			{
				NodeBranch branch = behaviourTree.nodeBranchies.GetFromID(slot.branchID);
				if (branch != null)
				{
					TreeNodeBase childNode = behaviourTree.GetNodeFromID(branch.childNodeID) as TreeNodeBase;
					if (childNode != null)
					{
						order = childNode.CalculatePriority(order);
					}
				}
			}

			if (currentLink != null)
			{
				_CurrentIndex = childrenLink.IndexOf(currentLink);
			}

			return order;
		}

		private NodeStatus _ChildNodeStatus = NodeStatus.Running;

		void InitializeChildStatus(bool interrupt, bool isRevaluator)
		{
			if (!interrupt || isRevaluator)
			{
				CompositeBehaviour compositeBehaviour = behaviour as CompositeBehaviour;
				if (compositeBehaviour != null)
				{
					_CurrentIndex = compositeBehaviour.GetBeginIndex();
				}
				else
				{
					_CurrentIndex = -1;
				}
			}

			_ChildNodeStatus = NodeStatus.Running;
		}

		internal override bool OnActivate(bool active, bool interrupt, bool isRevaluator)
		{
			if (!base.OnActivate(active, interrupt, isRevaluator))
			{
				return false;
			}

			if (active)
			{
				InitializeChildStatus(interrupt, isRevaluator);
			}

			return true;
		}

		internal override void OnRestart()
		{
			InitializeChildStatus(false, false);

			base.OnRestart();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 実行する際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when executing.
		/// </summary>
#endif
		protected override void OnExecute()
		{
			CompositeBehaviour compositeBehaviour = behaviour as CompositeBehaviour;
			if (compositeBehaviour == null)
			{
				FinishExecute(false);
				return;
			}

			if (0 <= _CurrentIndex && _CurrentIndex < childrenLink.Count && compositeBehaviour.CanExecute(_ChildNodeStatus))
			{
				NodeLinkSlot childLink = childrenLink[_CurrentIndex];
				behaviourTree.Push(childLink);
			}
			else
			{
				FinishExecute(_ChildNodeStatus == NodeStatus.Success);
			}
		}

		internal void OnInterruput(TreeNodeBase node)
		{
			CompositeBehaviour compositeBehaviour = behaviour as CompositeBehaviour;
			if (compositeBehaviour != null)
			{
				_CurrentIndex = compositeBehaviour.GetInterruptIndex(node);
			}
			else
			{
				_CurrentIndex = -1;
			}
		}

		internal override void OnChildExecuted(NodeStatus childStatus)
		{
			CompositeBehaviour compositeBehaviour = behaviour as CompositeBehaviour;
			if (compositeBehaviour != null)
			{
				_CurrentIndex = compositeBehaviour.GetNextIndex(_CurrentIndex);
			}
			else
			{
				_CurrentIndex++;
			}

			_ChildNodeStatus = childStatus;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードの名前を取得
		/// </summary>
		/// <returns>ノードの名前</returns>
#else
		/// <summary>
		/// Get node name.
		/// </summary>
		/// <returns>Node name</returns>
#endif
		public override string GetName()
		{
			return name;
		}
	}
}