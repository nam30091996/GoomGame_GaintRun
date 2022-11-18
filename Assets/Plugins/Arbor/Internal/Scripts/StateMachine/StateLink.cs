//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Stateの遷移先を格納するクラス。
	/// </summary>
#else
	/// <summary>
	/// Class that contains a transition destination State.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class StateLink
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 名前。入力しなかった場合はパスが表示される。
		/// </summary>
#else
		/// <summary>
		/// Name. The path is displayed if you do not enter.
		/// </summary>
#endif
		public string name;

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移先StateのID。
		/// </summary>
#else
		/// <summary>
		/// ID of transition destination State.
		/// </summary>
#endif
		public int stateID;

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移するタイミング
		/// </summary>
#else
		/// <summary>
		/// Transition timing.
		/// </summary>
#endif
		[FormerlySerializedAs("immediateTransition")]
		public TransitionTiming transitionTiming = TransitionTiming.LateUpdateDontOverwrite;

#if ARBOR_DOC_JA
		/// <summary>
		/// 即時遷移するかどうか。
		/// </summary>
#else
		/// <summary>
		/// Whether to immediately transition.
		/// </summary>
#endif
		[System.Obsolete("use transitionTiming.")]
		public bool immediateTransition
		{
			get
			{
				return transitionTiming == TransitionTiming.Immediate;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		[System.NonSerialized]
		public Bezier2D bezier = new Bezier2D();

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		public bool lineColorChanged = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		public Color lineColor = Color.white;

#if ARBOR_DOC_JA
		/// <summary>
		/// 遷移回数
		/// </summary>
#else
		/// <summary>
		/// Transition count.
		/// </summary>
#endif
		public uint transitionCount
		{
			get;
			set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// リルートかどうか。
		/// </summary>
#else
		/// <summary>
		/// Whether reroute.
		/// </summary>
#endif
		public bool isReroute
		{
			get;
			set;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// StateLink constructor
		/// </summary>
#endif
		public StateLink()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// StateLinkコピーコンストラクタ
		/// </summary>
		/// <param name="stateLink">コピー元StateLink</param>
#else
		/// <summary>
		/// StateLink copy constructor.
		/// </summary>
		/// <param name="stateLink">Copy source StateLink</param>
#endif
		public StateLink(StateLink stateLink)
		{
			name = stateLink.name;
			stateID = stateLink.stateID;
			transitionTiming = stateLink.transitionTiming;
			lineColorChanged = stateLink.lineColorChanged;
			lineColor = stateLink.lineColor;
		}
	}
}
