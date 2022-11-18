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
	/// DataSlotを接続するクラス。
	/// </summary>
#else
	/// <summary>
	/// Class that connects DataSlot.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class DataBranch : ISerializationCallbackReceiver
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// ブランチのID。
		/// </summary>
#else
		/// <summary>
		/// ID of branch.
		/// </summary>
#endif
		public int branchID;

#if ARBOR_DOC_JA
		/// <summary>
		/// 描画するかどうか。エディタ用。
		/// </summary>
#else
		/// <summary>
		/// Whether to draw. For the editor.
		/// </summary>
#endif
		public bool enabled;

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力側のBehaviour
		/// </summary>
#else
		/// <summary>
		/// Input side Behaviour
		/// </summary>
#endif
		public Object inBehaviour;

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力側のnodeID.
		/// </summary>
#else
		/// <summary>
		/// Input side nodeID
		/// </summary>
#endif
		public int inNodeID;

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力側のBehaviour
		/// </summary>
#else
		/// <summary>
		/// Output side Behaviour
		/// </summary>
#endif
		public Object outBehaviour;

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力側のnodeID
		/// </summary>
#else
		/// <summary>
		/// Output side nodeID
		/// </summary>
#endif
		public int outNodeID;

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続する線のベジェ曲線。エディタ用
		/// </summary>
#else
		/// <summary>
		/// Bezier curve of the line to be connected. For editor
		/// </summary>
#endif
		public Bezier2D lineBezier = new Bezier2D();

		#endregion // Serialize fields

		#region Private fields

		private object _Value;

		[System.NonSerialized]
		internal DataSlotField _OutputSlotField = null;

		[System.NonSerialized]
		internal DataSlotField _InputSlotField = null;

		[System.NonSerialized]
		private DataBranch _OutputRerouteBranch = null;

		[System.NonSerialized]
		private List<DataBranch> _InputRerouteBranchies = null;

		#endregion // Private fields

		#region Properties

		void Calculate()
		{
			Calculator calculator = outBehaviour as Calculator;
			if (calculator != null)
			{
				calculator.Calculate();
			}
			else
			{
				if (_OutputRerouteBranch == null)
				{
					NodeGraph nodeGraph = outBehaviour as NodeGraph;
					if (nodeGraph != null)
					{
						DataBranchRerouteNode rerouteNode = nodeGraph.dataBranchRerouteNodes.GetFromID(outNodeID);
						if (rerouteNode != null)
						{
							DataBranch branch = rerouteNode.link.inputSlot.GetBranch();
							if (branch != null)
							{
								_OutputRerouteBranch = branch;
							}
						}
					}
				}

				if (_OutputRerouteBranch != null)
				{
					_OutputRerouteBranch.Calculate();
				}

			}
		}

		void SetDirty()
		{
			isUsed = true;

			NodeGraph nodeGraph = null;

			Calculator calculator = inBehaviour as Calculator;
			if (calculator != null)
			{
				calculator.SetDirty();

				nodeGraph = calculator.nodeGraph;
			}
			else
			{
				nodeGraph = inBehaviour as NodeGraph;
				if (nodeGraph != null)
				{
					if (_InputRerouteBranchies == null)
					{
						_InputRerouteBranchies = new List<DataBranch>();

						DataBranchRerouteNode rerouteNode = nodeGraph.dataBranchRerouteNodes.GetFromID(inNodeID);
						if (rerouteNode != null)
						{
							int count = rerouteNode.link.outputSlot.branchCount;
							for (int i = 0; i < count; i++)
							{
								DataBranch branch = rerouteNode.link.outputSlot.GetBranch(i);
								_InputRerouteBranchies.Add(branch);
							}
						}
					}

					if (_InputRerouteBranchies != null)
					{
						foreach (var branch in _InputRerouteBranchies)
						{
							branch.value = _Value;
						}
					}
				}
				else
				{
					NodeBehaviour nodeBehaviour = inBehaviour as NodeBehaviour;
					if (nodeBehaviour != null)
					{
						nodeGraph = nodeBehaviour.nodeGraph;
					}
				}
			}

			if (nodeGraph != null)
			{
				nodeGraph.StateChanged();
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を取得設定する。
		/// Calculatorの出力スロットと接続している場合は必要に応じて値を更新してから取得する。
		/// </summary>
#else
		/// <summary>
		/// get set the value.
		/// When connecting to the output slot of Calculator, update it as necessary and obtain it.
		/// </summary>
#endif
		public object value
		{
			get
			{
				Calculate();
				return _Value;
			}
			set
			{
				isUsed = true;

				if (_Value != value)
				{
					_Value = value;

					updatedTime = Time.unscaledTime;

					SetDirty();
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 現在の値を取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the current value.
		/// </summary>
#endif
		public object currentValue
		{
			get
			{
				return _Value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値が使われているかどうかを取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets whether or not a value is used.
		/// </summary>
#endif
		public bool isUsed
		{
			get;
			private set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用
		/// </summary>
#else
		/// <summary>
		/// For editor
		/// </summary>
#endif
		public Color outputSlotColor
		{
			get;
			set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用
		/// </summary>
#else
		/// <summary>
		/// For editor
		/// </summary>
#endif
		public Color inputSlotColor
		{
			get;
			set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を表示するかどうかを取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets whether or not a value is visible.
		/// </summary>
#endif
		public bool showDataValue = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を表示するかどうかを取得する。
		/// </summary>
#else
		/// <summary>
		/// Gets whether or not a value is visible.
		/// </summary>
#endif
		[System.Obsolete("use showDataValue")]
		public bool isVisible
		{
			get
			{
				return showDataValue;
			}
			set
			{
				showDataValue = true;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// valueを更新した timeScale に依存しない時間。
		/// </summary>
#else
		/// <summary>
		/// Time that does not depend on timeScale when value is updated.
		/// </summary>
#endif
		public float updatedTime
		{
			get;
			private set;
		}

		void FindOutputSlot()
		{
			NodeBehaviour nodeBehaviour = outBehaviour as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				int slotCount = nodeBehaviour.dataSlotFieldCount;
				for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
				{
					DataSlotField slotInfo = nodeBehaviour.GetDataSlotField(slotIndex);
					if (slotInfo == null)
					{
						continue;
					}
					OutputSlotBase s = slotInfo.slot as OutputSlotBase;
					if (s != null && s.IsConnected(this))
					{
						_OutputSlotField = slotInfo;
						return;
					}
				}
			}
		}

		void RebuildOutputSlot()
		{
			if (_OutputSlotField == null && outBehaviour != null)
			{
				_OutputSlotField = null;

				NodeBehaviour nodeBehaviour = outBehaviour as NodeBehaviour;
				if (nodeBehaviour != null)
				{
					FindOutputSlot();

					//if (_OutputSlotField == null)
					//{
					//	nodeBehaviour.RebuildDataSlotFields();

					//	FindOutputSlot();
					//}
				}
				else
				{
					NodeGraph nodeGraph = outBehaviour as NodeGraph;
					if (nodeGraph != null)
					{
						DataBranchRerouteNode rerouteNode = nodeGraph.GetNodeFromID(outNodeID) as DataBranchRerouteNode;
						if (rerouteNode != null)
						{
							_OutputSlotField = rerouteNode.slotField;
						}
					}
				}
			}
		}

		void FindInputSlot()
		{
			NodeBehaviour nodeBehaviour = inBehaviour as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				int slotCount = nodeBehaviour.dataSlotFieldCount;
				for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
				{
					DataSlotField slotInfo = nodeBehaviour.GetDataSlotField(slotIndex);
					if (slotInfo == null)
					{
						continue;
					}
					InputSlotBase s = slotInfo.slot as InputSlotBase;
					if (s != null && s.IsConnected(this))
					{
						_InputSlotField = slotInfo;
						break;
					}
				}
			}
		}

		void RebuildInputSlot()
		{
			if (_InputSlotField == null && inBehaviour != null)
			{
				_InputSlotField = null;

				NodeBehaviour nodeBehaviour = inBehaviour as NodeBehaviour;
				if (nodeBehaviour != null)
				{
					FindInputSlot();

					//if (_InputSlotField == null)
					//{
					//	nodeBehaviour.RebuildDataSlotFields();

					//	FindInputSlot();
					//}
				}
				else
				{
					NodeGraph nodeGraph = inBehaviour as NodeGraph;
					if (nodeGraph != null)
					{
						DataBranchRerouteNode rerouteNode = nodeGraph.GetNodeFromID(inNodeID) as DataBranchRerouteNode;
						if (rerouteNode != null)
						{
							_InputSlotField = rerouteNode.slotField;
						}
					}
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力スロットのDataSlotFieldを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get DataSlotField of output slot.
		/// </summary>
#endif
		public DataSlotField outputSlotField
		{
			get
			{
				RebuildOutputSlot();

				if (_OutputSlotField != null)
				{
					return _OutputSlotField;
				}

				return null;
			}
			internal set
			{
				_OutputSlotField = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力スロットを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the output slot.
		/// </summary>
#endif
		public DataSlot outputSlot
		{
			get
			{
				DataSlotField slotField = outputSlotField;
				if (slotField != null)
				{
					return slotField.slot;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力スロットのFieldInfoを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the FieldInfo of the output slot.
		/// </summary>
#endif
		public System.Reflection.FieldInfo outputSlotFieldInfo
		{
			get
			{
				DataSlotField slotField = outputSlotField;
				if (slotField != null)
				{
					return slotField.fieldInfo;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力する型
		/// </summary>
#else
		/// <summary>
		/// Output type.
		/// </summary>
#endif
		public System.Type outputType
		{
			get
			{
				DataSlotField slotField = outputSlotField;
				if (slotField != null)
				{
					return slotField.connectableType;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力スロットが有効であるかを返す。
		/// </summary>
#else
		/// <summary>
		/// Returns whether the output slot is valid.
		/// </summary>
#endif
		public bool isValidOutputSlot
		{
			get
			{
				return (outNodeID != 0 || outBehaviour is MonoBehaviour) && outputSlot != null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力スロットのFieldInfoを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the FieldInfo of the input slot.
		/// </summary>
#endif
		public DataSlotField inputSlotField
		{
			get
			{
				RebuildInputSlot();

				if (_InputSlotField != null)
				{
					return _InputSlotField;
				}

				return null;
			}
			internal set
			{
				_InputSlotField = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力スロットを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the input slot.
		/// </summary>
#endif
		public DataSlot inputSlot
		{
			get
			{
				DataSlotField slotField = inputSlotField;
				if (slotField != null)
				{
					return slotField.slot;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力スロットのFieldInfoを取得する。
		/// </summary>
#else
		/// <summary>
		/// Get the FieldInfo of the input slot.
		/// </summary>
#endif
		public System.Reflection.FieldInfo inputSlotFieldInfo
		{
			get
			{
				DataSlotField slotField = inputSlotField;
				if (slotField != null)
				{
					return slotField.fieldInfo;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力する型。
		/// </summary>
#else
		/// <summary>
		/// Input type.
		/// </summary>
#endif
		public System.Type inputType
		{
			get
			{
				DataSlotField slotField = inputSlotField;
				if (slotField != null)
				{
					return slotField.connectableType;
				}

				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 入力スロットが有効であるかを返す。
		/// </summary>
#else
		/// <summary>
		/// Returns whether the input slot is valid.
		/// </summary>
#endif
		public bool isValidInputSlot
		{
			get
			{
				return (inNodeID != 0 || inBehaviour is MonoBehaviour) && !object.ReferenceEquals(inputSlot, null);
			}
		}

		#endregion // Properties

		#region Public methods

#if ARBOR_DOC_JA
		/// <summary>
		/// Behaviourを変更する。
		/// </summary>
#else
		/// <summary>
		/// Change Behavior.
		/// </summary>
#endif
		public void SetBehaviour(int inNodeID, Object inBehaviour, int outNodeID, Object outBehaviour)
		{
			this.inNodeID = inNodeID;
			this.inBehaviour = inBehaviour;
			this.outNodeID = outNodeID;
			this.outBehaviour = outBehaviour;

			_OutputRerouteBranch = null;
			_InputRerouteBranchies = null;

			Calculator calculator = outBehaviour as Calculator;
			if (calculator != null)
			{
				calculator.SetDirty();
			}
		}

		internal void RebuildSlotField()
		{
			SetDirtySlotField();

			RebuildInputSlot();
			RebuildOutputSlot();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットフィールドがダーティであるとマークする
		/// </summary>
#else
		/// <summary>
		/// Mark slot field as dirty
		/// </summary>
#endif
		public void SetDirtySlotField()
		{
			_InputSlotField = null;
			_OutputSlotField = null;
		}

		#endregion // Public methods

		#region ISerializationCallbackReceiver

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_InputSlotField = null;
			_OutputSlotField = null;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		#endregion // ISerializationCallbackReceiver
	}
}
