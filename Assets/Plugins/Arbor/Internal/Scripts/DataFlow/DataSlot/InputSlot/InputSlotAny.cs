//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 型を指定する入力スロットクラス
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="ClassTypeConstraintAttribute" /></description></item>
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Input slot class specifying type
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="ClassTypeConstraintAttribute" /></description></item>
	/// <item><description><see cref="SlotTypeAttribute" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class InputSlotAny : InputSlotBase
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
		/// InputSlotAnyデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// InputSlotAny default constructor
		/// </summary>
#endif
		public InputSlotAny()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// InputSlotAnyコンストラクタ
		/// </summary>
		/// <param name="inputSlot">コピー元の入力スロット</param>
#else
		/// <summary>
		/// InputSlotAny constructor
		/// </summary>
		/// <param name="inputSlot">Copy source input slot</param>
#endif
		internal InputSlotAny(InputSlotBase inputSlot)
		{
			Copy(inputSlot);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// InputSlotAnyコンストラクタ
		/// </summary>
		/// <param name="targetType">入力の型</param>
#else
		/// <summary>
		/// InputSlotAny constructor
		/// </summary>
		/// <param name="targetType">Input type</param>
#endif
		[System.Obsolete("use ClassExtendsAttribute or SlotTypeAttribute in the field.", true)]
		public InputSlotAny(System.Type targetType)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を取得する
		/// </summary>
		/// <param name="value">取得する値</param>
		/// <returns>値が取得できたらtrueを返す。</returns>
#else
		/// <summary>
		/// Get the value
		/// </summary>
		/// <param name="value">The value you get</param>
		/// <returns>Returns true if the value can be obtained.</returns>
#endif
		public bool GetValue(ref object value)
		{
			DataBranch b = branch;

			if (b == null)
			{
				return false;
			}

			value = b.value;

			if (!b.isUsed)
			{
				return false;
			}

			value = DynamicReflection.DynamicUtility.Rebox(value);

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を取得する
		/// </summary>
		/// <param name="value">取得する値</param>
		/// <returns>ブランチが接続されているかどうか。</returns>
#else
		/// <summary>
		/// Get the value
		/// </summary>
		/// <param name="value">The value you get</param>
		/// <returns>Whether the branch is connected.</returns>
#endif
		public bool GetValue<T>(ref T value)
		{
			object obj = null;
			if (GetValue(ref obj))
			{
				value = (T)obj;
				return true;
			}

			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値のobjectを取得する。
		/// </summary>
		/// <returns>値のobjectを返す。</returns>
#else
		/// <summary>
		/// Get the value object.
		/// </summary>
		/// <returns>Returns the value object.</returns>
#endif
		protected override object GetValueObject()
		{
			object value = null;
			if (GetValue(ref value))
			{
				return value;
			}
			return null;
		}
	}
}