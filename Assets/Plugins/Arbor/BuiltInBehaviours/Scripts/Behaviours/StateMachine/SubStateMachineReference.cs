//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

using Arbor.ObjectPooling;

namespace Arbor.StateMachine.StateBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 子グラフとして外部ArborFSMを再生する。
	/// </summary>
#else
	/// <summary>
	/// Play external ArborFSM as a child graph.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("StateMachine/SubStateMachineReference")]
	[BuiltInBehaviour]
	public sealed class SubStateMachineReference : StateBehaviour, INodeGraphContainer, INodeBehaviourSerializationCallbackReceiver
	{
		#region Serialize Fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 参照する外部FSM
		/// </summary>
#else
		/// <summary>
		/// Reference external FSM
		/// </summary>
#endif
		[SerializeField]
		[SlotType(typeof(ArborFSM))]
		private FlexibleComponent _ExternalFSM = new FlexibleComponent();

#if ARBOR_DOC_JA
		/// <summary>
		/// ObjectPoolを使用してインスタンス化するフラグ。
		/// </summary>
#else
		/// <summary>
		/// Flag to instantiate using ObjectPool.
		/// </summary>
#endif
		[SerializeField]
		private FlexibleBool _UsePool = new FlexibleBool();

#if ARBOR_DOC_JA
		/// <summary>
		/// トリガーを子FSMに受け渡すフラグ
		/// </summary>
#else
		/// <summary>
		/// Flag to pass trigger to child FSM
		/// </summary>
#endif
		[SerializeField]
		private FlexibleBool _PassThroughTrigger = new FlexibleBool(true);

#if ARBOR_DOC_JA
		/// <summary>
		/// グラフの引数<br/>
		/// <list type="bullet">
		/// <item><description>+ボタンから、パラメータを選択するかパラメータ名を指定して作成。</description></item>
		/// <item><description>パラメータを選択し、-ボタンをクリックで削除。</description></item>
		/// </list>
		/// </summary>
#else
		/// <summary>
		/// Arguments of the graph<br/>
		/// <list type="bullet">
		/// <item><description>Create from the + button by selecting a parameter or specifying a parameter name.</description></item>
		/// <item><description>Select the parameter and delete it by clicking the - button.</description></item>
		/// </list>
		/// </summary>
#endif
		[SerializeField]
		private GraphArgumentList _ArgumentList = new GraphArgumentList();

#if ARBOR_DOC_JA
		/// <summary>
		/// 成功時の遷移<br />
		/// 遷移メソッド : OnFinishNodeGraph
		/// </summary>
#else
		/// <summary>
		/// Transition on success<br />
		/// Transition Method : OnFinishNodeGraph
		/// </summary>
#endif
		[SerializeField]
		private StateLink _SuccessLink = new StateLink();

#if ARBOR_DOC_JA
		/// <summary>
		/// 失敗時の遷移<br />
		/// 遷移メソッド : OnFinishNodeGraph
		/// </summary>
#else
		/// <summary>
		/// Transition on failure<br />
		/// Transition Method : OnFinishNodeGraph
		/// </summary>
#endif
		[SerializeField]
		private StateLink _FailureLink = new StateLink();

		[SerializeField]
		[HideInInspector]
		private int _SerializeVersion = 0;

		#region old

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_ExternalFSM")]
		private ArborFSM _OldExternalFSM = null;

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_PassThroughTrigger")]
		private bool _OldPassThroughTrigger = true;

		#endregion // old

		#endregion // Serialize Fields

		private const int kCurrentSerializeVersion = 2;

		private ArborFSM _CacheExternalFSM;
		private ArborFSM _RuntimeFSM;

		public ArborFSM runtimeFSM
		{
			get
			{
				return _RuntimeFSM;
			}
		}

		void OnEnable()
		{
			if (_RuntimeFSM != null && _RuntimeFSM.playState != PlayState.Stopping)
			{
				_RuntimeFSM.gameObject.SetActive(true);
			}
		}

		void OnDisable()
		{
			if (_RuntimeFSM != null && _RuntimeFSM.playState != PlayState.Stopping)
			{
				_RuntimeFSM.gameObject.SetActive(false);
			}
		}

		void CreateFSM()
		{
			ArborFSM externalFSM = _ExternalFSM.value as ArborFSM;

			if (_CacheExternalFSM == externalFSM)
			{
				return;
			}

			_CacheExternalFSM = externalFSM;

			if (_RuntimeFSM != null)
			{
				ObjectPool.Destroy(_RuntimeFSM.gameObject);
				_RuntimeFSM = null;
			}

			if (_CacheExternalFSM == null)
			{
				return;
			}

			_RuntimeFSM = NodeGraph.Instantiate<ArborFSM>(_CacheExternalFSM, this, _UsePool.value);
			_RuntimeFSM.playOnStart = false;
			_RuntimeFSM.updateSettings.type = UpdateType.Manual;
			_RuntimeFSM.gameObject.SetActive(false);
		}

		// Use this for enter state
		public override void OnStateBegin()
		{
			using (new ProfilerScope("SubStateMachineReference.OnStateBegin"))
			{
				CreateFSM();

				if (_RuntimeFSM != null)
				{
					_ArgumentList.UpdateInput(_RuntimeFSM, GraphArgumentUpdateTiming.Enter);

					_RuntimeFSM.gameObject.SetActive(true);
					_RuntimeFSM.Play();
				}
			}
		}

		public override void OnStateUpdate()
		{
			if (_RuntimeFSM != null)
			{
				_ArgumentList.UpdateInput(_RuntimeFSM, GraphArgumentUpdateTiming.Execute);

				_RuntimeFSM.ExecuteUpdate();
			}
		}

		public override void OnStateLateUpdate()
		{
			if (_RuntimeFSM != null)
			{
				_ArgumentList.UpdateInput(_RuntimeFSM, GraphArgumentUpdateTiming.Execute);

				_RuntimeFSM.ExecuteLateUpdate();
			}
		}

		// Use this for exit state
		public override void OnStateEnd()
		{
			using (new ProfilerScope("SubStateMachineReference.OnStateEnd"))
			{
				if (_RuntimeFSM != null)
				{
					_RuntimeFSM.Stop();
					_RuntimeFSM.gameObject.SetActive(false);

					_ArgumentList.UpdateOutput(_RuntimeFSM);
				}
			}
		}

		protected override void OnGraphPause()
		{
			if (_RuntimeFSM != null)
			{
				_RuntimeFSM.Pause();
			}
		}

		protected override void OnGraphResume()
		{
			if (_RuntimeFSM != null)
			{
				_RuntimeFSM.Resume();
			}
		}

		protected override void OnGraphStop()
		{
			if (_RuntimeFSM != null)
			{
				_RuntimeFSM.Stop();
				_RuntimeFSM.gameObject.SetActive(false);
			}
		}

		public override void OnStateTrigger(string message)
		{
			if (_PassThroughTrigger.value && _RuntimeFSM != null)
			{
				_RuntimeFSM.SendTrigger(message);
			}
		}

		int INodeGraphContainer.GetNodeGraphCount()
		{
			return _RuntimeFSM != null ? 1 : 0;
		}

		T INodeGraphContainer.GetNodeGraph<T>(int index)
		{
			return _RuntimeFSM as T;
		}

		void INodeGraphContainer.SetNodeGraph(int index, NodeGraph graph)
		{
			_RuntimeFSM = graph as ArborFSM;
		}

		void INodeGraphContainer.OnFinishNodeGraph(NodeGraph graph, bool success)
		{
			if (_RuntimeFSM != null)
			{
				_RuntimeFSM.Stop();
				_RuntimeFSM.gameObject.SetActive(false);
			}

			if (success)
			{
				Transition(_SuccessLink);
			}
			else
			{
				Transition(_FailureLink);
			}
		}

		void Reset()
		{
			_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_ExternalFSM = (FlexibleComponent)_OldExternalFSM;
		}

		void SerializeVer2()
		{
			_PassThroughTrigger = (FlexibleBool)_OldPassThroughTrigger;
		}

		void Serialize()
		{
			while (_SerializeVersion != kCurrentSerializeVersion)
			{
				switch (_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_SerializeVersion++;
						break;
					case 1:
						SerializeVer2();
						_SerializeVersion++;
						break;
					default:
						_SerializeVersion = kCurrentSerializeVersion;
						break;
				}
			}
		}

		void INodeBehaviourSerializationCallbackReceiver.OnAfterDeserialize()
		{
			Serialize();
		}

		void INodeBehaviourSerializationCallbackReceiver.OnBeforeSerialize()
		{
			Serialize();
		}
	}
}