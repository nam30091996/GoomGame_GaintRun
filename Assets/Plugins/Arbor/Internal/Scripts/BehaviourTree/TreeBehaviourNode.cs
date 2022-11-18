﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.BehaviourTree
{
#if ARBOR_DOC_JA
	/// <summary>
	/// TreeNodeBehaviourを持つノードの基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class of node with TreeNodeBehaviour
	/// </summary>
#endif
	[System.Serializable]
	public abstract class TreeBehaviourNode : TreeNodeBase, INodeBehaviourContainer
	{
		#region Serialize fields

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private Object _Behaviour = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private DecoratorList _DecoratorList = new DecoratorList();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ServiceList _ServiceList = new ServiceList();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private bool _BreakPoint;

		#endregion // Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// メインのBehaviour
		/// </summary>
#else
		/// <summary>
		/// Main behaviour
		/// </summary>
#endif
		public TreeNodeBehaviour behaviour
		{
			get
			{
				return _Behaviour as TreeNodeBehaviour;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorのリスト
		/// </summary>
#else
		/// <summary>
		/// Decorator list
		/// </summary>
#endif
		public DecoratorList decoratorList
		{
			get
			{
				return _DecoratorList;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceのリスト
		/// </summary>
#else
		/// <summary>
		/// Service list
		/// </summary>
#endif
		public ServiceList serviceList
		{
			get
			{
				return _ServiceList;
			}
		}

#if ARABOR_DOC_JA
		/// <summary>
		/// ブレークポイント。
		/// このプロパティがtrueのとき、ノードがアクティブになったタイミングでエディタがポーズ状態になります。
		/// </summary>
#else
		/// <summary>
		/// Break point.
		/// When this property is true, the editor will be paused when the node becomes active.
		/// </summary>
#endif
		public bool breakPoint
		{
			get
			{
				return _BreakPoint;
			}
			set
			{
				_BreakPoint = value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// TreeBehaviourNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
#else
		/// <summary>
		/// TreeBehaviourNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
#endif
		public TreeBehaviourNode(NodeGraph nodeGraph, int nodeID)
			: base(nodeGraph, nodeID)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// メインのBehaviourのObjectを取得。
		/// </summary>
		/// <returns>メインのBehaviourのObject</returns>
#else
		/// <summary>
		/// Get Main Behaviour Object.
		/// </summary>
		/// <returns>Main Behaviour Object</returns>
#endif
		public Object GetBehaviourObject()
		{
			return _Behaviour;
		}

		#region Decorators

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorを追加。
		/// </summary>
		/// <param name="type">追加するStateBehaviourの型</param>
		/// <returns>追加したStateBehaviour</returns>
#else
		/// <summary>
		/// Adds the Decorator.
		/// </summary>
		/// <param name="type">Type of add Decorator</param>
		/// <returns>Added Decorator</returns>
#endif
		public Decorator AddDecorator(System.Type type)
		{
			Decorator decorator = Decorator.Create(this, type);

			_DecoratorList.Add(decorator);

			return decorator;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorを追加。
		/// </summary>
		/// <typeparam name="T">追加するDecoratorの型</typeparam>
		/// <returns>追加したDecorator</returns>
#else
		/// <summary>
		/// Adds the Decorator.
		/// </summary>
		/// <typeparam name="T">Type of add Decorator</typeparam>
		/// <returns>Added Decorator</returns>
#endif
		public T AddDecorator<T>() where T : Decorator
		{
			T decorator = Decorator.Create<T>(this);

			_DecoratorList.Add(decorator);

			return decorator;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorを挿入。
		/// </summary>
		/// <param name="index">挿入先インデックス</param>
		/// <param name="type">追加するDecoratorの型</param>
		/// <returns>挿入したDecorator</returns>
#else
		/// <summary>
		/// Insert the Decorator.
		/// </summary>
		/// <param name="index">Insertion destination index</param>
		/// <param name="type">Type of add Decorator</param>
		/// <returns>Inserted Decorator</returns>
#endif
		public Decorator InsertDecorator(int index, System.Type type)
		{
			Decorator decorator = Decorator.Create(this, type);

			_DecoratorList.Insert(index, decorator);

			return decorator;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorを挿入。
		/// </summary>
		/// <typeparam name="T">挿入するDecoratorの型</typeparam>
		/// <param name="index">挿入先インデックス</param>
		/// <returns>挿入したDecorator</returns>
#else
		/// <summary>
		/// Insert the Decorator.
		/// </summary>
		/// <typeparam name="T">Type of insert Decorator</typeparam>
		/// <param name="index">Insertion destination index</param>
		/// <returns>Inserted Decorator</returns>
#endif
		public T InsertDecorator<T>(int index) where T : Decorator
		{
			T decorator = Decorator.Create<T>(this);

			_DecoratorList.Insert(index, decorator);

			return decorator;
		}

		#endregion // Decorators

		#region Services

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceを追加。
		/// </summary>
		/// <param name="type">追加するServiceの型</param>
		/// <returns>追加したService</returns>
#else
		/// <summary>
		/// Adds the Service.
		/// </summary>
		/// <param name="type">Type of add Service</param>
		/// <returns>Added Service</returns>
#endif
		public Service AddService(System.Type type)
		{
			Service service = Service.Create(this, type);

			_ServiceList.Add(service);

			return service;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceを追加。
		/// </summary>
		/// <typeparam name="T">追加するServiceの型</typeparam>
		/// <returns>追加したService</returns>
#else
		/// <summary>
		/// Adds the Service.
		/// </summary>
		/// <typeparam name="T">Type of add Service</typeparam>
		/// <returns>Added Service</returns>
#endif
		public T AddService<T>() where T : Service
		{
			T service = Service.Create<T>(this);

			_ServiceList.Add(service);

			return service;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceを挿入。
		/// </summary>
		/// <param name="index">挿入先インデックス</param>
		/// <param name="type">追加するServiceの型</param>
		/// <returns>挿入したService</returns>
#else
		/// <summary>
		/// Insert the Service.
		/// </summary>
		/// <param name="index">Insertion destination index</param>
		/// <param name="type">Type of add Service</param>
		/// <returns>Inserted Service</returns>
#endif
		public Service InsertService(int index, System.Type type)
		{
			Service service = Service.Create(this, type);

			_ServiceList.Insert(index, service);

			return service;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceを挿入。
		/// </summary>
		/// <typeparam name="T">挿入するServiceの型</typeparam>
		/// <param name="index">挿入先インデックス</param>
		/// <returns>挿入したService</returns>
#else
		/// <summary>
		/// Insert the Service.
		/// </summary>
		/// <typeparam name="T">Type of insert Service</typeparam>
		/// <param name="index">Insertion destination index</param>
		/// <returns>Inserted Service</returns>
#endif
		public T InsertService<T>(int index) where T : Service
		{
			T service = Service.Create<T>(this);

			_ServiceList.Insert(index, service);

			return service;
		}

		#endregion // Services

#if ARBOR_DOC_JA
		/// <summary>
		/// Nodeが所属するNodeGraphが変わった際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when the NodeGraph to which the Node belongs has changed.
		/// </summary>
#endif
		protected override void OnGraphChanged()
		{
			TreeNodeBehaviour sorceBehaviour = _Behaviour as TreeNodeBehaviour;
			_Behaviour = null;

			ComponentUtility.MoveBehaviour(this, sorceBehaviour);

			_DecoratorList.MoveBehaviour(this);
			_ServiceList.MoveBehaviour(this);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeBehaviourを含んでいるかをチェックする。
		/// </summary>
		/// <param name="behaviour">チェックするNodeBehaviour</param>
		/// <returns>NodeBehaviourを含んでいる場合にtrueを返す。</returns>
#else
		/// <summary>
		/// Check if it contains NodeBehaviour.
		/// </summary>
		/// <param name="behaviour">Check NodeBehaviour</param>
		/// <returns>Returns true if it contains NodeBehaviour.</returns>
#endif
		public override bool IsContainsBehaviour(NodeBehaviour behaviour)
		{
			if (this.behaviour == behaviour)
			{
				return true;
			}

			if (_DecoratorList.ContainsObject(behaviour))
			{
				return true;
			}

			if (_ServiceList.ContainsObject(behaviour))
			{
				return true;
			}

			return false;
		}

		void DestroyBehaviour(Object behaviour)
		{
			nodeGraph.DisconnectDataBranch(behaviour);

			NodeBehaviour nodeBehaviour = behaviour as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				NodeBehaviour.Destroy(nodeBehaviour);
			}
			else if (behaviour != null)
			{
				ComponentUtility.Destroy(behaviour);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Decoratorの順番を移動する。
		/// </summary>
		/// <param name="fromIndex">移動させたいインデックス。</param>
		/// <param name="toNode">移動先のNode。</param>
		/// <param name="toIndex">移動先のインデックス。</param>
#else
		/// <summary>
		/// Move the order of Decorator.
		/// </summary>
		/// <param name="fromIndex">The moving want index.</param>
		/// <param name="toNode">The destination Node.</param>
		/// <param name="toIndex">The destination index.</param>
#endif
		public void MoveDecorator(int fromIndex, TreeBehaviourNode toNode, int toIndex)
		{
			if (toNode == null)
			{
				return;
			}

			if (this == toNode)
			{
				_DecoratorList.Move(fromIndex, toIndex);
			}
			else
			{
				Object behaviour = _DecoratorList.GetObject(fromIndex);

				TreeNodeBehaviour nodeBehaviour = behaviour as TreeNodeBehaviour;
				if (nodeBehaviour != null)
				{
					_DecoratorList._Objects.RemoveAt(fromIndex);

					ComponentUtility.RecordObject(nodeBehaviour, "Move Behaviour");

					nodeBehaviour.Initialize(nodeGraph, toNode.nodeID);

					ComponentUtility.SetDirty(nodeBehaviour);

					toNode._DecoratorList._Objects.Insert(toIndex, behaviour);
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceの順番を移動する。
		/// </summary>
		/// <param name="fromIndex">移動させたいインデックス。</param>
		/// <param name="toNode">移動先のNode。</param>
		/// <param name="toIndex">移動先のインデックス。</param>
#else
		/// <summary>
		/// Move the order of Service.
		/// </summary>
		/// <param name="fromIndex">The moving want index.</param>
		/// <param name="toNode">The destination Node.</param>
		/// <param name="toIndex">The destination index.</param>
#endif
		public void MoveService(int fromIndex, TreeBehaviourNode toNode, int toIndex)
		{
			if (toNode == null)
			{
				return;
			}

			if (this == toNode)
			{
				_ServiceList.Move(fromIndex, toIndex);
			}
			else
			{
				Object behaviour = _ServiceList.GetObject(fromIndex);

				TreeNodeBehaviour nodeBehaviour = behaviour as TreeNodeBehaviour;
				if (nodeBehaviour != null)
				{
					_ServiceList._Objects.RemoveAt(fromIndex);

					ComponentUtility.RecordObject(nodeBehaviour, "Move Behaviour");

					nodeBehaviour.Initialize(nodeGraph, toNode.nodeID);

					ComponentUtility.SetDirty(nodeBehaviour);

					toNode._ServiceList._Objects.Insert(toIndex, behaviour);
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Behaviourを破棄。
		/// </summary>
#else
		/// <summary>
		/// Destroy behaviour.
		/// </summary>
#endif
		public void DestroyBehaviour()
		{
			DestroyBehaviour(_Behaviour);
			_Behaviour = null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 全てのBehaviourを破棄。
		/// </summary>
#else
		/// <summary>
		/// Destroy all behaviour.
		/// </summary>
#endif
		public void DestroyAllBehaviour()
		{
			DestroyBehaviour(_Behaviour);
			_Behaviour = null;

			_DecoratorList.DestroyAll(this);
			_ServiceList.DestroyAll(this);
		}

		int INodeBehaviourContainer.GetNodeBehaviourCount()
		{
			return 1 + _DecoratorList.count + _ServiceList.count;
		}

		T INodeBehaviourContainer.GetNodeBehaviour<T>(int index)
		{
			if (index < 1)
			{
				return _Behaviour as T;
			}
			index--;

			if (index < _DecoratorList.count)
			{
				return _DecoratorList.GetObject(index) as T;
			}
			index -= _DecoratorList.count;

			if (index < _ServiceList.count)
			{
				return _ServiceList.GetObject(index) as T;
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// メインのBehaviourを設定
		/// </summary>
		/// <param name="behaviour">Behaviour</param>
#else
		/// <summary>
		/// Set Main Behaviour.
		/// </summary>
		/// <param name="behaviour">Behaviour</param>
#endif
		protected void SetBehaviour(TreeNodeBehaviour behaviour)
		{
			_Behaviour = behaviour;
		}

		void INodeBehaviourContainer.SetNodeBehaviour(int index, NodeBehaviour behaviour)
		{
			if (index < 1)
			{
				SetBehaviour(behaviour as TreeNodeBehaviour);
				return;
			}
			index--;

			if (index < _DecoratorList.count)
			{
				_DecoratorList.SetObject(index, behaviour);
				return;
			}
			index -= _DecoratorList.count;

			if (index < _ServiceList.count)
			{
				_ServiceList.SetObject(index, behaviour);
				return;
			}
		}

		void ActivateBehaviour(bool active)
		{
			if (behaviour != null)
			{
				behaviour.ActivateInternal(active, true);
			}
		}

		void ActivateDecorators(bool active)
		{
			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					decorator.ActivateInternal(active, true);
				}
			}
		}

		void ActivateServices(bool active)
		{
			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.ActivateInternal(active, true);
				}
			}

			if (active)
			{
				UpdateServices();
			}
		}

		internal override bool OnActivate(bool active, bool interrupt, bool isRevaluator)
		{
			if (!base.OnActivate(active, interrupt, isRevaluator))
			{
				return false;
			}

			ActivateDecorators(active);

			if (active)
			{
				// RegisterRevaluation
				bool firstRegister = false;
				if (HasConditionCheck())
				{
					firstRegister = behaviourTree.RegisterRevaluation(this);
				}

				// Interrupt reevaluated
				if (firstRegister && !interrupt)
				{
					// Check First Condition
					if (!ConditionCheck(0))
					{
						// Failure if the condition is false
						return false;
					}
				}
			}

			ActivateBehaviour(active);
			ActivateServices(active);

			return true;
		}

		internal override void OnPause()
		{
			if (behaviour != null)
			{
				behaviour.PauseInternal();
			}

			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					decorator.PauseInternal();
				}
			}

			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.PauseInternal();
				}
			}
		}

		internal override void OnResume()
		{
			if (behaviour != null)
			{
				behaviour.ResumeInternal();
			}

			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					decorator.ResumeInternal();
				}
			}

			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.ResumeInternal();
				}
			}
		}

		internal override void OnStop()
		{
			if (behaviour != null)
			{
				behaviour.StopInternal();
			}

			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					decorator.StopInternal();
					decorator.OnClearRevaluation();
				}
			}

			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.StopInternal();
				}
			}
		}

		void UpdateServices()
		{
			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.CallUpdateInternal();
				}
			}
		}

		internal override void OnUpdate()
		{
			UpdateServices();
		}

		internal override bool OnFinishExecute(bool result)
		{
			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					result = decorator.CallFinishExecuteInternal(result);
				}
			}

			return result;
		}

		internal override bool RepeatCheck(bool nodeResult)
		{
			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					if (decorator.CallRepeatCheckInternal(nodeResult))
					{
						return true;
					}
				}
			}

			return false;
		}

		internal override void OnRestart()
		{
			base.OnRestart();

			ActivateBehaviour(false);
			ActivateBehaviour(true);
		}

		internal override bool HasConditionCheck()
		{
			return true;
		}

		internal override bool OnConditionCheck(AbortFlags abortFlags)
		{
			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled && decorator.HasConditionCheck() && decorator.CompareAbortFlags(abortFlags))
				{
					if (!decorator.CallConditionCheckInternal())
					{
						return false;
					}
				}
			}

			return true;
		}

		internal override bool HasAbortFlags(AbortFlags abortFlags)
		{
			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled && decorator.HasConditionCheck() && decorator.CompareAbortFlags(abortFlags))
				{
					return true;
				}
			}
			return false;
		}

		internal override void OnAbort()
		{
			behaviour.AbortInternal();

			int decoratorCount = _DecoratorList.count;
			for (int decoratorIndex = 0; decoratorIndex < decoratorCount; decoratorIndex++)
			{
				Decorator decorator = _DecoratorList[decoratorIndex];
				if (decorator != null && decorator.behaviourEnabled)
				{
					decorator.AbortInternal();
				}
			}

			int serviceCount = _ServiceList.count;
			for (int serviceIndex = 0; serviceIndex < serviceCount; serviceIndex++)
			{
				Service service = _ServiceList[serviceIndex];
				if (service != null && service.behaviourEnabled)
				{
					service.AbortInternal();
				}
			}
		}
	}
}