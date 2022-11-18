﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Example
{
	/// <summary>
	/// Example calculator that uses the DataLink attribute
	/// </summary>
	[AddComponentMenu("")]
	[AddBehaviourMenu("Example/DataLinkExampleNewDataCalculator")]
	public sealed class DataLinkExampleNewDataCalculator : Calculator
	{
		/// <summary>
		/// string field with DataLink attribute
		/// </summary>
		[SerializeField]
		[DataLink]
		private string _StringValue = "";

		/// <summary>
		/// int field with DataLink attribute
		/// </summary>
		[SerializeField]
		[DataLink]
		private int _IntValue = 0;

		/// <summary>
		/// Output DataLinkExampleData
		/// </summary>
		[SerializeField]
		private OutputSlotDataLinkExampleData _Output = new OutputSlotDataLinkExampleData();

		/// <summary>
		/// Called when calculating an calculator node
		/// </summary>
		public override void OnCalculate()
		{
			// new DataLinkExampleData
			DataLinkExampleData exampleData = new DataLinkExampleData();

			// set values of DataLinkExampleData
			exampleData.stringValue = _StringValue;
			exampleData.intValue = _IntValue;

			// output data
			_Output.SetValue(exampleData);
		}
	}
}
