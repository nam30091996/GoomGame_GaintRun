//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 更新タイプ。
	/// </summary>
#else
	/// <summary>
	/// Update type.
	/// </summary>
#endif
	[Internal.Documentable]
	public enum UpdateType
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 毎フレーム更新。
		/// </summary>
#else
		/// <summary>
		/// Update every frame.
		/// </summary>
#endif
		EveryFrame,

#if ARBOR_DOC_JA
		/// <summary>
		/// 秒を指定して更新。
		/// </summary>
#else
		/// <summary>
		/// Updated by specifying seconds.
		/// </summary>
#endif
		SpecifySeconds,

#if ARBOR_DOC_JA
		/// <summary>
		/// 手動で更新。<br/>
		/// 更新方法はComponentのスクリプトリファレンスを参照してください。
		/// </summary>
#else
		/// <summary>
		/// Update manually.<br/>
		/// Refer to the script reference of Component for the update method.
		/// </summary>
#endif
		Manual,
	}
}