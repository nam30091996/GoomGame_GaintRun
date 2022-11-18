//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// VariableとVariableListの基底クラス。
	/// </summary>
#else
	/// <summary>
	/// Base class of Variable　and VariableList.
	/// </summary>
#endif
	[HideType(true)]
	public abstract class InternalVariableBase : MonoBehaviour
	{
		#region Serialize Fields

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		private ParameterContainerInternal _ParameterContainer;

		#endregion // Serialize Fields

		internal static ParameterContainerInternal s_CreatingParameterContainer;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの値の型
		/// </summary>
#else
		/// <summary>
		/// Value type of parameter
		/// </summary>
#endif
		public abstract System.Type valueType
		{
			get;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値のオブジェクト
		/// </summary>
#else
		/// <summary>
		/// Value object
		/// </summary>
#endif
		public abstract object valueObject
		{
			get;
			set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the NodeGraph.
		/// </summary>
#endif
		public ParameterContainerInternal parameterContainer
		{
			get
			{
				if (_ParameterContainer == null)
				{
					ParameterContainerInternal[] containers = gameObject.GetComponents<ParameterContainerInternal>();
					foreach (ParameterContainerInternal container in containers)
					{
						Parameter parameter = container.FindParameterContainsVariable(this);
						if (parameter != null)
						{
							_ParameterContainer = container;
							break;
						}
					}
				}
				if (_ParameterContainer == null && s_CreatingParameterContainer != null)
				{
					_ParameterContainer = s_CreatingParameterContainer;
				}
				return _ParameterContainer;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
		/// <param name="container">ParameterContainerInternal</param>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
		/// <param name="container">ParameterContainerInternal</param>
#endif
		public void Initialize(ParameterContainerInternal container)
		{
#if !ARBOR_DEBUG
			hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif

			_ParameterContainer = container;
		}
	}
}