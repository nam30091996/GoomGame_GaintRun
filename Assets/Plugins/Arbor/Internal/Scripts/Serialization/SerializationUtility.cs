//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor.Serialization
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Unityシリアライズに関するUtilityクラス
	/// </summary>
#else
	/// <summary>
	/// Utility class for Unity serialization
	/// </summary>
#endif
	public static class SerializationUtility
	{
		private static readonly HashSet<System.Type> s_SerializablePrimitiveTypes;
		private static readonly HashSet<System.Type> s_SerializableUnityTypes;
		private static readonly List<System.Type> s_SerializableStructs;
		private static readonly System.Type s_ExposedReferenceType;

		static SerializationUtility()
		{
			s_SerializablePrimitiveTypes = new HashSet<System.Type>() {
				typeof(sbyte),
				typeof(byte),
				typeof(char),
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(float),
				typeof(double),
				typeof(bool),
				typeof(string)
			};

			s_SerializableUnityTypes = new HashSet<System.Type>()
			{
				typeof(Vector2),
				typeof(Vector3),
				typeof(Vector4),
				typeof(Rect),
				typeof(Quaternion),
				typeof(Matrix4x4),
				typeof(Color),
				typeof(Color32),
				typeof(LayerMask),
				typeof(Bounds),
#if !ARBOR_DLL && UNITY_2017_2_OR_NEWER
				typeof(RectInt),
				typeof(BoundsInt),
				typeof(Vector2Int),
				typeof(Vector3Int),
#endif
			};

			s_SerializableStructs = new List<System.Type>()
			{
				typeof(AnimationCurve),
				typeof(Color32),
				typeof(Gradient),
				typeof(GUIStyle),
				typeof(RectOffset),
				typeof(Matrix4x4),
#if !ARBOR_DLL && UNITY_2017_1_OR_NEWER
				typeof(PropertyName),
#endif
			};

#if ARBOR_DLL
			System.Reflection.Assembly assembly = System.Reflection.Assembly.Load( "UnityEngine.dll" );

			System.Type rectIntType = assembly.GetType("UnityEngine.RectInt");
			if (rectIntType != null)
			{
				s_SerializableUnityTypes.Add(rectIntType);
			}

			System.Type boundsIntType = assembly.GetType("UnityEngine.BoundsInt");
			if (boundsIntType != null)
			{
				s_SerializableUnityTypes.Add(boundsIntType);
			}

			System.Type vector2IntType = assembly.GetType("UnityEngine.Vector2Int");
			if (vector2IntType != null)
			{
				s_SerializableUnityTypes.Add(vector2IntType);
			}

			System.Type vector3IntType = assembly.GetType("UnityEngine.Vector3Int");
			if (vector3IntType != null)
			{
				s_SerializableUnityTypes.Add(vector3IntType);
			}

			System.Type propertyNameType = assembly.GetType( "UnityEngine.PropertyName" );
			if( propertyNameType != null )
			{
				s_SerializableStructs.Add(propertyNameType);
			}

			s_ExposedReferenceType = assembly.GetType( "UnityEngine.ExposedReference`1" );
#elif UNITY_2017_1_OR_NEWER
			s_ExposedReferenceType = typeof(ExposedReference<>);
#else
			s_ExposedReferenceType = null;
#endif
		}

		private static bool IsSerializablePrimitive(System.Type type)
		{
			return s_SerializablePrimitiveTypes.Contains(type);
		}

		private static bool IsGenericDictionary(System.Type type)
		{
			return TypeUtility.IsGeneric(type, typeof(Dictionary<,>));
		}

		private static bool IsUnityEngineObject(System.Type type)
		{
			return type == typeof(Object) || TypeUtility.IsSubclassOf(type, typeof(Object));
		}

		private static bool IsSerializableUnityStruct(System.Type type)
		{
			foreach (System.Type structType in s_SerializableStructs)
			{
				if (TypeUtility.IsAssignableFrom(structType, type))
					return true;
			}

			return false;
		}

		private static bool IsNonSerialized(System.Type type)
		{
			if (type == null)
				return true;
			if (TypeUtility.IsEnum(type))
				return true;
			if (TypeUtility.ContainsGenericParameters(type))
				return true;
			if (type == typeof(object))
				return true;
			if (type.FullName.StartsWith("System.")) //can this be done better?
				return true;
			if (type.IsArray)
				return true;
			if (type == typeof(MonoBehaviour))
				return true;
			if (type == typeof(ScriptableObject))
				return true;
			return false;
		}

		private static bool ShouldHaveHadSerializableAttribute(System.Type type)
		{
			return s_SerializableUnityTypes.Contains(type);
		}

		private static bool ShouldImplementIDeserializable(System.Type type)
		{
			if (s_ExposedReferenceType != null && type == s_ExposedReferenceType)
			{
				return true;
			}

			if (IsNonSerialized(type))
			{
				return false;
			}

			if (TypeUtility.IsGenericType(type))
			{
				if (s_ExposedReferenceType != null && type.GetGenericTypeDefinition() == s_ExposedReferenceType)
				{
					return true;
				}
#if !UNITY_2020_1_OR_NEWER
				return false;
#endif
			}

			try
			{
				if (ShouldHaveHadSerializableAttribute(type))
				{
					return true;
				}

				bool isSerializable = TypeUtility.IsSerializable(type) && !AttributeHelper.HasAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(TypeUtility.GetMemberInfo(type));

				if (TypeUtility.IsValueType(type))
				{
					return isSerializable;
				}
				else
				{
					return isSerializable || TypeUtility.IsSubclassOf(type, typeof(MonoBehaviour)) || TypeUtility.IsSubclassOf(type, typeof(ScriptableObject));
				}
			}
			catch (System.Exception)
			{
				return false;
			}
		}

		private static bool IsGenericList(System.Type type)
		{
			return TypeUtility.IsGeneric(type, typeof(List<>));
		}

		private static bool IsSerializableArray(System.Type type)
		{
			return IsSupportedArray(type) && IsSerializableType(ElementTypeOfArray(type));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// サポートしている配列かどうかを返す。
		/// </summary>
		/// <param name="type">判定する型</param>
		/// <returns>サポートしている配列であればtrueを返す。</returns>
#else
		/// <summary>
		/// Returns whether it is an array supported.
		/// </summary>
		/// <param name="type">Determining type</param>
		/// <returns>Returns true if the array is supported.</returns>
#endif
		public static bool IsSupportedArray(System.Type type)
		{
			if (!(type.IsArray || IsGenericList(type)))
				return false;

			// We don't support arrays like byte[,] etc
			if (type.IsArray && type.GetArrayRank() > 1)
				return false;

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 配列の要素の型を返す。
		/// </summary>
		/// <param name="type">配列の型</param>
		/// <returns>配列の要素の型</returns>
#else
		/// <summary>
		/// Returns the type of the array element.
		/// </summary>
		/// <param name="type">Array type</param>
		/// <returns>The type of the array element</returns>
#endif
		public static System.Type ElementTypeOfArray(System.Type type)
		{
			if (type.IsArray)
			{
				return type.GetElementType();
			}

			return TypeUtility.GetGenericArguments(type)[0];
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 配列であれば要素の型を返す。
		/// </summary>
		/// <param name="type">型</param>
		/// <returns>配列であれば要素の型を返す。そうでなければtypeをそのまま返す。</returns>
#else
		/// <summary>
		/// If it is an array it returns the type of the element.
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns>If it is an array it returns the type of the element. Otherwise return type unchanged.</returns>
#endif
		public static System.Type ElementType(System.Type type)
		{
			return IsSupportedArray(type) ? ElementTypeOfArray(type) : type;
		}

		private static bool IsSerializableType(System.Type type)
		{
			if (IsGenericDictionary(type))
				return false;

			return IsSerializablePrimitive(type)
				|| TypeUtility.IsEnum(type)
				|| IsUnityEngineObject(type)
				|| IsSerializableUnityStruct(type)
				|| ShouldImplementIDeserializable(type);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// フィールドの型がシリアライズ可能かどうかを返す。
		/// </summary>
		/// <param name="type">フィールドの型</param>
		/// <returns>シリアライズ可能であればtrueを返す。</returns>
#else
		/// <summary>
		/// Returns whether the field type is serializable.
		/// </summary>
		/// <param name="type">Field type</param>
		/// <returns>Returns true if serializable.</returns>
#endif
		public static bool IsSerializableFieldType(System.Type type)
		{
			return IsSerializableType(type) || IsSerializableArray(type);
		}

#if UNITY_2019_3_OR_NEWER
		private static bool IsSupportedReferenceElement(System.Type type)
		{
			return TypeUtility.IsClass(type) || TypeUtility.IsInterface(type);
		}

		public static bool IsSupportedReferenceType(System.Type type)
		{
			if (type.IsArray && type.GetArrayRank() == 1)
			{
				return IsSupportedReferenceElement(ElementTypeOfArray(type));
			}

			if (TypeUtility.IsGenericType(type))
			{
				return IsGenericList(type) && IsSupportedReferenceElement(ElementTypeOfArray(type));
			}

			return IsSupportedReferenceElement(type);
		}
#endif

#if ARBOR_DOC_JA
		/// <summary>
		/// フィールドがシリアライズ可能かどうかを返す。
		/// </summary>
		/// <param name="fieldInfo">フィールド</param>
		/// <returns>シリアライズ可能であればtrueを返す。</returns>
#else
		/// <summary>
		/// Returns whether the field is serializable.
		/// </summary>
		/// <param name="fieldInfo">FieldInfo</param>
		/// <returns>Returns true if serializable.</returns>
#endif
		public static bool IsSerializableField(System.Reflection.FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				return false;
			}

			if (fieldInfo.IsSpecialName)
			{
				return false;
			}

#if UNITY_2019_3_OR_NEWER
			if (AttributeHelper.HasAttribute<SerializeReference>(fieldInfo))
			{
				return IsSupportedReferenceType(fieldInfo.FieldType);
			}
#endif

			if (!fieldInfo.IsPublic && !AttributeHelper.HasAttribute<SerializeField>(fieldInfo))
			{
				return false;
			}

			if (!IsSerializableFieldType(fieldInfo.FieldType))
			{
				return false;
			}

			return true;
		}
	}
}