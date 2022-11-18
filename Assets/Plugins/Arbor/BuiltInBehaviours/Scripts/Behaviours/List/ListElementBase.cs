//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections;

namespace Arbor.StateMachine.StateBehaviours
{
	using Arbor.Events;

#if ARBOR_DOC_JA
	/// <summary>
	/// Listに要素を設定する挙動の基本クラス
	/// </summary>
#else
	/// <summary>
	/// Base class for behavior of setting elements in List
	/// </summary>
#endif
	public abstract class ListElementBase : ListBehaviourBase
	{
		[SerializeField]
		[HideInInspector]
		private ParameterType _ParameterType = ParameterType.Unknown;

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素
		/// </summary>
#else
		/// <summary>
		/// Element
		/// </summary>
#endif
		[SerializeField]
		[Internal.DocumentLabel("Element")]
		[Internal.DocumentOrder(1000)]
		private ParameterList _ParameterList = new ParameterList();

		protected sealed override object OnExecute(System.Type elementType, object array, ListInstanceType outputType)
		{
			ParameterType parameterType = ArborEventUtility.GetParameterType(elementType, true);

			if (_ParameterType != parameterType)
			{
				Debug.LogError("The parameter type has changed : " + this, this);
				return null;
			}

			IList list = _ParameterList.GetParameterList(_ParameterType);
			if (list == null)
			{
				return null;
			}

			int count = list.Count;
			if (count == 0)
			{
				return null;
			}

			IValueContainer valueContainer = list[0] as IValueContainer;

			if (valueContainer == null)
			{
				return null;
			}

			object value = valueContainer.GetValue();

			return OnExecute(elementType, array, value, outputType);
		}

		protected abstract object OnExecute(System.Type elementType, object array, object value, ListInstanceType outputType);
	}
}