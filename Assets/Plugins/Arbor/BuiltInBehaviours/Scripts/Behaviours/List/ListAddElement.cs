//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.StateMachine.StateBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Listに要素を追加する。
	/// </summary>
	/// <remarks>
	/// IL2CPPなどのAOT環境では、List&lt;指定した型&gt;がコード上で使用していないと正常動作しない可能性があります。<br />
	/// 詳しくは、<a href="https://caitsithware.com/assets/arbor/docs/ja/manual/dataflow/list.html#AOTRestrictions">事前コンパイル(AOT)での制限</a>を参照してください。
	/// </remarks>
#else
	/// <summary>
	/// Add elements to List.
	/// </summary>
	/// <remarks>
	/// In an AOT environment such as IL2CPP, List&lt;specified type&gt; may not work properly unless it is used in the code.<br />
	/// See <a href="https://caitsithware.com/assets/arbor/docs/en/manual/dataflow/list.html#AOTRestrictions">Ahead-of-Time (AOT) Restrictions</a> for more information.
	/// </remarks>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("List/List.AddElement")]
	[BehaviourTitle("List.AddElement", useNicifyName = false)]
	[BuiltInBehaviour]
	public sealed class ListAddElement : ListElementBase
	{
		protected override object OnExecute(System.Type elementType, object array, object value, ListInstanceType outputType)
		{
			return ListAccessor.Get(elementType).AddElement(array, value, outputType);
		}
	}
}