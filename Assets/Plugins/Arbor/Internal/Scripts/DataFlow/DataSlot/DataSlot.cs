//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// データフローと接続するスロットのインターフェイス
	/// </summary>
#else
	/// <summary>
	/// The interface of the slot that connects to the data flow
	/// </summary>
#endif
	public interface IDataSlot
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// スロットの種類
		/// </summary>
#else
		/// <summary>
		/// Slot type
		/// </summary>
#endif
		SlotType slotType
		{
			get;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットに格納されるデータの型
		/// </summary>
#else
		/// <summary>
		/// The type of data stored in the slot
		/// </summary>
#endif
		System.Type dataType
		{
			get;
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
		void Disconnect();
	}

#if ARBOR_DOC_JA
	/// <summary>
	/// 演算ノードを接続するためのスロット。
	/// </summary>
#else
	/// <summary>
	/// Slot for connecting a calculator node.
	/// </summary>
#endif
	[System.Serializable]
	public abstract class DataSlot : IDataSlot
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// スロットの種類
		/// </summary>
#else
		/// <summary>
		/// Slot type
		/// </summary>
#endif
		public abstract SlotType slotType
		{
			get;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットが属しているステートマシン
		/// </summary>
#else
		/// <summary>
		/// State machine slot belongs
		/// </summary>
#endif
		[FormerlySerializedAs("stateMachine")]
		public NodeGraph nodeGraph;

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットのArborEditor上の位置(Editor Only)
		/// </summary>
#else
		/// <summary>
		/// Position on ArborEditor of slot(Editor Only)
		/// </summary>
#endif
		[System.NonSerialized]
		public Rect position;

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続が変更されたときのコールバックイベント
		/// </summary>
#else
		/// <summary>
		/// Callback event when connection is changed
		/// </summary>
#endif
		public event System.Action<bool> onConnectionChanged;

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットに格納されるデータの型
		/// </summary>
#else
		/// <summary>
		/// The type of data stored in the slot
		/// </summary>
#endif
		public abstract System.Type dataType
		{
			get;
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
		public abstract void Disconnect();

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続状態をクリアする。DataBranchは残るため、コピー＆ペーストなどで接続状態のみ不要になった時に呼ぶ。
		/// </summary>
#else
		/// <summary>
		/// Clear the connection status. Since the DataBranch remains, call it when the connection status is no longer needed by copy and paste.
		/// </summary>
#endif
		public abstract void ClearBranch();

		internal void ConnectionChanged(bool isConnect)
		{
			if (onConnectionChanged != null)
			{
				onConnectionChanged(isConnect);
			}
		}

		internal abstract void DirtyBranchCache();
	}
}
