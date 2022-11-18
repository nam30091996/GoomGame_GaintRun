//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.ParameterBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// <see cref="Arbor.ParameterBehaviours.GetParameterCalculator" />の内部クラス。
	/// </summary>
#else
	/// <summary>
	/// Internal class of <see cref="Arbor.ParameterBehaviours.GetParameterCalculator" />.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[HideBehaviour()]
	public class GetParameterCalculatorInternal : Calculator, INodeBehaviourSerializationCallbackReceiver
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// 取得するパラメータ。
		/// </summary>
#else
		/// <summary>
		/// Parameters to get.
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		private ParameterReference _ParameterReference = new ParameterReference();

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータの値を出力するスロット。
		/// </summary>
#else
		/// <summary>
		/// A slot that outputs the value of the parameter.
		/// </summary>
#endif
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideSlotFields]
		private OutputSlotTypable _Output = new OutputSlotTypable();

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		private bool _IsInGraphParameter = false;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		private int _SerializeVersion = 0;

#if ARBOR_DOC_JA
		/// <summary>
		/// グラフ内パラメータを参照しているかどうか。
		/// </summary>
#else
		/// <summary>
		///Whether this refers to a parameter in the graph.
		/// </summary>
#endif
		public bool isInGraphParameter
		{
			get
			{
				return _IsInGraphParameter;
			}
		}

		private const int kCurrentSerializeVersion = 1;

#if ARBOR_DOC_JA
		/// <summary>
		/// パラメータを設定する。
		/// </summary>
		/// <param name="parameter">パラメータ</param>
#else
		/// <summary>
		/// Set parameter.
		/// </summary>
		/// <param name="parameter">Parameter</param>
#endif
		public void SetParameter(Parameter parameter)
		{
			_ParameterReference.container = parameter.container;
			_ParameterReference.id = parameter.id;
			_ParameterReference.name = parameter.name;

			if (_Output == null)
			{
				_Output = new OutputSlotTypable();
			}
			_Output.SetType(parameter.valueType);

			SetupIsInGraphParameter();
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
		public override void OnCalculate()
		{
			Parameter parameter = _ParameterReference.parameter;
			if (parameter == null)
			{
				return;
			}

			if (TypeUtility.IsAssignableFrom(_Output.dataType, parameter.valueType))
			{
				_Output.SetValue(parameter.value);
			}
			else
			{
				Debug.LogWarning("Parameter type and data slot type are different.", this);
			}
		}

		void SetUpIsInGraphParameterInternal()
		{
			ParameterContainerBase container = _ParameterReference.container;
			_IsInGraphParameter = (nodeGraph.parameterContainer != null && container != null && nodeGraph.parameterContainer == container);
		}

		void SetupIsInGraphParameter()
		{
#if !ARBOR_DEBUG_IN_GRAPH_PARAMETER
			if (_ParameterReference.type != ParameterReferenceType.Constant)
			{
				_IsInGraphParameter = false;
				return;
			}

			NodeGraph nodeGraph = this.nodeGraph;

			if (nodeGraph == null)
			{
				_IsInGraphParameter = false;
				return;
			}

			if (!nodeGraph.isDeserialized)
			{
				nodeGraph.onAfterDeserialize += SetUpIsInGraphParameterInternal;
			}
			else
			{
				SetUpIsInGraphParameterInternal();
			}
#endif
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		void Reset()
		{
			_SerializeVersion = kCurrentSerializeVersion;
		}

		void SerializeVer1()
		{
			SetupIsInGraphParameter();
		}

		void Serialize()
		{
			while (_SerializeVersion != kCurrentSerializeVersion)
			{
				switch (_SerializeVersion)
				{
					case 0:
						SerializeVer1();
						_SerializeVersion++;
						break;
					default:
						_SerializeVersion = kCurrentSerializeVersion;
						break;
				}
			}
		}

		void INodeBehaviourSerializationCallbackReceiver.OnBeforeSerialize()
		{
			Serialize();
		}

		void INodeBehaviourSerializationCallbackReceiver.OnAfterDeserialize()
		{
			Serialize();
		}
	}
}