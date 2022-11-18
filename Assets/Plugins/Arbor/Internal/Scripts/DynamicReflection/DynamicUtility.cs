//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Reflection;

namespace Arbor.DynamicReflection
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 動的な型のユーティリティクラス
	/// </summary>
#else
	/// <summary>
	/// Dynamic type utility class
	/// </summary>
#endif
	public static class DynamicUtility
	{
#if NETFX_CORE
		private static readonly DynamicMethod s_MemberwiseClone = null;
#else
		private static readonly System.Func<object, object> s_MemberwiseClone = null;
#endif

		static DynamicUtility()
		{
			MethodInfo memberwiseCloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
#if NETFX_CORE
			s_MemberwiseClone = DynamicMethod.GetMethod(memberwiseCloneMethod);
#else
			s_MemberwiseClone = (System.Func<object, object>)System.Delegate.CreateDelegate(typeof(System.Func<object, object>), memberwiseCloneMethod);
#endif
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// オブジェクトをキャストする。
		/// </summary>
		/// <param name="obj">キャストするオブジェクト</param>
		/// <param name="type">キャストする型</param>
		/// <param name="ignoreThrowException">例外を無視するフラグ</param>
		/// <returns>キャストされた値</returns>
#else
		/// <summary>
		/// Cast the object.
		/// </summary>
		/// <param name="obj">The object to cast</param>
		/// <param name="type">Casting type</param>
		/// <param name="ignoreThrowException">Flag to ignore exceptions</param>
		/// <returns>Casted value</returns>
#endif
		public static object Cast(object obj, System.Type type, bool ignoreThrowException = true)
		{
			if (obj == null || type == null || type == typeof(void))
			{
				return null;
			}

			var objType = obj.GetType();

			if (objType == type)
			{
				return obj;
			}

			if (!TypeUtility.IsValueType(type))
			{
				if (TypeUtility.IsAssignableFrom(type, objType) || TypeUtility.IsAssignableFrom(objType, type))
				{
					return obj;
				}
				return null;
			}

			try
			{
				if (EnumFieldUtility.IsEnum(type))
				{
					return System.Enum.ToObject(type, obj);
				}

				return System.Convert.ChangeType(obj, type);
			}
			catch (System.Exception ex)
			{
				if (ignoreThrowException)
				{
					Debug.LogException(ex);
				}
				else
				{
					throw;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 型のデフォルト値を返す。
		/// </summary>
		/// <param name="type">デフォルト値の型</param>
		/// <returns>デフォルト値</returns>
#else
		/// <summary>
		/// Returns the default value of type.
		/// </summary>
		/// <param name="type">Default value type</param>
		/// <returns>Default value</returns>
#endif
		public static object GetDefault(System.Type type)
		{
			if (type == null || type == typeof(void) || !TypeUtility.IsValueType(type) || System.Nullable.GetUnderlyingType(type) != null)
			{
				return null;
			}
			return System.Activator.CreateInstance(type);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// objectを再ボックス化し値をコピーする。
		/// </summary>
		/// <param name="obj">object</param>
		/// <returns>再ボックス化したobject</returns>
		/// <remarks>値型ではない場合はコピーされない。</remarks>
#else
		/// <summary>
		/// Rebox object and copy the value.
		/// </summary>
		/// <param name="obj">object</param>
		/// <returns>Reboxed object</returns>
		/// <remarks>If it is not a value type, it is not copied.</remarks>
#endif
		public static object Rebox(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			System.Type type = obj.GetType();

			if (type == null || !TypeUtility.IsValueType(type))
			{
				return obj;
			}

			try
			{
#if NETFX_CORE
				obj = s_MemberwiseClone.Invoke(obj, null);
#else
				obj = s_MemberwiseClone(obj);
#endif
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}

			return obj;
		}
	}
}