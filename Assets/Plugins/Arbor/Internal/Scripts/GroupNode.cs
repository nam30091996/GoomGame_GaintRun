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
	/// グループノードを表すクラス
	/// </summary>
#else
	/// <summary>
	/// Class that represents the group node
	/// </summary>
#endif
	[System.Serializable]
	public sealed class GroupNode : Node, ISerializationCallbackReceiver, ISerializeVersionCallbackReceiver
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 自動整列
		/// </summary>
#else
		/// <summary>
		/// Auto Alignment
		/// </summary>
#endif
		public enum AutoAlignment
		{
#if ARBOR_DOC_JA
			/// <summary>
			/// なし
			/// </summary>
#else
			/// <summary>
			/// None
			/// </summary>
#endif
			None = 0,

#if ARBOR_DOC_JA
			/// <summary>
			/// 垂直方向
			/// </summary>
#else
			/// <summary>
			/// Vertical
			/// </summary>
#endif
			Vertical,

#if ARBOR_DOC_JA
			/// <summary>
			/// 水平方向
			/// </summary>
#else
			/// <summary>
			/// Horizontal
			/// </summary>
#endif
			Horizonal,
		};

		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// グループノードの名前。
		/// </summary>
#else
		/// <summary>
		/// The name of the group node.
		/// </summary>
#endif
		public string name = "New Group";

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードの色
		/// </summary>
#else
		/// <summary>
		/// Node color
		/// </summary>
#endif
		public Color color = Color.white;

#if ARBOR_DOC_JA
		/// <summary>
		/// オートレイアウト
		/// </summary>
#else
		/// <summary>
		/// Auto Layout
		/// </summary>
#endif
		public AutoAlignment autoAlignment = AutoAlignment.None;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private SerializeVersion _SerializeVersion = new SerializeVersion();

		#region old

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("_SerializeVersion")]
		private int _SerializeVersionOld = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("_IsInitialized")]
		private bool _IsInitializedOld = true;

		#endregion // old

		#endregion // Serialize fields

		private const int kCurrentSerializeVersion = 1;

#if ARBOR_DOC_JA
		/// <summary>
		/// GroupNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
		/// <remarks>
		/// グループノードの生成は<see cref="NodeGraph.CreateGroup()"/>を使用してください。
		/// </remarks>
#else
		/// <summary>
		/// GroupNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
		/// <remarks>
		/// Please use the <see cref = "NodeGraph.CreateGroup()" /> group node creating.
		/// </remarks>
#endif
		public GroupNode(NodeGraph nodeGraph, int nodeID) : base(nodeGraph, nodeID)
		{
			// Initialize when calling from script.
			_SerializeVersion.Initialize(this);
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

		void SerializeVer1()
		{
			color = Color.white;
		}

		#region ISerializeVersionCallbackReceiver

		int ISerializeVersionCallbackReceiver.newestVersion
		{
			get
			{
				return kCurrentSerializeVersion;
			}
		}

		void ISerializeVersionCallbackReceiver.OnInitialize()
		{
			_SerializeVersion.version = kCurrentSerializeVersion;
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
			if (_IsInitializedOld)
			{
				_SerializeVersion.version = _SerializeVersionOld;
			}
		}

		#endregion // ISerializeVersionCallbackReceiver

		#region ISerializationCallbackReceiver

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_SerializeVersion.AfterDeserialize();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			_SerializeVersion.BeforeDeserialize();
		}

		#endregion ISerializationCallbackReceiver
	}
}
