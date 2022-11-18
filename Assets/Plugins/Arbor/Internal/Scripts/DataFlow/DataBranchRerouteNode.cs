//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Reflection; // Needed to use Type.GetField with NETFX_CORE

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// DataBranchのリルートノード。
	/// </summary>
#else
	/// <summary>
	/// Reroute node of DataBranch.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class DataBranchRerouteNode : Node, ISerializationCallbackReceiver
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// リルートスロット
		/// </summary>
#else
		/// <summary>
		/// Reroute slot
		/// </summary>
#endif
		public RerouteSlot link = new RerouteSlot();

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
		/// linkのDataSlotField
		/// </summary>
#else
		/// <summary>
		/// link's DataSlotField
		/// </summary>
#endif
		public DataSlotField slotField
		{
			get;
			private set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchRerouteNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
		/// <param name="type">値の型</param>
		/// <remarks>
		/// DataBranchRerouteNodeの生成は<see cref="NodeGraph.CreateDataBranchRerouteNode(Vector2,System.Type)"/>を使用してください。
		/// </remarks>
#else
		/// <summary>
		/// DataBranchRerouteNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
		/// <param name="type">Value type</param>
		/// <remarks>
		/// Please use the <see cref = "NodeGraph.CreateDataBranchRerouteNode(Vector2,System.Type)" /> DataBranchRerouteNode creating.
		/// </remarks>
#endif
		public DataBranchRerouteNode(NodeGraph nodeGraph, int nodeID, System.Type type) : base(nodeGraph, nodeID)
		{
			link.type = type;

			SetupDataBranchSlotField();
		}

		[System.NonSerialized]
		private bool _DelayUpdateDataBranchSlotField = false;

		void SetupDataBranchSlotField()
		{
			FieldInfo linkFieldInfo = MemberCache.GetFieldInfo(this.GetType(), "link");
			slotField = new DataSlotField(link, linkFieldInfo);
		}

		void RegisterUpdateSlotField()
		{
			if (_DelayUpdateDataBranchSlotField)
			{
				return;
			}

			_DelayUpdateDataBranchSlotField = true;
			nodeGraph.onAfterDeserialize += UpdateDataBranchSlotField;
		}

		void UpdateDataBranchSlotField()
		{
			_DelayUpdateDataBranchSlotField = false;
			link.DirtyBranchCache();

			slotField.ClearDataBranchSlotField();
			slotField.SetupDataBranchSlotField();
		}

		#region ISerializationCallbackReceiver

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			SetupDataBranchSlotField();
			RegisterUpdateSlotField();
		}

		#endregion // ISerializationCallbackReceiver
	}
}