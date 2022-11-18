//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Events
{
#if !NETFX_CORE
	[System.Reflection.Obfuscation(Exclude = true)]
#endif
	[System.Serializable]
	internal sealed class Argument
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ClassTypeReference _Type = new ClassTypeReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private string _Name = "";

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ArgumentAttributes _Attributes = ArgumentAttributes.None;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ParameterType _ParameterType = ParameterType.Unknown;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private int _ParameterIndex = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private int _OutputSlotIndex = 0;

		public System.Type type
		{
			get
			{
				return _Type.type;
			}
		}

		public ParameterType parameterType
		{
			get
			{
				return _ParameterType;
			}
		}

		public string name
		{
			get
			{
				return _Name;
			}
			internal set
			{
				_Name = value;
			}
		}

		public ArgumentAttributes attributes
		{
			get
			{
				return _Attributes;
			}
		}

		public bool isOut
		{
			get
			{
				return (_Attributes & ArgumentAttributes.Out) == ArgumentAttributes.Out;
			}
		}

		public int parameterIndex
		{
			get
			{
				return _ParameterIndex;
			}
		}

		public int outputSlotIndex
		{
			get
			{
				return _OutputSlotIndex;
			}
		}

		internal IValueContainer valueContainer
		{
			get;
			set;
		}

		internal OutputSlotTypable outputSlot
		{
			get;
			set;
		}
	}
}