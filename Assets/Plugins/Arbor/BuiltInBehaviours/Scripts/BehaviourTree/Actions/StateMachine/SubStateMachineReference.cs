//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

using Arbor.ObjectPooling;

namespace Arbor.BehaviourTree.Actions
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
	public sealed class SubStateMachineReference : ActionBehaviour, INodeGraphContainer, INodeBehaviourSerializationCallbackReceiver
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

		[SerializeField]
		[HideInInspector]
		private int _SerializeVersion = 0;

		#region old

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_ExternalFSM")]
		private ArborFSM _OldExternalFSM = null;

		#endregion // old

		#endregion // Serialize Fields

		private const int kCurrentSerializeVersion = 1;

		private ArborFSM _CacheExternalFSM;
		private ArborFSM _RuntimeFSM;

		public ArborFSM runtimeFSM
		{
			get
			{
				return _RuntimeFSM;
			}
		}

		private bool _IsFinished = false;
		private bool _Success = false;

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

		protected override void OnStart()
		{
			CreateFSM();

			_IsFinished = false;
			if (_RuntimeFSM != null)
			{
				_ArgumentList.UpdateInput(_RuntimeFSM, GraphArgumentUpdateTiming.Enter);

				_RuntimeFSM.gameObject.SetActive(true);
				_RuntimeFSM.Play();
			}
		}

		protected override void OnExecute()
		{
			if (_RuntimeFSM != null)
			{
				_ArgumentList.UpdateInput(_RuntimeFSM, GraphArgumentUpdateTiming.Execute);

				_RuntimeFSM.ExecuteUpdate(true);

				if (_IsFinished)
				{
					_RuntimeFSM.Stop();
					_RuntimeFSM.gameObject.SetActive(false);

					FinishExecute(_Success);
				}
			}
		}

		protected override void OnEnd()
		{
			if (_RuntimeFSM != null)
			{
				_RuntimeFSM.Stop();
				_RuntimeFSM.gameObject.SetActive(false);

				_ArgumentList.UpdateOutput(_RuntimeFSM);
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
			_IsFinished = true;
			_Success = success;
		}

		void Reset()
		{
			_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			_ExternalFSM = (FlexibleComponent)_OldExternalFSM;
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