//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Attributeのヘルパークラス。
	/// </summary>
#else
	/// <summary>
	/// A helper class for Attribute.
	/// </summary>
#endif
	public static class AttributeHelper
	{
		private static readonly Dictionary<MemberInfo, Attribute[]> _Attributes = new Dictionary<MemberInfo, Attribute[]>();

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeを取得。
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attributes</returns>
#else
		/// <summary>
		/// Get Attributes.
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attributes</returns>
#endif
		public static Attribute[] GetAttributes(MemberInfo member)
		{
			Attribute[] attributes = null;
			if (!_Attributes.TryGetValue(member, out attributes))
			{
				attributes = member.GetCustomAttributes(typeof(Attribute), false) as Attribute[];
				_Attributes.Add(member, attributes);
			}

			return attributes;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeを取得
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <param name="targetType">取得する型</param>
		/// <returns>Attribute</returns>
#else
		/// <summary>
		/// Get Attribute (generic)
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <param name="targetType">Target Type</param>
		/// <returns>Attribute</returns>
#endif
		public static Attribute GetAttribute(MemberInfo member, System.Type targetType)
		{
			if (member == null)
			{
				return null;
			}

			Attribute[] attributes = GetAttributes(member);
			int attributeCount = attributes.Length;
			for (int index = 0; index < attributeCount; index++)
			{
				Attribute attribute = attributes[index];

				System.Type attributeType = attribute.GetType();

				if (attributeType == targetType || TypeUtility.IsSubclassOf(attributeType, targetType))
				{
					return attribute;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeを取得 (generic)
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attribute</returns>
#else
		/// <summary>
		/// Get Attribute (generic)
		/// </summary>
		/// <typeparam name="T">Target Type</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attribute</returns>
#endif
		public static T GetAttribute<T>(MemberInfo member) where T : Attribute
		{
			if (member == null)
			{
				return null;
			}

			Attribute[] attributes = GetAttributes(member);
			int attributeCount = attributes.Length;
			for (int index = 0; index < attributeCount; index++)
			{
				Attribute attribute = attributes[index];

				if (attribute is T)
				{
					return attribute as T;
				}
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeの配列を取得 (generic)
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attributeの配列</returns>
#else
		/// <summary>
		/// Get Attribute (generic)
		/// </summary>
		/// <typeparam name="T">Target Type</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attribute</returns>
#endif
		public static T[] GetAttributes<T>(MemberInfo member) where T : Attribute
		{
			if (member == null)
			{
				return null;
			}

			List<T> list = new List<T>();

			Attribute[] attributes = GetAttributes(member);
			int attributeCount = attributes.Length;
			for (int index = 0; index < attributeCount; index++)
			{
				T attribute = attributes[index] as T;

				if (attribute != null)
				{
					list.Add(attribute);
				}
			}

			return list.ToArray();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeがあるかどうか。
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <param name="targetType">取得する型</param>
		/// <returns>Attributeがあるかどうか。</returns>
#else
		/// <summary>
		/// Whether has Attribute
		/// </summary>
		/// <param name="member">MemberInfo</param>
		/// <param name="targetType">Target Type</param>
		/// <returns>Whether has attribute.</returns>
#endif
		public static bool HasAttribute(MemberInfo member, System.Type targetType)
		{
			return (GetAttribute(member, targetType) != null);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Attributeがあるかどうか。
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Attributeがあるかどうか。</returns>
#else
		/// <summary>
		/// Whether has Attribute
		/// </summary>
		/// <typeparam name="T">Target Type</typeparam>
		/// <param name="member">MemberInfo</param>
		/// <returns>Whether has attribute.</returns>
#endif
		public static bool HasAttribute<T>(MemberInfo member) where T : Attribute
		{
			return (GetAttribute<T>(member) != null);
		}
	}
}
