//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 値が格納されていることを示すインターフェイス
	/// </summary>
#else
	/// <summary>
	/// Interface that indicates that the value is stored
	/// </summary>
#endif
	public interface IValueContainer
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 値を取得する
		/// </summary>
		/// <returns>格納されている値</returns>
#else
		/// <summary>
		/// Get value
		/// </summary>
		/// <returns>Stored value</returns>
#endif
		object GetValue();
	}
}