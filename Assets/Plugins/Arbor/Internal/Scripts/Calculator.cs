//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// Stateの挙動を定義するクラス。継承して利用する。
	/// </summary>
	/// <remarks>
	/// 使用可能な属性 : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="AddBehaviourMenu" /></description></item>
	/// <item><description><see cref="HideBehaviour" /></description></item>
	/// <item><description><see cref="BehaviourTitle" /></description></item>
	/// <item><description><see cref="BehaviourHelp" /></description></item>
	/// </list>
	/// </remarks>
#else
	/// <summary>
	/// Class that defines the behavior of the State. Inherited and to use.
	/// </summary>
	/// <remarks>
	/// Available Attributes : <br/>
	/// <list type="bullet">
	/// <item><description><see cref="AddBehaviourMenu" /></description></item>
	/// <item><description><see cref="HideBehaviour" /></description></item>
	/// <item><description><see cref="BehaviourTitle" /></description></item>
	/// <item><description><see cref="BehaviourHelp" /></description></item>
	/// </list>
	/// </remarks>
#endif
	[AddComponentMenu("")]
	public class Calculator : NodeBehaviour
	{

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorNodeを取得。
		/// </summary>
#else
		/// <summary>
		/// Get the CalculatorNode.
		/// </summary>
#endif
		public CalculatorNode calculatorNode
		{
			get
			{
				return node as CalculatorNode;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorIDを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the Calculator identifier.
		/// </summary>
#endif
		public int calculatorID
		{
			get
			{
				return nodeID;
			}
		}

		bool _IsDirty = true;

		private bool _HasRandomFlexiblePrimitive = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// 変更されているかどうか。
		/// </summary>
#else
		/// <summary>
		/// Whether it has been changed.
		/// </summary>
#endif
		public bool isDirty
		{
			get
			{
				if (CallCheckDirty() || _IsDirty || _HasRandomFlexiblePrimitive)
				{
					return true;
				}

				int slotCount = dataSlotFieldCount;
				for (int slotIndex = 0; slotIndex < slotCount; slotIndex++)
				{
					DataSlotField slotInfo = GetDataSlotField(slotIndex);
					if (slotInfo == null)
					{
						continue;
					}
					InputSlotBase s = slotInfo.slot as InputSlotBase;
					if (s != null)
					{
						DataBranch branch = s.branch;
						if (branch != null)
						{
							Calculator outCalculator = branch.outBehaviour as Calculator;
							if (outCalculator != null && outCalculator.isDirty)
							{
								return true;
							}
						}
					}
				}

				return false;
			}
		}

		bool CallCheckDirty()
		{
			try
			{
				return OnCheckDirty();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
				return false;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 変更されているか判定する際に呼ばれる。
		/// </summary>
		/// <returns>変更されている場合はtrue、そうでなければfalseを返す。</returns>
#else
		/// <summary>
		/// It is called when judging whether it has been changed.
		/// </summary>
		/// <returns>Returns true if it has been changed, false otherwise.</returns>
#endif
		public virtual bool OnCheckDirty()
		{
			return false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// OnCalculateを呼んでほしい場合に呼び出す。
		/// </summary>
#else
		/// <summary>
		/// Call if you want call OnCalculate.
		/// </summary>
#endif
		public void SetDirty()
		{
			_IsDirty = true;
		}

		private List<Parameter> _CachedParameters = null;
		private List<Parameter> cachedParameters
		{
			get
			{
				if (_CachedParameters == null)
				{
					_CachedParameters = new List<Parameter>();
					EachField<ParameterReference>.Find(this, GetType(), (r) =>
					{
						Parameter parameter = r.parameter;
						if (parameter != null)
						{
							_CachedParameters.Add(parameter);
						}
					});
				}
				return _CachedParameters;
			}
		}

		private bool _IsSettedOnChanged;

		void SetOnChanged()
		{
			if (_IsSettedOnChanged)
			{
				ReleaseOnChanged();
				_CachedParameters.Clear();
				_CachedParameters = null;
				_IsSettedOnChanged = false;
			}

			List<Parameter> parameters = cachedParameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				Parameter parameter = parameters[i];
				parameter.onChanged += ParameterOnChanged;
			}

			_HasRandomFlexiblePrimitive = false;
			EachField<IFlexibleField>.Find(this, GetType(), (f) =>
			{
				FlexiblePrimitiveBase flexiblePrimitive = f as FlexiblePrimitiveBase;
				if (flexiblePrimitive != null && flexiblePrimitive.type == FlexiblePrimitiveType.Random)
				{
					_HasRandomFlexiblePrimitive = true;
				}
			});

			_IsSettedOnChanged = true;
		}

		void ReleaseOnChanged()
		{
			if (_IsSettedOnChanged)
			{
				List<Parameter> parameters = cachedParameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					Parameter parameter = parameters[i];
					parameter.onChanged -= ParameterOnChanged;
				}
				_IsSettedOnChanged = false;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はスクリプトのインスタンスがロードされたときに呼び出される。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the script instance is being loaded.
		/// </summary>
#endif
		protected virtual void Awake()
		{
			SetOnChanged();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はMonoBehaviourが破棄されるときに呼び出される。
		/// </summary>
#else
		/// <summary>
		/// This function is called when MonoBehaivour will be destroyed.
		/// </summary>
#endif
		protected virtual void OnDestroy()
		{
			ReleaseOnChanged();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はスクリプトがロードされた時やインスペクターの値が変更されたときに呼び出されます（この呼出はエディター上のみ）
		/// </summary>
#else
		/// <summary>
		/// This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
		/// </summary>
#endif
		protected override void OnValidate()
		{
			base.OnValidate();

			if (Application.isPlaying)
			{
				if (isActiveAndEnabled && _IsSettedOnChanged)
				{
					SetOnChanged();
				}

				SetDirty();
			}
		}

		void ParameterOnChanged(Parameter parameter)
		{
			SetDirty();
		}

		internal static Calculator CreateCalculator(Node node, System.Type type)
		{
			System.Type classType = typeof(Calculator);
			if (type != classType && !TypeUtility.IsSubclassOf(type, classType))
			{
				throw new System.ArgumentException("The type `" + type.Name + "' must be convertible to `Calculator' in order to use it as parameter `type'", "type");
			}

			return CreateNodeBehaviour(node, type) as Calculator;
		}

		void CallCalculate()
		{
			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnCalculate();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}

			_IsDirty = false;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 必要であれば演算する。
		/// </summary>
#else
		/// <summary>
		/// It is calculated, if necessary.
		/// </summary>
#endif
		public void Calculate()
		{
			if (isDirty)
			{
				CallCalculate();
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算される際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// It called when it is calculated .
		/// </summary>
#endif
		public virtual void OnCalculate()
		{
		}
	}
}
