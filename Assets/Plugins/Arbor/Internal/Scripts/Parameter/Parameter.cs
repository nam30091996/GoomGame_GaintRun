//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ParameterContainerに格納されるParameterのクラス。
	/// </summary>
#else
	/// <summary>
	/// Class of Parameter to be stored in the ParameterContainer.
	/// </summary>
#endif
	[System.Serializable]
	public sealed partial class Parameter
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータを変更した時に呼ばれるデリゲート。
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// Delegate called when changing Parameter.
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public delegate void DelegateOnChanged(Parameter parameter);

#if ARBOR_DOC_JA
		/// <summary>
		/// このパラメータが格納されているコンテナ。
		/// </summary>
#else
		/// <summary>
		/// Container this parameter is stored.
		/// </summary>
#endif
		public ParameterContainerInternal container;

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
		/// パラメータの型。
		/// </summary>
#else
		/// <summary>
		/// Type.
		/// </summary>
#endif
		public Type type;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの名前。
		/// </summary>
#else
		/// <summary>
		/// Name.
		/// </summary>
#endif
		public string name;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal int _ParameterIndex;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal bool _IsPublicSet = true;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal bool _IsPublicGet = true;

#if ARBOR_DOC_JA
		/// <summary>
		/// objectReferenceValueやEnumの型。
		/// </summary>
#else
		/// <summary>
		/// the type of objectReferenceValue and Enum.
		/// </summary>
#endif
		public ClassTypeReference referenceType = new ClassTypeReference();

#if ARBOR_DOC_JA
		/// <summary>
		/// このパラメータが外部グラフから設定可能かどうかを返す。(グラフ内パラメータ用)
		/// </summary>
#else
		/// <summary>
		/// Returns whether this parameter can be set from an external graph. (For parameters in graph)
		/// </summary>
#endif
		public bool isPublicSet
		{
			get
			{
				return _IsPublicSet;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// このパラメータが外部グラフから取得可能かどうかを返す。(グラフ内パラメータ用)
		/// </summary>
#else
		/// <summary>
		/// Returns whether this parameter can be get from an external graph. (For parameters in graph)
		/// </summary>
#endif
		public bool isPublicGet
		{
			get
			{
				return _IsPublicGet;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 指定したParameter.Typeで扱う値の型がIList&lt;&gt;であるかを判断する。
		/// </summary>
		/// <param name="type">パラメータのタイプ</param>
		/// <returns>値の型がIList&lt;&gt;であればtrueを返す。</returns>
#else
		/// <summary>
		/// Judges whether the type of the value handled by the specified Parameter.Type is IList&lt;&gt;.
		/// </summary>
		/// <param name="type">Parameter type</param>
		/// <returns>Returns true if the value type is IListIList&lt;&gt;.</returns>
#endif
		public static bool IsListType(Parameter.Type type)
		{
			var valueType = GetValueType(type);
			return TypeUtility.IsGeneric(valueType, typeof(IList<>));
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの値の型を取得する。
		/// </summary>
		/// <param name="type">パラメータのタイプ</param>
		/// <param name="referenceType">参照する型（Enum, Component, Variableで使用）</param>
		/// <returns>パラメータの値の型</returns>
#else
		/// <summary>
		/// Get the value type of the parameter.
		/// </summary>
		/// <param name="type">Type of parameter</param>
		/// <param name="referenceType">Reference type (used for Enum, Component, Variable)</param>
		/// <returns>Value type of the parameter</returns>
#endif
		public static System.Type GetValueType(Parameter.Type type, System.Type referenceType = null)
		{
			switch (type)
			{
				case Type.Int:
					return typeof(int);
				case Type.Long:
					return typeof(long);
				case Type.Float:
					return typeof(float);
				case Type.Bool:
					return typeof(bool);
				case Type.GameObject:
					return typeof(GameObject);
				case Type.String:
					return typeof(string);
				case Type.Enum:
					{
						if (referenceType != null)
						{
							return referenceType;
						}
						return typeof(System.Enum);
					}
				case Type.Vector2:
					return typeof(Vector2);
				case Type.Vector3:
					return typeof(Vector3);
				case Type.Quaternion:
					return typeof(Quaternion);
				case Type.Rect:
					return typeof(Rect);
				case Type.Bounds:
					return typeof(Bounds);
				case Type.Color:
					return typeof(Color);
				case Type.Transform:
					return typeof(Transform);
				case Type.RectTransform:
					return typeof(RectTransform);
				case Type.Rigidbody:
					return typeof(Rigidbody);
				case Type.Rigidbody2D:
					return typeof(Rigidbody2D);
				case Type.Component:
					{
						if (referenceType != null)
						{
							return referenceType;
						}
						return typeof(Component);
					}
				case Type.AssetObject:
					{
						if (referenceType != null)
						{
							return referenceType;
						}
						return typeof(Object);
					}
				case Type.Variable:
					{
						if (referenceType != null)
						{
							return referenceType;
						}
					}
					break;
				case Type.IntList:
					return typeof(IList<int>);
				case Type.LongList:
					return typeof(IList<long>);
				case Type.FloatList:
					return typeof(IList<float>);
				case Type.BoolList:
					return typeof(IList<bool>);
				case Type.StringList:
					return typeof(IList<string>);
				case Type.EnumList:
					if (referenceType != null)
					{
						return ListUtility.GetIListType(referenceType);
					}
					return typeof(IList<System.Enum>);
				case Type.Vector2List:
					return typeof(IList<Vector2>);
				case Type.Vector3List:
					return typeof(IList<Vector3>);
				case Type.QuaternionList:
					return typeof(IList<Quaternion>);
				case Type.RectList:
					return typeof(IList<Rect>);
				case Type.BoundsList:
					return typeof(IList<Bounds>);
				case Type.ColorList:
					return typeof(IList<Color>);
				case Type.GameObjectList:
					return typeof(IList<GameObject>);
				case Type.ComponentList:
					if (referenceType != null)
					{
						return ListUtility.GetIListType(referenceType);
					}
					return typeof(IList<Component>);
				case Type.AssetObjectList:
					if (referenceType != null)
					{
						return ListUtility.GetIListType(referenceType);
					}
					return typeof(IList<Object>);
				case Type.VariableList:
					{
						if (referenceType != null)
						{
							return ListUtility.GetIListType(referenceType);
						}
					}
					break;
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値の型を取得する。
		/// </summary>
#else
		/// <summary>
		/// Get value type.
		/// </summary>
#endif
		public System.Type valueType
		{
			get
			{
				System.Type referenceType = null;
				switch (type)
				{
					case Type.Variable:
						{
							VariableBase variable = Internal_GetObject() as VariableBase;
							if (variable != null)
							{
								return variable.valueType;
							}
						}
						break;
					case Type.VariableList:
						{
							VariableListBase variable = Internal_GetObject() as VariableListBase;
							if (variable != null)
							{
								return variable.valueType;
							}
						}
						break;
					default:
						referenceType = this.referenceType;
						break;
				}

				return GetValueType(type, referenceType);
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
				switch (type)
				{
					case Type.Int:
						return intValue;
					case Type.Long:
						return longValue;
					case Type.Float:
						return floatValue;
					case Type.Bool:
						return boolValue;
					case Type.GameObject:
						return gameObjectValue;
					case Type.String:
						return stringValue;
					case Type.Enum:
						{
							System.Type enumType = referenceType.type;
							if (EnumFieldUtility.IsEnum(enumType))
							{
								return enumValue;
							}

							return enumIntValue;
						}
					case Type.Vector2:
						return vector2Value;
					case Type.Vector3:
						return vector3Value;
					case Type.Quaternion:
						return quaternionValue;
					case Type.Rect:
						return rectValue;
					case Type.Bounds:
						return boundsValue;
					case Type.Color:
						return colorValue;
					case Type.Transform:
						return transformValue;
					case Type.RectTransform:
						return rectTransformValue;
					case Type.Rigidbody:
						return rigidbodyValue;
					case Type.Rigidbody2D:
						return rigidbody2DValue;
					case Type.Component:
						return componentValue;
					case Type.AssetObject:
						return objectReferenceValue;
					case Type.Variable:
						return variableValue;
					case Type.IntList:
						return intListValue;
					case Type.LongList:
						return longListValue;
					case Type.FloatList:
						return floatListValue;
					case Type.BoolList:
						return boolListValue;
					case Type.StringList:
						return stringListValue;
					case Type.EnumList:
						return Internal_GetEnumList();
					case Type.Vector2List:
						return vector2ListValue;
					case Type.Vector3List:
						return vector3ListValue;
					case Type.QuaternionList:
						return quaternionListValue;
					case Type.RectList:
						return rectListValue;
					case Type.BoundsList:
						return boundsListValue;
					case Type.ColorList:
						return colorListValue;
					case Type.GameObjectList:
						return gameObjectListValue;
					case Type.ComponentList:
						return Internal_GetComponentList();
					case Type.AssetObjectList:
						return Internal_GetAssetObjectList();
					case Type.VariableList:
						return variableListValue;
				}

				return null;
			}
			set
			{
				try
				{
					switch (type)
					{
						case Type.Int:
							intValue = (int)value;
							break;
						case Type.Long:
							longValue = (long)value;
							break;
						case Type.Float:
							floatValue = (float)value;
							break;
						case Type.Bool:
							boolValue = (bool)value;
							break;
						case Type.GameObject:
							gameObjectValue = (GameObject)value;
							break;
						case Type.String:
							stringValue = (string)value;
							break;
						case Type.Enum:
							if (value != null)
							{
								System.Type valueType = value.GetType();
								if (EnumFieldUtility.IsEnum(valueType))
								{
									enumValue = (System.Enum)value;
								}
								else if (valueType == typeof(int))
								{
									enumIntValue = System.Convert.ToInt32(value);
								}
							}
							break;
						case Type.Vector2:
							vector2Value = (Vector2)value;
							break;
						case Type.Vector3:
							vector3Value = (Vector3)value;
							break;
						case Type.Quaternion:
							quaternionValue = (Quaternion)value;
							break;
						case Type.Rect:
							rectValue = (Rect)value;
							break;
						case Type.Bounds:
							boundsValue = (Bounds)value;
							break;
						case Type.Color:
							colorValue = (Color)value;
							break;
						case Type.Transform:
							transformValue = (Transform)value;
							break;
						case Type.RectTransform:
							rectTransformValue = (RectTransform)value;
							break;
						case Type.Rigidbody:
							rigidbodyValue = (Rigidbody)value;
							break;
						case Type.Rigidbody2D:
							rigidbody2DValue = (Rigidbody2D)value;
							break;
						case Type.Component:
							componentValue = (Component)value;
							break;
						case Type.AssetObject:
							objectReferenceValue = (Object)value;
							break;
						case Type.Variable:
							variableValue = value;
							break;
						case Type.IntList:
							intListValue = (IList<int>)value;
							break;
						case Type.LongList:
							longListValue = (IList<long>)value;
							break;
						case Type.FloatList:
							floatListValue = (IList<float>)value;
							break;
						case Type.BoolList:
							boolListValue = (IList<bool>)value;
							break;
						case Type.StringList:
							stringListValue = (IList<string>)value;
							break;
						case Type.EnumList:
							Internal_SetEnumList(value, true);
							break;
						case Type.Vector2List:
							vector2ListValue = (IList<Vector2>)value;
							break;
						case Type.Vector3List:
							vector3ListValue = (IList<Vector3>)value;
							break;
						case Type.QuaternionList:
							quaternionListValue = (IList<Quaternion>)value;
							break;
						case Type.RectList:
							rectListValue = (IList<Rect>)value;
							break;
						case Type.BoundsList:
							boundsListValue = (IList<Bounds>)value;
							break;
						case Type.ColorList:
							colorListValue = (IList<Color>)value;
							break;
						case Type.GameObjectList:
							gameObjectListValue = (IList<GameObject>)value;
							break;
						case Type.ComponentList:
							Internal_SetComponentList(value, true);
							break;
						case Type.AssetObjectList:
							Internal_SetAssetObjectList(value, true);
							break;
						case Type.VariableList:
							variableListValue = value;
							break;
					}
				}
				catch (System.InvalidCastException ex)
				{
					Debug.LogException(ex);
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値が変更された際に呼び出されるコールバック関数。
		/// </summary>
#else
		/// <summary>
		/// Callback function to be called when the value is changed.
		/// </summary>
#endif
		public event DelegateOnChanged onChanged;

		internal void DoChanged()
		{
			if (onChanged != null)
			{
				onChanged(this);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を変更した際に呼び出す。
		/// </summary>
#else
		/// <summary>
		/// Call when you change the value.
		/// </summary>
#endif
		[System.Obsolete("There is no need to call it.")]
		public void OnChanged()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を文字列形式に変換する。
		/// </summary>
		/// <returns>変換した文字列</returns>
#else
		/// <summary>
		/// Convert value to string format.
		/// </summary>
		/// <returns>Converted string</returns>
#endif
		public override string ToString()
		{
			return System.Convert.ToString(value);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を文字列形式に変換する。
		/// </summary>
		/// <param name="format">数値書式指定フォーマット(Int,Long,Floatのみ)</param>
		/// <returns>変換した文字列</returns>
		/// <remarks>数値書式指定フォーマットの詳細については、次を参照してください。<a href="https://msdn.microsoft.com/ja-jp/library/dwhawy9k(v=vs.110).aspx">標準の数値書式指定文字列</a>、<a href="https://msdn.microsoft.com/ja-jp/library/0c899ak8(v=vs.110).aspx">カスタム数値書式指定文字列</a></remarks>
#else
		/// <summary>
		/// Convert value to string format.
		/// </summary>
		/// <param name="format">Numeric format string (Int, Long, Float only)</param>
		/// <returns>Converted string</returns>
		/// <remarks>For more information about numeric format specifiers, see <a href="https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx">Standard Numeric Format Strings</a> and <a href="https://msdn.microsoft.com/en-us/library/0c899ak8(v=vs.110).aspx">Custom Numeric Format Strings</a>.</remarks>
#endif
		public string ToString(string format)
		{
			string s;

			switch (type)
			{
				case Parameter.Type.Int:
					s = intValue.ToString(format);
					break;
				case Parameter.Type.Long:
					s = longValue.ToString(format);
					break;
				case Parameter.Type.Float:
					s = floatValue.ToString(format);
					break;
				case Parameter.Type.String:
					s = stringValue;
					break;
				default:
					s = System.Convert.ToString(value);
					break;
			}

			return s;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		internal void ChangeContainer(ParameterContainerInternal container)
		{
			if (!Application.isEditor || Application.isPlaying)
			{
				throw new System.NotSupportedException();
			}

			this.container = container;

			switch (type)
			{
				case Type.Variable:
					{
						VariableBase sourceVariable = Internal_GetObject() as VariableBase;
						Internal_SetObject(null);

						ComponentUtility.MoveVariable(this, sourceVariable);
					}
					break;
				case Type.VariableList:
					{
						VariableListBase sourceVariable = Internal_GetObject() as VariableListBase;
						Internal_SetObject(null);

						ComponentUtility.MoveVariableList(this, sourceVariable);
					}
					break;
			}
		}
	}
}
