//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.BehaviourTree
{
#if ARBOR_DOC_JA
	/// <summary>
	/// TreeBehaviourNodeの挙動を定義する基本クラス。
	/// </summary>
#else
	/// <summary>
	/// Base class that defines the behavior of TreeBehaviourNode.
	/// </summary>
#endif
	[AddComponentMenu("")]
	public class TreeNodeBehaviour : NodeBehaviour
	{
		#region Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// ArborEditorWindow上での開閉状態。
		/// </summary>
#else
		/// <summary>
		/// Expanded on ArborEditorWindow.
		/// </summary>
#endif
		[HideInInspector]
		public bool expanded = true;

		#endregion // Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// ビヘイビアツリーを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the behaviour tree.
		/// </summary>
#endif
		public BehaviourTreeInternal behaviourTree
		{
			get
			{
				return nodeGraph as BehaviourTreeInternal;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// TreeNodeBaseを取得。
		/// </summary>
#else
		/// <summary>
		/// Get the TreeNodeBase.
		/// </summary>
#endif
		public TreeNodeBase treeNode
		{
			get
			{
				return node as TreeNodeBase;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// enabledの初期化を行うために呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called to perform enabled initialization.
		/// </summary>
#endif
		protected sealed override void OnInitializeEnabled()
		{
			if (treeNode.isActive)
			{
				CallActiveEvent();
			}
			else
			{
				enabled = false;
			}
		}

		private bool _IsAwake = false;
		internal bool _IsCalledEvent = false;

		internal void ActivateInternal(bool active, bool changeState)
		{
			if (active && !enabled)
			{
				enabled = true;
				if (changeState)
				{
					CallActiveEvent();
				}
			}
			else if (!active && enabled)
			{
				if (changeState)
				{
					CallInactiveEvent();
				}

				enabled = false;
			}
		}

		void CallActiveEvent()
		{
			if (_IsCalledEvent)
			{
				return;
			}

			UpdateDataLink(DataLinkUpdateTiming.Enter);

			if (!_IsAwake)
			{
				_IsAwake = true;

				try
				{
					OnAwake();
				}
				catch (System.Exception ex)
				{
					Debug.LogException(ex, this);
				}
			}

			try
			{
				OnStart();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}

			_IsCalledEvent = true;
		}

		void CallInactiveEvent()
		{
			if (!_IsCalledEvent)
			{
				return;
			}

			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnEnd();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}

			_IsCalledEvent = false;
		}

		void CallAbortEvent()
		{
			if (!_IsCalledEvent)
			{
				return;
			}

			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnAbort();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal void PauseInternal()
		{
			if (_IsCalledEvent)
			{
				CallPauseEvent();
			}
			enabled = false;
		}

		internal void ResumeInternal()
		{
			enabled = true;
			if (_IsCalledEvent)
			{
				CallResumeEvent();
			}
		}

		internal void StopInternal()
		{
			if (_IsCalledEvent)
			{
				CallStopEvent();
				CallInactiveEvent();
			}

			enabled = false;
		}

		internal void AbortInternal()
		{
			CallAbortEvent();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数は自ノードが初めてアクティブになったときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the own node becomes active for the first time.
		/// </summary>
#endif
		protected virtual void OnAwake()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数は自ノードがアクティブになったときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the own node becomes active.
		/// </summary>
#endif
		protected virtual void OnStart()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数は自ノードが中止されるときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the own node is aborted.
		/// </summary>
#endif
		protected virtual void OnAbort()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数は自ノードが終了したときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the own node ends.
		/// </summary>
#endif
		protected virtual void OnEnd()
		{
		}
	}
}