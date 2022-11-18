//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System;

namespace Arbor.Internal
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ドキュメントから隠す属性
	/// </summary>
#else
	/// <summary>
	/// Attributes to hide in documents
	/// </summary>
#endif
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class HideInDocument : Attribute
	{
	}
}