﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Component型の出力スロット
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Component type of output slot
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class OutputSlotComponent : OutputSlot<Component>
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[ClassComponent]
		[HideSlotFields]
		private ClassTypeReference _Type = new ClassTypeReference(typeof(Component));

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットに格納されるデータの型
		/// </summary>
#else
		/// <summary>
		/// The type of data stored in the slot
		/// </summary>
#endif
		public override System.Type dataType
		{
			get
			{
				return _Type.type ?? typeof(Component);
			}
		}
	}
}