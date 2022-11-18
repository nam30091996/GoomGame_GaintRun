//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// コメントを表すクラス
	/// </summary>
#else
	/// <summary>
	/// Class that represents a comment
	/// </summary>
#endif
	[System.Serializable]
	public sealed class CommentNode : Node
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// コメントIDを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the comment identifier.
		/// </summary>
#endif
		[System.Obsolete("use Node.nodeID")]
		public int commentID
		{
			get
			{
				return nodeID;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// コメント文字列。
		/// </summary>
#else
		/// <summary>
		/// Comment string.
		/// </summary>
#endif
		public string comment = string.Empty;

#if ARBOR_DOC_JA
		/// <summary>
		/// CommentNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
		/// <remarks>
		/// コメントノードの生成は<see cref="NodeGraph.CreateComment()"/>を使用してください。
		/// </remarks>
#else
		/// <summary>
		/// CommentNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
		/// <remarks>
		/// Please use the <see cref = "NodeGraph.CreateComment()" /> comment node creating.
		/// </remarks>
#endif
		public CommentNode(NodeGraph nodeGraph, int nodeID) : base(nodeGraph, nodeID)
		{
		}
	}
}
