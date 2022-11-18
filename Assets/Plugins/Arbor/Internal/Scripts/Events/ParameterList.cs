//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Arbor.Events
{
#if ARBOR_DOC_JA
	/// <summary>
	/// データーフローをサポートするパラメータリスト。
	/// </summary>
#else
	/// <summary>
	/// Parameter list that supports data flow.
	/// </summary>
#endif
	[System.Serializable]
	public sealed class ParameterList
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleInt> _IntParameters = new List<FlexibleInt>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleLong> _LongParameters = new List<FlexibleLong>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleFloat> _FloatParameters = new List<FlexibleFloat>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleBool> _BoolParameters = new List<FlexibleBool>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleString> _StringParameters = new List<FlexibleString>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleVector2> _Vector2Parameters = new List<FlexibleVector2>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleVector3> _Vector3Parameters = new List<FlexibleVector3>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleQuaternion> _QuaternionParameters = new List<FlexibleQuaternion>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleRect> _RectParameters = new List<FlexibleRect>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleBounds> _BoundsParameters = new List<FlexibleBounds>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleColor> _ColorParameters = new List<FlexibleColor>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleGameObject> _GameObjectParameters = new List<FlexibleGameObject>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleComponent> _ComponentParameters = new List<FlexibleComponent>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleEnumAny> _EnumParameters = new List<FlexibleEnumAny>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		internal List<FlexibleAssetObject> _AssetObjectParameters = new List<FlexibleAssetObject>();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideSlotFields]
		internal List<InputSlotTypable> _InputSlotParameters = new List<InputSlotTypable>();

#if ARBOR_DOC_JA
		/// <summary>
		/// 要素を追加する。
		/// </summary>
		/// <param name="valueType">値の型</param>
		/// <param name="slot">設定する入力スロット</param>
		/// <returns>追加したパラメータのタイプ</returns>
#else
		/// <summary>
		/// Add an element.
		/// </summary>
		/// <param name="valueType">Value type</param>
		/// <param name="slot">Input slot to set</param>
		/// <returns>Type of parameter added</returns>
#endif
		public ParameterType AddElement(System.Type valueType, InputSlotBase slot)
		{
			bool isValidSlot = slot != null && (slot.nodeGraph != null && slot.branchID != 0);

			ParameterType parameterType = ArborEventUtility.GetParameterType(valueType, true);

			switch (parameterType)
			{
				case ParameterType.Int:
					{
						FlexibleInt flexibleInt = new FlexibleInt();
						if (isValidSlot)
						{
							flexibleInt.SetSlot(slot);
						}
						_IntParameters.Add(flexibleInt);
					}
					break;
				case ParameterType.Long:
					{
						FlexibleLong flexibleLong = new FlexibleLong();
						if (isValidSlot)
						{
							flexibleLong.SetSlot(slot);
						}
						_LongParameters.Add(flexibleLong);
					}
					break;
				case ParameterType.Float:
					{
						FlexibleFloat flexibleFloat = new FlexibleFloat();
						if (isValidSlot)
						{
							flexibleFloat.SetSlot(slot);
						}

						_FloatParameters.Add(flexibleFloat);
					}
					break;
				case ParameterType.Bool:
					{
						FlexibleBool flexibleBool = new FlexibleBool();
						if (isValidSlot)
						{
							flexibleBool.SetSlot(slot);
						}

						_BoolParameters.Add(flexibleBool);
					}
					break;
				case ParameterType.String:
					{
						FlexibleString flexibleString = new FlexibleString();
						if (isValidSlot)
						{
							flexibleString.SetSlot(slot);
						}

						_StringParameters.Add(flexibleString);
					}
					break;
				case ParameterType.Vector2:
					{
						FlexibleVector2 flexibleVector2 = new FlexibleVector2();
						if (isValidSlot)
						{
							flexibleVector2.SetSlot(slot);
						}
						_Vector2Parameters.Add(flexibleVector2);
					}
					break;
				case ParameterType.Vector3:
					{
						FlexibleVector3 flexibleVector3 = new FlexibleVector3();
						if (isValidSlot)
						{
							flexibleVector3.SetSlot(slot);
						}
						_Vector3Parameters.Add(flexibleVector3);
					}
					break;
				case ParameterType.Quaternion:
					{
						FlexibleQuaternion flexibleQuaternion = new FlexibleQuaternion();
						if (isValidSlot)
						{
							flexibleQuaternion.SetSlot(slot);
						}
						_QuaternionParameters.Add(flexibleQuaternion);
					}
					break;
				case ParameterType.Rect:
					{
						FlexibleRect flexibleRect = new FlexibleRect();
						if (isValidSlot)
						{
							flexibleRect.SetSlot(slot);
						}
						_RectParameters.Add(flexibleRect);
					}
					break;
				case ParameterType.Bounds:
					{
						FlexibleBounds flexibleBounds = new FlexibleBounds();
						if (isValidSlot)
						{
							flexibleBounds.SetSlot(slot);
						}
						_BoundsParameters.Add(flexibleBounds);
					}
					break;
				case ParameterType.Color:
					{
						FlexibleColor flexibleColor = new FlexibleColor();
						if (isValidSlot)
						{
							flexibleColor.SetSlot(slot);
						}
						_ColorParameters.Add(flexibleColor);
					}
					break;
				case ParameterType.GameObject:
					{
						FlexibleGameObject flexibleGameObject = new FlexibleGameObject();
						if (isValidSlot)
						{
							flexibleGameObject.SetSlot(slot);
						}
						_GameObjectParameters.Add(flexibleGameObject);
					}
					break;
				case ParameterType.Component:
					{
						FlexibleComponent flexibleComponent = new FlexibleComponent();
						if (isValidSlot)
						{
							flexibleComponent.SetSlot(slot);
						}
						_ComponentParameters.Add(flexibleComponent);
					}
					break;
				case ParameterType.Enum:
					{
						FlexibleEnumAny flexibleEnumAny = new FlexibleEnumAny();
						if (isValidSlot)
						{
							flexibleEnumAny.SetSlot(slot);
						}
						_EnumParameters.Add(flexibleEnumAny);
					}
					break;
				case ParameterType.AssetObject:
					{
						FlexibleAssetObject flexibleAssetObject = new FlexibleAssetObject();
						if (isValidSlot)
						{
							flexibleAssetObject.SetSlot(slot);
						}
						_AssetObjectParameters.Add(flexibleAssetObject);
					}
					break;
				case ParameterType.Slot:
					{
						InputSlotTypable inputSlotTypable = new InputSlotTypable(valueType);
						if (isValidSlot)
						{
							inputSlotTypable.Copy(slot);
						}
						_InputSlotParameters.Add(inputSlotTypable);
					}
					break;
			}

			return parameterType;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータのリストを取得する。
		/// </summary>
		/// <param name="parameterType">パラメータのタイプ</param>
		/// <returns>パラメータのリスト</returns>
#else
		/// <summary>
		/// Get a list of parameters.
		/// </summary>
		/// <param name="parameterType">Parameter type</param>
		/// <returns>List of parameters</returns>
#endif
		public IList GetParameterList(ParameterType parameterType)
		{
			switch (parameterType)
			{
				case ParameterType.Int:
					return _IntParameters;
				case ParameterType.Long:
					return _LongParameters;
				case ParameterType.Float:
					return _FloatParameters;
				case ParameterType.Bool:
					return _BoolParameters;
				case ParameterType.String:
					return _StringParameters;
				case ParameterType.Vector2:
					return _Vector2Parameters;
				case ParameterType.Vector3:
					return _Vector3Parameters;
				case ParameterType.Quaternion:
					return _QuaternionParameters;
				case ParameterType.Rect:
					return _RectParameters;
				case ParameterType.Bounds:
					return _BoundsParameters;
				case ParameterType.Color:
					return _ColorParameters;
				case ParameterType.GameObject:
					return _GameObjectParameters;
				case ParameterType.Component:
					return _ComponentParameters;
				case ParameterType.Enum:
					return _EnumParameters;
				case ParameterType.AssetObject:
					return _AssetObjectParameters;
				case ParameterType.Slot:
					return _InputSlotParameters;
			}

			return null;
		}
	}
}