//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Component型の入力スロット(ジェネリック)
	/// </summary>
#else
	/// <summary>
	/// Component type of input slot(Generic)
	/// </summary>
#endif
	public class InputSlotComponent<T> : InputSlot<T> where T : Component
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// <see cref="InputSlotComponent{T}"/>を<see cref="InputSlotComponent"/>にキャスト。
		/// </summary>
		/// <param name="inputSlot"><see cref="InputSlotComponent{T}"/></param>
#else
		/// <summary>
		/// Cast <see cref="InputSlotComponent{T}"/> to <see cref="InputSlotComponent"/>.
		/// </summary>
		/// <param name="inputSlot"><see cref="InputSlotComponent{T}"/></param>
#endif
		public static explicit operator InputSlotComponent(InputSlotComponent<T> inputSlot)
		{
			InputSlotComponent inputSlotComponent = new InputSlotComponent();
			inputSlotComponent.Copy(inputSlot);
			return inputSlotComponent;
		}
	}
}