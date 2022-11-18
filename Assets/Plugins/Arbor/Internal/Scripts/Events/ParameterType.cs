//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor.Events
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ArborEventで使用できる引数の型のタイプ
	/// </summary>
#else
	/// <summary>
	/// Types of argument types available for ArborEvent
	/// </summary>
#endif
	public enum ParameterType
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// int型
		/// </summary>
#else
		/// <summary>
		/// int type
		/// </summary>
#endif
		Int,

#if ARBOR_DOC_JA
		/// <summary>
		/// long型
		/// </summary>
#else
		/// <summary>
		/// long type
		/// </summary>
#endif
		Long,

#if ARBOR_DOC_JA
		/// <summary>
		/// float型
		/// </summary>
#else
		/// <summary>
		/// float type
		/// </summary>
#endif
		Float,

#if ARBOR_DOC_JA
		/// <summary>
		/// bool型
		/// </summary>
#else
		/// <summary>
		/// bool type
		/// </summary>
#endif
		Bool,

#if ARBOR_DOC_JA
		/// <summary>
		/// string型
		/// </summary>
#else
		/// <summary>
		/// string type
		/// </summary>
#endif
		String,

#if ARBOR_DOC_JA
		/// <summary>
		/// Vector2型
		/// </summary>
#else
		/// <summary>
		/// Vector2 type
		/// </summary>
#endif
		Vector2,

#if ARBOR_DOC_JA
		/// <summary>
		/// Vector3型
		/// </summary>
#else
		/// <summary>
		/// Vector3 type
		/// </summary>
#endif
		Vector3,

#if ARBOR_DOC_JA
		/// <summary>
		/// Quaternion型
		/// </summary>
#else
		/// <summary>
		/// Quaternion type
		/// </summary>
#endif
		Quaternion,

#if ARBOR_DOC_JA
		/// <summary>
		/// Rect型
		/// </summary>
#else
		/// <summary>
		/// Rect type
		/// </summary>
#endif
		Rect,

#if ARBOR_DOC_JA
		/// <summary>
		/// Bounds型
		/// </summary>
#else
		/// <summary>
		/// Bounds type
		/// </summary>
#endif
		Bounds,

#if ARBOR_DOC_JA
		/// <summary>
		/// Color型
		/// </summary>
#else
		/// <summary>
		/// Color type
		/// </summary>
#endif
		Color,

#if ARBOR_DOC_JA
		/// <summary>
		/// GameObject型
		/// </summary>
#else
		/// <summary>
		/// GameObject type
		/// </summary>
#endif
		GameObject,

#if ARBOR_DOC_JA
		/// <summary>
		/// Component型
		/// </summary>
#else
		/// <summary>
		/// Component type
		/// </summary>
#endif
		Component,

#if ARBOR_DOC_JA
		/// <summary>
		/// enum型
		/// </summary>
#else
		/// <summary>
		/// enum type
		/// </summary>
#endif
		Enum,

#if ARBOR_DOC_JA
		/// <summary>
		/// AssetObject型
		/// </summary>
#else
		/// <summary>
		/// AssetObject type
		/// </summary>
#endif
		AssetObject,

#if ARBOR_DOC_JA
		/// <summary>
		/// データスロット
		/// </summary>
		/// <remarks>ArborEventUtility.GetParameterTypeメソッドがUnknownを返した場合に使用する。</remarks>
#else
		/// <summary>
		/// Data slot
		/// </summary>
		/// <remarks>Used when the ArborEventUtility.GetParameterType method returns Unknown.</remarks>
#endif
		Slot = 0x1000,

#if ARBOR_DOC_JA
		/// <summary>
		/// 不明
		/// </summary>
#else
		/// <summary>
		/// Unknown
		/// </summary>
#endif
		Unknown = 0xFFFFFFF,
	}
}