//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Componentパラメータの参照(ジェネリック)。
	/// </summary>
#else
	/// <summary>
	/// Reference Component parameters(Generic).
	/// </summary>
#endif
	public class ComponentParameterReference<T> : ParameterReference where T : Component
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの値。
		/// </summary>
#else
		/// <summary>
		/// Value of the parameter
		/// </summary>
#endif
		public T value
		{
			get
			{
				Parameter parameter = this.parameter;
				if (parameter != null)
				{
					return parameter.GetComponent<T>();
				}

				return null;
			}
			set
			{
				Parameter parameter = this.parameter;
				if (parameter != null)
				{
					parameter.SetComponent<T>(value);
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// <see cref="ComponentParameterReference{T}"/>を<see cref="ComponentParameterReference"/>にキャスト。
		/// </summary>
		/// <param name="parameterReference"><see cref="ComponentParameterReference{T}"/></param>
#else
		/// <summary>
		/// Cast <see cref="ComponentParameterReference{T}"/> to <see cref="ComponentParameterReference"/>.
		/// </summary>
		/// <param name="parameterReference"><see cref="ComponentParameterReference{T}"/></param>
#endif
		public static explicit operator ComponentParameterReference(ComponentParameterReference<T> parameterReference)
		{
			ComponentParameterReference componentParameterReference = new ComponentParameterReference();
			componentParameterReference.Copy(parameterReference);
			return componentParameterReference;
		}
	}
}