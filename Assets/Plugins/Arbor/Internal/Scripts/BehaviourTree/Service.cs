﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.BehaviourTree
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 自ノードがアクティな時に実行されるクラス。継承して利用する。
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="AddBehaviourMenu" /></description></item>
	/// <item><description><see cref="HideBehaviour" /></description></item>
	/// <item><description><see cref="BehaviourTitle" /></description></item>
	/// <item><description><see cref="BehaviourHelp" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Class executed when its own node is active. Inherit and use it.
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="AddBehaviourMenu" /></description></item>
	/// <item><description><see cref="HideBehaviour" /></description></item>
	/// <item><description><see cref="BehaviourTitle" /></description></item>
	/// <item><description><see cref="BehaviourHelp" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[AddComponentMenu("")]
	public class Service : TreeNodeBehaviour
	{
		#region Serialize fields

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		private bool _BehaviourEnabled = true;

		#endregion // Serialize fields

#if ARBOR_DOC_JA
		/// <summary>
		/// 有効状態を取得/設定。
		/// </summary>
		/// <value>
		///   <c>true</c> 有効; その他、 <c>false</c>.
		/// </value>
#else
		/// <summary>
		/// Gets or sets a value indicating whether [behaviour enabled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [behaviour enabled]; otherwise, <c>false</c>.
		/// </value>
#endif
		public bool behaviourEnabled
		{
			get
			{
				return _BehaviourEnabled;
			}
			set
			{
				if (_BehaviourEnabled != value)
				{
					_BehaviourEnabled = value;
					if (treeNode.isActive)
					{
						if (_BehaviourEnabled)
						{
							ActivateInternal(true, !_IsCalledEvent);
						}
						else
						{
							ActivateInternal(false, false);
						}
					}
				}
			}
		}

		internal static Service Create(Node node, System.Type type)
		{
			System.Type classType = typeof(Service);
			if (type != classType && !TypeUtility.IsSubclassOf(type, classType))
			{
				throw new System.ArgumentException("The type `" + type.Name + "' must be convertible to `Service' in order to use it as parameter `type'", "type");
			}

			return CreateNodeBehaviour(node, type) as Service;
		}

		internal static Type Create<Type>(Node node) where Type : Service
		{
			return CreateNodeBehaviour<Type>(node);
		}

		internal void CallUpdateInternal()
		{
			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnUpdate();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Serviceの更新。実行フレーム毎に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Update of Service. It is called execution frame.
		/// </summary>
#endif
		protected virtual void OnUpdate()
		{
		}
	}
}