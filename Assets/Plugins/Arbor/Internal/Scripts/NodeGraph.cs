//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using Arbor.ObjectPooling;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ノードグラフの基本クラス。
	/// </summary>
#else
	/// <summary>
	/// Base class of node graph.
	/// </summary>
#endif
	[AddComponentMenu("")]
	public abstract class NodeGraph : MonoBehaviour, ISerializationCallbackReceiver, IPoolCallbackReceiver
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// グラフの名前。<br/>
		/// 一つのGameObjectに複数のグラフがある場合の識別や検索に使用する。
		/// </summary>
#else
		/// <summary>
		/// The Graph name.<br/>
		/// It is used for identification and retrieval when there is more than one Graph in one GameObject.
		/// </summary>
#endif
		[FormerlySerializedAs("fsmName")]
		[Internal.DocumentLabel("Name")]
		public string graphName = "";

#if ARBOR_DOC_JA
		/// <summary>
		/// 無限ループのデバッグ設定
		/// </summary>
#else
		/// <summary>
		/// Debug setting of infinite loop
		/// </summary>
#endif
		[Internal.DocumentOrder(1000)]
		public DebugInfiniteLoopSettings debugInfiniteLoopSettings = new DebugInfiniteLoopSettings();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private Object _OwnerBehaviour = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private ParameterContainerInternal _ParameterContainer = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private List<CalculatorNode> _Calculators = new List<CalculatorNode>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private List<CommentNode> _Comments = new List<CommentNode>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private List<GroupNode> _Groups = new List<GroupNode>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		[FormerlySerializedAs("_CalculatorBranchRerouteNodes")]
		private DataBranchRerouteNodeList _DataBranchRerouteNodes = new DataBranchRerouteNodeList();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		[FormerlySerializedAs("_CalculatorBranchies")]
		private List<DataBranch> _DataBranchies = new List<DataBranch>();

		#endregion // Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 親グラフ
		/// </summary>
#else
		/// <summary>
		/// Parent graph
		/// </summary>
#endif
		public NodeGraph parentGraph
		{
			get
			{
				NodeBehaviour owner = ownerBehaviour;
				if (owner != null)
				{
					return owner.nodeGraph;
				}
				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ルートグラフ
		/// </summary>
#else
		/// <summary>
		/// Root graph
		/// </summary>
#endif
		public NodeGraph rootGraph
		{
			get
			{
				NodeGraph current = this;
				while (true)
				{
					NodeGraph parent = current.parentGraph;
					if (parent == null)
					{
						break;
					}
					if (current == parent)
					{
						Debug.LogError("current == this");
						break;
					}
					current = parent;
				}
				return current;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// このグラフの所有者であるNodeBehaviourのObject
		/// </summary>
#else
		/// <summary>
		/// Object of NodeBehaviour own this graph
		/// </summary>
#endif
		public Object ownerBehaviourObject
		{
			get
			{
				return _OwnerBehaviour;
			}
			set
			{
				_OwnerBehaviour = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// このグラフの所有者であるNodeBehaviour
		/// </summary>
#else
		/// <summary>
		/// NodeBehaviour is the owner of this graph
		/// </summary>
#endif
		public NodeBehaviour ownerBehaviour
		{
			get
			{
				return _OwnerBehaviour as NodeBehaviour;
			}
			set
			{
				_OwnerBehaviour = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// このグラフ内に割り当てられているParameterContainer
		/// </summary>
#else
		/// <summary>
		/// The ParameterContainer assigned in this graph
		/// </summary>
#endif
		public ParameterContainerInternal parameterContainer
		{
			get
			{
				return _ParameterContainer;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードグラフの表示名。graphNameが空かnullの場合は"(No Name)"を返す。
		/// </summary>
#else
		/// <summary>
		/// Display name of the node graph. If graphName is empty or null, it returns "(No Name)".
		/// </summary>
#endif
		public string displayGraphName
		{
			get
			{
				if (string.IsNullOrEmpty(graphName))
				{
					return "(No Name)";
				}
				return graphName;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 現在の無限ループデバッグ設定。
		/// </summary>
		/// <remarks>子グラフの場合はルートグラフの無限ループデバッグ設定を返す。</remarks>
#else
		/// <summary>
		/// Current infinite loop debug setting.
		/// </summary>
		/// <remarks>If it is a child graph, return the infinite loop debug setting of the route graph.</remarks>
#endif
		public DebugInfiniteLoopSettings currentDebugInfiniteLoopSettings
		{
			get
			{
				if (rootGraph != null)
				{
					return rootGraph.debugInfiniteLoopSettings;
				}

				return debugInfiniteLoopSettings;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 外部グラフから生成されたフラグ
		/// </summary>
#else
		/// <summary>
		/// Flag instantiated from external graph
		/// </summary>
#endif
		public bool external
		{
			get;
			private set;
		}

		[System.NonSerialized]
		private List<Node> _Nodes = new List<Node>();

#if ARBOR_DOC_JA
		/// <summary>
		/// Nodeの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of Node.
		/// </summary>
#endif
		public int nodeCount
		{
			get
			{
				return _Nodes.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Nodeをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>Node</returns>
#else
		/// <summary>
		/// Get Node from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>Node</returns>
#endif
		public Node GetNodeFromIndex(int index)
		{
			return _Nodes[index];
		}

		private Dictionary<int, Node> _DicNodes;

		private Dictionary<int, Node> dicNodes
		{
			get
			{
				if (_DicNodes == null)
				{
					_DicNodes = new Dictionary<int, Node>();

					int nodeCount = _Nodes.Count;
					for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
					{
						Node node = _Nodes[nodeIndex];
						_DicNodes.Add(node.nodeID, node);
					}
				}

				return _DicNodes;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードIDを指定して<see cref="Arbor.Node" />を取得する。
		/// </summary>
		/// <param name="nodeID">ノードID</param>
		/// <returns>見つかった<see cref="Arbor.Node" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.Node" /> from the node identifier.
		/// </summary>
		/// <param name="nodeID">The node identifier.</param>
		/// <returns>Found <see cref = "Arbor.Node" />. Returns null if not found.</returns>
#endif
		public Node GetNodeFromID(int nodeID)
		{
			Node result = null;
			if (dicNodes.TryGetValue(nodeID, out result))
			{
				if (result.nodeID == nodeID)
				{
					if (_Nodes.Contains(result))
					{
						return result;
					}
					else
					{
						dicNodes.Remove(nodeID);
					}
				}
			}

			int nodeCount = _Nodes.Count;
			for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
			{
				Node node = _Nodes[nodeIndex];
				if (node.nodeID == nodeID)
				{
					dicNodes.Add(node.nodeID, node);
					return node;
				}
			}

			return null;
		}

		internal bool IsUniqueNodeID(int nodeID)
		{
			return nodeID != 0 && GetNodeFromID(nodeID) == null;
		}

		internal int GetUniqueNodeID()
		{
			int count = _Nodes.Count;

			System.Random random = new System.Random(count);

			while (true)
			{
				int nodeID = random.Next();

				if (IsUniqueNodeID(nodeID))
				{
					return nodeID;
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorNodeの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of CalculatorNode.
		/// </summary>
#endif
		public int calculatorCount
		{
			get
			{
				return _Calculators.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorNodeをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>CalculatorNode</returns>
#else
		/// <summary>
		/// Get CalculatorNode from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>CalculatorNode</returns>
#endif
		public CalculatorNode GetCalculatorFromIndex(int index)
		{
			return _Calculators[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorNodeのインデックスを取得
		/// </summary>
		/// <param name="calculator">CalculatorNode</param>
		/// <returns>インデックス。ない場合は-1を返す。</returns>
#else
		/// <summary>
		/// Get CalculatorNode index.
		/// </summary>
		/// <param name="calculator">CalculatorNode</param>
		/// <returns>Index. If not, it returns -1.</returns>
#endif
		public int GetCalculatorIndex(CalculatorNode calculator)
		{
			return _Calculators.IndexOf(calculator);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 全ての<see cref="Arbor.CalculatorNode" />を取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets all of <see cref = "Arbor.CalculatorNode" />.
		/// </summary>
#endif
		[System.Obsolete("use calculatorCount and GetCalculatorFromIndex()")]
		public CalculatorNode[] calculators
		{
			get
			{
				return _Calculators.ToArray();
			}
		}

		private Dictionary<int, CalculatorNode> _DicCalculators;

		private Dictionary<int, CalculatorNode> dicCalculators
		{
			get
			{
				if (_DicCalculators == null)
				{
					_DicCalculators = new Dictionary<int, CalculatorNode>();

					int calculatorCount = _Calculators.Count;
					for (int calculatorIndex = 0; calculatorIndex < calculatorCount; calculatorIndex++)
					{
						CalculatorNode calculator = _Calculators[calculatorIndex];
						_DicCalculators.Add(calculator.nodeID, calculator);
					}
				}

				return _DicCalculators;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ノードIDを指定して<see cref="Arbor.CalculatorNode" />を取得する。
		/// </summary>
		/// <param name="calculatorID">演算ノードID</param>
		/// <returns>見つかった<see cref="Arbor.CalculatorNode" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.CalculatorNode" /> from the calculator identifier.
		/// </summary>
		/// <param name="calculatorID">The calculator identifier.</param>
		/// <returns>Found <see cref = "Arbor.CalculatorNode" />. Returns null if not found.</returns>
#endif
		public CalculatorNode GetCalculatorFromID(int calculatorID)
		{
			CalculatorNode result = null;
			if (dicCalculators.TryGetValue(calculatorID, out result))
			{
				if (result.nodeID == calculatorID)
				{
					return result;
				}
			}

			int calculatorCount = _Calculators.Count;
			for (int calculatorIndex = 0; calculatorIndex < calculatorCount; calculatorIndex++)
			{
				CalculatorNode calculator = _Calculators[calculatorIndex];
				if (calculator.nodeID == calculatorID)
				{
					dicCalculators.Add(calculator.nodeID, calculator);
					return calculator;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ノードを生成。
		/// </summary>
		/// <param name="nodeID">ノード ID</param>
		/// <param name="calculatorType">Calculatorの型</param>
		/// <returns>生成した演算ノード。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create calculator.
		/// </summary>
		/// <param name="nodeID">Node ID</param>
		/// <param name="calculatorType">Calculator type</param>
		/// <returns>The created calculator. If the node ID is not unique, return null without creating it.</returns>
#endif
		public CalculatorNode CreateCalculator(int nodeID, System.Type calculatorType)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateCalculator id(" + nodeID + ") is not unique.");
				return null;
			}

			CalculatorNode calculator = new CalculatorNode(this, nodeID, calculatorType);

			ComponentUtility.RecordObject(this, "Created Calculator");

			_Calculators.Add(calculator);
			RegisterNode(calculator);

			ComponentUtility.SetDirty(this);

			return calculator;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ノードを生成。
		/// </summary>
		/// <param name="calculatorType">Calculatorの型</param>
		/// <returns>生成した演算ノード。</returns>
#else
		/// <summary>
		/// Create calculator.
		/// </summary>
		/// <param name="calculatorType">Calculator type</param>
		/// <returns>The created calculator.</returns>
#endif
		public CalculatorNode CreateCalculator(System.Type calculatorType)
		{
			return CreateCalculator(GetUniqueNodeID(), calculatorType);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Calculatorが属しているCalculatorNodeの取得。
		/// </summary>
		/// <param name="calculator">Calculator</param>
		/// <returns> Calculatorが属しているCalculatorNode。ない場合はnullを返す。</returns>
#else
		/// <summary>
		/// Acquisition of CalculatorNodes Calculator belongs.
		/// </summary>
		/// <param name="calculator">Calculator</param>
		/// <returns>CalculatorNodes Calculator belongs. Return null if not.</returns>
#endif
		public CalculatorNode FindCalculator(Calculator calculator)
		{
			return _Calculators.Find(calculatorNode => calculatorNode.calculator == calculator);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ノードの削除。
		/// </summary>
		/// <param name="calculatorNode">削除する演算ノード。</param>
		/// <returns>削除した場合にtrue</returns>
#else
		/// <summary>
		/// Delete calculator.
		/// </summary>
		/// <param name="calculatorNode">Calculator that you want to delete.</param>
		/// <returns>true if deleted</returns>
#endif
		public bool DeleteCalculator(CalculatorNode calculatorNode)
		{
			Object calculatorObj = calculatorNode.GetObject();

			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");
			_Calculators.Remove(calculatorNode);

			if (_DicCalculators != null)
			{
				_DicCalculators.Remove(calculatorNode.nodeID);
			}

			RemoveNode(calculatorNode);

			DisconnectDataBranch(calculatorObj);

			NodeBehaviour nodeBehaviour = calculatorObj as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				NodeBehaviour.Destroy(nodeBehaviour);
			}
			else if (calculatorObj != null)
			{
				ComponentUtility.Destroy(calculatorObj);
			}

			ComponentUtility.SetDirty(this);

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CommentNodeの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of CommentNode.
		/// </summary>
#endif
		public int commentCount
		{
			get
			{
				return _Comments.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CommentNodeをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>CommentNode</returns>
#else
		/// <summary>
		/// Get CommentNode from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>CommentNode</returns>
#endif
		public CommentNode GetCommentFromIndex(int index)
		{
			return _Comments[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CommentNodeのインデックスを取得
		/// </summary>
		/// <param name="comment">CommentNode</param>
		/// <returns>インデックス。ない場合は-1を返す。</returns>
#else
		/// <summary>
		/// Get CommentNode index.
		/// </summary>
		/// <param name="comment">CommentNode</param>
		/// <returns>Index. If not, it returns -1.</returns>
#endif
		public int GetCommentIndex(CommentNode comment)
		{
			return _Comments.IndexOf(comment);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 全ての<see cref="Arbor.CommentNode" />を取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets all of <see cref = "Arbor.CommentNode" />.
		/// </summary>
#endif
		[System.Obsolete("use commentCount and GetCommentFromIndex()")]
		public CommentNode[] comments
		{
			get
			{
				return _Comments.ToArray();
			}
		}

		private Dictionary<int, CommentNode> _DicComments;

		private Dictionary<int, CommentNode> dicComments
		{
			get
			{
				if (_DicComments == null)
				{
					_DicComments = new Dictionary<int, CommentNode>();

					int commentCount = _Comments.Count;
					for (int commentIndex = 0; commentIndex < commentCount; commentIndex++)
					{
						CommentNode comment = _Comments[commentIndex];
						_DicComments.Add(comment.nodeID, comment);
					}
				}

				return _DicComments;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// コメントIDを指定して<see cref="Arbor.CommentNode" />を取得する。
		/// </summary>
		/// <param name="commentID">コメントID</param>
		/// <returns>見つかった<see cref="Arbor.CommentNode" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.CommentNode" /> from the comment identifier.
		/// </summary>
		/// <param name="commentID">The comment identifier.</param>
		/// <returns>Found <see cref = "Arbor.CommentNode" />. Returns null if not found.</returns>
#endif
		public CommentNode GetCommentFromID(int commentID)
		{
			CommentNode result = null;
			if (dicComments.TryGetValue(commentID, out result))
			{
				if (result.nodeID == commentID)
				{
					return result;
				}
			}

			int commentCount = _Comments.Count;
			for (int commentIndex = 0; commentIndex < commentCount; commentIndex++)
			{
				CommentNode comment = _Comments[commentIndex];
				if (comment.nodeID == commentID)
				{
					dicComments.Add(comment.nodeID, comment);
					return comment;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// コメントを生成。
		/// </summary>
		/// <param name="nodeID">ノードID</param>
		/// <returns>生成したコメント。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create comment.
		/// </summary>
		/// <param name="nodeID">Node ID</param>
		/// <returns>The created comment. If the node ID is not unique, return null without creating it.</returns>
#endif
		public CommentNode CreateComment(int nodeID)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateComment id(" + nodeID + ") is not unique.");
				return null;
			}

			CommentNode comment = new CommentNode(this, nodeID);

			ComponentUtility.RecordObject(this, "Created Comment");

			_Comments.Add(comment);
			RegisterNode(comment);

			ComponentUtility.SetDirty(this);

			return comment;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// コメントを生成。
		/// </summary>
		/// <returns>生成したコメント。</returns>
#else
		/// <summary>
		/// Create comment.
		/// </summary>
		/// <returns>The created comment.</returns>
#endif
		public CommentNode CreateComment()
		{
			return CreateComment(GetUniqueNodeID());
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// コメントの削除。
		/// </summary>
		/// <param name="comment">削除するコメント。</param>
#else
		/// <summary>
		/// Delete comment.
		/// </summary>
		/// <param name="comment">Comment that you want to delete.</param>
#endif
		public void DeleteComment(CommentNode comment)
		{
			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");
			_Comments.Remove(comment);

			if (_DicComments != null)
			{
				_DicComments.Remove(comment.nodeID);
			}

			RemoveNode(comment);

			ComponentUtility.SetDirty(this);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GroupNodeの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of GroupNode.
		/// </summary>
#endif
		public int groupCount
		{
			get
			{
				return _Groups.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GroupNodeをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>GroupNode</returns>
#else
		/// <summary>
		/// Get GroupNode from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>GroupNode</returns>
#endif
		public GroupNode GetGroupFromIndex(int index)
		{
			return _Groups[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// グループを生成。
		/// </summary>
		/// <param name="nodeID">ノード ID</param>
		/// <returns>生成したグループ。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create group.
		/// </summary>
		/// <param name="nodeID">Node ID</param>
		/// <returns>The created group. If the node ID is not unique, return null without creating it.</returns>
#endif
		public GroupNode CreateGroup(int nodeID)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateGroup id(" + nodeID + ") is not unique.");
				return null;
			}

			GroupNode group = new GroupNode(this, nodeID);

			ComponentUtility.RecordObject(this, "Created Group");

			_Groups.Add(group);
			RegisterNode(group);

			ComponentUtility.SetDirty(this);

			return group;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// グループを生成。
		/// </summary>
		/// <returns>生成したグループ。</returns>
#else
		/// <summary>
		/// Create group.
		/// </summary>
		/// <returns>The created group.</returns>
#endif
		public GroupNode CreateGroup()
		{
			return CreateGroup(GetUniqueNodeID());
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// グループの削除。
		/// </summary>
		/// <param name="group">削除するグループ。</param>
#else
		/// <summary>
		/// Delete group.
		/// </summary>
		/// <param name="group">Group that you want to delete.</param>
#endif
		public void DeleteGroup(GroupNode group)
		{
			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");
			_Groups.Remove(group);

			if (_DicGroups != null)
			{
				_DicGroups.Remove(group.nodeID);
			}

			RemoveNode(group);

			ComponentUtility.SetDirty(this);
		}

		private Dictionary<int, GroupNode> _DicGroups;

		private Dictionary<int, GroupNode> dicGroups
		{
			get
			{
				if (_DicGroups == null)
				{
					_DicGroups = new Dictionary<int, GroupNode>();

					int groupCount = _Groups.Count;
					for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
					{
						GroupNode group = _Groups[groupIndex];
						_DicGroups.Add(group.nodeID, group);
					}
				}

				return _DicGroups;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// グループIDを指定して<see cref="Arbor.GroupNode" />を取得する。
		/// </summary>
		/// <param name="groupID">グループID</param>
		/// <returns>見つかった<see cref="Arbor.GroupNode" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.GroupNode" /> from the group identifier.
		/// </summary>
		/// <param name="groupID">The group identifier.</param>
		/// <returns>Found <see cref = "Arbor.GroupNode" />. Returns null if not found.</returns>
#endif
		public GroupNode GetGroupFromID(int groupID)
		{
			GroupNode result = null;
			if (dicGroups.TryGetValue(groupID, out result))
			{
				if (result.nodeID == groupID)
				{
					return result;
				}
			}

			int groupCount = _Groups.Count;
			for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
			{
				GroupNode group = _Groups[groupIndex];
				if (group.nodeID == groupID)
				{
					dicGroups.Add(group.nodeID, group);
					return group;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeリスト
		/// </summary>
#else
		/// <summary>
		/// DataBranchRerouteNode list
		/// </summary>
#endif
		public DataBranchRerouteNodeList dataBranchRerouteNodes
		{
			get
			{
				return _DataBranchRerouteNodes;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeリスト
		/// </summary>
#else
		/// <summary>
		/// DataBranchRerouteNode list
		/// </summary>
#endif
		[System.Obsolete("use dataBranchRerouteNode")]
		public DataBranchRerouteNodeList calculatorBranchRerouteNodes
		{
			get
			{
				return dataBranchRerouteNodes;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeを生成。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <param name="type">値の型</param>
		/// <param name="nodeID">ノード ID</param>
		/// <returns>生成したDataBranchRerouteNode。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create DataBranchRerouteNode.
		/// </summary>
		/// <param name="position">Position of the node</param>
		/// <param name="type">Value type</param>
		/// <param name="nodeID">Node ID</param>
		/// <returns>The created DataBranchRerouteNode. If the node ID is not unique, return null without creating it.</returns>
#endif
		public DataBranchRerouteNode CreateDataBranchRerouteNode(Vector2 position, System.Type type, int nodeID)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateDataBranchRerouteNode id(" + nodeID + ") is not unique.");
				return null;
			}

			DataBranchRerouteNode rerouteNode = new DataBranchRerouteNode(this, nodeID, type);
			rerouteNode.position = new Rect(position.x, position.y, 300, 0);

			ComponentUtility.RecordObject(this, "Created DataBranchRerouteNode");

			_DataBranchRerouteNodes.Add(rerouteNode);
			RegisterNode(rerouteNode);

			ComponentUtility.SetDirty(this);

			return rerouteNode;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeを生成。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <param name="type">値の型</param>
		/// <param name="nodeID">ノード ID</param>
		/// <returns>生成したDataBranchRerouteNode。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create DataBranchRerouteNode.
		/// </summary>
		/// <param name="position">Position of the node</param>
		/// <param name="type">Value type</param>
		/// <param name="nodeID">Node ID</param>
		/// <returns>The created DataBranchRerouteNode. If the node ID is not unique, return null without creating it.</returns>
#endif
		[System.Obsolete("use CreateDataBranchRerouteNode(Vector2 position, System.Type type, int nodeID)")]
		public DataBranchRerouteNode CreateCalculatorBranchRerouteNode(Vector2 position, System.Type type, int nodeID)
		{
			return CreateDataBranchRerouteNode(position, type, nodeID);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeを生成。
		/// </summary>
		/// <returns>生成したDataBranchRerouteNode。</returns>
#else
		/// <summary>
		/// Create DataBranchRerouteNode.
		/// </summary>
		/// <returns>The created DataBranchRerouteNode.</returns>
#endif
		public DataBranchRerouteNode CreateDataBranchRerouteNode(Vector2 position, System.Type type)
		{
			return CreateDataBranchRerouteNode(position, type, GetUniqueNodeID());
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeを生成。
		/// </summary>
		/// <returns>生成したDataBranchRerouteNode。</returns>
#else
		/// <summary>
		/// Create DataBranchRerouteNode.
		/// </summary>
		/// <returns>The created DataBranchRerouteNode.</returns>
#endif
		[System.Obsolete("use CreateDataBranchRerouteNode(Vector2 position,System.Type type)")]
		public DataBranchRerouteNode CreateCalculatorBranchRerouteNode(Vector2 position, System.Type type)
		{
			return CreateDataBranchRerouteNode(position, type);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeの削除。
		/// </summary>
		/// <param name="rerouteNode">削除するDataBranchRerouteNode。</param>
#else
		/// <summary>
		/// Delete DataBranchRerouteNode.
		/// </summary>
		/// <param name="rerouteNode">DataBranchRerouteNode that you want to delete.</param>
#endif
		public void DeleteDataBranchRerouteNode(DataBranchRerouteNode rerouteNode)
		{
			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");

			DataBranch inputBranch = rerouteNode.link.inputSlot.GetBranch();
			if (inputBranch != null)
			{
				DeleteDataBranch(inputBranch);
			}

			int branchCount = rerouteNode.link.outputSlot.branchCount;
			for (int branchIndex = branchCount - 1; branchIndex >= 0; --branchIndex)
			{
				DataBranch branch = rerouteNode.link.outputSlot.GetBranch(branchIndex);
				if (branch != null)
				{
					DeleteDataBranch(branch);
				}
			}

			_DataBranchRerouteNodes.Remove(rerouteNode);
			RemoveNode(rerouteNode);

			ComponentUtility.SetDirty(this);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeの削除。
		/// </summary>
		/// <param name="rerouteNode">削除するDataBranchRerouteNode。</param>
#else
		/// <summary>
		/// Delete DataBranchRerouteNode.
		/// </summary>
		/// <param name="rerouteNode">DataBranchRerouteNode that you want to delete.</param>
#endif
		[System.Obsolete("use DeleteDataBranchRerouteNode(DataBranchRerouteNode rerouteNode)")]
		public void DeleteCalculatorBranchRerouteNode(DataBranchRerouteNode rerouteNode)
		{
			DeleteDataBranchRerouteNode(rerouteNode);
		}

		internal void RegisterNode(Node node)
		{
			if (!_Nodes.Contains(node))
			{
				_Nodes.Add(node);
				if (_DicNodes != null)
				{
					_DicNodes.Add(node.nodeID, node);
				}
			}
		}

		internal void RemoveNode(Node node)
		{
			_Nodes.Remove(node);
			if (_DicNodes != null)
			{
				_DicNodes.Remove(node.nodeID);
			}
		}

		void ClearNodesDictionary()
		{
			if (_DicNodes != null)
			{
				_DicNodes.Clear();
			}
		}

		void ClearNodes()
		{
			ClearNodesDictionary();
			_Nodes.Clear();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードの削除。
		/// </summary>
		/// <param name="node">削除するノード</param>
		/// <returns>削除した場合はtrue、していなければfalseを返す。</returns>
#else
		/// <summary>
		/// Delete node.
		/// </summary>
		/// <param name="node">The node to delete</param>
		/// <returns>Returns true if deleted, false otherwise.</returns>
#endif
		protected abstract bool OnDeleteNode(Node node);

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードが変更された際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when the node is changed.
		/// </summary>
#endif
		public virtual void OnValidateNodes()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードの削除。
		/// </summary>
		/// <param name="node">削除するノード。</param>
		/// <returns>削除した場合にtrue</returns>
#else
		/// <summary>
		/// Delete node.
		/// </summary>
		/// <param name="node">Node that you want to delete.</param>
		/// <returns>true if deleted</returns>
#endif
		public bool DeleteNode(Node node)
		{
			if (!node.IsDeletable())
			{
				return false;
			}

			CalculatorNode calculatorNode = node as CalculatorNode;
			if (calculatorNode != null)
			{
				return DeleteCalculator(calculatorNode);
			}

			CommentNode commentNode = node as CommentNode;
			if (commentNode != null)
			{
				DeleteComment(commentNode);
				return true;
			}

			GroupNode groupNode = node as GroupNode;
			if (groupNode != null)
			{
				DeleteGroup(groupNode);
				return true;
			}

			DataBranchRerouteNode rerouteNode = node as DataBranchRerouteNode;
			if (rerouteNode != null)
			{
				DeleteDataBranchRerouteNode(rerouteNode);
				return true;
			}

			return OnDeleteNode(node);
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private bool _IsEditor = false;

		bool IsEditor()
		{
			NodeGraph current = this;

			while (current != null)
			{
				if (current._IsEditor)
				{
					return true;
				}
				current = current.parentGraph;
			}

			return false;
		}

		bool IsMove()
		{
			if (IsEditor())
			{
				return false;
			}

			int count = nodeCount;
			for (int nodeIndex = 0; nodeIndex < count; nodeIndex++)
			{
				Node node = GetNodeFromIndex(nodeIndex);
				if (node.nodeGraph != this)
				{
					return true;
				}
				else
				{
					INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
					if (behaviours != null)
					{
						int behaviourCount = behaviours.GetNodeBehaviourCount();
						for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
						{
							NodeBehaviour behaviour = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);
							if (behaviour != null && behaviour.nodeGraph != this)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		bool IsMoveContainer()
		{
			if (IsEditor())
			{
				return false;
			}

			if (_ParameterContainer != null && _ParameterContainer.owner != this)
			{
				return true;
			}

			return false;
		}

		void MoveBranch(DataBranch branch)
		{
			int inNodeID = branch.inNodeID;
			Object inBehaviour = branch.inBehaviour;
			int outNodeID = branch.outNodeID;
			Object outBehaviour = branch.outBehaviour;

			int count = nodeCount;
			for (int nodeIndex = 0; nodeIndex < count; nodeIndex++)
			{
				Node node = GetNodeFromIndex(nodeIndex);

				INodeBehaviourContainer nodeBehaviours = node as INodeBehaviourContainer;
				if (nodeBehaviours != null)
				{
					int behaviourCount = nodeBehaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						NodeBehaviour behaviour = nodeBehaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex);

						int slotCount = behaviour.dataSlotFieldCount;
						for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
						{
							DataSlotField slotInfo = behaviour.GetDataSlotField(slotIndex);
							if (slotInfo == null)
							{
								continue;
							}
							DataSlot s = slotInfo.slot;

							IInputSlot inputSlot = s as IInputSlot;
							if (inputSlot != null)
							{
								if (inputSlot.IsConnected(branch))
								{
									inNodeID = node.nodeID;
									inBehaviour = behaviour;
								}
							}

							IOutputSlot outputSlot = s as IOutputSlot;
							if (outputSlot != null)
							{
								if (outputSlot.IsConnected(branch))
								{
									outNodeID = node.nodeID;
									outBehaviour = behaviour;
								}
							}
						}
					}
				}

				DataBranchRerouteNode rerouteNode = node as DataBranchRerouteNode;
				if (rerouteNode != null)
				{
					if (rerouteNode.link.inputSlot.IsConnected(branch))
					{
						inNodeID = node.nodeID;
						inBehaviour = null;
					}

					if (rerouteNode.link.outputSlot.IsConnected(branch))
					{
						outNodeID = node.nodeID;
						outBehaviour = null;
					}
				}
			}

			if (inBehaviour == null)
			{
				inBehaviour = this;
			}
			if (outBehaviour == null)
			{
				outBehaviour = this;
			}
			branch.SetBehaviour(inNodeID, inBehaviour, outNodeID, outBehaviour);

			branch.inputSlot.nodeGraph = branch.outputSlot.nodeGraph = this;

			branch.RebuildSlotField();
		}

		void DestroyUnusedNodeBehaviour(NodeBehaviour behaviour)
		{
			if (behaviour.nodeGraph != this)
			{
				return;
			}

			Node node = FindNodeContainsBehaviour(behaviour);
			if (node == null)
			{
				NodeBehaviour.Destroy(behaviour);
			}
		}

		void DestroyUnusedParameterContainer(ParameterContainerInternal container)
		{
			if (container.owner != this)
			{
				return;
			}

			if (_ParameterContainer != container)
			{
				ComponentUtility.Destroy(container);
			}
		}

		void DestroyUnusedSubComponents()
		{
			if (this == null)
			{
				return;
			}

			foreach (ParameterContainerInternal container in GetComponents<ParameterContainerInternal>())
			{
				DestroyUnusedParameterContainer(container);
			}

			foreach (NodeBehaviour behaviour in GetComponents<NodeBehaviour>())
			{
				DestroyUnusedNodeBehaviour(behaviour);
			}
		}

		private bool _IsValidateDelay = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// MonoBehaviour.OnValidate を参照してください
		/// </summary>
#else
		/// <summary>
		/// See MonoBehaviour.OnValidate.
		/// </summary>
#endif
		protected virtual void OnValidate()
		{
			ClearNodesDictionary();

			if (_DicCalculators != null)
			{
				_DicCalculators.Clear();
			}
			if (_DicComments != null)
			{
				_DicComments.Clear();
			}
			if (_DicGroups != null)
			{
				_DicGroups.Clear();
			}

			if (!_IsValidateDelay)
			{
				_IsValidateDelay = true;
				ComponentUtility.DelayCall(OnValidateDelay);
			}

			ComponentUtility.RefreshNodeGraph(this);
		}

		bool IsValidDataBranch(DataBranch branch)
		{
			if (branch == null)
			{
				return false;
			}

			if (!DataSlotField.IsConnectable(branch.inputSlotField, branch.outputSlotField))
			{
				return false;
			}

			if (CheckLoopDataBranch(branch.inNodeID, branch.inBehaviour, branch.outNodeID, branch.outBehaviour))
			{
				return false;
			}

			return true;
		}

		void RefreshDataSlot(DataSlot slot)
		{
			IInputSlot inputSlot = slot as IInputSlot;
			if (inputSlot != null)
			{
				DataBranch branch = inputSlot.GetBranch();
				if ((branch != null && branch.inputSlot != inputSlot) || !IsValidDataBranch(branch))
				{
					inputSlot.ResetBranch();
				}
			}

			IOutputSlot outputSlot = slot as IOutputSlot;
			if (outputSlot != null)
			{
				for (int i = outputSlot.branchCount - 1; i >= 0; i--)
				{
					DataBranch branch = outputSlot.GetBranch(i);
					if ((branch != null && branch.outputSlot != outputSlot) || !IsValidDataBranch(branch))
					{
						outputSlot.RemoveBranchAt(i);
					}
				}
			}
		}

		void RefreshDataBranchies()
		{
			if (_IsEditor)
			{
				return;
			}

			foreach (var node in _Nodes)
			{
				INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
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

						behaviour.RebuildDataSlotFields();

						int slotCount = behaviour.dataSlotFieldCount;
						for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
						{
							DataSlotField slotField = behaviour.GetDataSlotField(slotIndex);
							if (slotField == null)
							{
								continue;
							}

							DataSlot slot = slotField.slot;

							RefreshDataSlot(slot);
						}
					}
				}
				else
				{
					DataBranchRerouteNode rerouteNode = node as DataBranchRerouteNode;
					if (rerouteNode != null)
					{
						RefreshDataSlot(rerouteNode.link);
					}
				}
			}

			List<DataBranch> deleteBranchies = new List<DataBranch>();

			int branchCount = _DataBranchies.Count;
			for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
			{
				DataBranch branch = _DataBranchies[branchIndex];
				if (branch.branchID == 0)
				{
					Debug.LogError("branch id 0");
				}
				branch.SetDirtySlotField();
				if (!branch.isValidInputSlot || !branch.isValidOutputSlot)
				{
					deleteBranchies.Add(branch);
				}
			}

			foreach (DataBranch branch in deleteBranchies)
			{
				DeleteDataBranch(branch);
			}
		}

		void OnValidateDelay()
		{
			if (this == null || gameObject == null)
			{
				return;
			}

			RefreshDataBranchies();

			if (IsMove())
			{
				for (int count = nodeCount, nodeIndex = 0; nodeIndex < count; nodeIndex++)
				{
					Node node = GetNodeFromIndex(nodeIndex);
					node.ChangeGraph(this);
				}

				int branchCount = dataBranchCount;
				for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
				{
					DataBranch branch = GetDataBranchFromIndex(branchIndex);

					MoveBranch(branch);
				}
			}

			if (IsMoveContainer())
			{
				if (_ParameterContainer != null)
				{
					ComponentUtility.MoveParameterContainer(this);
				}
			}

			DestroyUnusedSubComponents();

			_IsValidateDelay = false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeBehaviourが属しているノードの取得。
		/// </summary>
		/// <param name="behaviour">NodeBehaviour</param>
		/// <returns>NodeBehaviourが属しているノード。ない場合はnullを返す。</returns>
#else
		/// <summary>
		/// Acquisition of nodes NodeBehaviour belongs.
		/// </summary>
		/// <param name="behaviour">NodeBehaviour</param>
		/// <returns>Nodess NodeBehaviour belongs. Return null if not.</returns>
#endif
		public Node FindNodeContainsBehaviour(NodeBehaviour behaviour)
		{
			for (int count = _Nodes.Count, nodeIndex = 0; nodeIndex < count; nodeIndex++)
			{
				Node node = _Nodes[nodeIndex];
				if (node.IsContainsBehaviour(behaviour))
				{
					return node;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of DataBranch.
		/// </summary>
#endif
		public int dataBranchCount
		{
			get
			{
				return _DataBranchies.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of DataBranch.
		/// </summary>
#endif
		[System.Obsolete("use dataBranchCount")]
		public int calculatorBranchCount
		{
			get
			{
				return dataBranchCount;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataBranch</returns>
#else
		/// <summary>
		/// Get DataBranch from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataBranch</returns>
#endif
		public DataBranch GetDataBranchFromIndex(int index)
		{
			return _DataBranchies[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataBranch</returns>
#else
		/// <summary>
		/// Get DataBranch from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataBranch</returns>
#endif
		[System.Obsolete("use GetDataBranchFromIndex(int index)")]
		public DataBranch GetCalculatorBranchFromIndex(int index)
		{
			return GetDataBranchFromIndex(index);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchのインデックスを取得
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>インデックス。ない場合は-1を返す。</returns>
#else
		/// <summary>
		/// Get DataBranch index.
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>Index. If not, it returns -1.</returns>
#endif
		public int GetDataBranchIndex(DataBranch branch)
		{
			return _DataBranchies.IndexOf(branch);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchのインデックスを取得
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>インデックス。ない場合は-1を返す。</returns>
#else
		/// <summary>
		/// Get DataBranch index.
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>Index. If not, it returns -1.</returns>
#endif
		[System.Obsolete("use GetDataBranchIndex(DataBranch)")]
		public int GetCalculatorBranchIndex(DataBranch branch)
		{
			return GetDataBranchIndex(branch);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 全ての<see cref="Arbor.DataBranch" />を取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets all of <see cref = "Arbor.DataBranch" />.
		/// </summary>
#endif
		[System.Obsolete("use dataBranchCount and GetDataBranchFromIndex()")]
		public DataBranch[] calculatorBranchies
		{
			get
			{
				return _DataBranchies.ToArray();
			}
		}

		int GetUniqueBranchID()
		{
			int count = _DataBranchies.Count;

			System.Random random = new System.Random(count);

			while (true)
			{
				int branchID = random.Next();

				if (branchID != 0 && GetDataBranchFromID(branchID) == null)
				{
					return branchID;
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ブランチIDを指定して<see cref="Arbor.DataBranch" />を取得する。
		/// </summary>
		/// <param name="branchID">演算ブランチID</param>
		/// <returns>見つかった<see cref="Arbor.DataBranch" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.DataBranch" /> from the calculator branch identifier.
		/// </summary>
		/// <param name="branchID">The calculator branch identifier.</param>
		/// <returns>Found <see cref = "Arbor.DataBranch" />. Returns null if not found.</returns>
#endif
		public DataBranch GetDataBranchFromID(int branchID)
		{
			if (branchID == 0)
			{
				return null;
			}

			int branchCount = _DataBranchies.Count;
			for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
			{
				DataBranch branch = _DataBranchies[branchIndex];
				if (branch.branchID == branchID)
				{
					return branch;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ブランチIDを指定して<see cref="Arbor.DataBranch" />を取得する。
		/// </summary>
		/// <param name="branchID">演算ブランチID</param>
		/// <returns>見つかった<see cref="Arbor.DataBranch" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.DataBranch" /> from the calculator branch identifier.
		/// </summary>
		/// <param name="branchID">The calculator branch identifier.</param>
		/// <returns>Found <see cref = "Arbor.DataBranch" />. Returns null if not found.</returns>
#endif
		[System.Obsolete("use GetDataBranchFromID(int)")]
		public DataBranch GetCalculatorBranchFromID(int branchID)
		{
			return GetDataBranchFromID(branchID);
		}

		DataBranch CreateDataBranch(int branchID)
		{
			DataBranch branch = new DataBranch();
			branch.branchID = branchID;

			ComponentUtility.RecordObject(this, "Created Branch");

			_DataBranchies.Add(branch);

			ComponentUtility.SetDirty(this);

			return branch;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotの接続
		/// </summary>
		/// <param name="branchID">作成するDataBranchのID</param>
		/// <param name="inputNodeID">入力ノードID</param>
		/// <param name="inputObj">入力オブジェクト</param>
		/// <param name="inputSlot">入力スロット</param>
		/// <param name="outputNodeID">出力ノードID</param>
		/// <param name="outputObj">出力オブジェクト</param>
		/// <param name="outputSlot">出力スロット</param>
		/// <returns>接続したDataBranch</returns>
#else
		/// <summary>
		/// Connect DataSlot.
		/// </summary>
		/// <param name="branchID">ID of the DataBranch to be created</param>
		/// <param name="inputNodeID">Input node ID.</param>
		/// <param name="inputObj">Input object.</param>
		/// <param name="inputSlot">Input slot.</param>
		/// <param name="outputNodeID">Output node ID.</param>
		/// <param name="outputObj">Output object.</param>
		/// <param name="outputSlot">Output slot.</param>
		/// <returns>Connected DataBranch</returns>
#endif
		public DataBranch ConnectDataBranch(int branchID, int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)
		{
			if (GetDataBranchFromID(branchID) != null)
			{
				Debug.LogError("It already exists branchID.");
				return null;
			}

			if (CheckLoopDataBranch(inputNodeID, inputObj, outputNodeID, outputObj))
			{
				Debug.LogError("Calculator node has become an infinite loop.");
				return null;
			}

			if (inputSlot != null && inputSlot.slotType == SlotType.Output)
			{
				throw new System.ArgumentException("inputSlot is not InputSlot or RerouteSlot");
			}

			if (outputSlot != null && outputSlot.slotType == SlotType.Input)
			{
				throw new System.ArgumentException("outputSlot is not OutputSlot or RerouteSlot");
			}

			DataBranch branch = CreateDataBranch(branchID);

			Object setInputObj = inputObj;
			if (setInputObj == null)
			{
				setInputObj = this;
			}
			Object setOutputObj = outputObj;
			if (setOutputObj == null)
			{
				setOutputObj = this;
			}
			branch.SetBehaviour(inputNodeID, setInputObj, outputNodeID, setOutputObj);

			List<Object> records = new List<Object>();
			records.Add(this);
			if (inputObj != null)
			{
				records.Add(inputObj);
			}
			if (outputObj != null)
			{
				records.Add(outputObj);
			}
			ComponentUtility.RecordObjects(records.ToArray(), "Connect Calculator");

			if (outputSlot != null)
			{
				outputSlot.nodeGraph = this;
				IOutputSlot outSlot = outputSlot as IOutputSlot;
				if (outSlot != null)
				{
					outSlot.AddBranch(branch);
				}
			}

			if (inputSlot != null)
			{
				inputSlot.nodeGraph = this;
				IInputSlot inSlot = inputSlot as IInputSlot;
				if (inSlot != null)
				{
					inSlot.SetBranch(branch);
				}
			}

			branch.RebuildSlotField();

			if (inputObj != null)
			{
				ComponentUtility.SetDirty(inputObj);
			}
			if (outputObj != null)
			{
				ComponentUtility.SetDirty(outputObj);
			}
			ComponentUtility.SetDirty(this);

			return branch;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotの接続
		/// </summary>
		/// <param name="branchID">作成するDataBranchのID</param>
		/// <param name="inputNodeID">入力ノードID</param>
		/// <param name="inputObj">入力オブジェクト</param>
		/// <param name="inputSlot">入力スロット</param>
		/// <param name="outputNodeID">出力ノードID</param>
		/// <param name="outputObj">出力オブジェクト</param>
		/// <param name="outputSlot">出力スロット</param>
		/// <returns>接続したDataBranch</returns>
#else
		/// <summary>
		/// Connect DataSlot.
		/// </summary>
		/// <param name="branchID">ID of the DataBranch to be created</param>
		/// <param name="inputNodeID">Input node ID.</param>
		/// <param name="inputObj">Input object.</param>
		/// <param name="inputSlot">Input slot.</param>
		/// <param name="outputNodeID">Output node ID.</param>
		/// <param name="outputObj">Output object.</param>
		/// <param name="outputSlot">Output slot.</param>
		/// <returns>Connected DataBranch</returns>
#endif
		[System.Obsolete("use ConnectDataBranch(int branchID, int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)")]
		public DataBranch ConnectCalculatorBranch(int branchID, int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)
		{
			return ConnectDataBranch(branchID, inputNodeID, inputObj, inputSlot, outputNodeID, outputObj, outputSlot);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotの接続
		/// </summary>
		/// <param name="inputNodeID">入力ノードID</param>
		/// <param name="inputObj">入力オブジェクト</param>
		/// <param name="inputSlot">入力スロット</param>
		/// <param name="outputNodeID">出力ノードID</param>
		/// <param name="outputObj">出力オブジェクト</param>
		/// <param name="outputSlot">出力スロット</param>
		/// <returns>接続したDataBranch</returns>
#else
		/// <summary>
		/// Connect DataSlot.
		/// </summary>
		/// <param name="inputNodeID">Input node ID.</param>
		/// <param name="inputObj">Input object.</param>
		/// <param name="inputSlot">Input slot.</param>
		/// <param name="outputNodeID">Output node ID.</param>
		/// <param name="outputObj">Output object.</param>
		/// <param name="outputSlot">Output slot.</param>
		/// <returns>Connected DataBranch</returns>
#endif
		public DataBranch ConnectDataBranch(int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)
		{
			return ConnectDataBranch(GetUniqueBranchID(), inputNodeID, inputObj, inputSlot, outputNodeID, outputObj, outputSlot);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotの接続
		/// </summary>
		/// <param name="inputNodeID">入力ノードID</param>
		/// <param name="inputObj">入力オブジェクト</param>
		/// <param name="inputSlot">入力スロット</param>
		/// <param name="outputNodeID">出力ノードID</param>
		/// <param name="outputObj">出力オブジェクト</param>
		/// <param name="outputSlot">出力スロット</param>
		/// <returns>接続したDataBranch</returns>
#else
		/// <summary>
		/// Connect DataSlot.
		/// </summary>
		/// <param name="inputNodeID">Input node ID.</param>
		/// <param name="inputObj">Input object.</param>
		/// <param name="inputSlot">Input slot.</param>
		/// <param name="outputNodeID">Output node ID.</param>
		/// <param name="outputObj">Output object.</param>
		/// <param name="outputSlot">Output slot.</param>
		/// <returns>Connected DataBranch</returns>
#endif
		[System.Obsolete("use ConnectDataBranch(int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)")]
		public DataBranch ConnectCalculatorBranch(int inputNodeID, Object inputObj, DataSlot inputSlot, int outputNodeID, Object outputObj, DataSlot outputSlot)
		{
			return ConnectDataBranch(inputNodeID, inputObj, inputSlot, outputNodeID, outputObj, outputSlot);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 内部的に使用するメソッド。特に呼び出す必要はありません。
		/// </summary>
#else
		/// <summary>
		/// Method to be used internally. In particular there is no need to call.
		/// </summary>
#endif
		public void DisconnectDataBranch(Object obj)
		{
			if (obj == null)
			{
				return;
			}

			ComponentUtility.RecordObject(obj, "Disconnect DataBranch");

			List<DataBranch> branchies = new List<DataBranch>();

			for (int branchIndex = 0; branchIndex < _DataBranchies.Count; branchIndex++)
			{
				DataBranch branch = GetDataBranchFromIndex(branchIndex);
				if (branch.inBehaviour == obj || branch.outBehaviour == obj)
				{
					branchies.Add(branch);
				}
			}

			int branchCount = branchies.Count;
			for (int branchIndex = 0; branchIndex < branchCount; branchIndex++)
			{
				DataBranch branch = branchies[branchIndex];
				DeleteDataBranch(branch);
			}

			ComponentUtility.SetDirty(obj);
		}

		internal void Internal_DeleteDataBranch(DataBranch branch)
		{
			_DataBranchies.Remove(branch);

			DataSlot outputSlot = branch.outputSlot;
			IOutputSlot outSlot = outputSlot as IOutputSlot;
			if (outSlot != null)
			{
				outSlot.RemoveBranch(branch);
			}

			DataSlot inputSlot = branch.inputSlot;
			IInputSlot inSlot = inputSlot as IInputSlot;
			if (inSlot != null)
			{
				inSlot.RemoveBranch(branch);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの削除。
		/// </summary>
		/// <param name="branch">削除するDataBranch。</param>
#else
		/// <summary>
		/// Delete DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch that you want to delete.</param>
#endif
		public void DeleteDataBranch(DataBranch branch)
		{
			List<Object> records = new List<Object>();
			records.Add(this);

			Object inBehaviour = branch.inBehaviour;
			if (inBehaviour != null && inBehaviour is MonoBehaviour)
			{
				records.Add(inBehaviour);
			}
			Object outBehaviour = branch.outBehaviour;
			if (outBehaviour != null && outBehaviour is MonoBehaviour)
			{
				records.Add(outBehaviour);
			}
			ComponentUtility.RecordObjects(records.ToArray(), "Delete Branch");

			Internal_DeleteDataBranch(branch);

			ComponentUtility.SetDirty(this);
			if (outBehaviour != null)
			{
				ComponentUtility.SetDirty(outBehaviour);
			}
			if (inBehaviour != null)
			{
				ComponentUtility.SetDirty(inBehaviour);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの削除。
		/// </summary>
		/// <param name="branch">削除するDataBranch。</param>
#else
		/// <summary>
		/// Delete DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch that you want to delete.</param>
#endif
		[System.Obsolete("use DeleteDataBranch(DataBranch)")]
		public void DeleteCalculatorBranch(DataBranch branch)
		{
			DeleteDataBranch(branch);
		}

		bool _IsDelayRefresh = false;

		internal void DelayRefresh()
		{
			if (!_IsDelayRefresh)
			{
				_IsDelayRefresh = true;
				ComponentUtility.DelayCall(OnRefreshDelay);
			}
		}

		void OnRefreshDelay()
		{
			if (this != null)
			{
				Refresh();
			}

			_IsDelayRefresh = false;
		}

		internal void Refresh()
		{
			bool isPlaying = Application.isPlaying && isActiveAndEnabled;

			if (!_IsEditor && rootGraph == this)
			{
				hideFlags &= ~(HideFlags.HideInInspector);
			}

			int nodeCount = _Nodes.Count;
			for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
			{
				INodeBehaviourContainer behaviours = _Nodes[nodeIndex] as INodeBehaviourContainer;
				if (behaviours != null)
				{
					int behaviourCount = behaviours.GetNodeBehaviourCount();
					for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
					{
						Object behaviourObj = behaviours.GetNodeBehaviour<Object>(behaviourIndex);
						NodeBehaviour.RefreshBehaviour(behaviourObj, isPlaying);
					}
				}
			}

			if (!_IsDelayRefresh)
			{
				RefreshDataBranchies();
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		/// <summary>
		/// Register nodes
		/// </summary>
		protected abstract void OnRegisterNodes();

		void RegisterNodes()
		{
			ClearNodes();

			for (int i = 0; i < _Calculators.Count; i++)
			{
				RegisterNode(_Calculators[i]);
			}

			for (int i = 0; i < _Comments.Count; i++)
			{
				RegisterNode(_Comments[i]);
			}

			for (int i = 0; i < _Groups.Count; i++)
			{
				RegisterNode(_Groups[i]);
			}

			for (int i = 0; i < _DataBranchRerouteNodes.count; i++)
			{
				RegisterNode(_DataBranchRerouteNodes[i]);
			}

			OnRegisterNodes();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// デシリアライズ済みかどうかを返す。
		/// </summary>
#else
		/// <summary>
		/// Returns whether or not deserialization has been done.
		/// </summary>
#endif
		public bool isDeserialized
		{
			get;
			private set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// デシリアライズ後のコールバック
		/// </summary>
#else
		/// <summary>
		/// Callback after deserialization
		/// </summary>
#endif
		public event System.Action onAfterDeserialize;

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			RegisterNodes();

			if (onAfterDeserialize != null)
			{
				onAfterDeserialize();
				onAfterDeserialize = null;
			}

			isDeserialized = true;

			if (ComponentUtility.editorProcessor != null)
			{
				DelayRefresh();
			}
		}

		private static NodeGraph GetGraphInternal(NodeGraph[] graphs, System.Type type, string name)
		{
			foreach (NodeGraph graph in graphs)
			{
				if (graph.graphName.Equals(name))
				{
					System.Type classType = graph.GetType();

					if (TypeUtility.IsAssignableFrom(type, classType))
					{
						return graph;
					}
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にあるNodeGraphを名前で取得する。
		/// </summary>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get the NodeGraph that in the scene with the name.
		/// </summary>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static NodeGraph FindGraph(string name)
		{
			return FindGraph(name, typeof(NodeGraph));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にあるNodeGraphを名前で取得する。
		/// </summary>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <param name="type">検索するNodeGraphのType。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get the NodeGraph that in the scene with the name.
		/// </summary>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <param name="type">The type of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static NodeGraph FindGraph(string name, System.Type type)
		{
			return GetGraphInternal(NodeGraph.FindObjectsOfType<NodeGraph>(), type, name);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にあるNodeGraphを名前で取得する。
		/// </summary>
		/// <typeparam name="T">検索するNodeGraphのType。</typeparam>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get the NodeGraph that in the scene with the name.
		/// </summary>
		/// <typeparam name="T">The type of the search NodeGraph.</typeparam>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static T FindGraph<T>(string name) where T : NodeGraph
		{
			return GetGraphInternal(NodeGraph.FindObjectsOfType<NodeGraph>(), typeof(T), name) as T;
		}

		private static System.Array GetGraphsInternal(NodeGraph[] graphs, System.Type type, string name, bool useSearchTypeAsArrayReturnType)
		{
			System.Collections.ArrayList array = new System.Collections.ArrayList();

			foreach (NodeGraph graph in graphs)
			{
				if (graph.graphName.Equals(name))
				{
					System.Type classType = graph.GetType();

					if (TypeUtility.IsAssignableFrom(type, classType))
					{
						array.Add(graph as NodeGraph);
					}
				}
			}

			if (useSearchTypeAsArrayReturnType)
			{
				return array.ToArray(type);
			}
			else
			{
				return array.ToArray();
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にある同一名のNodeGraphを取得する。
		/// </summary>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is in the scene.
		/// </summary>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static NodeGraph[] FindGraphs(string name)
		{
			return FindGraphs(name, typeof(NodeGraph));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にある同一名のNodeGraphを取得する。
		/// </summary>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <param name="type">検索するNodeGraphのType。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is in the scene.
		/// </summary>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <param name="type">The type of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static NodeGraph[] FindGraphs(string name, System.Type type)
		{
			return (NodeGraph[])GetGraphsInternal(NodeGraph.FindObjectsOfType<NodeGraph>(), type, name, false);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// シーン内にある同一名のNodeGraphを取得する。
		/// </summary>
		/// <typeparam name="T">検索するNodeGraphのType。</typeparam>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is in the scene.
		/// </summary>
		/// <typeparam name="T">The type of the search NodeGraph.</typeparam>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static T[] FindGraphs<T>(string name) where T : NodeGraph
		{
			return (T[])GetGraphsInternal(NodeGraph.FindObjectsOfType<NodeGraph>(), typeof(T), name, true);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされているNodeGraphを名前で取得する。
		/// </summary>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get NodeGraph in the name that has been attached to the GameObject.
		/// </summary>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static NodeGraph FindGraph(GameObject gameObject, string name)
		{
			return FindGraph(gameObject, name, typeof(NodeGraph));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされているNodeGraphを名前で取得する。
		/// </summary>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <param name="type">検索するNodeGraphのType。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get NodeGraph in the name that has been attached to the GameObject.
		/// </summary>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <param name="type">The type of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static NodeGraph FindGraph(GameObject gameObject, string name, System.Type type)
		{
			return GetGraphInternal(gameObject.GetComponents<NodeGraph>(), type, name);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされているNodeGraphを名前で取得する。
		/// </summary>
		/// <typeparam name="T">検索するNodeGraphのType。</typeparam>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraph。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Get NodeGraph in the name that has been attached to the GameObject.
		/// </summary>
		/// <typeparam name="T">The type of the search NodeGraph.</typeparam>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Found NodeGraph. Returns null if not found.</returns>
#endif
		public static T FindGraph<T>(GameObject gameObject, string name) where T : NodeGraph
		{
			return (T)GetGraphInternal(gameObject.GetComponents<NodeGraph>(), typeof(T), name);
		}


#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされている同一名のNodeGraphを取得する。
		/// </summary>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is attached to a GameObject.
		/// </summary>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static NodeGraph[] FindGraphs(GameObject gameObject, string name)
		{
			return FindGraphs(gameObject, name, typeof(NodeGraph));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされている同一名のNodeGraphを取得する。
		/// </summary>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <param name="type">検索するNodeGraphのType。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is attached to a GameObject.
		/// </summary>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <param name="type">The type of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static NodeGraph[] FindGraphs(GameObject gameObject, string name, System.Type type)
		{
			return (NodeGraph[])GetGraphsInternal(gameObject.GetComponents<NodeGraph>(), type, name, false);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObjectにアタッチされている同一名のNodeGraphを取得する。
		/// </summary>
		/// <typeparam name="T">検索するNodeGraphのType。</typeparam>
		/// <param name="gameObject">検索したいGameObject。</param>
		/// <param name="name">検索するNodeGraphの名前。</param>
		/// <returns>見つかったNodeGraphの配列。</returns>
#else
		/// <summary>
		/// Get the NodeGraph of the same name that is attached to a GameObject.
		/// </summary>
		/// <typeparam name="T">The type of the search NodeGraph.</typeparam>
		/// <param name="gameObject">Want to search GameObject.</param>
		/// <param name="name">The name of the search NodeGraph.</param>
		/// <returns>Array of found NodeGraph.</returns>
#endif
		public static T[] FindGraphs<T>(GameObject gameObject, string name) where T : NodeGraph
		{
			return (T[])GetGraphsInternal(gameObject.GetComponents<NodeGraph>(), typeof(T), name, true);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchがループしているかをチェックする。
		/// </summary>
		/// <param name="inputNodeID">入力スロット側ノードID</param>
		/// <param name="inputObj">入力スロット側Object</param>
		/// <param name="outputNodeID">出力スロット側ノードID</param>
		/// <param name="outputObj">出力スロット側Object</param>
		/// <returns>ループしている場合はtrueを返す。</returns>
#else
		/// <summary>
		/// Check if DataBranch is looping.
		/// </summary>
		/// <param name="inputNodeID">Input slot side node ID</param>
		/// <param name="inputObj">Input slot side Object</param>
		/// <param name="outputNodeID">Output slot side node ID</param>
		/// <param name="outputObj">Output slot side Object</param>
		/// <returns>Returns true if it is looping.</returns>
#endif
		public bool CheckLoopDataBranch(int inputNodeID, Object inputObj, int outputNodeID, Object outputObj)
		{
			if (outputObj is StateBehaviour)
			{
				return false;
			}

			NodeBehaviour inBehaviour = inputObj as NodeBehaviour;
			NodeBehaviour outBehaviour = outputObj as NodeBehaviour;
			if (inBehaviour != null && outBehaviour != null && inBehaviour == outBehaviour)
			{
				// Same Behaviour
				return true;
			}

			if (inBehaviour == null && outBehaviour == null && inputNodeID == outputNodeID)
			{
				// Same RerouteNode
				return true;
			}

			if (outBehaviour != null)
			{
				int slotCount = outBehaviour.dataSlotFieldCount;
				for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
				{
					DataSlotField slotInfo = outBehaviour.GetDataSlotField(slotIndex);
					if (slotInfo == null)
					{
						continue;
					}
					InputSlotBase s = slotInfo.slot as InputSlotBase;
					if (s != null)
					{
						DataBranch branch = s.branch;
						if (branch != null)
						{
							if (CheckLoopDataBranch(inputNodeID, inputObj, branch.outNodeID, branch.outBehaviour))
							{
								return true;
							}
						}
					}
				}
			}
			else
			{
				DataBranchRerouteNode rerouteNode = _DataBranchRerouteNodes.GetFromID(outputNodeID);
				if (rerouteNode != null)
				{
					DataBranch branch = rerouteNode.link.inputSlot.GetBranch();
					if (branch != null)
					{
						if (CheckLoopDataBranch(inputNodeID, inputObj, branch.outNodeID, branch.outBehaviour))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchがループしているかをチェックする。
		/// </summary>
		/// <param name="inputNodeID">入力スロット側ノードID</param>
		/// <param name="inputObj">入力スロット側Object</param>
		/// <param name="outputNodeID">出力スロット側ノードID</param>
		/// <param name="outputObj">出力スロット側Object</param>
		/// <returns>ループしている場合はtrueを返す。</returns>
#else
		/// <summary>
		/// Check if DataBranch is looping.
		/// </summary>
		/// <param name="inputNodeID">Input slot side node ID</param>
		/// <param name="inputObj">Input slot side Object</param>
		/// <param name="outputNodeID">Output slot side node ID</param>
		/// <param name="outputObj">Output slot side Object</param>
		/// <returns>Returns true if it is looping.</returns>
#endif
		[System.Obsolete("use CheckLoopDataBranch(int inputNodeID, Object inputObj, int outputNodeID, Object outputObj)")]
		public bool CheckLoopCalculatorBranch(int inputNodeID, Object inputObj, int outputNodeID, Object outputObj)
		{
			return CheckLoopDataBranch(inputNodeID, inputObj, outputNodeID, outputObj);
		}

		void DestroySubComponents(Node node)
		{
			INodeBehaviourContainer behaviours = node as INodeBehaviourContainer;
			if (behaviours != null)
			{
				int behaviourCount = behaviours.GetNodeBehaviourCount();
				for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					Object behaviourObj = behaviours.GetNodeBehaviour<Object>(behaviourIndex);
					NodeBehaviour behaviour = behaviourObj as NodeBehaviour;
					if (behaviour != null)
					{
						NodeBehaviour.Destroy(behaviour);
					}
					else if (behaviourObj != null)
					{
						ComponentUtility.Destroy(behaviourObj);
					}
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphのコールバック用デリゲート
		/// </summary>
		/// <param name="nodeGraph">イベントが起きたNodeGraph</param>
#else
		/// <summary>
		/// Delegate for NodeGraph callback
		/// </summary>
		/// <param name="nodeGraph">Event occurred NodeGraph</param>
#endif
		public delegate void NodeGraphCallback(NodeGraph nodeGraph);

#if ARBOR_DOC_JA
		/// <summary>
		/// 破棄される際のコールバック
		/// </summary>
#else
		/// <summary>
		/// Call back when being destroyed
		/// </summary>
#endif
		public event NodeGraphCallback destroyCallback;

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態が変わった際のコールバック
		/// </summary>
#else
		/// <summary>
		/// Call back when the state changes
		/// </summary>
#endif
		public event NodeGraphCallback stateChangedCallback;

		internal void StateChanged()
		{
			if (stateChangedCallback != null)
			{
				stateChangedCallback(this);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はMonoBehaviourが破棄されるときに呼び出される。
		/// </summary>
#else
		/// <summary>
		/// This function is called when MonoBehaivour will be destroyed.
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		public virtual void OnDestroy()
		{
			DestroySubComponents();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 内部的に使用するメソッド。特に呼び出す必要はありません。
		/// </summary>
#else
		/// <summary>
		/// Method to be used internally. In particular there is no need to call.
		/// </summary>
#endif
		public void DestroySubComponents(bool callback = true)
		{
			int nodeCount = _Nodes.Count;
			for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
			{
				Node node = _Nodes[nodeIndex];
				DestroySubComponents(node);
			}

			if (_ParameterContainer != null)
			{
				ComponentUtility.Destroy(_ParameterContainer);
			}

			if (callback && destroyCallback != null)
			{
				destroyCallback(this);
				destroyCallback = null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Resetもしくは生成時のコールバック。
		/// </summary>
#else
		/// <summary>
		/// Reset or create callback.
		/// </summary>
#endif
		protected virtual void OnReset()
		{
		}

		void ResetInternal()
		{
			foreach (NodeBehaviour behaviour in GetComponents<NodeBehaviour>())
			{
				if (behaviour.nodeGraph == this)
				{
					ComponentUtility.Destroy(behaviour);
				}
			}

			OnReset();
		}

		void Initialize()
		{
			if (Application.isPlaying)
			{
				ResetInternal();
			}

			isDeserialized = true;
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void Reset()
		{
			ResetInternal();
		}

		internal void PauseInternal()
		{
			int calculatorCount = _Calculators.Count;
			for (int i = 0; i < calculatorCount; i++)
			{
				CalculatorNode calculatorNode = _Calculators[i];
				Calculator calculator = calculatorNode.calculator;
				if (ComponentUtility.IsValidObject(calculator))
				{
					calculator.CallPauseEvent();
				}
			}
		}

		internal void ResumeInternal()
		{
			int calculatorCount = _Calculators.Count;
			for (int i = 0; i < calculatorCount; i++)
			{
				CalculatorNode calculatorNode = _Calculators[i];
				Calculator calculator = calculatorNode.calculator;
				if (ComponentUtility.IsValidObject(calculator))
				{
					calculator.CallResumeEvent();
				}
			}
		}

		internal void StopInternal()
		{
			int calculatorCount = _Calculators.Count;
			for (int i = 0; i < calculatorCount; i++)
			{
				CalculatorNode calculatorNode = _Calculators[i];
				Calculator calculator = calculatorNode.calculator;
				if (ComponentUtility.IsValidObject(calculator))
				{
					calculator.CallStopEvent();
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphの作成
		/// </summary>
		/// <param name="gameObject">GameObject</param>
		/// <param name="classType">NodeGraphの型</param>
		/// <returns>作成したNodeGraph</returns>
#else
		/// <summary>
		/// Create NodeGraph
		/// </summary>
		/// <param name="gameObject">GameObject</param>
		/// <param name="classType">NodeGraph type</param>
		/// <returns>The created NodeGraph</returns>
#endif
		public static NodeGraph Create(GameObject gameObject, System.Type classType)
		{
			if (!TypeUtility.IsSubclassOf(classType, typeof(NodeGraph)))
			{
				return null;
			}

			NodeGraph nodeGraph = ComponentUtility.AddComponent(gameObject, classType) as NodeGraph;
			nodeGraph.Initialize();

			return nodeGraph;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphの作成
		/// </summary>
		/// <typeparam name="GraphType">NodeGraphの型</typeparam>
		/// <param name="gameObject">GameObject</param>
		/// <returns>作成したNodeGraph</returns>
#else
		/// <summary>
		/// Create NodeGraph
		/// </summary>
		/// <typeparam name="GraphType">NodeGraph type</typeparam>
		/// <param name="gameObject">GameObject</param>
		/// <returns>The created NodeGraph</returns>
#endif
		public static GraphType Create<GraphType>(GameObject gameObject) where GraphType : NodeGraph
		{
			GraphType nodeGraph = ComponentUtility.AddComponent<GraphType>(gameObject);
			nodeGraph.Initialize();

			return nodeGraph;
		}

		static NodeGraph Internal_Instantiate(NodeGraph sourceGraph, bool usePool)
		{
			if (usePool)
			{
				return ObjectPool.Instantiate(sourceGraph) as NodeGraph;
			}
			else
			{
				return Object.Instantiate(sourceGraph) as NodeGraph;
			}
		}

		static T Internal_Instantiate<T>(T sourceGraph, bool usePool) where T : NodeGraph
		{
			if (usePool)
			{
				return ObjectPool.Instantiate<T>(sourceGraph);
			}
			else
			{
				return Object.Instantiate<T>(sourceGraph);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphを生成
		/// </summary>
		/// <param name="sourceGraph">生成元のグラフ</param>
		/// <param name="ownerBehaviour">グラフの所有権を持つNodeBehaviour</param>
		/// <param name="usePool">ObjectPoolを使用してインスタンス化するフラグ。</param>
		/// <returns>生成したグラフ</returns>
#else
		/// <summary>
		/// Instantiate NodeGraph
		/// </summary>
		/// <param name="sourceGraph">Source graph</param>
		/// <param name="ownerBehaviour">NodeBehaviour with chart ownership</param>
		/// <param name="usePool">Flag to instantiate using ObjectPool.</param>
		/// <returns>Instantiated graph</returns>
#endif
		public static NodeGraph Instantiate(NodeGraph sourceGraph, NodeBehaviour ownerBehaviour, bool usePool = false)
		{
			NodeGraph nodeGraph = Internal_Instantiate(sourceGraph, usePool);
			if (nodeGraph != null)
			{
				nodeGraph.external = true;
				if (ownerBehaviour != null)
				{
					nodeGraph.ownerBehaviour = ownerBehaviour;
					nodeGraph.transform.SetParent(ownerBehaviour.transform, false);
				}
			}
			return nodeGraph;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphを生成
		/// </summary>
		/// <typeparam name="GraphType">グラフの型</typeparam>
		/// <param name="sourceGraph">生成元のグラフ</param>
		/// <param name="ownerBehaviour">グラフの所有権を持つNodeBehaviour</param>
		/// <param name="usePool">ObjectPoolを使用してインスタンス化するフラグ。</param>
		/// <returns>生成したグラフ</returns>
#else
		/// <summary>
		/// Instantiate NodeGraph
		/// </summary>
		/// <typeparam name="GraphType">Graph type</typeparam>
		/// <param name="sourceGraph">Source graph</param>
		/// <param name="ownerBehaviour">NodeBehaviour with chart ownership</param>
		/// <param name="usePool">Flag to instantiate using ObjectPool.</param>
		/// <returns>Instantiated graph</returns>
#endif
		public static GraphType Instantiate<GraphType>(GraphType sourceGraph, NodeBehaviour ownerBehaviour, bool usePool = false) where GraphType : NodeGraph
		{
			GraphType nodeGraph = Internal_Instantiate<GraphType>(sourceGraph, usePool);
			if (nodeGraph != null)
			{
				nodeGraph.external = true;
				if (ownerBehaviour != null)
				{
					nodeGraph.ownerBehaviour = ownerBehaviour;
					nodeGraph.transform.SetParent(ownerBehaviour.transform, false);
				}

				nodeGraph.Refresh();
			}
			return nodeGraph;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphの破棄
		/// </summary>
		/// <param name="nodeGraph">NodeGraph</param>
#else
		/// <summary>
		/// Destroy NodeGraph
		/// </summary>
		/// <param name="nodeGraph">NodeGraph</param>
#endif
		public static void Destroy(NodeGraph nodeGraph)
		{
			if (!Application.isPlaying)
			{
				nodeGraph.OnDestroy();
			}
			ComponentUtility.Destroy(nodeGraph);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// グラフを文字列に変換（デバッグ用）。
		/// </summary>
		/// <returns>変換された文字列</returns>
#else
		/// <summary>
		/// Convert graph to string (for debugging).
		/// </summary>
		/// <returns>Converted string</returns>
#endif
		public override string ToString()
		{
			return string.Format("{0} ({1})", graphName, GetType().Name);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 再開する際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when resuming.
		/// </summary>
#endif
		public void OnPoolResume()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// プールに格納された際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when stored in the pool.
		/// </summary>
#endif
		public void OnPoolSleep()
		{
			_OwnerBehaviour = null;
			external = false;
		}
	}
}