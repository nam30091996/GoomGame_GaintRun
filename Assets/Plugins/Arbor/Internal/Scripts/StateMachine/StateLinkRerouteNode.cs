//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// StateLinkのリルートノード
	/// </summary>
#else
	/// <summary>
	/// StateLink's reroute node
	/// </summary>
#endif
	[System.Serializable]
	public sealed class StateLinkRerouteNode : Node
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移先ステート
		/// </summary>
#else
		/// <summary>
		/// Transition destination state
		/// </summary>
#endif
		public StateLink link = new StateLink();

#if ARBOR_DOC_JA
		/// <summary>
		/// ラインの方向
		/// </summary>
#else
		/// <summary>
		/// Direction of line
		/// </summary>
#endif
		public Vector2 direction = Vector2.right;

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
		/// <remarks>
		/// StateLinkRerouteNodeの生成は<see cref="ArborFSMInternal.CreateStateLinkRerouteNode(Vector2)"/>を使用してください。
		/// </remarks>
#else
		/// <summary>
		/// StateLinkRerouteNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
		/// <remarks>
		/// Please use the <see cref = "ArborFSMInternal.CreateStateLinkRerouteNode(Vector2)" /> StateLinkRerouteNode creating.
		/// </remarks>
#endif
		public StateLinkRerouteNode(NodeGraph nodeGraph, int nodeID) : base(nodeGraph, nodeID)
		{
		}
	}
}