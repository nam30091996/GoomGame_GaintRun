﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// 演算ノードを表すクラス
	/// </summary>
#else
	/// <summary>
	/// Class that represents a calculator
	/// </summary>
#endif
	[System.Serializable]
	public sealed class CalculatorNode : Node, INodeBehaviourContainer
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("_Calculator")]
		private Object _Object;

#if ARBOR_DOC_JA
		/// <summary>
		/// 演算ノードIDを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the calculator node identifier.
		/// </summary>
#endif
		[System.Obsolete("use Node.nodeID")]
		public int calculatorID
		{
			get
			{
				return nodeID;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 割り当てているCalculatorを取得。
		/// </summary>
#else
		/// <summary>
		/// Get the attached Calculator.
		/// </summary>
#endif
		public Calculator calculator
		{
			get
			{
				return _Object as Calculator;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 割り当てているCalculatorのObjectを取得。
		/// </summary>
		/// <returns>割り当てているCalculatorのObject</returns>
#else
		/// <summary>
		/// Get the Object of the attached Calculator
		/// </summary>
		/// <returns>The Object of the attached Calculator</returns>
#endif
		public Object GetObject()
		{
			return _Object;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// CalculatorNodeのコンストラクタ
		/// </summary>
		/// <param name="nodeGraph">このノードを持つNodeGraph</param>
		/// <param name="nodeID">ノードID</param>
		/// <param name="calculatorType">Calculatorの型</param>
		/// <remarks>
		/// 演算ノードの生成は<see cref="NodeGraph.CreateCalculator(System.Type)"/>を使用してください。
		/// </remarks>
#else
		/// <summary>
		/// CalculatorNode constructor
		/// </summary>
		/// <param name="nodeGraph">NodeGraph with this node</param>
		/// <param name="nodeID">Node ID</param>
		/// <param name="calculatorType">Calculator type</param>
		/// <remarks>
		/// Please use the <see cref = "NodeGraph.CreateCalculator(System.Type)" /> Calculator Node creating.
		/// </remarks>
#endif
		public CalculatorNode(NodeGraph nodeGraph, int nodeID, System.Type calculatorType) : base(nodeGraph, nodeID)
		{
			_Object = Calculator.CreateCalculator(this, calculatorType);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Calculatorを作成する。エディタで使用する。
		/// </summary>
#else
		/// <summary>
		/// Create a Calculator. Use it in the editor.
		/// </summary>
#endif
		public Calculator CreateCalculator(System.Type calculatorType)
		{
			_Object = Calculator.CreateCalculator(this, calculatorType);
			return _Object as Calculator;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Nodeが所属するNodeGraphが変わった際に呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called when the NodeGraph to which the Node belongs has changed.
		/// </summary>
#endif
		protected override void OnGraphChanged()
		{
			Calculator sorceCalculator = _Object as Calculator;
			_Object = null;

			ComponentUtility.MoveBehaviour(this, sorceCalculator);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeBehaviourを含んでいるかをチェックする。
		/// </summary>
		/// <param name="behaviour">チェックするNodeBehaviour</param>
		/// <returns>NodeBehaviourを含んでいる場合にtrueを返す。</returns>
#else
		/// <summary>
		/// Check if it contains NodeBehaviour.
		/// </summary>
		/// <param name="behaviour">Check NodeBehaviour</param>
		/// <returns>Returns true if it contains NodeBehaviour.</returns>
#endif
		public override bool IsContainsBehaviour(NodeBehaviour behaviour)
		{
			Calculator calculator = behaviour as Calculator;
			return this.calculator == calculator;
		}

		int INodeBehaviourContainer.GetNodeBehaviourCount()
		{
			return 1;
		}

		T INodeBehaviourContainer.GetNodeBehaviour<T>(int index)
		{
			if (index == 0)
			{
				return _Object as T;
			}
			return null;
		}

		void INodeBehaviourContainer.SetNodeBehaviour(int index, NodeBehaviour behaviour)
		{
			if (index == 0)
			{
				_Object = behaviour;
			}
		}
	}
}
