//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 出力スロットのジェネリッククラス
	/// </summary>
	/// <typeparam name="T">データの型</typeparam>
#else
	/// <summary>
	/// Generic class of the output slot
	/// </summary>
	/// <typeparam name="T">Type of data</typeparam>
#endif
	[System.Serializable]
	public class OutputSlot<T> : OutputSlotBase
	{
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
				return typeof(T);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を設定する
		/// </summary>
		/// <param name="value">設定する値</param>
#else
		/// <summary>
		/// Set the value
		/// </summary>
		/// <param name="value">The value to be set</param>
#endif
		public void SetValue(T value)
		{
			SetValueInternal(value);
		}
	}
}