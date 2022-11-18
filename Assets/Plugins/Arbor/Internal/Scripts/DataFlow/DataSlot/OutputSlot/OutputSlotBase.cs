//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 出力スロット
	/// </summary>
#else
	/// <summary>
	/// Output slot
	/// </summary>
#endif
	[System.Serializable]
	public abstract class OutputSlotBase : DataSlot, IOutputSlot, ISerializationCallbackReceiver, ISerializeVersionCallbackReceiver
	{
		[System.NonSerialized]
		private bool _DirtyBranchCache = true;

		[System.NonSerialized]
		private List<DataBranch> _BranchCacheList = new List<DataBranch>();

		internal override sealed void DirtyBranchCache()
		{
			_DirtyBranchCache = true;
		}

		#region Serialize Fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続先のブランチのリスト
		/// </summary>
#else
		/// <summary>
		/// List of the destination branch
		/// </summary>
#endif
		public List<int> branchIDs = new List<int>();

		[SerializeField]
		[HideInInspector]
		private SerializeVersion _OutputSlotBase_SerializeVersion = new SerializeVersion();

		#endregion // Serialize Fields

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットの種類
		/// </summary>
#else
		/// <summary>
		/// Slot type
		/// </summary>
#endif
		public override SlotType slotType
		{
			get
			{
				return SlotType.Output;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの個数を取得。
		/// </summary>
		/// <returns>DataBranchの個数</returns>
#else
		/// <summary>
		/// Get count of DataBranch.
		/// </summary>
		/// <returns>Count of DataBranch</returns>
#endif
		public int branchCount
		{
			get
			{
				return branchIDs.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの追加。
		/// </summary>
		/// <param name="branch">DataBranch</param>
#else
		/// <summary>
		/// Addition of DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
#endif
		public void AddBranch(DataBranch branch)
		{
			if (branchIDs.Contains(branch.branchID))
			{
				return;
			}

			branchIDs.Add(branch.branchID);
			DirtyBranchCache();

			ConnectionChanged(true);
		}

		#region ISerializeVersionCallbackReceiver

		private const int kCurrentSerializeVersion = 1;

		int ISerializeVersionCallbackReceiver.newestVersion
		{
			get
			{
				return kCurrentSerializeVersion;
			}
		}

		void ISerializeVersionCallbackReceiver.OnInitialize()
		{
			_OutputSlotBase_SerializeVersion.version = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			List<int> newBranchIDs = new List<int>();
			foreach (var branchID in branchIDs)
			{
				if (newBranchIDs.Contains(branchID))
				{
					continue;
				}

				newBranchIDs.Add(branchID);
			}

			branchIDs = newBranchIDs;
		}

		void ISerializeVersionCallbackReceiver.OnSerialize(int version)
		{
			switch (version)
			{
				case 0:
					SerializeVer1();
					break;
			}
		}

		void ISerializeVersionCallbackReceiver.OnVersioning()
		{
		}

		#endregion // ISerializeVersionCallbackReceiver

		#region ISerializationCallbackReceiver

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_OutputSlotBase_SerializeVersion.AfterDeserialize();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_OutputSlotBase_SerializeVersion.BeforeDeserialize();
		}

		#endregion // ISerializationCallbackReceiver

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotBaseのコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// OutputSlotBase constructor
		/// </summary>
#endif
		public OutputSlotBase() : base()
		{
			// Initialize when calling from script.
			_OutputSlotBase_SerializeVersion.Initialize(this);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの削除。
		/// </summary>
		/// <param name="branch">DataBranch</param>
#else
		/// <summary>
		/// Remove DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
#endif
		public bool RemoveBranch(DataBranch branch)
		{
			if (branchIDs.Remove(branch.branchID))
			{
				if (branchIDs.Count == 0)
				{
					nodeGraph = null;
				}

				DirtyBranchCache();

				ConnectionChanged(false);

				return true;
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの削除。
		/// </summary>
		/// <param name="index">インデックス</param>
#else
		/// <summary>
		/// Remove DataBranch.
		/// </summary>
		/// <param name="index">index</param>
#endif
		public bool RemoveBranchAt(int index)
		{
			if (index < 0 || branchIDs.Count <= index)
			{
				return false;
			}

			branchIDs.RemoveAt(index);

			if (branchIDs.Count == 0)
			{
				nodeGraph = null;
			}

			DirtyBranchCache();

			ConnectionChanged(false);

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの取得。
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataBranch</returns>
#else
		/// <summary>
		/// Get DataBranch.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataBranch</returns>
#endif
		public DataBranch GetBranch(int index)
		{
			if (_DirtyBranchCache)
			{
				_DirtyBranchCache = false;
				_BranchCacheList.Clear();

				if (nodeGraph != null)
				{
					int idCount = branchIDs.Count;
					for (int idIndex = 0; idIndex < idCount; idIndex++)
					{
						int branchID = branchIDs[idIndex];
						_BranchCacheList.Add(nodeGraph.GetDataBranchFromID(branchID));
					}
				}
			}

			return _BranchCacheList[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchと接続しているか判定する。
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>接続している場合にtrue、それ以外はfalse。</returns>
#else
		/// <summary>
		/// It judges whether it is connected with DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>True if connected, false otherwise.</returns>
#endif
		public bool IsConnected(DataBranch branch)
		{
			return branchIDs.Contains(branch.branchID);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続を切断する。
		/// </summary>
#else
		/// <summary>
		/// Disconnect the connection.
		/// </summary>
#endif
		public override void Disconnect()
		{
			if (nodeGraph == null)
			{
				return;
			}

			int branchCount = this.branchCount;
			for (int i = branchCount - 1; i >= 0; --i)
			{
				DataBranch branch = GetBranch(i);
				if (branch != null && branch.outputSlot == this)
				{
					nodeGraph.DeleteDataBranch(branch);
				}
			}
		}

		internal void SetValueInternal(object value)
		{
			if (nodeGraph == null)
			{
				return;
			}

			int idCount = branchIDs.Count;
			for (int idIndex = 0; idIndex < idCount; idIndex++)
			{
				DataBranch branch = GetBranch(idIndex);
				if (branch != null)
				{
					branch.value = value;
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続状態をクリアする。DataBranchは残るため、コピー＆ペーストなどで接続状態のみ不要になった時に呼ぶ。
		/// </summary>
#else
		/// <summary>
		/// Clear the connection status. Since the DataBranch remains, call it when the connection status is no longer needed by copy and paste.
		/// </summary>
#endif
		public override void ClearBranch()
		{
			nodeGraph = null;
			branchIDs.Clear();
			DirtyBranchCache();
		}

		internal void Copy(OutputSlotBase outputSlot)
		{
			nodeGraph = outputSlot.nodeGraph;
			branchIDs = new List<int>(outputSlot.branchIDs);
			DirtyBranchCache();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの追加。
		/// </summary>
		/// <param name="branch">DataBranch</param>
#else
		/// <summary>
		/// Addition of DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
#endif
		[System.Obsolete("use AddBranch()")]
		public void AddOutputBranch(DataBranch branch)
		{
			AddBranch(branch);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの削除。
		/// </summary>
		/// <param name="branch">DataBranch</param>
#else
		/// <summary>
		/// Remove DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
#endif
		[System.Obsolete("use RemoveBranch()")]
		public void RemoveOutputBranch(DataBranch branch)
		{
			RemoveBranch(branch);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの個数を取得。
		/// </summary>
		/// <returns>DataBranchの個数</returns>
#else
		/// <summary>
		/// Get count of DataBranch.
		/// </summary>
		/// <returns>Count of DataBranch</returns>
#endif
		[System.Obsolete("use GetBranchCount()")]
		public int GetOutputBranchCount()
		{
			return branchCount;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchの取得。
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataBranch</returns>
#else
		/// <summary>
		/// Get DataBranch.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataBranch</returns>
#endif
		[System.Obsolete("use GetBranch()")]
		public DataBranch GetOutputBranch(int index)
		{
			return GetBranch(index);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataBranchと接続しているか判定する。
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>接続している場合にtrue、それ以外はfalse。</returns>
#else
		/// <summary>
		/// It judges whether it is connected with DataBranch.
		/// </summary>
		/// <param name="branch">DataBranch</param>
		/// <returns>True if connected, false otherwise.</returns>
#endif
		[System.Obsolete("use IsConnected()")]
		public bool IsConnectedOutput(DataBranch branch)
		{
			return IsConnected(branch);
		}
	}
}