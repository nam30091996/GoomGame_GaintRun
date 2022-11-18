//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor
{
#if !NETFX_CORE
	[System.Reflection.Obfuscation(Exclude = true)]
#endif
	[System.Serializable]
	internal sealed class ParameterLegacy
	{
		public ParameterContainerInternal container = null;

		public int id = 0;

		public Parameter.Type type = Parameter.Type.Int;

		public string name = "";

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("intValue")]
		internal int _IntValue = 0;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("longValue")]
		internal long _LongValue = 0L;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("floatValue")]
		internal float _FloatValue = 0f;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("boolValue")]
		internal bool _BoolValue = false;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("stringValue")]
		internal string _StringValue = "";

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("gameObjectValue")]
		internal GameObject _GameObjectValue = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("vector2Value")]
		internal Vector2 _Vector2Value = Vector2.zero;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("vector3Value")]
		internal Vector3 _Vector3Value = Vector3.zero;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[EulerAngles]
		[FormerlySerializedAs("quaternionValue")]
		internal Quaternion _QuaternionValue = Quaternion.identity;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("rectValue")]
		internal Rect _RectValue = new Rect();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("boundsValue")]
		internal Bounds _BoundsValue = new Bounds();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("colorValue")]
		internal Color _ColorValue = Color.white;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("objectReferenceValue")]
		internal Object _ObjectReferenceValue = null;

		public ClassTypeReference referenceType = new ClassTypeReference();
	}
}
