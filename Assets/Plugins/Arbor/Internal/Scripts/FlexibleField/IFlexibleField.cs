//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// FlexibleFieldのインターフェイス
	/// </summary>
#else
	/// <summary>
	/// FlexibleField interface
	/// </summary>
#endif
	public interface IFlexibleField : IValueContainer
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 値をobjectで返す。
		/// </summary>
		/// <returns>値のobject</returns>
#else
		/// <summary>
		/// Return the value as object.
		/// </summary>
		/// <returns>The value object</returns>
#endif
		object GetValueObject();
	}
}