//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 型を指定する入力スロットクラス
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Input slot class specifying type
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="HideSlotFields" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[System.Serializable]
	public sealed class InputSlotTypable : InputSlotBase
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideSlotFields]
		private ClassTypeReference _Type = new ClassTypeReference();

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
				return _Type.type;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// InputSlotTypableのコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// InputSlotTypable constructor
		/// </summary>
#endif
		public InputSlotTypable()
		{
			_Type = new ClassTypeReference();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// InputSlotTypableのコンストラクタ
		/// </summary>
		/// <param name="type">スロットに格納するデータ型</param>
#else
		/// <summary>
		/// InputSlotTypable constructor
		/// </summary>
		/// <param name="type">Data type to be stored in the slot</param>
#endif
		public InputSlotTypable(System.Type type)
		{
			SetType(type);
		}

		internal void SetType(System.Type type)
		{
			_Type = new ClassTypeReference(type);
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