//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections;

namespace Arbor.Calculators
{
	using Arbor.Events;

#if ARBOR_DOC_JA
	/// <summary>
	/// Listの要素が格納されているインデックスを取得する。
	/// </summary>
	/// <remarks>
	/// IL2CPPなどのAOT環境では、List&lt;指定した型&gt;がコード上で使用していないと正常動作しない可能性があります。<br />
	/// 詳しくは、<a href="https://caitsithware.com/assets/arbor/docs/ja/manual/dataflow/list.html#AOTRestrictions">事前コンパイル(AOT)での制限</a>を参照してください。
	/// </remarks>
#else
	/// <summary>
	/// Gets the index where the List elements are stored.
	/// </summary>
	/// <remarks>
	/// In an AOT environment such as IL2CPP, List&lt;specified type&gt; may not work properly unless it is used in the code.<br />
	/// See <a href="https://caitsithware.com/assets/arbor/docs/en/manual/dataflow/list.html#AOTRestrictions">Ahead-of-Time (AOT) Restrictions</a> for more information.
	/// </remarks>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("List/List.IndexOf")]
	[BehaviourTitle("List.IndexOf", useNicifyName = false)]
	[BuiltInBehaviour]
	public sealed class ListIndexOfCalculator : Calculator
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 要素の型
		/// </summary>
#else
		/// <summary>
		/// Element type
		/// </summary>
#endif
		[SerializeField]
		[ClassNotStaticConstraint]
		[TypeFilter((TypeFilterFlags)(-1) & ~(TypeFilterFlags.Static))]
		private ClassTypeReference _ElementType = new ClassTypeReference();

		[SerializeField]
		[Internal.HideInDocument]
		private ParameterType _ParameterType = ParameterType.Unknown;

#if ARBOR_DOC_JA
		/// <summary>
		/// Listの入力スロット
		/// </summary>
#else
		/// <summary>
		/// List input slot
		/// </summary>
#endif
		[SerializeField]
		[HideSlotFields]
		private InputSlotTypable _Input = new InputSlotTypable();

#if ARBOR_DOC_JA
		/// <summary>
		/// 結果の出力スロット。インデックスが見つからなかった場合は-1を返す。
		/// </summary>
#else
		/// <summary>
		/// Resulting output slot. Returns -1 if no index was found.
		/// </summary>
#endif
		[SerializeField]
		private OutputSlotInt _Output = new OutputSlotInt();

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
		private ParameterList _ParameterList = new ParameterList();

		// Use this for calculate
		public override void OnCalculate()
		{
			System.Type elementType = _ElementType.type;

			if (elementType == null)
			{
				if (!string.IsNullOrEmpty(_ElementType.assemblyTypeName))
				{
					Debug.LogError("Type not found. It may have been deleted or renamed : " + this, this);
				}
				else
				{
					Debug.LogError("The element type is not specified : " + this, this);
				}

				return;
			}

			ParameterType parameterType = ArborEventUtility.GetParameterType(elementType, true);

			if (_ParameterType != parameterType)
			{
				Debug.LogError("The parameter type has changed : " + this, this);
				return;
			}

			object array = null;
			if (!_Input.GetValue(ref array))
			{
				return;
			}

			IList list = _ParameterList.GetParameterList(_ParameterType);
			if (list != null)
			{
				int count = list.Count;
				if (count == 0)
				{
					return;
				}

				IValueContainer valueContainer = list[0] as IValueContainer;

				object value = valueContainer.GetValue();

				int index = ListAccessor.Get(elementType).IndexOf(array, value);
				_Output.SetValue(index);
			}
			else
			{
				_Output.SetValue(-1);
			}
		}
	}
}