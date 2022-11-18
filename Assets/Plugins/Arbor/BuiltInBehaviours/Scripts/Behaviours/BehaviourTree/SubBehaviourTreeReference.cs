//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

using Arbor.ObjectPooling;

namespace Arbor.StateMachine.StateBehaviours
{
	using Arbor.BehaviourTree;

#if ARBOR_DOC_JA
	/// <summary>
	/// 子グラフとして外部BehaviourTreeを再生する。
	/// </summary>
#else
	/// <summary>
	/// Play external BehaviourTree as a child graph.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("BehaviourTree/SubBehaviourTreeReference")]
	[BuiltInBehaviour]
	public sealed class SubBehaviourTreeReference : StateBehaviour, INodeGraphContainer, INodeBehaviourSerializationCallbackReceiver
	{
		#region Serialize Fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 参照する外部BT
		/// </summary>
#else
		/// <summary>
		/// Reference external BT
		/// </summary>
#endif
		[SerializeField]
		[SlotType(typeof(BehaviourTree))]
		private FlexibleComponent _ExternalBT = new FlexibleComponent();

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
		[FormerlySerializedAs("_ExternalBT")]
		private BehaviourTree _OldExternalBT = null;

		#endregion // old

		#endregion // Serialize Fields

		private const int kCurrentSerializeVersion = 1;

		private BehaviourTree _CacheExternalBT;
		private BehaviourTree _RuntimeBT;

		public BehaviourTree runtimeBT
		{
			get
			{
				return _RuntimeBT;
			}
		}

		void OnEnable()
		{
			if (_RuntimeBT != null && _RuntimeBT.playState != PlayState.Stopping)
			{
				_RuntimeBT.gameObject.SetActive(true);
			}
		}

		void OnDisable()
		{
			if (_RuntimeBT != null && _RuntimeBT.playState != PlayState.Stopping)
			{
				_RuntimeBT.gameObject.SetActive(false);
			}
		}

		void CreateBT()
		{
			BehaviourTree externalBT = _ExternalBT.value as BehaviourTree;

			if (_CacheExternalBT == externalBT)
			{
				return;
			}

			_CacheExternalBT = externalBT;

			if (_RuntimeBT != null)
			{
				ObjectPool.Destroy(_RuntimeBT.gameObject);
				_RuntimeBT = null;
			}

			if (_CacheExternalBT == null)
			{
				return;
			}

			_RuntimeBT = NodeGraph.Instantiate<BehaviourTree>(_CacheExternalBT, this, _UsePool.value);
			_RuntimeBT.playOnStart = false;
			_RuntimeBT.updateSettings.type = UpdateType.Manual;
			_RuntimeBT.gameObject.SetActive(false);
		}

		// Use this for enter state
		public override void OnStateBegin()
		{
			CreateBT();

			if (_RuntimeBT != null)
			{
				_ArgumentList.UpdateInput(_RuntimeBT, GraphArgumentUpdateTiming.Enter);

				_RuntimeBT.gameObject.SetActive(true);
				_RuntimeBT.Play();
			}
		}

		public override void OnStateUpdate()
		{
			if (_RuntimeBT != null)
			{
				_ArgumentList.UpdateInput(_RuntimeBT, GraphArgumentUpdateTiming.Execute);

				_RuntimeBT.Execute();
			}
		}

		public override void OnStateEnd()
		{
			if (_RuntimeBT != null)
			{
				_RuntimeBT.Stop();
				_RuntimeBT.gameObject.SetActive(false);

				_ArgumentList.UpdateOutput(_RuntimeBT);
			}
		}

		protected override void OnGraphPause()
		{
			if (_RuntimeBT != null)
			{
				_RuntimeBT.Pause();
			}
		}

		protected override void OnGraphResume()
		{
			if (_RuntimeBT != null)
			{
				_RuntimeBT.Resume();
			}
		}

		protected override void OnGraphStop()
		{
			if (_RuntimeBT != null)
			{
				_RuntimeBT.Stop();
				_RuntimeBT.gameObject.SetActive(false);
			}
		}

		int INodeGraphContainer.GetNodeGraphCount()
		{
			return _RuntimeBT != null ? 1 : 0;
		}

		T INodeGraphContainer.GetNodeGraph<T>(int index)
		{
			return _RuntimeBT as T;
		}

		void INodeGraphContainer.SetNodeGraph(int index, NodeGraph graph)
		{
			_RuntimeBT = graph as BehaviourTree;
		}

		void INodeGraphContainer.OnFinishNodeGraph(NodeGraph graph, bool success)
		{
			if (_RuntimeBT != null)
			{
				_RuntimeBT.Stop();
				_RuntimeBT.gameObject.SetActive(false);
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
			_ExternalBT = (FlexibleComponent)_OldExternalBT;
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