//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ParameterContainerを識別するための基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class to identify the ParameterContainer
	/// </summary>
#endif
	[AddComponentMenu("")]
	public class ParameterContainerBase : MonoBehaviour
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 実体のParameterContainerを返す。
		/// </summary>
#else
		/// <summary>
		/// It returns the ParameterContainer entity.
		/// </summary>
#endif
		public ParameterContainerInternal container
		{
			get
			{
				ParameterContainerInternal parameterContainer = this as ParameterContainerInternal;
				if (parameterContainer == null)
				{
					GlobalParameterContainerInternal globalParameterContainer = this as GlobalParameterContainerInternal;
					if (globalParameterContainer != null)
					{
						parameterContainer = globalParameterContainer.instance;
					}
				}
				return parameterContainer;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 元のParameterContainerを返す。
		/// </summary>
#else
		/// <summary>
		/// It returns the original ParameterContainer.
		/// </summary>
#endif
		public ParameterContainerInternal defaultContainer
		{
			get
			{
				ParameterContainerInternal parameterContainer = this as ParameterContainerInternal;
				if (parameterContainer == null)
				{
					GlobalParameterContainerInternal globalParameterContainer = this as GlobalParameterContainerInternal;
					if (globalParameterContainer != null)
					{
						parameterContainer = globalParameterContainer.prefab;
					}
				}
				return parameterContainer;
			}
		}
	}
}
