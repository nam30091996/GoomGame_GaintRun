//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// パラメータの参照。
	/// </summary>
#else
	/// <summary>
	/// Reference parameters.
	/// </summary>
#endif
	[System.Serializable]
	public class ParameterReference : IValueContainer
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ParameterReferenceType _Type;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("container")]
		private ParameterContainerBase _Container;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[ClassExtends(typeof(ParameterContainerBase))]
		private InputSlotComponent _Slot = new InputSlotComponent();

#if ARBOR_DOC_JA
		/// <summary>
		/// ID。
		/// </summary>
#else
		/// <summary>
		/// ID.
		/// </summary>
#endif
		public int id;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータ名。
		/// </summary>
#else
		/// <summary>
		/// Paramenter name.
		/// </summary>
#endif
		public string name;

#if ARBOR_DOC_JA
		/// <summary>
		/// ParameterContainerの参照タイプ
		/// </summary>
#else
		/// <summary>
		/// Reference type of ParameterContainer
		/// </summary>
#endif
		public ParameterReferenceType type
		{
			get
			{
				return _Type;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 格納しているコンテナ。
		/// </summary>
#else
		/// <summary>
		/// Is stored to that container.
		/// </summary>
#endif
		public ParameterContainerBase container
		{
			get
			{
				switch (_Type)
				{
					case ParameterReferenceType.Constant:
						return _Container;
					case ParameterReferenceType.DataSlot:
						return _Slot.GetValue<ParameterContainerBase>();
				}

				return null;
			}
			set
			{
				_Container = value;
				if (_Container != null && _Type == ParameterReferenceType.DataSlot)
				{
					ParameterContainerInternal parameterContainer = _Container.container;
					if (parameterContainer != null)
					{
						id = parameterContainer.GetParamID(name);
					}
				}
				_Type = ParameterReferenceType.Constant;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 参照する<see cref="Parameter.Type"/>を返す。
		/// </summary>
#else
		/// <summary>
		/// Returns the <see cref="Parameter.Type"/> to be referenced.
		/// </summary>
#endif
		[System.Obsolete("use Arbor.Internal.ParameterTypeAttribute.")]
		public virtual Parameter.Type? referenceType
		{
			get
			{
				Internal.ParameterTypeAttribute parameterTypeAttribute = AttributeHelper.GetAttribute<Internal.ParameterTypeAttribute>(TypeUtility.GetMemberInfo(this.GetType()));
				if (parameterTypeAttribute != null)
				{
					return parameterTypeAttribute.parameterType;
				}
				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータを取得する。存在しない場合はnull。
		/// </summary>
#else
		/// <summary>
		/// Get the parameters. null if it does not exist.
		/// </summary>
#endif
		public Parameter parameter
		{
			get
			{
				ParameterContainerBase containerBase = container;
				if (containerBase == null)
				{
					return null;
				}
				ParameterContainerInternal parameterContainer = containerBase.container;

				if (parameterContainer == null)
				{
					return null;
				}

				return (_Type == ParameterReferenceType.Constant) ? parameterContainer.GetParam(id) : parameterContainer.GetParam(name);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 定数指定しているコンテナ
		/// </summary>
#else
		/// <summary>
		/// Container specifying a constant
		/// </summary>
#endif
		public ParameterContainerBase constantContainer
		{
			get
			{
				return _Container;
			}
			set
			{
				_Container = value;
			}
		}

		internal void Copy(ParameterReference parameterReference)
		{
			_Type = parameterReference._Type;
			_Container = parameterReference._Container;
			_Slot = parameterReference._Slot;
		}

		object IValueContainer.GetValue()
		{
			Parameter parameter = this.parameter;
			if (parameter != null)
			{
				return parameter.value;
			}

			return null;
		}
	}
}
