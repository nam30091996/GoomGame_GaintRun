//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.BehaviourTree
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ビヘイビアツリーのコア部分。<br/>
	/// GameObjectにアタッチして使用する。
	/// </summary>
	/// <remarks>
	/// Open EditorボタンをクリックとArbor Editor Windowが開く。
	/// </remarks>
#else
	/// <summary>
	/// Core part of BehaviourTree.<br/>
	/// Is used by attaching to GameObject.
	/// </summary>
	/// <remarks>
	/// Click on the Open Editor button to open the Arbor Editor Window.
	/// </remarks>
#endif
	[AddComponentMenu("Arbor/BehaviourTree", 10)]
	[BuiltInComponent]
	[HelpURL(ArborReferenceUtility.componentUrl + "behaviourtree.html")]
#if UNITY_2018_1_OR_NEWER
	[ExcludeFromPreset]
#endif
	public sealed class BehaviourTree : BehaviourTreeInternal
	{
	}
}