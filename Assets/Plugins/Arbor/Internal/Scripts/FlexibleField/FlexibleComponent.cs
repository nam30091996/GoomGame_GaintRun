//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 参照方法が複数ある柔軟なComponent型を扱うクラス。
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
	/// Class to handle a flexible Component type reference method there is more than one.
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
	public sealed class FlexibleComponent : FlexibleSceneObjectBase, ISerializationCallbackReceiver
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private Component _Value = null;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private ComponentParameterReference _Parameter = new ComponentParameterReference();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField] private InputSlotComponent _Slot = new InputSlotComponent();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[HideInInspector]
		[SerializeField] private ClassTypeReference _OverrideConstraintType = new ClassTypeReference();

		private bool _IsDirtyConstraint = true;
		private ClassConstraintInfo _ConstraintInfo = null;

		private NodeGraph _CachedTargetGraph = null;
		private Component _CachedComponent = null;

#if ARBOR_DOC_JA
		/// <summary>
		/// Parameterを返す。TypeがParameter以外の場合はnull。
		/// </summary>
#else
		/// <summary>
		/// It return a Paramter. It is null if Type is other than Parameter.
		/// </summary>
#endif
		public Parameter parameter
		{
			get
			{
				if (_Type == FlexibleSceneObjectType.Parameter)
				{
					return _Parameter.parameter;
				}
				return null;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値を返す
		/// </summary>
#else
		/// <summary>
		/// It returns a value
		/// </summary>
#endif
		public Component value
		{
			get
			{
				Component value = null;
				switch (_Type)
				{
					case FlexibleSceneObjectType.Constant:
						value = _Value;
						break;
					case FlexibleSceneObjectType.Parameter:
						value = _Parameter.value;
						break;
					case FlexibleSceneObjectType.DataSlot:
						_Slot.GetValue(ref value);
						break;
					case FlexibleSceneObjectType.Hierarchy:
						{
							if (_CachedTargetGraph == null || _CachedComponent == null)
							{
								_CachedComponent = null;
								_CachedTargetGraph = this.targetGraph;
								if (_CachedTargetGraph != null)
								{
									// connectableBaseType is NodeGraph?
									if (TypeUtility.IsAssignableFrom(connectableBaseType, _CachedTargetGraph.GetType()))
									{
										_CachedComponent = _CachedTargetGraph;
									}

									if (_CachedComponent == null)
									{
										// connectableBaseType is ParameterContainer?
										ParameterContainerInternal parameterContainer = _CachedTargetGraph.parameterContainer;
										if (parameterContainer != null && TypeUtility.IsAssignableFrom(connectableBaseType, parameterContainer.GetType()))
										{
											_CachedComponent = parameterContainer;
										}
									}

									if (_CachedComponent == null)
									{
										_CachedComponent = _CachedTargetGraph.GetComponent(connectableBaseType);
									}
								}
							}
							value = _CachedComponent;
						}
						break;
				}

				return value;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 型制約の上書き。<br/>
		/// Typeが<see cref="FlexibleSceneObjectType.Hierarchy"/>の場合にGetComponentを行う際の型に使用される。
		/// </summary>
#else
		/// <summary>
		/// Overriding type constraints.<br/>
		/// Used for GetComponent when Type is <see cref="FlexibleSceneObjectType.Hierarchy"/>.
		/// </summary>
#endif
		public System.Type overrideConstraintType
		{
			get
			{
				return _OverrideConstraintType.type;
			}
			set
			{
				if (_OverrideConstraintType.type != value)
				{
					_OverrideConstraintType.type = value;
					SetDirtyConstraint();
				}
			}
		}

		ClassConstraintInfo CreateConstraintInfo()
		{
			System.Type overrideType = this.overrideConstraintType;
			if (overrideType != null)
			{
				return new ClassConstraintInfo() { baseType = overrideType };
			}

			ClassTypeConstraintAttribute constraint = AttributeHelper.GetAttribute<ClassTypeConstraintAttribute>(fieldInfo);
			if (constraint != null && TypeUtility.IsAssignableFrom(typeof(Component), constraint.GetBaseType(fieldInfo)))
			{
				return new ClassConstraintInfo() { constraintAttribute = constraint, constraintFieldInfo = fieldInfo };
			}

			SlotTypeAttribute slotTypeAttribute = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
			if (slotTypeAttribute != null && TypeUtility.IsAssignableFrom(typeof(Component), slotTypeAttribute.connectableType))
			{
				return new ClassConstraintInfo() { slotTypeAttribute = slotTypeAttribute };
			}

			return null;
		}

		ClassConstraintInfo constraintInfo
		{
			get
			{
				if (_IsDirtyConstraint)
				{
					_ConstraintInfo = CreateConstraintInfo();
					_IsDirtyConstraint = false;
				}
				return _ConstraintInfo;
			}
		}

		System.Type connectableBaseType
		{
			get
			{
				ClassConstraintInfo constraintInfo = this.constraintInfo;
				if (constraintInfo != null)
				{
					System.Type connectableType = constraintInfo.GetConstraintBaseType();
					if (connectableType != null && TypeUtility.IsAssignableFrom(typeof(Component), connectableType))
					{
						return connectableType;
					}
				}

				return typeof(Component);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentデフォルトコンストラクタ
		/// </summary>
#else
		/// <summary>
		/// FlexibleComponent default constructor
		/// </summary>
#endif
		public FlexibleComponent()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentコンストラクタ
		/// </summary>
		/// <param name="component">Component</param>
#else
		/// <summary>
		/// FlexibleComponent constructor
		/// </summary>
		/// <param name="component">Component</param>
#endif
		public FlexibleComponent(Component component)
		{
			_Type = FlexibleSceneObjectType.Constant;
			_Value = component;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentコンストラクタ
		/// </summary>
		/// <param name="parameter">Parameter</param>
#else
		/// <summary>
		/// FlexibleComponent constructor
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public FlexibleComponent(ComponentParameterReference parameter)
		{
			_Type = FlexibleSceneObjectType.Parameter;
			_Parameter = parameter;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentコンストラクタ
		/// </summary>
		/// <param name="slot">スロット</param>
#else
		/// <summary>
		/// FlexibleComponent constructor
		/// </summary>
		/// <param name="slot">Slot</param>
#endif
		public FlexibleComponent(InputSlotComponent slot)
		{
			_Type = FlexibleSceneObjectType.DataSlot;
			_Slot = slot;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentコンストラクタ。
		/// </summary>
		/// <param name="hierarchyType">参照するオブジェクトのヒエラルキータイプ</param>
#else
		/// <summary>
		/// FlexibleComponent constructor.
		/// </summary>
		/// <param name="hierarchyType">Hierarchy type of referenced object</param>
#endif
		public FlexibleComponent(FlexibleHierarchyType hierarchyType)
		{
			_Type = FlexibleSceneObjectType.Hierarchy;
			_HierarchyType = hierarchyType;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleComponentをComponentにキャスト。
		/// </summary>
		/// <param name="flexible">FlexibleComponent</param>
#else
		/// <summary>
		/// Cast FlexibleColor to Component.
		/// </summary>
		/// <param name="flexible">FlexibleComponent</param>
#endif
		public static explicit operator Component(FlexibleComponent flexible)
		{
			return flexible.value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ComponentをFlexibleComponentにキャスト。
		/// </summary>
		/// <param name="value">Component</param>
#else
		/// <summary>
		/// Cast Component to FlexibleComponent.
		/// </summary>
		/// <param name="value">Component</param>
#endif
		public static explicit operator FlexibleComponent(Component value)
		{
			return new FlexibleComponent(value);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 値をobjectで返す。
		/// </summary>
		/// <returns>値のobject</returns>
#else
		/// <summary>
		/// Return the value as object.
		/// </summary>
		/// <returns>The value object</returns>
#endif
		public override object GetValueObject()
		{
			return value;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// FlexibleSceneObjectType.ConstantのObjectを返す。
		/// </summary>
		/// <returns>Constantの時のObject値</returns>
#else
		/// <summary>
		/// Returns an Object of FlexibleSceneObjectType.Constant.
		/// </summary>
		/// <returns>Object value at Constant</returns>
#endif
		public override Object GetConstantObject()
		{
			return _Value;
		}

		internal void SetSlot(InputSlotBase slot)
		{
			_Type = FlexibleSceneObjectType.DataSlot;
			_Slot.Copy(slot);
		}

		void SetDirtyConstraint()
		{
			_IsDirtyConstraint = true;
			_CachedComponent = null;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			SetDirtyConstraint();
		}
	}
}
