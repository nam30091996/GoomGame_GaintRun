//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// どんな型も出力する出力スロットクラス
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Output slot class outputting any type
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class OutputSlotAny : OutputSlotBase
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
				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotAnyデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// OutputSlotAny default constructor
		/// </summary>
#endif
		public OutputSlotAny()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotAnyコンストラクタ
		/// </summary>
		/// <param name="outputSlot">コピー元の入力スロット</param>
#else
		/// <summary>
		/// OutputSlotAny constructor
		/// </summary>
		/// <param name="outputSlot">Copy source input slot</param>
#endif
		internal OutputSlotAny(OutputSlotBase outputSlot)
		{
			Copy(outputSlot);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OutputSlotAnyコンストラクタ
		/// </summary>
		/// <param name="targetType">出力の型</param>
#else
		/// <summary>
		/// OutputSlotAny constructor
		/// </summary>
		/// <param name="targetType">Output type</param>
#endif
		[System.Obsolete("use SlotTypeAttribute for the field.", true)]
		public OutputSlotAny(System.Type targetType)
		{
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
		public void SetValue(object value)
		{
			SetValueInternal(value);
		}
	}
}