//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 型を指定するパラメータ参照。
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
	/// Parameter reference specifying type.
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
	[Internal.Constraintable()]
	public sealed partial class AnyParameterReference : ParameterReference
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの型
		/// </summary>
#else
		/// <summary>
		/// Parameter Type
		/// </summary>
#endif
		[System.Obsolete]
		public System.Type parameterType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// タイプに応じた値を取得する。
		/// </summary>
#else
		/// <summary>
		/// Get values according to type.
		/// </summary>
#endif
		public object value
		{
			get
			{
				Parameter parameter = this.parameter;
				if (parameter != null)
				{
					return parameter.value;
				}

				return null;
			}
			set
			{
				Parameter parameter = this.parameter;
				if (parameter != null)
				{
					parameter.value = value;
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// AnyParameterReferenceの作成。
		/// </summary>
#else
		/// <summary>
		/// Create AnyParameterReference
		/// </summary>
#endif
		public AnyParameterReference()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		///AnyParameterReferenceの作成。
		/// </summary>
		/// <param name="parameterType">パラメータの型</param>
#else
		/// <summary>
		/// Create AnyParameterReference
		/// </summary>
		/// <param name="parameterType">Parameter Type</param>
#endif
		[System.Obsolete("use ClassExtendsAttribute or SlotTypeAttribute in the field.", true)]
		public AnyParameterReference(System.Type parameterType)
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// AnyParameterReferenceの作成。
		/// </summary>
		/// <param name="parameterReference">コピー元のParameterReference</param>
#else
		/// <summary>
		/// Create AnyParameterReference
		/// </summary>
		/// <param name="parameterReference">Copy Source ParameterReference</param>
#endif

		public AnyParameterReference(ParameterReference parameterReference)
		{
			Copy(parameterReference);
		}
	}
}