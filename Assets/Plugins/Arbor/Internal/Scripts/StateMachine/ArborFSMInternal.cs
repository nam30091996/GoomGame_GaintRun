﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// <see cref="Arbor.ArborFSM" />の内部クラス。
	/// 実際にGameObjectにアタッチするには<see cref="Arbor.ArborFSM" />を使用する。
	/// </summary>
#else
	/// <summary>
	/// Internal class of <see cref="Arbor.ArborFSM" />.
	/// To actually attach to GameObject is to use the <see cref = "Arbor.ArborFSM" />.
	/// </summary>
#endif
	[AddComponentMenu("")]
	public class ArborFSMInternal : NodeGraph
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 開始時に再生するフラグ。
		/// </summary>
#else
		/// <summary>
		/// Flag to be played at the start.
		/// </summary>
#endif
		public bool playOnStart = true;

#if ARBOR_DOC_JA
		/// <summary>
		/// 更新に関する設定。
		/// </summary>
#else
		/// <summary>
		/// Settings related to updating.
		/// </summary>
#endif
		public UpdateSettings updateSettings = new UpdateSettings();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		private int _StartStateID;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private List<State> _States = new List<State>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private StateLinkRerouteNodeList _StateLinkRerouteNodes = new StateLinkRerouteNodeList();

		#endregion // Serialize fields

		[System.NonSerialized]
		private State _CurrentState = null;

		[System.NonSerialized]
		private State _ReservedState = null;

		[System.NonSerialized]
		private StateLink _ReservedStateLink = null;

		[System.NonSerialized]
		private State _PrevTransitionState = null;

		[System.NonSerialized]
		private State _NextTransitionState = null;

		[System.NonSerialized]
		private TransitionTiming _ReservedStateTransitionTiming;

		private PlayState _PlayState = PlayState.Stopping;

#if ARBOR_DOC_JA
		/// <summary>
		/// 再生状態
		/// </summary>
#else
		/// <summary>
		/// Play state
		/// </summary>
#endif
		public PlayState playState
		{
			get
			{
				return _PlayState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FSMの名前。<br/>
		/// 一つのGameObjectに複数のFSMがある場合の識別や検索に使用する。
		/// </summary>
#else
		/// <summary>
		/// The FSM name.<br/>
		/// It is used for identification and retrieval when there is more than one FSM in one GameObject.
		/// </summary>
#endif
		[System.Obsolete("use graphName")]
		public string fsmName
		{
			get
			{
				return graphName;
			}
			set
			{
				graphName = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 開始ステートのIDを取得する。
		/// </summary>
		/// <value>
		/// 開始ステートID。
		/// </value>
#else
		/// <summary>
		/// Gets the start state identifier.
		/// </summary>
		/// <value>
		/// The start state identifier.
		/// </value>
#endif
		public int startStateID
		{
			get
			{
				return _StartStateID;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 現在の<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <value>
		/// 現在の<see cref="Arbor.State" />。
		/// </value>
#else
		/// <summary>
		/// Gets <see cref="Arbor.State" /> of the current.
		/// </summary>
		/// <value>
		/// <see cref="Arbor.State" /> of the current.
		/// </value>
#endif
		public State currentState
		{
			get
			{
				return _CurrentState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 前のステート
		/// </summary>
#else
		/// <summary>
		/// Prev state
		/// </summary>
#endif
		public State prevTransitionState
		{
			get
			{
				return _PrevTransitionState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 次のステート
		/// </summary>
#else
		/// <summary>
		/// Next state
		/// </summary>
#endif
		public State nextTransitionState
		{
			get
			{
				return _NextTransitionState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移予約された<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <value>
		/// 遷移予約された<see cref="Arbor.State" />。
		/// </value>
#else
		/// <summary>
		/// Gets the transition reserved <see cref="Arbor.State" />.
		/// </summary>
		/// <value>
		/// Transition reserved <see cref="Arbor.State" />.
		/// </value>
#endif
		[System.Obsolete("Use reservedState.")]
		public State nextState
		{
			get
			{
				return reservedState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移予約された<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <value>
		/// 遷移予約された<see cref="Arbor.State" />。
		/// </value>
#else
		/// <summary>
		/// Gets the transition reserved <see cref="Arbor.State" />.
		/// </summary>
		/// <value>
		/// Transition reserved <see cref="Arbor.State" />.
		/// </value>
#endif
		public State reservedState
		{
			get
			{
				return _ReservedState;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移予約された<see cref="Arbor.StateLink" />を取得する。
		/// </summary>
		/// <value>
		/// 遷移予約された<see cref="Arbor.StateLink" />。
		/// </value>
#else
		/// <summary>
		/// Gets the transition reserved <see cref="Arbor.StateLink" />.
		/// </summary>
		/// <value>
		/// Transition reserved <see cref="Arbor.StateLink" />.
		/// </value>
#endif
		public StateLink reservedStateLink
		{
			get
			{
				return _ReservedStateLink;
			}
		}

		internal bool nextImmediateTransition
		{
			get
			{
				return _ReservedState != null && _ReservedStateTransitionTiming == TransitionTiming.Immediate;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Stateの数を取得。
		/// </summary>
#else
		/// <summary>
		///  Get a count of State.
		/// </summary>
#endif
		public int stateCount
		{
			get
			{
				return _States.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeリスト
		/// </summary>
#else
		/// <summary>
		/// StateLinkRerouteNode List
		/// </summary>
#endif
		public StateLinkRerouteNodeList stateLinkRerouteNodes
		{
			get
			{
				return _StateLinkRerouteNodes;
			}
		}

		private List<StateLink> _StateLinkHistory = new List<StateLink>();

#if ARBOR_DOC_JA
		/// <summary>
		/// 指定したStateLinkによって遷移したヒストリーでのインデックスを取得。
		/// </summary>
		/// <param name="stateLink">取得するStateLink</param>
		/// <returns>ヒストリーのインデックス。-1だと対象外。値が大きいほど古い遷移を指す。</returns>
#else
		/// <summary>
		/// Retrieve the index in the history that transited by the specified StateLink.
		/// </summary>
		/// <param name="stateLink">StateLink to acquire</param>
		/// <returns>Index of history. -1 is not eligible. Larger values indicate older transitions.</returns>
#endif
		public int IndexOfStateLinkHistory(StateLink stateLink)
		{
			int index = 0;
			for (int i = _StateLinkHistory.Count - 1; i >= 0; --i)
			{
				StateLink s = _StateLinkHistory[i];
				if (s == stateLink)
				{
					return index;
				}
				if (!s.isReroute)
				{
					index++;
				}
			}

			return -1;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Stateをインデックスから取得
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>State</returns>
#else
		/// <summary>
		/// Get State from index.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>State</returns>
#endif
		public State GetStateFromIndex(int index)
		{
			return _States[index];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Stateのインデックスを取得
		/// </summary>
		/// <param name="state">State</param>
		/// <returns>インデックス。ない場合は-1を返す。</returns>
#else
		/// <summary>
		/// Get State index.
		/// </summary>
		/// <param name="state">State</param>
		/// <returns>Index. If not, it returns -1.</returns>
#endif
		public int GetStateIndex(State state)
		{
			return _States.IndexOf(state);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 全ての<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <value>
		/// <see cref="Arbor.State" />の配列。
		/// </value>
#else
		/// <summary>
		/// Gets all of <see cref="Arbor.State" />.
		/// </summary>
		/// <value>
		/// Array of <see cref="Arbor.State" />.
		/// </value>
#endif
		[System.Obsolete("use stateCount and GetStateFromIndex()")]
		public State[] states
		{
			get
			{
				return _States.ToArray();
			}
		}

		private Dictionary<int, State> _DicStates;

		private Dictionary<int, State> dicStates
		{
			get
			{
				if (_DicStates == null)
				{
					_DicStates = new Dictionary<int, State>();

					int stateCount = _States.Count;
					for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
					{
						State state = _States[stateIndex];
						_DicStates.Add(state.nodeID, state);
					}
				}

				return _DicStates;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートIDを指定して<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <param name="stateID">ステートID</param>
		/// <returns>見つかった<see cref="Arbor.State" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.State" /> from the state identifier.
		/// </summary>
		/// <param name="stateID">The state identifier.</param>
		/// <returns>Found <see cref = "Arbor.State" />. Returns null if not found.</returns>
#endif
		public State GetStateFromID(int stateID)
		{
			State result = null;
			if (dicStates.TryGetValue(stateID, out result))
			{
				if (result.nodeID == stateID)
				{
					return result;
				}
			}

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.nodeID == stateID)
				{
					dicStates.Add(state.nodeID, state);
					return state;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkを指定して<see cref="Arbor.State" />を取得する。
		/// </summary>
		/// <param name="stateLink">StateLink</param>
		/// <returns>見つかった<see cref="Arbor.State" />。見つからなかった場合はnullを返す。</returns>
#else
		/// <summary>
		/// Gets <see cref="Arbor.State" /> from the StateLink.
		/// </summary>
		/// <param name="stateLink">StateLink</param>
		/// <returns>Found <see cref = "Arbor.State" />. Returns null if not found.</returns>
#endif
		public State GetState(StateLink stateLink)
		{
			if (stateLink == null)
			{
				return null;
			}

			int targetID = stateLink.stateID;
			while (targetID != 0)
			{
				Node targetNode = GetNodeFromID(targetID);

				State state = targetNode as State;
				if (state != null)
				{
					return state;
				}

				StateLinkRerouteNode stateLinkNode = targetNode as StateLinkRerouteNode;
				if (stateLinkNode == null)
				{
					return null;
				}

				targetID = stateLinkNode.link.stateID;
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートを生成。
		/// </summary>
		/// <param name="nodeID">ノードID</param>
		/// <param name="resident">常駐するかどうかのフラグ。</param>
		/// <returns>生成したステート。ノードIDが重複している場合は生成せずにnullを返す。</returns>
#else
		/// <summary>
		/// Create state.
		/// </summary>
		/// <param name="nodeID">Node ID</param>
		/// <param name="resident">Resident whether flags.</param>
		/// <returns>The created state. If the node ID is not unique, return null without creating it.</returns>
#endif
		public State CreateState(int nodeID, bool resident)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateState id(" + nodeID + ") is not unique.");
				return null;
			}

			State state = new State(this, nodeID, resident);

			ComponentUtility.RecordObject(this, "Created State");

			_States.Add(state);
			RegisterNode(state);

			if (!resident && _StartStateID == 0)
			{
				_StartStateID = state.nodeID;
			}

			ComponentUtility.SetDirty(this);

			return state;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートを生成。
		/// </summary>
		/// <param name="resident">常駐するかどうかのフラグ。</param>
		/// <returns>生成したステート。</returns>
#else
		/// <summary>
		/// Create state.
		/// </summary>
		/// <param name="resident">Resident whether flags.</param>
		/// <returns>The created state.</returns>
#endif
		public State CreateState(bool resident)
		{
			return CreateState(GetUniqueNodeID(), resident);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートを生成。
		/// </summary>
		/// <returns>生成したステート。</returns>
#else
		/// <summary>
		/// Create state.
		/// </summary>
		/// <returns>The created state.</returns>
#endif
		public State CreateState()
		{
			return CreateState(false);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートを名前で検索。
		/// </summary>
		/// <param name="stateName">検索するステートの名前。</param>
		/// <returns>見つかったステート。ない場合はnullを返す。</returns>
#else
		/// <summary>
		/// Search state by name.
		/// </summary>
		/// <param name="stateName">The name of the search state.</param>
		/// <returns>Found state. Return null if not.</returns>
#endif
		public State FindState(string stateName)
		{
			return _States.Find(state => state.name == stateName);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートを名前で検索。
		/// </summary>
		/// <param name="stateName">検索するステートの名前。</param>
		/// <returns>見つかったステートの配列。</returns>
#else
		/// <summary>
		/// Search state by name.
		/// </summary>
		/// <param name="stateName">The name of the search state.</param>
		/// <returns>Array of found state.</returns>
#endif
		public State[] FindStates(string stateName)
		{
			return _States.FindAll(state => state.name == stateName).ToArray();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateBehaviourが属しているステートの取得。
		/// </summary>
		/// <param name="behaviour">StateBehaviour</param>
		/// <returns>StateBehaviourが属しているステート。ない場合はnullを返す。</returns>
#else
		/// <summary>
		/// Acquisition of states StateBehaviour belongs.
		/// </summary>
		/// <param name="behaviour">StateBehaviour</param>
		/// <returns>States StateBehaviour belongs. Return null if not.</returns>
#endif
		public State FindStateContainsBehaviour(StateBehaviour behaviour)
		{
			return _States.Find(state => state.Contains(behaviour));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートの削除。
		/// </summary>
		/// <param name="state">削除するステート。</param>
		/// <returns>削除した場合にtrue</returns>
#else
		/// <summary>
		/// Delete state.
		/// </summary>
		/// <param name="state">State that you want to delete.</param>
		/// <returns>true if deleted</returns>
#endif
		public bool DeleteState(State state)
		{
			int stateID = state.nodeID;

			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");
			_States.Remove(state);
			if (_DicStates != null)
			{
				_DicStates.Remove(state.nodeID);
			}

			RemoveNode(state);

			if (_StartStateID == stateID)
			{
				ComponentUtility.RecordObject(this, "Delete Nodes");
				_StartStateID = 0;
				ComponentUtility.SetDirty(this);
			}

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State otherState = _States[stateIndex];
				if (otherState != state)
				{
					otherState.DisconnectState(stateID);
				}
			}

			state.DestroyBehaviours();

			ComponentUtility.RecordObject(this, "Delete Nodes");

			int stateLinkCount = _StateLinkRerouteNodes.count;
			for (int stateLinkIndex = 0; stateLinkIndex < stateLinkCount; stateLinkIndex++)
			{
				StateLinkRerouteNode otherStateLink = _StateLinkRerouteNodes[stateLinkIndex];
				if (otherStateLink.link.stateID == stateID)
				{
					otherStateLink.link.stateID = 0;
				}
			}

			ComponentUtility.SetDirty(this);

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeを作成する。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <param name="nodeID">ノードID</param>
		/// <param name="lineColor">ライン色</param>
		/// <returns>作成したStateLinkRerouteNode</returns>
#else
		/// <summary>
		/// Create StateLinkRerouteNode.
		/// </summary>
		/// <param name="position">Node position</param>
		/// <param name="nodeID">Node ID</param>
		/// <param name="lineColor">Line Color</param>
		/// <returns>The created StateLinkRerouteNode</returns>
#endif
		public StateLinkRerouteNode CreateStateLinkRerouteNode(Vector2 position, int nodeID, Color lineColor)
		{
			if (!IsUniqueNodeID(nodeID))
			{
				Debug.LogWarning("CreateStateLinkRerouteNode id(" + nodeID + ") is not unique.");
				return null;
			}

			StateLinkRerouteNode stateLinkRerouteNode = new StateLinkRerouteNode(this, nodeID);
			stateLinkRerouteNode.position = new Rect(position.x, position.y, 300, 0);
			stateLinkRerouteNode.link.lineColor = lineColor;
			stateLinkRerouteNode.link.lineColorChanged = true;

			ComponentUtility.RecordObject(this, "Created StateLinkRerouteNode");

			_StateLinkRerouteNodes.Add(stateLinkRerouteNode);
			RegisterNode(stateLinkRerouteNode);

			ComponentUtility.SetDirty(this);

			return stateLinkRerouteNode;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeを作成する。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <param name="nodeID">ノードID</param>
		/// <returns>作成したStateLinkRerouteNode</returns>
#else
		/// <summary>
		/// Create StateLinkRerouteNode.
		/// </summary>
		/// <param name="position">Node position</param>
		/// <param name="nodeID">Node ID</param>
		/// <returns>The created StateLinkRerouteNode</returns>
#endif
		public StateLinkRerouteNode CreateStateLinkRerouteNode(Vector2 position, int nodeID)
		{
			return CreateStateLinkRerouteNode(position, nodeID, Color.white);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeを作成する。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <param name="lineColor">ライン色</param>
		/// <returns>作成したStateLinkRerouteNode</returns>
#else
		/// <summary>
		/// Create StateLinkRerouteNode.
		/// </summary>
		/// <param name="position">Node position</param>
		/// <param name="lineColor">Line Color</param>
		/// <returns>The created StateLinkRerouteNode</returns>
#endif
		public StateLinkRerouteNode CreateStateLinkRerouteNode(Vector2 position, Color lineColor)
		{
			return CreateStateLinkRerouteNode(position, GetUniqueNodeID(), lineColor);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkRerouteNodeを作成する。
		/// </summary>
		/// <param name="position">ノードの位置</param>
		/// <returns>作成したStateLinkRerouteNode</returns>
#else
		/// <summary>
		/// Create StateLinkRerouteNode.
		/// </summary>
		/// <param name="position">Node position</param>
		/// <returns>The created StateLinkRerouteNode</returns>
#endif
		public StateLinkRerouteNode CreateStateLinkRerouteNode(Vector2 position)
		{
			return CreateStateLinkRerouteNode(position, Color.white);
		}

		bool DeleteStateLinkRerouteNode(StateLinkRerouteNode stateLinkRerouteNode)
		{
			int nodeID = stateLinkRerouteNode.nodeID;

			ComponentUtility.RegisterCompleteObjectUndo(this, "Delete Nodes");

			ComponentUtility.RecordObject(this, "Delete Nodes");
			_StateLinkRerouteNodes.Remove(stateLinkRerouteNode);
			RemoveNode(stateLinkRerouteNode);

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State otherState = _States[stateIndex];
				otherState.DisconnectState(nodeID);
			}

			ComponentUtility.RecordObject(this, "Delete Nodes");

			int stateLinkCount = _StateLinkRerouteNodes.count;
			for (int stateLinkIndex = 0; stateLinkIndex < stateLinkCount; stateLinkIndex++)
			{
				StateLinkRerouteNode otherStateLink = _StateLinkRerouteNodes[stateLinkIndex];
				if (stateLinkRerouteNode != otherStateLink && otherStateLink.link.stateID == nodeID)
				{
					otherStateLink.link.stateID = 0;
				}
			}

			ComponentUtility.SetDirty(this);

			return true;
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
		protected override bool OnDeleteNode(Node node)
		{
			State state = node as State;
			if (state != null)
			{
				return DeleteState(state);
			}

			StateLinkRerouteNode stateLinkNode = node as StateLinkRerouteNode;
			if (stateLinkNode != null)
			{
				return DeleteStateLinkRerouteNode(stateLinkNode);
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// MonoBehaviour.OnValidate を参照してください
		/// </summary>
#else
		/// <summary>
		/// See MonoBehaviour.OnValidate.
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected override void OnValidate()
		{
			if (_DicStates != null)
			{
				_DicStates.Clear();
			}

			base.OnValidate();
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
		protected sealed override void OnReset()
		{
			graphName = "New StateMachine";
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void Start()
		{
			Refresh();

			if (playOnStart)
			{
				Play();
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートマシンの再生を開始。
		/// </summary>
#else
		/// <summary>
		/// Start playing the state machine.
		/// </summary>
#endif
		public void Play()
		{
			if (!isActiveAndEnabled)
			{
				Debug.LogWarning("Only active can be played.");
				return;
			}

			if (_PlayState != PlayState.Stopping)
			{
				return;
			}

#if ARBOR_TRIAL
			if( Trial.TrialGUI.IsTrialLimitTime() )
			{
				Trial.TrialGUI.DisplayLimitLog();
				return;
			}
#endif

			_PlayState = PlayState.Playing;
			updateSettings.ClearTime();

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.Activate(true);
				}
			}

			BeginTransitionLoop();

			if (!NextState())
			{
				State nextState = GetStateFromID(_StartStateID);
				ChangeState(nextState);
			}

			EndTransitionLoop();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートマシンの再生を停止。
		/// </summary>
#else
		/// <summary>
		/// Stopping playback of the state machine.
		/// </summary>
#endif
		public void Stop()
		{
			if (_PlayState == PlayState.Stopping)
			{
				return;
			}

			_ReservedState = null;

			_InStateEvent = true;

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.Stop();
				}
			}

			if (!nextImmediateTransition)
			{
				// don't immediate transition in resident state.
				if (_CurrentState != null)
				{
					_CurrentState.Stop();
				}
			}

			if (nextImmediateTransition)
			{
				// immediate transition in resident state or currnet state.
				NextState();
			}

			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.Activate(false);
				}
			}

			if (_CurrentState != null)
			{
				_CurrentState.Activate(false);
			}

			_InStateEvent = false;

			StopInternal();

			ClearTransitionCount();

			_PlayState = PlayState.Stopping;

			_CurrentState = null;
			_NextTransitionState = null;
			_ReservedState = null;
			_ReservedStateLink = null;
			_PrevTransitionState = null;

			StateChanged();
		}

		void DoPause()
		{
			PauseInternal();

			if (_CurrentState != null)
			{
				_CurrentState.Pause();
			}

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.Pause();
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートマシンの再生を一時停止。
		/// </summary>
#else
		/// <summary>
		/// Pause playback of the state machine.
		/// </summary>
#endif
		public void Pause()
		{
			if (!isActiveAndEnabled || _PlayState != PlayState.Playing)
			{
				return;
			}

			_PlayState = PlayState.Pausing;

			DoPause();
		}

		void DoResume()
		{
			ResumeInternal();

			if (_CurrentState != null)
			{
				_CurrentState.Resume();
			}

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.Resume();
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ステートマシンの再生を再開。
		/// </summary>
#else
		/// <summary>
		/// Resume playing state machine.
		/// </summary>
#endif
		public void Resume()
		{
			if (!isActiveAndEnabled || _PlayState != PlayState.Pausing)
			{
				return;
			}

			_PlayState = PlayState.Playing;

			DoResume();
		}

		void ClearTransitionCount()
		{
			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				state.transitionCount = 0;

				int behaviourCount = state.behaviourCount;
				for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
				{
					StateBehaviour behaviour = state.GetBehaviourFromIndex(behaviourIndex);
					if (behaviour != null)
					{
						behaviour.ClearTransitionCount();
					}
				}
			}

			int rerouteCount = _StateLinkRerouteNodes.count;
			for (int rerouteIndex = 0; rerouteIndex < rerouteCount; rerouteIndex++)
			{
				StateLinkRerouteNode rerouteNode = _StateLinkRerouteNodes[rerouteIndex];
				rerouteNode.link.transitionCount = 0;
			}

			_StateLinkHistory.Clear();
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void OnEnable()
		{
			if (_PlayState != PlayState.InactivePausing)
			{
				return;
			}

			updateSettings.ClearTime();

			_PlayState = PlayState.Playing;

			DoResume();

			if (_PlayState == PlayState.Playing)
			{
				BeginTransitionLoop();

				if (!NextState())
				{
					if (_CurrentState == null)
					{
						State nextState = GetStateFromID(_StartStateID);
						ChangeState(nextState);
					}
				}

				EndTransitionLoop();
			}
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void OnDisable()
		{
			if (_PlayState != PlayState.Playing)
			{
				return;
			}

			_PlayState = PlayState.InactivePausing;

			DoPause();
		}

		bool NextState()
		{
			if (_ReservedState != null)
			{
				if (!_IsBreaked)
				{
					State nextState = _ReservedState;
					_ReservedState = null;

					ChangeState(nextState);
				}

				return true;
			}

			return false;
		}

		private bool _DidImmediateTransition = false;

		void UpdateStateInternal()
		{
			_DidImmediateTransition = false;

			_InStateEvent = true;

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.UpdateBehaviours();
				}
			}

			if (_CurrentState != null)
			{
				_CurrentState.UpdateBehaviours();
			}

			_InStateEvent = false;

			BeginTransitionLoop();

			if (nextImmediateTransition)
			{
				NextState();
				_DidImmediateTransition = true;
			}

			EndTransitionLoop();
		}

		void LateUpdateStateInternal()
		{
			if (_DidImmediateTransition)
			{
				_DidImmediateTransition = false;
				return;
			}

			_InStateEvent = true;

			int stateCount = _States.Count;
			for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
			{
				State state = _States[stateIndex];
				if (state.resident)
				{
					state.LateUpdateBehaviours();
				}
			}

			if (_CurrentState != null)
			{
				_CurrentState.LateUpdateBehaviours();
			}

			_InStateEvent = false;

			BeginTransitionLoop();

			NextState();

			EndTransitionLoop();

			_ExecuteLateUpdate = false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Updateを実行する。
		/// UpdateSettings.typeがManualの場合に任意のタイミングでこのメソッドを呼んでください。
		/// </summary>
		/// <param name="autoExecuteLateUpdate">自動的にExecuteLateUpdateを行うフラグ</param>
#else
		/// <summary>
		/// Perform an update.
		/// Please call this method at any timing when UpdateSettings.type is Manual.
		/// </summary>
		/// <param name="autoExecuteLateUpdate">Flag for ExecuteLateUpdate automatically</param>
#endif
		public void ExecuteUpdate(bool autoExecuteLateUpdate = false)
		{
			if (_PlayState != PlayState.Playing || updateSettings.type != UpdateType.Manual)
			{
				return;
			}

			_ExecuteLateUpdate = autoExecuteLateUpdate;
			UpdateStateInternal();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// LateUpdateを実行する。
		/// UpdateSettings.typeがManualの場合に任意のタイミングでこのメソッドを呼んでください。
		/// </summary>
#else
		/// <summary>
		/// Perform an LateUpdate.
		/// Please call this method at any timing when UpdateSettings.type is Manual.
		/// </summary>
#endif
		public void ExecuteLateUpdate()
		{
			if (_PlayState != PlayState.Playing || updateSettings.type != UpdateType.Manual)
			{
				return;
			}

			LateUpdateStateInternal();
		}

		private bool _ExecuteLateUpdate = false;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void Update()
		{
			if (_PlayState != PlayState.Playing)
			{
				return;
			}

			if (updateSettings.isUpdatableOnUpdate)
			{
				UpdateStateInternal();
				_ExecuteLateUpdate = true;
			}
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void LateUpdate()
		{
			if (_PlayState != PlayState.Playing)
			{
				return;
			}

			if (_ExecuteLateUpdate)
			{
				LateUpdateStateInternal();
			}

			_IsBreaked = false;

#if ARBOR_TRIAL
			if( !Application.isEditor )
			{
				Trial.TrialGUI.UpdateIfDirty();
				if( Trial.TrialGUI.IsTrialLimitTime() )
				{
					Stop();
					Trial.TrialGUI.DisplayLimitLog();
					return;
				}
			}
#endif
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// SendTriggerFlagsの全てが有効なフラグ
		/// </summary>
#else
		/// <summary>
		/// All flags in SendTriggerFlags are valid
		/// </summary>
#endif
		public const SendTriggerFlags allSendTrigger = (SendTriggerFlags)(-1);

		sealed class Trigger
		{
			public string message;
			public SendTriggerFlags flags = allSendTrigger;
		}

		private bool _InStateEvent = false;
		private bool _ChangingState = false;
		private bool _IsBreaked = false;
		private List<Trigger> _Triggers = new List<Trigger>();

		private bool _IsInfiniteLoopWarning = false;
		private int _TransitionCounter = 0;

		void BeginTransitionLoop()
		{
			_IsInfiniteLoopWarning = false;
			_TransitionCounter = 0;
		}

		bool UpdateTransitionLoop()
		{
			if (_IsInfiniteLoopWarning)
			{
				return false;
			}

			if (_TransitionCounter >= currentDebugInfiniteLoopSettings.maxLoopCount)
			{
				_IsInfiniteLoopWarning = true;
				return false;
			}
			else
			{
				_TransitionCounter++;
			}

			return true;
		}

		void EndTransitionLoop()
		{
			if (_IsInfiniteLoopWarning)
			{
				if (currentDebugInfiniteLoopSettings.enableLogging)
				{
					Debug.LogWarning("Over " + currentDebugInfiniteLoopSettings.maxLoopCount + " transitions per frame. Please check the infinite loop of " + ToString(), this);
				}

				if (currentDebugInfiniteLoopSettings.enableBreak)
				{
					Debug.Break();
				}
			}
		}

		void ChangeState(State nextState)
		{
			while (nextState != null)
			{
				_ChangingState = true;
				_InStateEvent = true;

				_NextTransitionState = nextState;

				if (_CurrentState != null)
				{
					_CurrentState.Activate(false);
					_PrevTransitionState = _CurrentState;
				}

				_NextTransitionState = null;

				if (!isActiveAndEnabled)
				{
					_CurrentState = null;
					_ReservedState = nextState;

					StateChanged();

					_ChangingState = false;
					_InStateEvent = false;

					return;
				}

				if (_ReservedStateLink != null)
				{
					StateLink nextStateLink = _ReservedStateLink;
					while (nextStateLink != null)
					{
						if (nextStateLink.transitionCount < uint.MaxValue)
						{
							nextStateLink.transitionCount++;
						}
						if (_StateLinkHistory.Contains(nextStateLink))
						{
							_StateLinkHistory.Remove(nextStateLink);
						}
						_StateLinkHistory.Add(nextStateLink);

						int stateLinkCount = 0;
						for (int i = 0, count = _StateLinkHistory.Count; i < count; ++i)
						{
							StateLink link = _StateLinkHistory[i];
							if (!link.isReroute)
							{
								stateLinkCount++;
							}
						}

						while (stateLinkCount > 5 && _StateLinkHistory.Count > 0)
						{
							StateLink link = _StateLinkHistory[0];
							if (!link.isReroute)
							{
								stateLinkCount--;
							}
							_StateLinkHistory.RemoveAt(0);
						}

						StateLinkRerouteNode rerouteNode = GetNodeFromID(nextStateLink.stateID) as StateLinkRerouteNode;
						if (rerouteNode != null)
						{
							nextStateLink = rerouteNode.link;
							nextStateLink.isReroute = true;
						}
						else
						{
							nextStateLink = null;
						}
					}
					_ReservedStateLink = null;
				}
				else
				{
					_StateLinkHistory.Clear();
				}

				_CurrentState = nextState;

				StateChanged();

				bool isBreaked = false;

				if (_CurrentState != null)
				{
					State currentState = _CurrentState;
					currentState.Activate(true);
					isBreaked = currentState.breakPoint;
				}

				_ChangingState = false;
				_InStateEvent = false;

				if (_PlayState != PlayState.Playing)
				{
					_Triggers.Clear();
					break;
				}

				for (int i = 0, count = _Triggers.Count; i < count; ++i)
				{
					Trigger trigger = _Triggers[i];
					SendTrigger(trigger.message, trigger.flags);
				}
				_Triggers.Clear();

				if (isBreaked)
				{
					_IsBreaked = isBreaked;
				}

				if (!isBreaked && nextImmediateTransition)
				{
					if (UpdateTransitionLoop())
					{
						nextState = _ReservedState;
						_ReservedState = null;
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
		}

		private bool IsValidTransition(State nextState, TransitionTiming transitionTiming)
		{
			if (nextState == null || nextState.stateMachine != this || nextState.resident)
			{
				return false;
			}

			switch (transitionTiming)
			{
				case TransitionTiming.Immediate:
					return true;
				case TransitionTiming.LateUpdateDontOverwrite:
					return _ReservedState == null;
				case TransitionTiming.LateUpdateOverwrite:
					return true;
			}

			return false;
		}

		private void TransitionInternal(State nextState, TransitionTiming transitionTiming)
		{
			switch (transitionTiming)
			{
				case TransitionTiming.Immediate:
					if (!_InStateEvent && isActiveAndEnabled)
					{
						if (_ReservedState != null)
						{
							_ReservedState = null;
						}

						ChangeState(nextState);
					}
					else
					{
						_ReservedStateTransitionTiming = transitionTiming;
						_ReservedState = nextState;
					}
					break;
				case TransitionTiming.LateUpdateDontOverwrite:
					if (_ReservedState == null)
					{
						_ReservedStateTransitionTiming = transitionTiming;
						_ReservedState = nextState;
					}
					break;
				case TransitionTiming.LateUpdateOverwrite:
					_ReservedStateTransitionTiming = transitionTiming;
					_ReservedState = nextState;
					break;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextState">遷移先のステート。</param>
		/// <param name="transitionTiming">遷移するタイミング。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextState">Destination state.</param>
		/// <param name="transitionTiming">Transition timing.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(State nextState, TransitionTiming transitionTiming)
		{
			_ReservedStateLink = null;

			if (IsValidTransition(nextState, transitionTiming))
			{
				TransitionInternal(nextState, transitionTiming);
				return true;
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextState">遷移先のステート。</param>
		/// <param name="immediateTransition">すぐに遷移するかどうか。falseの場合は現在フレームの最後(LateUpdate時)に遷移する。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextState">Destination state.</param>
		/// <param name="immediateTransition">Whether or not to transition immediately. If false I will transition to the end of the current frame (when LateUpdate).</param>
		/// <returns>Whether or not the transition</returns>
#endif
		[System.Obsolete("use Transition(State nextState,TransitionTiming transitionTiming).")]
		public bool Transition(State nextState, bool immediateTransition)
		{
			return Transition(nextState, immediateTransition ? TransitionTiming.Immediate : TransitionTiming.LateUpdateOverwrite);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移する。実際に遷移するタイミングは現在フレームの最後(LateUpdate時)。
		/// </summary>
		/// <param name="nextState">遷移先のステート。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition. Timing to actually transition current frame last (when LateUpdate).
		/// </summary>
		/// <param name="nextState">Destination state.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(State nextState)
		{
			return Transition(nextState, TransitionTiming.LateUpdateOverwrite);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextStateID">遷移先のステートID。</param>
		/// <param name="transitionTiming">遷移するタイミング。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextStateID">State ID for the transition destination.</param>
		/// <param name="transitionTiming">Transition timing.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(int nextStateID, TransitionTiming transitionTiming)
		{
			State nextState = GetStateFromID(nextStateID);
			return Transition(nextState, transitionTiming);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextStateID">遷移先のステートID。</param>
		/// <param name="immediateTransition">すぐに遷移するかどうか。falseの場合は現在フレームの最後(LateUpdate時)に遷移する。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextStateID">State ID for the transition destination.</param>
		/// <param name="immediateTransition">Whether or not to transition immediately. If false I will transition to the end of the current frame (when LateUpdate).</param>
		/// <returns>Whether or not the transition</returns>
#endif
		[System.Obsolete("use Transition(int nextStateID,TransitionTiming transitionTiming).")]
		public bool Transition(int nextStateID, bool immediateTransition)
		{
			return Transition(nextStateID, immediateTransition ? TransitionTiming.Immediate : TransitionTiming.LateUpdateOverwrite);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移する。実際に遷移するタイミングは現在フレームの最後(LateUpdate時)。
		/// </summary>
		/// <param name="nextStateID">遷移先のステートID。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition. Timing to actually transition current frame last (when LateUpdate).
		/// </summary>
		/// <param name="nextStateID">State ID for the transition destination.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(int nextStateID)
		{
			return Transition(nextStateID, TransitionTiming.LateUpdateOverwrite);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextStateLink">遷移の接続先。</param>
		/// <param name="transitionTiming">遷移するタイミング。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextStateLink">The destination of transition.</param>
		/// <param name="transitionTiming">Transition timing.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(StateLink nextStateLink, TransitionTiming transitionTiming)
		{
			if (nextStateLink.stateID != 0)
			{
				State nextState = GetState(nextStateLink);

				if (IsValidTransition(nextState, transitionTiming))
				{
					_ReservedStateLink = nextStateLink;

					TransitionInternal(nextState, transitionTiming);

					return true;
				}
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移
		/// </summary>
		/// <param name="nextStateLink">遷移の接続先。</param>
		/// <param name="immediateTransition">すぐに遷移するかどうか。falseの場合は現在フレームの最後(LateUpdate時)に遷移する。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition
		/// </summary>
		/// <param name="nextStateLink">The destination of transition.</param>
		/// <param name="immediateTransition">Whether or not to transition immediately. If false I will transition to the end of the current frame (when LateUpdate).</param>
		/// <returns>Whether or not the transition</returns>
#endif
		[System.Obsolete("use Transition(StateLink nextStateLink,TransitionTiming transitionTiming).")]
		public bool Transition(StateLink nextStateLink, bool immediateTransition)
		{
			return Transition(nextState, immediateTransition ? TransitionTiming.Immediate : TransitionTiming.LateUpdateOverwrite);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 状態遷移する。実際に遷移するタイミングは現在フレームの最後(LateUpdate時)。
		/// </summary>
		/// <param name="nextStateLink">遷移の接続先。</param>
		/// <returns>遷移できたかどうか</returns>
#else
		/// <summary>
		/// State transition. Timing to actually transition current frame last (when LateUpdate).
		/// </summary>
		/// <param name="nextStateLink">The destination of transition.</param>
		/// <returns>Whether or not the transition</returns>
#endif
		public bool Transition(StateLink nextStateLink)
		{
			return Transition(nextStateLink, nextStateLink.transitionTiming);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// トリガーの送信
		/// </summary>
		/// <param name="message">送信するメッセージ</param>
		/// <param name="flags">送信するトリガーフラグ</param>
#else
		/// <summary>
		/// Sending of trigger
		/// </summary>
		/// <param name="message">Message to be sent</param>
		/// <param name="flags">Trigger flag to send</param>
#endif
		public void SendTrigger(string message, SendTriggerFlags flags)
		{
			if (!(isActiveAndEnabled && _PlayState == PlayState.Playing))
			{
				return;
			}

			if (_ChangingState)
			{
				Trigger trigger = new Trigger()
				{
					message = message,
					flags = flags,
				};
				_Triggers.Add(trigger);
				return;
			}

			bool inStateEvent = _InStateEvent;
			_InStateEvent = true;

			if ((flags & SendTriggerFlags.ResidentStates) != 0)
			{
				int stateCount = _States.Count;
				for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
				{
					State state = _States[stateIndex];
					if (state.resident)
					{
						state.SendTrigger(message);
					}
				}
			}

			if ((flags & SendTriggerFlags.CurrentState) != 0)
			{
				if (_CurrentState != null)
				{
					_CurrentState.SendTrigger(message);
				}
			}

			_InStateEvent = inStateEvent;

			BeginTransitionLoop();

			if (!inStateEvent && nextImmediateTransition)
			{
				NextState();
			}

			EndTransitionLoop();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// トリガーの送信
		/// </summary>
		/// <param name="message">送信するメッセージ</param>
#else
		/// <summary>
		/// Sending of trigger
		/// </summary>
		/// <param name="message">Message to be sent</param>
#endif
		public void SendTrigger(string message)
		{
			// For calls from UnityEvent
			SendTrigger(message, allSendTrigger);
		}

		/// <summary>
		/// Register nodes
		/// </summary>
		protected sealed override void OnRegisterNodes()
		{
			for (int i = 0; i < _States.Count; i++)
			{
				RegisterNode(_States[i]);
			}

			for (int i = 0; i < _StateLinkRerouteNodes.count; i++)
			{
				RegisterNode(_StateLinkRerouteNodes[i]);
			}
		}
	}
}
