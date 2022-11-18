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
	/// Listに要素が含まれているかを判定する。
	/// </summary>
	/// <remarks>
	/// IL2CPPなどのAOT環境では、List&lt;指定した型&gt;がコード上で使用していないと正常動作しない可能性があります。<br />
	/// 詳しくは、<a href="https://caitsithware.com/assets/arbor/docs/ja/manual/dataflow/list.html#AOTRestrictions">事前コンパイル(AOT)での制限</a>を参照してください。
	/// </remarks>
#else
	/// <summary>
	/// Determines if the List contains elements.
	/// </summary>
	/// <remarks>
	/// In an AOT environment such as IL2CPP, List&lt;specified type&gt; may not work properly unless it is used in the code.<br />
	/// See <a href="https://caitsithware.com/assets/arbor/docs/en/manual/dataflow/list.html#AOTRestrictions">Ahead-of-Time (AOT) Restrictions</a> for more information.
	/// </remarks>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("List/List.Contains")]
	[BehaviourTitle("List.Contains", useNicifyName = false)]
	[BuiltInBehaviour]
	public sealed class ListContainsCalculator : Calculator
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
		[HideInInspector]
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
		/// 結果の出力スロット
		/// </summary>
#else
		/// <summary>
		/// Resulting output slot
		/// </summary>
#endif
		[SerializeField]
		private OutputSlotBool _Output = new OutputSlotBool();

#if ARBOR_DOC_JA
		/// <summary>
		/// 判定する値
		/// </summary>
#else
		/// <summary>
		/// 
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

				bool contains = ListAccessor.Get(elementType).Contains(array, value);
				_Output.SetValue(contains);
			}
			else
			{
				_Output.SetValue(false);
			}
		}
	}
}